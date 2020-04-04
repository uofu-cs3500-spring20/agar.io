using System;
using System.Drawing;
using System.Windows.Forms;

namespace ViewController
{
    partial class view
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        public Button connectbutton;

        Label ipaddresslabel;
        TextBox ipaddress;

        Label portlabel;
        TextBox port;

        Label namelabel;
        TextBox name;

        Label fps;

        Label ms;

        Label food;

        Label mass;

        Label position;

        Panel drawingpanel;

        GroupBox status;
        GroupBox box;
        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

            this.ipaddress = new TextBox();
            this.ipaddresslabel = new Label();
            this.portlabel = new Label();
            this.port = new TextBox();
            this.name = new TextBox();
            this.namelabel = new Label();
            this.fps = new Label();
            this.ms = new Label();
            this.mass = new Label();
            this.position = new Label();
            this.drawingpanel = new Panel();
            this.status = new GroupBox();
            this.box = new GroupBox();
            this.connectbutton = new Button()
            {
                Text = "Connect",
                Size = new System.Drawing.Size(70,30)
            };
            connectbutton.Location = new System.Drawing.Point(50,25);
            this.box.Controls.Add(connectbutton);

            box.FlatStyle = FlatStyle.Popup;
            
            //Add groups to control
            Controls.Add(box);
            Controls.Add(status);
            Controls.Add(drawingpanel);

            //Add items to groups

            //IPADDRESS
            box.Controls.Add(ipaddress);
            ipaddress.Size = new System.Drawing.Size(100,70);
            ipaddress.Text = "127.0.0.1";
            ipaddress.Location = new System.Drawing.Point(130,31);
            this.ipaddress.TextChanged += new EventHandler(IPAddress_TextChanged);
            box.Controls.Add(ipaddresslabel);


            //NAME
            box.Controls.Add(name);

            name.Size = new System.Drawing.Size(100, 70);
            name.Location = new System.Drawing.Point(450, 31);
            box.Controls.Add(namelabel);



            status.Controls.Add(fps);

            fps.Location = new Point(750,100);
            fps.Size = new Size(100,100);
            fps.Text = "FPS: ";
            fps.Visible = true;
            fps.ForeColor = Color.Red;
            fps.Show();
            status.Controls.Add(ms);
            status.Controls.Add(mass);


            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 900);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "agar.io";
            this.status.SuspendLayout();
            this.box.SuspendLayout();
            this.SuspendLayout();



            this.box.Location = new System.Drawing.Point(10, 0);
            this.box.Name = "box";
            this.box.Size = new System.Drawing.Size(980, 70);
            this.box.TabIndex = 0;
            this.box.TabStop = false;
            connectbutton.Enabled = true;
            this.connectbutton.MouseClick += new MouseEventHandler(ConnectButton_Click);


            this.status.Location = new System.Drawing.Point(700,60);
            this.status.Size = new System.Drawing.Size(290,900);


            this.drawingpanel.Location = new System.Drawing.Point(10,70);
            this.drawingpanel.Size = new System.Drawing.Size(690,800);
            this.drawingpanel.BackColor = Color.Black;
            
        }

     







        #endregion



    }
}

