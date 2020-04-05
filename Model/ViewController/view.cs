using Agario;
using Model;
using NetworkUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

        public delegate void ServerUpdateHandler();
        private event ServerUpdateHandler DataArrived;

        public view()
        {
            commandlist = new StringBuilder();
            world = new World();
            RegisterServerUpdate(Frame);
            InitializeComponent();


        }

        private void Frame()
        {

            try
            {
                MethodInvoker m = new MethodInvoker(() => { Invalidate(); });
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
            drawingpanel.BackColor = Color.Transparent;
            
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
                                    playerID = circle.ID;
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


       

        protected override void OnPaint(PaintEventArgs e)
        {

            Rectangle r = drawingpanel.ClientRectangle;
            float centerx = r.Width / 2;
            float centrey = r.Height / 2;

            if (!(world.Players is null))
            {
                world.Players.TryGetValue((int)playerID, out Circle player);
                int playerX = (int)player.LOC.X;
                int playerY = (int)player.LOC.Y;
                e.Graphics.TranslateTransform(-(playerX - centerx), -(playerY - centrey)); 
                e.Graphics.DrawRectangle(new Pen(Color.Black, 2), new Rectangle(5000, 5000, 5000, 5000));
                

                var argb = 0;

                Font f = new Font(FontFamily.GenericSansSerif, 10);
                argb = Convert.ToInt32(player.ARGB_COLOR);

                Color col = Color.FromArgb(argb);
                using Pen pen = new Pen(col);
                Brush b = pen.Brush;
                Brush d = Brushes.Black;
                Point p = new Point((int)playerX, (int)playerY);
                e.Graphics.FillEllipse(b, new Rectangle(p, new Size((int)player.RADIUS, (int)player.RADIUS)));
                e.Graphics.DrawString(username + $" : {playerX}, {playerY}", f, d, new PointF(playerX, playerY));


                DrawFood(e, p);
                DrawPlayers(e, p);
            }
            base.OnPaint(e);
        }
        public void DrawPlayers(PaintEventArgs e, Point point)
        {
            var argb = 0;

            foreach (Circle player in world.Players.Values)
            {

                Font f = new Font(FontFamily.GenericSansSerif, 10);
                argb = Convert.ToInt32(player.ARGB_COLOR);

                Color col = Color.FromArgb(argb);
                using Pen pen = new Pen(col);
                Brush b = pen.Brush;
                Brush d = Brushes.Black;
                Point p = new Point((int)player.LOC.X, (int)player.LOC.Y);
                e.Graphics.FillEllipse(b, new Rectangle(p, new Size((int)player.RADIUS, (int)player.RADIUS)));
                e.Graphics.DrawString(username + $" : {player.LOC.X}, {player.LOC.Y}", f, d, new PointF(player.LOC.X, player.LOC.Y));

            }
        }
        public void DrawFood(PaintEventArgs e, Point point)
        {


            if (!(world?.Food is null))
            {
                var argb = 0;

                Font f = new Font(FontFamily.GenericSansSerif, 10);
                foreach (Circle c in world.Food.Values)
                {
                    argb = Convert.ToInt32(c.ARGB_COLOR);

                    Color col = Color.FromArgb(argb);
                    using Pen pen = new Pen(col);
                    Brush b = pen.Brush;
                    Point p = new Point((int)c.LOC.X, (int)c.LOC.Y);

                    if ((p.X > (point.X - 399) && p.X < (point.X + 399)) && (p.Y > point.Y - 399 && p.Y < point.Y + 399))
                    {
                        e.Graphics.FillEllipse(b, new Rectangle(p, new Size((int)c.RADIUS, (int)c.RADIUS)));
                        e.Graphics.DrawString($"{p.X}, {p.Y}", f, b, p);
                    }


                }
            }
        }


        public void Zoom()
        {

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
                string message = $"(move,{-20},{3})";
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
