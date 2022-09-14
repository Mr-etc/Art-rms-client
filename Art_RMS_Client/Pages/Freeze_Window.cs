using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Art_RMS_Client.Pages
{
    public partial class Freeze_Window : Form
    {
        public Freeze_Window()
        {
            InitializeComponent();
        }
        public bool Close = true;
        private void Freeze_Window_Load(object sender, EventArgs e)
        {

        }

        private void Freeze_Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = Close;
        }
    }
}
