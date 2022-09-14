using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;

namespace Art_RMS_Client.Pages
{
    public partial class Dialog_Chat : Form
    {
        public bool Check_Cansel = true;
        public Dialog_Chat()
        {
            InitializeComponent();
        }
        private void Send_btn_Click(object sender, EventArgs e)
        {
            Thread.Sleep(500);
            Functions.Send("SEND_MESSAGE_CHAT|" + Textbox_Message.Text);
            Chat_Table.AppendText($"Вы: {Textbox_Message.Text} \n");
            Textbox_Message.Text = "";
        }

        public void Received(List<Array> Result)
        {
            Invoke(new ThreadStart(() =>
            {
                switch (Encoding.UTF8.GetString((byte[])Result[1]))
                {
                    case "SEND_MESSAGE_CHAT":
                        Chat_Table.AppendText($"Interlocutor: {Encoding.UTF8.GetString((byte[])Result[2])} \n");
                        break;
                    case "CLOSE_CHAT":
                        Check_Cansel = false;
                        this.Close();
                        break;
                }
            }));
        }

        private void Dialog_Chat_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Listener.Client.Connected != true)
                e.Cancel = false;
            else
                e.Cancel = Check_Cansel;

            if (e.Cancel == false)
            {
                foreach (Form frm in Listener.Forms)
                {
                    if (frm.Text == "Dialog_Chat")
                    {
                        Listener.Forms.Remove(frm);
                        break;
                    }
                }
            }
        }

        public void Close_Form()
        {
            Invoke(new ThreadStart(() =>
            {
                this.Close();
            }));
        }

        private void Textbox_Message_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Send_btn_Click(null, null);

        }
    }
}
