using System;
using System.Drawing;
using System.Drawing.Text;
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

        Playfield playfield;

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
            Controls.Add(playfield);

            //Add items to groups

            //IPADDRESS
            box.Controls.Add(ipaddress);
            ipaddress.Size = new System.Drawing.Size(100,70);
            ipaddress.Text = "127.0.0.1";
            ipaddress.Location = new System.Drawing.Point(130,31);
            box.Controls.Add(ipaddresslabel);


            //NAME
            box.Controls.Add(name);

            name.Size = new System.Drawing.Size(100, 70);
            name.Location = new System.Drawing.Point(450, 31);
            box.Controls.Add(namelabel);

            this.status.Location = new System.Drawing.Point(810, 60);
            this.status.Size = new System.Drawing.Size(180, 900);
            status.Controls.Add(ms);
            status.Controls.Add(mass);
            status.Controls.Add(fps);
            status.Controls.Add(position);

            Font statusFont = new Font(FontFamily.GenericSansSerif,10);
            fps.Font = statusFont;
            fps.Location = new Point(30,30);
            fps.Size = new Size(100,20);
            fps.Text = "FPS: ";
            fps.Visible = true;

            ms.Font = statusFont;
            ms.Location = new Point(30, 50);
            ms.Size = new Size(100, 20);
            ms.Text = "Ms: ";
            ms.Visible = true;

            mass.Font = statusFont;
            mass.Location = new Point(30, 70);
            mass.Size = new Size(200, 20);
            mass.Text = "Mass: ";
            mass.Visible = true;

            position.Font = statusFont;
            position.Location = new Point(20, 90);
            position.Size = new Size(200, 20);
            position.Text = "Loc: X:0 Y:0 ";
            position.Visible = true;


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


        
            

           

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.DoubleBuffered = true;
            
        }

     









        #endregion



    }
}

