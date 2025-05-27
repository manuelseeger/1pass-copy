using System.Runtime.InteropServices;
using OnePassword;
using OnePassword.Items;
using OnePassword.Vaults;

namespace ClipboardTo1Pass;

public class Program
{
    // Windows API constants for global hotkeys
    private const int MOD_CONTROL = 0x0002;
    private const int MOD_ALT = 0x0001;
    private const int HOTKEY_ID = 1;
    private const int VK_F12 = 0x7B;

    // Windows API imports
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("kernel32.dll")]
    private static extern uint GetLastError(); private static NotifyIcon? trayIcon;
    private static readonly string OnePasswordItemName = "_CP";
    private static OnePasswordManager? onePasswordManager;

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Initialize OnePassword manager
        try
        {
            InitializeOnePassword();
        }
        catch (Exception ex)
        {
            var errorMsg = $"Failed to initialize 1Password: {ex.Message}";
            Console.WriteLine(errorMsg);
            MessageBox.Show($"{errorMsg}\n\nPlease ensure:\n1. 1Password CLI is installed and in PATH\n2. You will be prompted to sign in when saving clipboard content",
                          "1Password Initialization Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

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

    private static void InitializeOnePassword()
    {
        Console.WriteLine("Initializing 1Password manager...");

        // Find 1Password CLI executable in PATH environment variable
        var opPath = FindExecutableInPath("op.exe") ?? FindExecutableInPath("op");
        if (string.IsNullOrEmpty(opPath))
        {
            throw new FileNotFoundException("1Password CLI (op.exe) not found in PATH. Please ensure 1Password CLI is installed and in your PATH environment variable.");
        }

        Console.WriteLine($"Found 1Password CLI at: {opPath}");

        // Create OnePasswordManagerOptions with the discovered path
        var options = new OnePasswordManagerOptions
        {
            Path = Path.GetDirectoryName(opPath) ?? "",
            Executable = Path.GetFileName(opPath) ?? "op.exe",
            AppIntegrated = true
        };
        // Create OnePassword manager with the configured options
        onePasswordManager = new OnePasswordManager(options);

        // Automatic sign-in removed here; sign-in will occur when clipboard hotkey is invoked
        Console.WriteLine("1Password manager configured successfully.");
    }

    /// <summary>
    /// Gets the first available vault, handling authentication if needed.
    /// </summary>
    /// <returns>The first vault if available, null if authentication fails or no vaults found</returns>
    private static Vault? GetAvailableVault()
    {
        try
        {
            if (onePasswordManager == null)
                return null;

            // Try fetching vaults; sign in on-demand if none available
            var vaults = onePasswordManager.GetVaults();
            if (vaults == null || !vaults.Any())
            {
                Console.WriteLine("No vaults found. Signing in on-demand...");
                onePasswordManager.SignIn();
                vaults = onePasswordManager.GetVaults();
                if (vaults == null || !vaults.Any())
                {
                    Console.WriteLine("No vaults available even after sign-in.");
                    return null;
                }
            }

            var vault = vaults.First();
            Console.WriteLine($"Using vault: {vault.Name}");
            return vault;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error accessing vaults: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Finds an executable in the system PATH environment variable.
    /// </summary>
    /// <param name="executableName">Name of the executable to find (e.g., "op.exe" or "op")</param>
    /// <returns>Full path to the executable if found, null otherwise</returns>
    private static string? FindExecutableInPath(string executableName)
    {
        var pathVariable = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrEmpty(pathVariable))
            return null;

        var paths = pathVariable.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

        foreach (var path in paths)
        {
            try
            {
                var fullPath = Path.Combine(path.Trim(), executableName);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
            catch (Exception)
            {
                // Skip invalid paths
                continue;
            }
        }

        return null;
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
            if (onePasswordManager == null)
            {
                ShowNotification("1Password not initialized", ToolTipIcon.Error);
                return;
            }
            // Sign in on-demand before saving clipboard content
            // Get first vault, if it fails, signin: 
            var vault = GetAvailableVault();
            if (vault == null)
            {
                Console.WriteLine("No vaults available, signing in to 1Password...");
                onePasswordManager.SignIn();
                vault = GetAvailableVault();
            }
            else
            {
                Console.WriteLine($"Using vault: {vault.Name}");
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
    }


    private static (bool Success, string Error) UpdateSecureNote(string content)
    {
        try
        {
            Console.WriteLine("Updating secure note...");

            if (onePasswordManager == null)
                return (false, "1Password manager not initialized");

            // Get an available vault (this will handle authentication if needed)
            var vault = GetAvailableVault();
            if (vault == null)
                return (false, "No vault available or authentication failed. Please sign in to 1Password.");

            // Try to find existing item
            var existingItems = onePasswordManager.GetItems(vault);
            var existingItem = existingItems?.FirstOrDefault(x => x.Title == OnePasswordItemName);

            if (existingItem != null)
            {
                Console.WriteLine("Item exists, updating...");

                // Get the full item details
                var fullItem = onePasswordManager.GetItem(existingItem.Id, vault.Id);
                if (fullItem != null)
                {
                    // Update the notes field - look for any field that can hold notes
                    var notesField = fullItem.Fields?.FirstOrDefault(f =>
                        f.Id == "notesPlain" ||
                        f.Label?.ToLower().Contains("notes") == true ||
                        f.Label?.ToLower().Contains("note") == true);

                    if (notesField != null)
                    {
                        notesField.Value = content;
                    }
                    else
                    {
                        // Find the first writable text field
                        var textField = fullItem.Fields?.FirstOrDefault(f => f.Value != null);
                        if (textField != null)
                        {
                            textField.Value = content;
                        }
                    }

                    onePasswordManager.EditItem(fullItem, vault);
                    Console.WriteLine("Update completed successfully");
                    return (true, "");
                }
                else
                {
                    return (false, "Failed to retrieve full item details");
                }
            }
            else
            {
                Console.WriteLine("Item doesn't exist, creating new one...");
                return CreateSecureNote(content, vault);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating secure note: {ex.Message}");
            return (false, ex.Message);
        }
    }

    private static (bool Success, string Error) CreateSecureNote(string content, Vault vault)
    {
        try
        {
            Console.WriteLine("Creating new secure note...");

            if (onePasswordManager == null)
                return (false, "1Password manager not initialized");

            // Get the secure note template
            var template = onePasswordManager.GetTemplate(Category.SecureNote);
            if (template == null)
                return (false, "Failed to get secure note template");

            // Set the title
            template.Title = OnePasswordItemName;

            // Set the notes content - look for existing notes field
            var notesField = template.Fields?.FirstOrDefault(f => f.Id == "notesPlain" || f.Label?.ToLower().Contains("notes") == true);
            if (notesField != null)
            {
                notesField.Value = content;
            }
            else
            {
                // If no notes field exists, try to find any writable field
                var textField = template.Fields?.FirstOrDefault(f => f.Value != null);
                if (textField != null)
                {
                    textField.Value = content;
                }
            }

            // Create the item
            var createdItem = onePasswordManager.CreateItem(template, vault);
            if (createdItem != null)
            {
                Console.WriteLine("Create completed successfully");
                return (true, "");
            }
            else
            {
                return (false, "Failed to create item");
            }
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
