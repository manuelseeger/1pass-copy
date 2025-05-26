using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
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
    private static extern uint GetLastError();

    private static NotifyIcon? trayIcon;
    private static readonly string OnePasswordItemName = "_CP";

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
    }

    public static void SaveClipboardTo1Password()
    {
        Console.WriteLine("=== SaveClipboardTo1Password called ===");
        try
        {
            Console.WriteLine("Checking clipboard content...");
            if (!Clipboard.ContainsText())
            {
                Console.WriteLine("No text found in clipboard");
                ShowNotification("No text found in clipboard", ToolTipIcon.Warning);
                return;
            }
            string clipboardText = Clipboard.GetText();
            Console.WriteLine($"Clipboard text length: {clipboardText?.Length ?? 0}");
            Console.WriteLine($"Clipboard text preview: {clipboardText?.Substring(0, Math.Min(50, clipboardText?.Length ?? 0))}...");

            if (string.IsNullOrWhiteSpace(clipboardText))
            {
                Console.WriteLine("Clipboard text is null or whitespace");
                ShowNotification("Clipboard is empty", ToolTipIcon.Warning);
                return;
            }

            Console.WriteLine("Getting current secure note content...");
            // Get current secure note content from 1Password
            var currentNote = GetCurrentSecureNote();
            Console.WriteLine($"Current note length: {currentNote.Length}");

            var updatedContent = clipboardText;

            Console.WriteLine($"Updated content length: {updatedContent.Length}");
            Console.WriteLine("Updating secure note...");

            // Update the secure note in 1Password
            var updateResult = UpdateSecureNote(updatedContent);
            if (updateResult.Success)
            {
                Console.WriteLine("SUCCESS: Clipboard content saved to 1Password!");
                ShowNotification($"Clipboard content saved to 1Password!", ToolTipIcon.Info);
                Console.WriteLine($"[{DateTime.Now}] Clipboard content saved to 1Password successfully.");
            }
            else
            {
                var errorMsg = $"Failed to save to 1Password: {updateResult.Error}";
                Console.WriteLine($"FAILURE: {errorMsg}");
                ShowNotification(errorMsg, ToolTipIcon.Error);
                Console.WriteLine($"[{DateTime.Now}] {errorMsg}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"EXCEPTION in SaveClipboardTo1Password: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            ShowNotification($"Error: {ex.Message}", ToolTipIcon.Error);
        }
        Console.WriteLine("=== SaveClipboardTo1Password completed ===");
    }
    private static string GetCurrentSecureNote()
    {
        try
        {
            Console.WriteLine("Starting GetCurrentSecureNote...");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var startInfo = new ProcessStartInfo
            {
                FileName = @"C:\Users\seeg\AppData\Local\Microsoft\WinGet\Links\op.exe",
                Arguments = $"item get \"{OnePasswordItemName}\" --format=json",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            Console.WriteLine($"Executing: {startInfo.FileName} {startInfo.Arguments}");
            Console.WriteLine("Waiting for 1Password CLI (this may take ~5 seconds)...");

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                throw new Exception("Failed to start 1Password CLI");
            }

            // Wait up to 30 seconds for the process to complete
            if (!process.WaitForExit(30000))
            {
                process.Kill();
                throw new Exception("1Password CLI timed out after 30 seconds");
            }

            stopwatch.Stop();
            Console.WriteLine($"1Password CLI completed in {stopwatch.ElapsedMilliseconds}ms");

            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();

            Console.WriteLine($"Get note exit code: {process.ExitCode}");
            if (!string.IsNullOrEmpty(stdout)) Console.WriteLine($"Get note stdout: {stdout.Substring(0, Math.Min(200, stdout.Length))}...");
            if (!string.IsNullOrEmpty(stderr)) Console.WriteLine($"Get note stderr: {stderr}");
            if (process.ExitCode != 0)
            {
                var error = stderr;
                throw new Exception($"1Password CLI error: {error}");
            }

            var json = stdout;
            Console.WriteLine($"1Password item JSON length: {json.Length}");            dynamic? item = JsonConvert.DeserializeObject(json);
            // Look for the notes field in the item            
            if (item?.fields != null)
            {
                try
                {
                    Console.WriteLine("Found fields in item");
                    var fields = item.fields;
                    if (fields != null)
                    {
                        foreach (dynamic field in fields)
                        {
                            if (field != null)
                            {
                                Console.WriteLine($"Field ID: {field?.id}, has value: {field?.value != null}");                                if (field?.id == "notesPlain" && field?.value != null)
                                {
                                    string noteContent = field!.value != null ? field.value.ToString() : "";
                                    Console.WriteLine($"Found notesPlain field with {noteContent.Length} characters");
                                    return noteContent;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing fields: {ex.Message}");
                }
            }

            Console.WriteLine("No notesPlain field found, returning empty string");
            return ""; // Return empty string if no notes found
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in GetCurrentSecureNote: {ex.Message}");
            return ""; // Return empty string on error, we'll create new content
        }
    }    private static (bool Success, string Error) UpdateSecureNote(string content)
    {
        try
        {
            Console.WriteLine("Starting UpdateSecureNote...");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Try to get the item first to check if it exists
            var getStartInfo = new ProcessStartInfo
            {
                FileName = @"C:\Users\seeg\AppData\Local\Microsoft\WinGet\Links\op.exe",
                Arguments = $"item get \"{OnePasswordItemName}\" --format=json",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            Console.WriteLine("Checking if item exists...");
            using var getProcess = Process.Start(getStartInfo);
            if (getProcess == null)
            {
                return (false, "Failed to start 1Password CLI process");
            }

            // Wait up to 30 seconds for the get process
            if (!getProcess.WaitForExit(30000))
            {
                getProcess.Kill();
                return (false, "1Password CLI timed out during item check");
            }

            if (getProcess.ExitCode != 0)
            {
                Console.WriteLine("Item doesn't exist, creating new one...");
                // Item doesn't exist, create it first
                return CreateSecureNote(content);
            }            Console.WriteLine("Item exists, updating...");            var editStartInfo = new ProcessStartInfo
            {
                FileName = @"C:\Users\seeg\AppData\Local\Microsoft\WinGet\Links\op.exe",
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            };
            
            // Build arguments as a single string for UseShellExecute = true
            var args = $"item edit \"{OnePasswordItemName}\" \"notesPlain={content}\"";
            editStartInfo.Arguments = args;

            Console.WriteLine($"Executing: op item edit \"{OnePasswordItemName}\" \"notesPlain=<content>\"");
            Console.WriteLine("Waiting for 1Password CLI update (this may take ~5 seconds)...");            using var process = Process.Start(editStartInfo);
            if (process == null)
            {
                return (false, "Failed to start 1Password CLI process");
            }

            Console.WriteLine($"Process started with PID: {process.Id}");
            Console.WriteLine($"Process has exited: {process.HasExited}");

            // Check periodically and report status
            var checkInterval = 2000; // Check every 2 seconds
            var totalWaitTime = 30000; // Total timeout of 30 seconds
            var elapsedTime = 0;

            while (elapsedTime < totalWaitTime)
            {
                if (process.WaitForExit(checkInterval))
                {
                    Console.WriteLine($"Process completed after {stopwatch.ElapsedMilliseconds}ms");
                    break;
                }
                
                elapsedTime += checkInterval;
                Console.WriteLine($"Still waiting... {elapsedTime/1000}s elapsed, process still running: {!process.HasExited}");
                
                if (process.HasExited)
                {
                    Console.WriteLine("Process has exited but WaitForExit returned false - this is unusual");
                    break;
                }
            }

            if (elapsedTime >= totalWaitTime && !process.HasExited)
            {
                Console.WriteLine("Timeout reached, killing process");
                process.Kill();
                return (false, "1Password CLI timed out during update");
            }            stopwatch.Stop();
            Console.WriteLine($"Update operation completed in {stopwatch.ElapsedMilliseconds}ms");

            Console.WriteLine($"Exit code: {process.ExitCode}");
            // Note: Cannot read stdout/stderr when UseShellExecute = true
            
            if (process.ExitCode != 0)
            {
                return (false, $"CLI exit code {process.ExitCode}");
            }

            return (true, "");
        }
        catch (Exception ex)
        {
            var errorMsg = $"Exception: {ex.Message}";
            Console.WriteLine($"Error updating secure note: {errorMsg}");
            return (false, errorMsg);
        }
    }

    private static (bool Success, string Error) CreateSecureNote(string content)
    {
        try
        {
            Console.WriteLine("Starting CreateSecureNote...");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();            var createStartInfo = new ProcessStartInfo
            {
                FileName = @"C:\Users\seeg\AppData\Local\Microsoft\WinGet\Links\op.exe",
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            };
            
            // Build arguments as a single string for UseShellExecute = true
            var args = $"item create --category=\"Secure Note\" --title=\"{OnePasswordItemName}\" \"notesPlain={content}\"";
            createStartInfo.Arguments = args;

            Console.WriteLine($"Creating new secure note: op item create --category=\"Secure Note\" --title=\"{OnePasswordItemName}\" \"notesPlain=<content>\"");
            Console.WriteLine("Waiting for 1Password CLI create (this may take ~5 seconds)...");

            using var process = Process.Start(createStartInfo);
            if (process == null)
            {
                return (false, "Failed to start 1Password CLI process");
            }

            // Wait up to 30 seconds for the create process
            if (!process.WaitForExit(30000))
            {
                process.Kill();
                return (false, "1Password CLI timed out during create");
            }            stopwatch.Stop();
            Console.WriteLine($"Create operation completed in {stopwatch.ElapsedMilliseconds}ms");

            Console.WriteLine($"Create exit code: {process.ExitCode}");
            // Note: Cannot read stdout/stderr when UseShellExecute = true

            if (process.ExitCode != 0)
            {
                return (false, $"Create CLI exit code {process.ExitCode}");
            }

            return (true, "");
        }
        catch (Exception ex)
        {
            var errorMsg = $"Create exception: {ex.Message}";
            Console.WriteLine($"Error creating secure note: {errorMsg}");
            return (false, errorMsg);
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
