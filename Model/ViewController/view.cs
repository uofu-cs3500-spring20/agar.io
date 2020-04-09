using Agario;
using Microsoft.Extensions.Logging;
using Model;
using NetworkingNS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ViewController
{
    public partial class view : Form
    {
        private static Preserved_Socket_State server;
        private string username;
        private World world;
        private bool failedConnect;
        private string failedMsg;
        private int playerID;
        private string moveCommands = $"(move,1,1)";
        private string splitCommands = $"";
        private ILogger logger;
        public delegate void ServerUpdateHandler();
        private event ServerUpdateHandler DataArrived;
        public int sX;
        public int sY;

        public view(ILogger logger)
        {

            this.logger = logger;
            world = new World();
            world.Players = new Dictionary<int, Circle>();
            world.Food = new Dictionary<int, Circle>();
            InitializeComponent();
            RegisterServerUpdate(Frame);
            playfield = new Playfield(world);
            this.playfield.Location = new Point(10, 70);
            this.playfield.Size = new Size(804, 804);
            this.playfield.MouseMove += new MouseEventHandler(On_Move);
            this.playfield.PreviewKeyDown += Playfield_PreviewKeyDown;
            this.playfield.MouseWheel += Playfield_MouseWheel;
            this.playfield.BorderStyle = BorderStyle.Fixed3D;
            this.Controls.Add(playfield);
            this.playfield.Focus();

        }

        private void Playfield_MouseWheel(object sender, MouseEventArgs e)
        {
            sX = e.Delta;
            sY = e.Delta;
       
                playfield.Zoom(sX,sY);
                Invalidate(true);
                   }

        private bool splitting = false;
        private void Playfield_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if(e.KeyCode == Keys.Space)
            {
                splitting = true;
                splitCommands = $"(split,{mouseX},{mouseY})";
            }
        }

        private void Frame()
        {

            if (moveCommands.Length > 0)
            {

                Networking.Send(server.socket, moveCommands.ToString());
                if (splitting)
                {
                    Networking.Send(server.socket, splitCommands.ToString());
                    splitting = false;
                }
                moveCommands = $"(move,{mouseX},{mouseY})";


            }

            try
            {
                MethodInvoker m = new MethodInvoker(() => { Invalidate(true); });
                Invoke(m);

                m = new MethodInvoker(updateStatusBox);

                Invoke(m);

            }
            catch { }
        }
        private void updateStatusBox()
        {
            world.Players.TryGetValue(playerID, out Circle c);

            this.mass.Text = "MASS: " + (int)c.MASS;
            this.position.Text = $"Loc: X: {(int)c.LOC.X} Y: {(int)c.LOC.Y}";
        }
        private void ConnectToServer(string ip, string name)
        {
            playfield.BackColor = Color.White;
            Networking.Connect_to_Server(OnConnect, ip);
            Networking.Connect_to_Server(OnConnect, ip);

        }

        public void OnConnect(Preserved_Socket_State state)
        {


            if (state.error_occured)
            {
                failedConnect = true;
                failedMsg = state.error_message;
                return;
            }
            server = state;
            server.on_data_received_handler = Startup;
            Networking.Send(server.socket, username + '\n');
            Networking.await_more_data(server);
        }

        public void Startup(Preserved_Socket_State state)
        {
            server.on_data_received_handler = DataReceived;
            MethodInvoker m = new MethodInvoker(() => this.playfield.Focus());
            Invoke(m);

           
            string startupmessage = server.Message;
            Circle player = JsonConvert.DeserializeObject<Circle>(startupmessage);
            if (player.NAME == username)
            {
                playerID = player.ID;
                playfield.playerID = playerID;
                world.Players.Add(player.ID, player);
            }

      
               Networking.await_more_data(server);
        }



        private void DataReceived(Preserved_Socket_State state)
        {


            Circle circle = new Circle();
            try
            {
                circle = JsonConvert.DeserializeObject<Circle>(state.Message);

                switch (circle.TYPE)
                {
                    case 0:
                        if (world.Food.TryGetValue(circle.ID, out Circle newCircle))
                        {

                            world.Food.Remove(circle.ID);
                            world.Food.Add(circle.ID, circle);


                            newCircle = circle;
                        }

                        world.Food.Add(circle.ID, circle);

                        break;

                    case 1:
                        if (world.Players.TryGetValue(circle.ID, out newCircle))
                        {
                            world.Players.Remove(circle.ID);
                            world.Players.Add(circle.ID, circle);
                        }
                        else
                        {
                            world.Players.Add(circle.ID, circle);
                        }

                        logger?.LogInformation("Logged: " + circle.NAME + " " + circle.LOC.ToString());
                        break;
                    case 2:
                        logger.LogInformation("HEARTBEAT");
                        DataArrived();
                        break;
                }
            }
            catch (Exception e) { logger.LogError("Incorrect Format for circle: " + e.Message); }


            if (!server.Has_More_Data())
            {
                Networking.await_more_data(server);

            }
        }





        public void RegisterServerUpdate(ServerUpdateHandler handler)
        {
            DataArrived += handler;
        }
        float mouseX = 0;
        float mouseY = 0;
        int x = 0;
        int y = 0;
        internal static int scaleX;
        internal static readonly int scaleY;

        private void On_Move(object sender, MouseEventArgs e)
        {

            if (world.Players.TryGetValue((int)playerID, out Circle player))
            {
                x = (int)player.LOC.X;
                y = (int)player.LOC.Y;
                CalculateMove(player, e);
            }
            


        }

        private void CalculateMove(Circle c, MouseEventArgs e)
        {
            double moveSpeed = c.MASS / 80;

            if (e.X != 0 && e.Y != 0 && !(server is null) && !(world.Players is null))
            {
                if (e.X < (playfield.Size.Width / 2))
                {
                    mouseX = -(200-(int)moveSpeed);
                }
                else
                {
                    mouseX = (200-(int)moveSpeed);
                }
                if (e.Y < (playfield.Size.Height / 2))
                {
                    mouseY = -(200-(int)moveSpeed);
                }
                else
                {
                    mouseY = (200-(int)moveSpeed);
                }


                if (y + mouseY == world.WORLDSIZE)
                {
                    mouseY -= 59;
                }
                else if (y - mouseY == 0)
                {
                    mouseY += 59;

                }
                if (x + mouseX == world.WORLDSIZE)
                {
                    mouseX -= 59;
                }
                else if (x - mouseX == 0)
                {
                    mouseX += 59;
                }

            }
        }

        private void ConnectButton_Click(object sender, MouseEventArgs e)
        {
            username = name.Text;
            ConnectToServer(ipaddress.Text, name.Text + '\n');
            connectbutton.Enabled = false;

        }

    }
}
