using Agario;
using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace ViewController
{
    public partial class Playfield : Panel
    {
        public delegate void DeathEvent();
        public event DeathEvent Death;
        private MethodInvoker m;
        private float oldMass = 50f;
        public int rendered
        {
            get; set;
        }
        public World world;
        private SoundPlayer soundPlayer;
        SoundPlayer soundPlayer2;
        public string username;
        public int playerID
        {
            get; set;
        }
        public float scaleX = 5;
        public float scaleY = 5;

        Size imageSize;
        private bool deathPlayed = false;


        public Playfield(World world)
        {
            rendered = 1;
            this.DoubleBuffered = true;
            this.world = world;
            soundPlayer = new SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "Minecraft-eat3.wav");
            soundPlayer2 = new SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "Roblox-death-sound.wav");
            soundPlayer2.Load();
            soundPlayer.Load();
        }
        protected override void OnPaint(PaintEventArgs e)
        {

            rendered++;

            imageSize = this.Size;
            if (!(world.Players is null) && world.WORLDSIZE > 0)
            {
                Dictionary<int, Circle> players;
                Dictionary<int, Circle> food;
                Circle player;
                lock (this.world)
                {
                    food = world.Food;
                    players = world.Players;
                }
                if (players.TryGetValue(playerID, out player))
                {
                    double playerY;
                    double playerX;
                    double ratio = (double)this.Size.Width / (double)world.WORLDSIZE;
                    if (player.NAME != "Admin")
                    {
                        playerX = player.LOC.X * ratio * scaleX;
                        playerY = player.LOC.Y * ratio * scaleY;
                    }
                    else
                    {
                        playerX = 2000 * ratio * scaleX;
                        playerY = 2000 * ratio * scaleY;
                    }
                    float playerOriginX = -(float)(playerX) + (imageSize.Width / 2);
                    float playerOriginY = -(float)(playerY) + (imageSize.Height / 2);

                    e.Graphics.TranslateTransform(playerOriginX, playerOriginY);

                        DrawGridLines(e);
                    

                    if (player.MASS > oldMass)
                    {
                        m = new MethodInvoker(() => soundPlayer.Play());
                        Invoke(m);
                        oldMass = (float)player.MASS;
                    }
                    if (player.MASS == 0)
                    {


                        if (!deathPlayed)
                        {
                            soundPlayer2.PlaySync();
                            deathPlayed = true;
                            Death();
                        }
                        DialogResult deathDialog = MessageBox.Show($"You lost!\nConnect to the server to try again!\n" +
                            "Final Stats:\n" +
                            $"Mass: {oldMass}","agar.io"
                            ,MessageBoxButtons.OK,MessageBoxIcon.Information);


                    }


                    lock (this.world)
                    {

                        foreach (Circle c in food.Values)
                        {
                            //Optimization: only draw what we need
                            if (player.NAME == "Admin"||(c.MASS > 0 && (c.LOC.X * ratio > (player.LOC.X - (4000 / scaleX)) * ratio)
                                && (c.LOC.X * ratio < (player.LOC.X + (4000 / scaleX)) * ratio)
                                && (c.LOC.Y * ratio > (player.LOC.Y - (4000 / scaleY)) * ratio)
                                && (c.LOC.Y * ratio > (player.LOC.Y - (4000 / scaleY)) * ratio)))
                                DrawFood(c, e);
                        }
                    }
                    lock (this.world)
                    {

                        foreach (Circle c in players.Values)
                        {
                            //Optimization: only draw what we need
                            if (player.NAME == "Admin"||(c.MASS > 0 && (c.LOC.X * ratio > (player.LOC.X - (4000 / scaleX)) * ratio)
                                && (c.LOC.X * ratio < (player.LOC.X + (4000 / scaleX)) * ratio)
                                && (c.LOC.Y * ratio > (player.LOC.Y - (4000 / scaleY)) * ratio)
                                && (c.LOC.Y * ratio > (player.LOC.Y - (4000 / scaleY)) * ratio)))
                                DrawPlayers(c, e);
                        }
                    }

                    Invalidate(true);
                }
            }
        }

        private void DrawGridLines(PaintEventArgs e)
        {
            using Pen pen = new Pen(Color.FromArgb(218,218,218),1f);
            double ratio = (double)this.Size.Width / (double)world.WORLDSIZE;
            for (int i = 0; i < world.WORLDSIZE; i += 100)
            {
                e.Graphics.DrawLine(pen, (int)(i*ratio*scaleX), 0, (int)(i * ratio * scaleX), (int)(world.WORLDSIZE*ratio*scaleX));
                e.Graphics.DrawLine(pen, 0, (int)(i * ratio * scaleY), (int)(world.WORLDSIZE*ratio*scaleY), (int)(i * ratio * scaleY));

            }
            e.Graphics.DrawLine(pen, (int)(5000 * ratio * scaleX), 0, (int)(5000 * ratio * scaleX), (int)(world.WORLDSIZE * ratio * scaleX));
            e.Graphics.DrawLine(pen, 0, (int)(5000 * ratio * scaleX), (int)(world.WORLDSIZE * ratio * scaleX), (int)(5000 * ratio * scaleX));

        }


        public void DrawPlayers(Circle c, PaintEventArgs e)
        {
            double ratio = (double)this.Size.Width / (double)world.WORLDSIZE;
            Circle circle = c;
            Font f = new Font(FontFamily.GenericSansSerif, 10);
            int argb = Convert.ToInt32(circle.ARGB_COLOR);
            Color col = Color.FromArgb(argb);
            using Pen pen = new Pen(col);
            Brush b = pen.Brush;
            Brush d = Brushes.Black;
            if (c.NAME != "Admin")
            {
                e.Graphics.DrawString($"{c.NAME}", f, b, new Rectangle(new Point((int)((c.LOC.X - (c.RADIUS / 2)) * ratio * scaleX), (int)((c.LOC.Y - (c.RADIUS / 2)) * ratio * scaleY) - (int)(c.RADIUS / 2) - 10), new Size((int)(c.NAME.Length * 12 * ratio * scaleY), (int)(20 * ratio * scaleY))));
                e.Graphics.DrawEllipse(new Pen(Color.Black, 2.2f), new Rectangle(new Point((int)((c.LOC.X - (c.RADIUS / 2)) * ratio * scaleX), (int)((c.LOC.Y - (c.RADIUS / 2)) * ratio * scaleY)), new Size((int)(c.RADIUS * ratio * scaleY), (int)(c.RADIUS * ratio * scaleY))));
                e.Graphics.FillEllipse(b, new Rectangle(new Point((int)((c.LOC.X - (c.RADIUS / 2)) * ratio * scaleX), (int)((c.LOC.Y - (c.RADIUS / 2)) * ratio * scaleY)), new Size((int)(c.RADIUS * ratio * scaleY), (int)(c.RADIUS * ratio * scaleY))));
            }
        }
        public void DrawFood(Circle c, PaintEventArgs e)
        {
            double ratio = (double)this.Size.Width / (double)world.WORLDSIZE;

            Circle circle = c;
            int argb = Convert.ToInt32(circle.ARGB_COLOR);
            Color col = Color.FromArgb(argb);
            using Pen pen = new Pen(col);
            Brush b = pen.Brush;
            Brush d = Brushes.Black;
            e.Graphics.DrawEllipse(new Pen(Color.Black, 2.2f), new Rectangle(new Point((int)((c.LOC.X - (c.RADIUS / 2)) * ratio * scaleY), (int)((c.LOC.Y - (c.RADIUS / 2)) * ratio * scaleY)), new Size((int)(c.RADIUS * ratio * scaleY), (int)(c.RADIUS * ratio * scaleY))));
            e.Graphics.FillEllipse(b, new Rectangle(new Point((int)((c.LOC.X - (c.RADIUS / 2)) * ratio * scaleY), (int)((c.LOC.Y - (c.RADIUS / 2)) * ratio * scaleY)), new Size((int)(c.RADIUS * ratio * scaleY), (int)(c.RADIUS * ratio * scaleY))));

        }


        public void Zoom(int sX, int sY)
        {
            if (sX < 0)
                if (scaleX - .1 > 0)
                    this.scaleX -= (float).1;
            if (sX > 0)
                this.scaleX += (float).1;

            if (sY < 0)
                if (scaleY - .1 > 0)
                    this.scaleY -= (float).1;
            if (sY > 0)
                this.scaleY += (float).1;
        }




        public void DeathArose(DeathEvent death)
        {

            Death += death;
        }

    }
}

