/// <summary> 
/// Author:    Gabriel Job && CS 3500 staff 
/// Partner:   N/A
/// Date:      4/13/20
/// Course:    CS 3500, University of Utah, School of Computing 
/// Copyright: CS 3500 and Gabe - This work may not be copied for use in Academic Coursework. 
/// 
/// I, Gabe, certify that I wrote this code from scratch and did not copy it in part or whole from  
/// another source.  All references used in the completion of the assignment are cited in my README file. 
/// </summary>
/// 
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
using System.Media;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace ViewController
{
    /// <summary>
    /// Class representing the player's view, while also handling all controlling of said view.
    /// </summary>
    public partial class view : Form
    {
        //Base objects
        private ILogger logger;
        public delegate void ServerUpdateHandler();
        private event ServerUpdateHandler DataArrived;
        private static Preserved_Socket_State server;
        private string username;
        private int playerID;
        private World world;
       
        //Player commands
        private string moveCommands = $"(move,1,1)";
        private string splitCommands = $"";
       
        //Player movement calculations.
        public int zoomX;
        public int zoomY;
        private int fpsCount;
        private int HBcount;
        private int MScount;
        private int eatenFood;
        private bool splitting = false;
        float mouseX = 0;
        float mouseY = 0;
        int playerX = 0;
        int playerY = 0;
        private DateTime time;

       /// <summary>
       /// Initial creation of our player's GUI
       /// </summary>
       /// <param name="logger">The logging object to use</param>
       public view(ILogger logger)
        {
            this.logger = logger;
            world = new World
            {
                Players = new Dictionary<int, Circle>(),
                Food = new Dictionary<int, Circle>()
            };
            InitializeComponent();
            RegisterServerUpdate(Frame);
            BuildPlayField();
        }

       /// <summary>
       /// Helper method to abstract creation of our playfield.
       /// </summary>
       private void BuildPlayField()
        {
            playfield = new Playfield(world);
            this.playfield.Location = new Point(10, 70);
            this.playfield.Size = new Size(804, 804);
            this.playfield.MouseMove += new MouseEventHandler(On_Move);
            this.playfield.PreviewKeyDown += Playfield_PreviewKeyDown;
            this.playfield.Death += Playfield_death;
            this.playfield.MouseWheel += Playfield_MouseWheel;
            this.playfield.BorderStyle = BorderStyle.Fixed3D;
            this.Controls.Add(playfield);
            this.playfield.Focus();
        }

       /// <summary>
       /// Method subscribed to playfield Death event.
       /// </summary>
       private void Playfield_death()
        {
            HandleDisconnect();
        }

       /// <summary>
       /// Simple method to detect player zooming.
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
       private void Playfield_MouseWheel(object sender, MouseEventArgs e)
        {
            zoomX = e.Delta;
            zoomY = e.Delta;
            playfield.Zoom(zoomX, zoomY);
            Invalidate(true);
        }

        
       /// <summary>
       /// Simple method to detect space presses
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
       private void Playfield_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                splitting = true;
                splitCommands = $"(split,{mouseX},{mouseY})";
            }
        }

       /// <summary>
       /// Where the magic happens...
       /// 
       /// Method to send invalidate to our playfield.
       /// </summary>
       private void Frame()
        {
            if (moveCommands.Length > 0 && username != "Admin" && username != "admin")
            {
                Networking.Send(server.socket, moveCommands.ToString());

                //Don't spam the server with splitting messages
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
                //First update playfield
                MethodInvoker m = new MethodInvoker(() => { Invalidate(true); });
                Invoke(m);

                //Now update status box
                CalculateFPSAndMS();
                m = new MethodInvoker(updateStatusBox);
                Invoke(m);

            }
            catch { }
        }


        /// <summary>
        /// Simple helper method to abstract calculation of FPS and MS
        /// </summary>
        private void CalculateFPSAndMS()
        {
            //FPS = Frames/(Time now)-(Start time)
            fpsCount = (int)(playfield.rendered / ((DateTime.Now - time).TotalSeconds));
            //MS = Heartbeats/(Time now)- (Start time)
            MScount = (int)(HBcount / ((DateTime.Now - time).TotalSeconds));
        }
        /// <summary>
        /// Helper method to handle all status changes
        /// </summary>
        private void updateStatusBox()
        {
            world.Players.TryGetValue(playerID, out Circle c);
            if (!username.Equals("admin") && (!username.Equals("Admin")))
            {
                this.mass.Text = "MASS: " + (int)c.MASS;
                this.position.Text = $"Loc: X: {(int)c.LOC.X} Y: {(int)c.LOC.Y}";
                this.ms.Text = $"MS: {MScount}";
                this.food.Text = $"Food: {eatenFood}";
                this.fps.Text = $"FPS: {(int)(fpsCount)}";
            }
            else
            {
                this.mass.Text = "MASS: admin";
                this.position.Text = $"Loc: X: admin";
            }
        }
       
        /// <summary>
        /// Helper method called from clicking connect.
        /// </summary>
        /// <param name="ip">The server to connect to</param>
        /// <param name="name">Username</param>
        private void ConnectToServer(string ip, string name)
        {

            playfield.BackColor = Color.FromArgb(209, 245, 255);
            Networking.Connect_to_Server(OnConnect, ip);
        }

        /// <summary>
        /// Method to handle initial connection to server
        /// </summary>
        /// <param name="state">Client connection</param>
        public void OnConnect(Preserved_Socket_State state)
        {
            //First handle error conditions
            if (!state.socket.Connected)
            {
                MethodInvoker m = new MethodInvoker(() =>
                {
                    DialogResult failedToConnect = MessageBox.Show($"Failed to connect to {ipaddress.Text}, please check the address and try again.", "agar.io Helper", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    connectbutton.Enabled = true;
                });
                Invoke(m);
                state = null;
                return;
            }
            if (state.error_occured)
            {
                connectbutton.Enabled = true;
                MethodInvoker m = new MethodInvoker(() =>
                {
                    DialogResult failedToConnect = MessageBox.Show($"Failed to connect to {ipaddress.Text}, please check the address and try again.", "agar.io Helper", MessageBoxButtons.OK);
                    connectbutton.Enabled = true;
                });
                Invoke(m);
                return;
            }

            //If everything went well, begin the server loop.
            server = state;
            server.on_data_received_handler = Startup;
            Networking.Send(server.socket, username + '\n');
            Networking.await_more_data(server);
        }

        /// <summary>
        /// Initial receive of player data to setup the world.
        /// </summary>
        /// <param name="state">Client connection</param>
        public void Startup(Preserved_Socket_State state)
        { 
            server.on_data_received_handler = DataReceived;
            string startupmessage = server.Message;

            //Check if our admin has connected
            Circle player = JsonConvert.DeserializeObject<Circle>(startupmessage);
            if (player.NAME == username)
            {
                playerID = player.ID;
                playfield.playerID = playerID;
                world.Players.Add(player.ID, player);
            }
            else if (player.NAME == "Admin" || (player.NAME == "admin"))
            {
                playerID = player.ID;
                playfield.playerID = playerID;
                world.Players.Add(player.ID, player);
            }

            //Misc Setup
            eatenFood = 0;
            time = DateTime.Now;
            MethodInvoker m = new MethodInvoker(() => this.playfield.Focus());
            Invoke(m);
            Networking.await_more_data(server);
        }



        private void DataReceived(Preserved_Socket_State state)
        {
            //Check if we're connected
            if (Disconnected(server))
            {
                HandleDisconnect();
                return;
            }

            Circle circle;

            //Try to deserialize the circle, catch any bad information.
            try
            {
                circle = JsonConvert.DeserializeObject<Circle>(state.Message);
                if (!(circle is null))
                {
                    switch (circle.TYPE)
                    {
                        //Food case
                        case 0:
                            if (world.Food.TryGetValue(circle.ID, out Circle newCircle))
                            {
                                if (newCircle.MASS > 0)
                                    eatenFood++;
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
                        //Player case
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
                            break;
                        //Heartbeat case
                        case 2:
                            logger.LogInformation("HEARTBEAT");
                            HBcount++;
                            DataArrived();
                            break;
                        //Admin case
                        case 3:
                            lock (world)
                            {
                                world.Players.Add(circle.ID, circle);
                            }
                            break;
                    }
                }

            }
            catch (Exception e) { logger.LogError("Incorrect Format for circle: " + e.Message); }


            if (!server.Has_More_Data())
            {
                Networking.await_more_data(server);
            }
        }

        /// <summary>
        /// Method to handle setting up a new world and view upon
        /// client disconnect.
        /// </summary>
        private void HandleDisconnect()
        {
            //Run a new thread to talk to GUI
            MethodInvoker m = new MethodInvoker(() =>
            {

                connectbutton.Enabled = true;
                server = null;
                playfield.Dispose();
                world = new World
                {
                    Players = new Dictionary<int, Circle>(),
                    Food = new Dictionary<int, Circle>()
                };
                BuildPlayField();
                Invalidate(true);
            });
            Invoke(m);

        }
        /// <summary>
        /// Helper method to check if a server connection exists.
        /// **UNUSED**
        /// </summary>
        /// <param name="state">The server.</param>
        /// <returns></returns>
        private bool Disconnected(Preserved_Socket_State state)
        {
            if (state.socket.Connected)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Event to handle form invalidates.
        /// </summary>
        /// <param name="handler"></param>
        public void RegisterServerUpdate(ServerUpdateHandler handler)
        {
            DataArrived += handler;
        }


        /// <summary>
        /// Method to handle mouse movement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Move(object sender, MouseEventArgs e)
        {

            if (!(world?.Players is null) && (world.Players.TryGetValue((int)playerID, out Circle player)))
            {
                playerX = (int)player.LOC.X;
                playerY = (int)player.LOC.Y;
                CalculateMove(player, e);
            }
        }

        /// <summary>
        /// Helper method for the On_Move method.
        /// </summary>
        /// <param name="c">The circle to move</param>
        /// <param name="e">The mouse event</param>
        private void CalculateMove(Circle c, MouseEventArgs e)
        {

            //Rudimentary movement, check we exist, then based on
            //the position of the mouse, move the player the maximum
            //possible, allowing the server to handle move speed.
            if (!(server is null) && !(world.Players is null) && !(c is null))
            {
                if (e.X <= (playfield.Size.Width / 2))
                {
                    mouseX = playerX - 500;
                }
                else
                {
                    mouseX = playerX + 500;
                }
                if (e.Y <= (playfield.Size.Height / 2))
                {
                    mouseY = playerY - 500;
                }
                else
                {
                    mouseY = playerY + 500;
                }
            }
        }

        /// <summary>
        /// Event method that handles a user clicking the connect button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectButton_Click(object sender, MouseEventArgs e)
        {
            username = name.Text;
            ConnectToServer(ipaddress.Text, name.Text + '\n');
            connectbutton.Enabled = false;
        }

    }
}
