using Agario;
using Microsoft.Extensions.Logging;
using Model;
using NetworkingNS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace ViewController
{
    public partial class view : Form
    {
        private static Preserved_Socket_State server;
        private string username;
        private World world;
        private int playerID;
        private string moveCommands = $"(move,1,1)";
        private string splitCommands = $"";
        private ILogger logger;
        public delegate void ServerUpdateHandler();
        private event ServerUpdateHandler DataArrived;
        public int sX;
        public int sY;
        private int fpsCount;
        private int HBcount;
        private int MScount;
        private DateTime time;

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

            playfield.Zoom(sX, sY);
            Invalidate(true);
        }

        private bool splitting = false;
        private void Playfield_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
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
                logger.LogInformation(moveCommands);
            }

            try
            {
                MethodInvoker m = new MethodInvoker(() => { Invalidate(true); });

                Invoke(m);
                CalculateFPSAndMS();

                m = new MethodInvoker(updateStatusBox);
                Invoke(m);

            }
            catch { }
        }
        private void CalculateFPSAndMS()
        {
            fpsCount = (int)(playfield.rendered / ((DateTime.Now - time).TotalSeconds));
            MScount = (int)(HBcount / ((DateTime.Now - time).TotalSeconds));

        }
        private void updateStatusBox()
        {
            world.Players.TryGetValue(playerID, out Circle c);
            if (!username.Equals("admin"))
            {
                this.mass.Text = "MASS: " + (int)c.MASS;
                this.position.Text = $"Loc: X: {(int)c.LOC.X} Y: {(int)c.LOC.Y}";
                this.ms.Text = $"MS: {MScount}";

                this.fps.Text = $"FPS: {(int)(fpsCount)}";
            }
            else
            {
                this.mass.Text = "MASS: " + "admin";
                this.position.Text = $"Loc: X: admin";
            }
        }
        private void ConnectToServer(string ip, string name)
        {
            playfield.BackColor = Color.White;
            Networking.Connect_to_Server(OnConnect, ip);

        }

        public void OnConnect(Preserved_Socket_State state)
        {
            if (!state.socket.Connected)
            {
                MethodInvoker m = new MethodInvoker(()=> connectbutton.Enabled = true);
                Invoke(m);
                state = null;
                return;
            }


            if (state.error_occured)
            {
                connectbutton.Enabled = true;
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
            else if (player.NAME == "Admin")
            {
                playerID = player.ID;
                playfield.playerID = playerID;
                world.Players.Add(player.ID, player);
            }

            time = DateTime.Now;
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
                            lock (world)
                            {
                                world.Food.Remove(circle.ID);
                                world.Food.Add(circle.ID, circle);

                            }
                        }
                        lock (world)
                        {
                            world.Food.Add(circle.ID, circle);
                        }
                        break;

                    case 1:
                        if (world.Players.TryGetValue(circle.ID, out newCircle))
                        {
                            lock (world)
                            {
                                world.Players.Remove(circle.ID);
                                world.Players.Add(circle.ID, circle);
                            }
                        }
                        else
                        {
                            lock (world)
                            {
                                world.Players.Add(circle.ID, circle);
                            }
                        }

                        // logger?.LogInformation("Logged: " + circle.NAME + " " + circle.LOC.ToString());
                        break;
                    case 2:
                        logger.LogInformation("HEARTBEAT");
                        HBcount++;
                        DataArrived();
                        break;
                    case 3:
                        lock (world)
                        {
                            world.Players.Add(circle.ID, circle);
                        }
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


            if (!(server is null) && !(world.Players is null))
            {
                if (e.X <= (playfield.Size.Width / 2))
                {
                    mouseX = x - 500;
                }
                else
                {
                    mouseX = x + 500;
                }
                if (e.Y <= (playfield.Size.Height / 2))
                {
                    mouseY = y - 500;
                }
                else
                {
                    mouseY = y + 500;
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
