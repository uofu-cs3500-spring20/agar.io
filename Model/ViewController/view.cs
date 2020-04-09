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
        private string commandlist = $"(move,1,1)";
        private ILogger logger;
        public delegate void ServerUpdateHandler();
        private event ServerUpdateHandler DataArrived;
        private Circle player;

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
            // this.playfield.MouseHover += new EventHandler(On_Move);
            this.playfield.BorderStyle = BorderStyle.Fixed3D;
            this.Controls.Add(playfield);
            this.playfield.Focus();

        }

        private void Frame()
        {

            if (commandlist.Length > 0)
            {

                Networking.Send(server.socket, commandlist.ToString());

                commandlist = $"(move,{mouseX},{mouseY})";
            }

            try
            {
                MethodInvoker m = new MethodInvoker(() => { Invalidate(true); });
                Invoke(m);
            }
            catch { }
        }

        public World GetWorld()
        {
            return null;
        }

        private void ConnectToServer(string ip, string name)
        {
            playfield.BackColor = Color.White;
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
            string startupmessage = server.Message;
            Circle player = JsonConvert.DeserializeObject<Circle>(startupmessage);
            if (player.NAME == username)
            {
                playerID = player.ID;
                playfield.playerID = playerID;
                world.Players.Add(player.ID, player);
            }

            if (!server.Has_More_Data())
                Networking.await_more_data(server);
        }



        Dictionary<int, Circle> Additions = new Dictionary<int, Circle>();


        private void DataReceived(Preserved_Socket_State state)
        {


            Circle circle = new Circle();
            try
            {
                circle = JsonConvert.DeserializeObject<Circle>(state.Message);
                if(Additions.TryGetValue(circle.ID, out Circle newCircle))
                {
                    circle = newCircle;
                }
                else
                {
                    Additions.Add(circle.ID, circle);
                }

            }
            catch (Exception e) { logger.LogError("Incorrect Format for circle: " + e.Message); }

            logger.LogInformation($"{Additions.Count}");

            if (!server.Has_More_Data())
            {
                UpdateGui();
                Networking.await_more_data(server);

            }
        }


        public void UpdateGui()
        {
            Dictionary<int, Circle> playersAddition = new Dictionary<int, Circle>();
            Dictionary<int, Circle> foodAddition = new Dictionary<int, Circle>();


            lock (world)
            {
                foreach (Circle circle in Additions.Values)
                {
                    switch (circle.TYPE)
                    {
                        case 0:
                            if (!foodAddition.ContainsKey(circle.ID))
                            {
                                foodAddition.Add(circle.ID, circle);
                                //logger.LogInformation(foodAddition.Count + "");
                                if (circle.MASS == 0)
                                    logger.LogInformation("EMPTY FOOD: ++++");
                            }
                            break;

                        case 1:
                            if (!playersAddition.ContainsKey(circle.ID))
                                playersAddition.Add(circle.ID, circle);
                            logger.LogInformation("Logged: " + circle.NAME + " " + circle.LOC.ToString());
                            break;
                        case 2:
                            logger.LogInformation("HEARTBEAT");


                            world.Food = foodAddition;
                            world.Players = playersAddition;
                            DataArrived();
                            break;
                    }
                }
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

            }
            if (e.X != 0 && e.Y != 0 && !(server is null) && !(world.Players is null))
            {
                if (e.X < 460)
                {
                    mouseX = -100;
                }
                else
                {
                    mouseX = 100;
                }
                if (e.Y < 460)
                {
                    mouseY = -100;
                }
                else
                {
                    mouseY = 100;
                }

            }


        }

        private void ConnectButton_Click(object sender, MouseEventArgs e)
        {
            username = name.Text;
            ConnectToServer(ipaddress.Text, name.Text + '\n');
            connectbutton.Enabled = false;

        }

        private void IPAddress_TextChanged(object sender, EventArgs e)
        {
            // throw new NotImplementedException();
        }


    }
}
