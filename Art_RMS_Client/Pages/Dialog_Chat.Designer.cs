namespace Art_RMS_Client.Pages
{
    partial class Dialog_Chat
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Send_btn = new System.Windows.Forms.Button();
            this.Textbox_Message = new System.Windows.Forms.TextBox();
            this.Chat_Table = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // Send_btn
            // 
            this.Send_btn.Location = new System.Drawing.Point(280, 294);
            this.Send_btn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Send_btn.Name = "Send_btn";
            this.Send_btn.Size = new System.Drawing.Size(93, 28);
            this.Send_btn.TabIndex = 5;
            this.Send_btn.Text = "Send";
            this.Send_btn.UseVisualStyleBackColor = true;
            this.Send_btn.Click += new System.EventHandler(this.Send_btn_Click);
            // 
            // Textbox_Message
            // 
            this.Textbox_Message.Location = new System.Drawing.Point(3, 297);
            this.Textbox_Message.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Textbox_Message.Name = "Textbox_Message";
            this.Textbox_Message.Size = new System.Drawing.Size(268, 22);
            this.Textbox_Message.TabIndex = 4;
            this.Textbox_Message.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Textbox_Message_KeyDown);
            // 
            // Chat_Table
            // 
            this.Chat_Table.Location = new System.Drawing.Point(0, 0);
            this.Chat_Table.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Chat_Table.Name = "Chat_Table";
            this.Chat_Table.ReadOnly = true;
            this.Chat_Table.Size = new System.Drawing.Size(380, 286);
            this.Chat_Table.TabIndex = 6;
            this.Chat_Table.Text = "";
            // 
            // Dialog_Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 327);
            this.Controls.Add(this.Chat_Table);
            this.Controls.Add(this.Send_btn);
            this.Controls.Add(this.Textbox_Message);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Dialog_Chat";
            this.Text = "Dialog_Chat";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Dialog_Chat_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Send_btn;
        private System.Windows.Forms.TextBox Textbox_Message;
        public System.Windows.Forms.RichTextBox Chat_Table;
    }
}