using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace ClipboardTo1Pass;

public class Program
{    // Windows API constants for global hotkeys
    private const int WM_HOTKEY = 0x0312;
    private const int MOD_CONTROL = 0x0002;
    private const int MOD_ALT = 0x0001;
    private const int HOTKEY_ID = 1;
    private const int VK_F12 = 0x7B;    // Windows API imports
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("kernel32.dll")]
    private static extern uint GetLastError();    private static NotifyIcon? trayIcon;
    private static readonly string OnePasswordItemName = "_CP";    private static ProcessStartInfo CreateOnePasswordProcessStartInfo(string arguments, bool useShellExecute = false)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "op",
            Arguments = arguments,
            UseShellExecute = useShellExecute,
            CreateNoWindow = true
        };

        if (useShellExecute)
        {
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
        else
        {
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
        }

        return startInfo;
    }

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Create a hidden form to handle hotkey messages
        var form = new HiddenForm();
        // Create system tray icon
        CreateTrayIcon();

        Console.WriteLine("Clipboard to 1Password tool started.");
        Console.WriteLine("Press Ctrl+Alt+F12 to save clipboard content to 1Password.");
        Console.WriteLine("Right-click the system tray icon to exit.");

        // Ensure the form handle is created
        var handle = form.Handle; // This forces handle creation

        // Small delay to ensure everything is ready
        System.Threading.Thread.Sleep(100);

        // Register global hotkey (Ctrl+Alt+F12)
        if (!RegisterHotKey(form.Handle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_F12))
        {
            var error = GetLastError();
            var errorMsg = $"Failed to register hotkey Ctrl+Alt+F12. Error code: {error}";
            Console.WriteLine(errorMsg);
            MessageBox.Show($"{errorMsg}\n\nPossible solutions:\n1. Run as Administrator\n2. Close other applications that might use this hotkey\n3. Use the system tray menu instead",
                          "Hotkey Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            // Don't exit - still allow tray menu usage
        }
        else
        {
            Console.WriteLine("Hotkey Ctrl+Alt+F12 registered successfully!");
        }

        // Run the application
        Application.Run();

        // Cleanup
        UnregisterHotKey(form.Handle, HOTKEY_ID);
        trayIcon?.Dispose();
    }

    private static void CreateTrayIcon()
    {
        trayIcon = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            Text = "Clipboard to 1Password",
            Visible = true
        };

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Save Clipboard Now", null, (s, e) => SaveClipboardTo1Password());
        contextMenu.Items.Add("-");
        contextMenu.Items.Add("Exit", null, (s, e) => Application.Exit());
        trayIcon.ContextMenuStrip = contextMenu;
        trayIcon.DoubleClick += (s, e) => SaveClipboardTo1Password();
    }    public static void SaveClipboardTo1Password()
    {
        Console.WriteLine("=== SaveClipboardTo1Password called ===");
        try
        {
            if (!Clipboard.ContainsText())
            {
                Console.WriteLine("No text found in clipboard");
                ShowNotification("No text found in clipboard", ToolTipIcon.Warning);
                return;
            }
            
            string? clipboardText = Clipboard.GetText();
            Console.WriteLine($"Clipboard text length: {clipboardText?.Length ?? 0}");

            if (string.IsNullOrWhiteSpace(clipboardText))
            {
                Console.WriteLine("Clipboard text is null or whitespace");
                ShowNotification("Clipboard is empty", ToolTipIcon.Warning);
                return;
            }

            // Update the secure note in 1Password
            var updateResult = UpdateSecureNote(clipboardText);
            if (updateResult.Success)
            {
                Console.WriteLine("SUCCESS: Clipboard content saved to 1Password!");
                ShowNotification("Clipboard content saved to 1Password!", ToolTipIcon.Info);
            }
            else
            {
                var errorMsg = $"Failed to save to 1Password: {updateResult.Error}";
                Console.WriteLine($"FAILURE: {errorMsg}");
                ShowNotification(errorMsg, ToolTipIcon.Error);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"EXCEPTION in SaveClipboardTo1Password: {ex.Message}");
            ShowNotification($"Error: {ex.Message}", ToolTipIcon.Error);
        }
        Console.WriteLine("=== SaveClipboardTo1Password completed ===");
    }private static string GetCurrentSecureNote()
    {
        try
        {
            Console.WriteLine("Getting current secure note...");
            var startInfo = CreateOnePasswordProcessStartInfo($"item get \"{OnePasswordItemName}\" --format=json");

            using var process = Process.Start(startInfo);
            if (process == null)
                throw new Exception("Failed to start 1Password CLI");

            if (!process.WaitForExit(30000))
            {
                process.Kill();
                throw new Exception("1Password CLI timed out");
            }

            if (process.ExitCode != 0)
                throw new Exception($"1Password CLI error: {process.StandardError?.ReadToEnd() ?? "Unknown error"}");            var json = process.StandardOutput?.ReadToEnd() ?? "";
            dynamic? item = JsonConvert.DeserializeObject(json);
            
            if (item?.fields != null)
            {
                try
                {
                    foreach (dynamic field in item.fields!)
                    {
                        if (field?.id == "notesPlain" && field?.value != null)
                            return field?.value?.ToString() ?? "";
                    }
                }
                catch
                {
                    // Ignore JSON parsing issues
                }
            }

            return "";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in GetCurrentSecureNote: {ex.Message}");
            return "";
        }
    }    private static (bool Success, string Error) UpdateSecureNote(string content)
    {
        try
        {
            Console.WriteLine("Updating secure note...");

            // Check if item exists first
            var getStartInfo = CreateOnePasswordProcessStartInfo($"item get \"{OnePasswordItemName}\" --format=json");
            
            using var getProcess = Process.Start(getStartInfo);
            if (getProcess == null)
                return (false, "Failed to start 1Password CLI process");

            if (!getProcess.WaitForExit(30000))
            {
                getProcess.Kill();
                return (false, "1Password CLI timed out during item check");
            }

            if (getProcess.ExitCode != 0)
            {
                Console.WriteLine("Item doesn't exist, creating new one...");
                return CreateSecureNote(content);
            }

            Console.WriteLine("Item exists, updating...");
            var editStartInfo = CreateOnePasswordProcessStartInfo($"item edit \"{OnePasswordItemName}\" \"notesPlain={content}\"", useShellExecute: true);

            using var process = Process.Start(editStartInfo);
            if (process == null)
                return (false, "Failed to start 1Password CLI process");

            if (!process.WaitForExit(30000))
            {
                process.Kill();
                return (false, "1Password CLI timed out during update");
            }

            if (process.ExitCode != 0)
                return (false, $"CLI exit code {process.ExitCode}");

            Console.WriteLine("Update completed successfully");
            return (true, "");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating secure note: {ex.Message}");
            return (false, ex.Message);
        }
    }    private static (bool Success, string Error) CreateSecureNote(string content)
    {
        try
        {
            Console.WriteLine("Creating new secure note...");
            var createStartInfo = CreateOnePasswordProcessStartInfo($"item create --category=\"Secure Note\" --title=\"{OnePasswordItemName}\" \"notesPlain={content}\"", useShellExecute: true);

            using var process = Process.Start(createStartInfo);
            if (process == null)
                return (false, "Failed to start 1Password CLI process");

            if (!process.WaitForExit(30000))
            {
                process.Kill();
                return (false, "1Password CLI timed out during create");
            }

            if (process.ExitCode != 0)
                return (false, $"Create CLI exit code {process.ExitCode}");

            Console.WriteLine("Create completed successfully");
            return (true, "");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating secure note: {ex.Message}");
            return (false, ex.Message);
        }
    }

    private static void ShowNotification(string message, ToolTipIcon icon)
    {
        trayIcon?.ShowBalloonTip(3000, "Clipboard to 1Password", message, icon);
    }
}

// Hidden form to handle Windows messages
public class HiddenForm : Form
{
    public HiddenForm()
    {
        WindowState = FormWindowState.Minimized;
        ShowInTaskbar = false;
        Visible = false;
    }
    protected override void WndProc(ref Message m)
    {
        const int WM_HOTKEY = 0x0312;

        if (m.Msg == WM_HOTKEY)
        {
            // Hotkey was pressed - save clipboard content on the main thread
            Console.WriteLine("=== HOTKEY TRIGGERED ===");
            this.BeginInvoke(new Action(() => Program.SaveClipboardTo1Password()));
        }

        base.WndProc(ref m);
    }
}
