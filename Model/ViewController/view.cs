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
        private SocketState server;
        private string username;
        private World world;
        private bool failedConnect;
        private string failedMsg;



        public delegate void ServerUpdateHandler();
        private event ServerUpdateHandler DataArrived;

        public view()
        {
            world = new World();
            RegisterServerUpdate(Frame);
            InitializeComponent();


        }

        private void Frame()
        {
           
            try
            {
                MethodInvoker m = new MethodInvoker(()=> {Refresh(); });
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
            username = name + '\n';
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
            server.CallMe = InitialReceive;

            Networking.Send(server.TheSocket, username);
            Networking.GetData(server);
        }

        private void InitialReceive(SocketState obj)
        {
            drawingpanel.BackColor = Color.Transparent;

            string totalData = server.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");
            Dictionary<int, Circle> circles = new Dictionary<int, Circle>();
            if (!server.ErrorOccured)
            {
                try
                {
                    foreach (string s in parts)
                    {
                        if (s.Length > 1)
                        {
                            Circle circle = JsonConvert.DeserializeObject<Circle>(s);
                            circles.Add(circle.ID, circle);
                        }
                    }
                }
                catch { }
                server.CallMe = AwaitData;
                Networking.GetData(server);

            }
        }

        private void AwaitData(SocketState state)
        {

            string totalData = server.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            Dictionary<int, Player> players = new Dictionary<int, Player>();
            Dictionary<int, Food> pows = new Dictionary<int, Food>();
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

                        if(!(circle is null))
                         circles?.Add(circle.ID, circle);




                    }

                }
            }
            catch { }
            //Pass all of the lists to the world to process further
            world.circles = circles;
            DataArrived();
            Networking.GetData(server);
        }
        public void RegisterServerUpdate(ServerUpdateHandler handler)
        {
            DataArrived += handler;
        }


        public bool HasMoreData(SocketState state)
        {
            return false;
        }


        public void SendData(SocketState state)
        {

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //Invalidate();
            if (!(world?.circles is null))
            {


              
                var argb = 0;
                double X;
                double Y;
                foreach (Circle c in world.circles.Values)
                {
                    argb = Convert.ToInt32(c.ARGB_COLOR);

                    Color col = Color.FromArgb(argb);
                    using Pen pen = new Pen(col);
                    Brush b = pen.Brush;
                    Point p = new Point((int)c.LOC.X, (int)c.LOC.Y);

                    if(p.X < )
                    e.Graphics.FillEllipse(b,new Rectangle(p,new Size((int)c.RADIUS,(int)c.RADIUS)));
                  
                   

                }

            }
            base.OnPaint(e);
        }


        public void Zoom()
        {

        }


        private void ConnectButton_Click(object sender, MouseEventArgs e)
        {
            ConnectToServer(ipaddress.Text, name.Text);
        }

        private void IPAddress_TextChanged(object sender, EventArgs e)
        {
           // throw new NotImplementedException();
        }


    }
}
