using Agario;
using Microsoft.Extensions.Logging;
using Model;
using NetworkUtil;
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
        private static SocketState server;
        private string username;
        private World world;
        private bool failedConnect;
        private string failedMsg;
        private float playerID;
        private StringBuilder commandlist;
        private ILogger logger;
        public delegate void ServerUpdateHandler();
        private event ServerUpdateHandler DataArrived;
        private Circle player;
        public view(ILogger logger)
        {
            this.logger = logger;
            commandlist = new StringBuilder();
            world = new World();
            InitializeComponent();
            RegisterServerUpdate(Frame);
            playfield = new Playfield(world);
            this.playfield.Location = new Point(10, 70);
            this.playfield.Size = new Size(804, 804);
            this.playfield.MouseMove += new MouseEventHandler(On_Move);

            this.playfield.BorderStyle = BorderStyle.Fixed3D;
            this.Controls.Add(playfield);
            this.playfield.Focus();

        }

        private void Frame()
        {

            try
            {
                MethodInvoker m = new MethodInvoker(() => {Invalidate(true); });
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
            Networking.ConnectToServer(OnConnect, ip, 11000);
           

        }

        public void OnConnect(SocketState state)
        {


            if (state.ErrorOccured)
            {
                failedConnect = true;
                failedMsg = state.ErrorMessage;
                return;
            }
            server = state;
            server.CallMe = AwaitData;

            Networking.Send(server.TheSocket, username + '\n');
            Networking.GetData(server);
        }

        private void AwaitData(SocketState state)
        {

            string totalData = server.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            Dictionary<int, Circle> players = new Dictionary<int, Circle>();
            Dictionary<int, Circle> food = new Dictionary<int, Circle>();
            Dictionary<int, Circle> circles = new Dictionary<int, Circle>();

            try
            {
                //Eliminate race conditions
                lock (world)
                {

                    //Here's where we need to differentiate the different json objects into their own lists.
                    foreach (string message in parts)
                    {

                        // Ignore empty strings added by the regex splitter
                        if (message.Length == 0)
                            continue;
                        // The regex splitter will include the last string even if it doesn't end with a '\n',
                        // So we need to ignore it if this happens. 
                        if (message[message.Length - 1] != '\n')
                            break;

                        //Begin deserializing the server's messages, while adding them to lists to be passed along
                        Circle circle = JsonConvert.DeserializeObject<Circle>(message);

                        if (!(circle is null))
                        {
                            if (circle.TYPE == 0)
                            {
                                food?.Add(circle.ID, circle);
                            }
                            else if (circle.TYPE == 1)
                            {
                                circle.ISPLAYER = true;
                                players?.Add(circle.ID, circle);
                                if (circle.NAME.Equals(username))
                                {
                                    logger.LogInformation($"Movement: {circle.LOC.X}, {circle.LOC.Y}");
                                    playerID = circle.ID;
                                    player = circle;
                                    playfield.playerID = circle.ID;
                                    playfield.username = circle.NAME;
                                }
                            }else if(circle.TYPE == 2)
                            {
                                circles.Add(circle.ID,circle);
                            }

                        }

                    }

                }
            }
            catch { }
            //Pass all of the lists to the world to process further
            world.Players = players;
            world.Food = food;
            Networking.GetData(server);
            DataArrived();

        }
        public void RegisterServerUpdate(ServerUpdateHandler handler)
        {
            DataArrived += handler;
        }


        public bool HasMoreData(SocketState state)
        {
            return false;
        }

     
        private void On_Move(object sender, MouseEventArgs e)
        {
            if (e.X != 0 && e.Y != 0 && !(server is null) && !(world.Players is null))
            {
                float mouseX = e.X;
                float mouseY = e.Y;
                world.Players.TryGetValue((int)playerID, out Circle player);
                int x = (int)player.LOC.X;
                int y = (int)player.LOC.Y;


                string message = $"(move,{(int)1},{(int)1})";
                Networking.Send(server.TheSocket,message);
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
