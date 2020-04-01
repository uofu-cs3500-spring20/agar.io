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
            box.FlatStyle = FlatStyle.Popup;
            
            //Add groups to control
            Controls.Add(box);
            Controls.Add(status);
            Controls.Add(drawingpanel);

            //Add items to groups
            box.Controls.Add(ipaddress);
            box.Controls.Add(ipaddresslabel);
            box.Controls.Add(port);
            box.Controls.Add(portlabel);
            box.Controls.Add(name);
            box.Controls.Add(namelabel);

            status.Controls.Add(fps);
            status.Controls.Add(ms);
            status.Controls.Add(mass);


            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 900);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "agar.io";
            this.box.SuspendLayout();
            this.SuspendLayout();



            this.box.Controls.Add(connectbutton);
            this.box.Location = new System.Drawing.Point(10, 0);
            this.box.Name = "box";
            this.box.Size = new System.Drawing.Size(980, 70);
            this.box.TabIndex = 0;
            this.box.TabStop = false;
            connectbutton.Enabled = true;



            this.status.Location = new System.Drawing.Point(900,400);
        }
     
        
        
        #endregion



    }
}

