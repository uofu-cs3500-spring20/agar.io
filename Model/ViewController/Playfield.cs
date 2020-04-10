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
        private bool playing = false;
        private float oldMass = 50f;
        public World world;
        private SoundPlayer soundPlayer;
        public string username;
        public int playerID
        {
            get; set;
        }
        public float scaleX = 5;
        public float scaleY = 5;

        Size imageSize;
        public Playfield(World world)
        {
            this.DoubleBuffered = true;
            this.world = world;
            soundPlayer = new SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "Minecraft-eat3.wav");

        }
        protected override void OnPaint(PaintEventArgs e)
        {


            // Invalidate(true);
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
                         playerX = 2500 * ratio * scaleX;
                         playerY = 2500 * ratio * scaleY;
                    }
                    float playerOriginX = -(float)(playerX) + (imageSize.Width / 2) - (player.RADIUS / 2);
                    float playerOriginY = -(float)(playerY) + (imageSize.Height / 2) - (player.RADIUS / 2);

                    e.Graphics.TranslateTransform(playerOriginX, playerOriginY);
                    DrawBorder(e, world.WORLDSIZE * ratio * scaleY + 10);

                    if (player.MASS == 0)
                    {

                        Font f = new Font(FontFamily.GenericSansSerif, 80);

                        Color col = Color.Black;
                        using Pen pen = new Pen(col);
                        Brush b = pen.Brush;
                        Brush d = Brushes.Black;
                        e.Graphics.DrawString("GAME OVER", f, b, new PointF((float)playerX - (imageSize.Width / 2), (float)playerY));


                    }
                   
                    if (player.MASS > oldMass && !playing)
                    {
                        playing = true;
                        soundPlayer.Play();
                    }
                    oldMass = (float)player.MASS;
                   lock (this.world)
                    {
                      
                        foreach (Circle c in food.Values)
                        {
                            //Optimization: only draw what we need
                            if (c.MASS > 0 && (c.LOC.X * ratio > (player.LOC.X - (4000 / scaleX)) * ratio)
                                && (c.LOC.X * ratio < (player.LOC.X + (4000 / scaleX)) * ratio)
                                && (c.LOC.Y * ratio > (player.LOC.Y - (4000 / scaleY)) * ratio)
                                && (c.LOC.Y * ratio > (player.LOC.Y - (4000 / scaleY)) * ratio))
                                DrawFood(c, e);
                        }
                    }
                    lock (this.world)
                    {
                       
                        foreach (Circle c in players.Values)
                        {
                            //Optimization: only draw what we need
                            if (c.MASS > 0 && (c.LOC.X * ratio > (player.LOC.X - (4000 / scaleX)) * ratio)
                                && (c.LOC.X * ratio < (player.LOC.X + (4000 / scaleX)) * ratio)
                                && (c.LOC.Y * ratio > (player.LOC.Y - (4000 / scaleY)) * ratio)
                                && (c.LOC.Y * ratio > (player.LOC.Y - (4000 / scaleY)) * ratio))
                                DrawPlayers(c, e);
                        }
                    }
                    


                }
            }
            playing = false;
            base.OnPaint(e);
        }

        private void DrawBorder(PaintEventArgs e, double size)
        {
            using Pen pen = new Pen(Color.Black);
            Brush b = pen.Brush;
            Brush d = Brushes.Black;
            e.Graphics.DrawRectangle(pen, new Rectangle(new Point(0, 0), new Size((int)size, (int)size)));
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
            e.Graphics.DrawEllipse(new Pen(Color.Black, 2.2f), new Rectangle(new Point((int)((c.LOC.X - (c.RADIUS / 2)) * ratio * scaleX), (int)((c.LOC.Y - (c.RADIUS / 2)) * ratio * scaleY)), new Size((int)(c.RADIUS * ratio * scaleY), (int)(c.RADIUS * ratio * scaleY))));
            e.Graphics.FillEllipse(b, new Rectangle(new Point((int)((c.LOC.X - (c.RADIUS / 2)) * ratio * scaleX), (int)((c.LOC.Y - (c.RADIUS / 2)) * ratio * scaleY)), new Size((int)(c.RADIUS * ratio * scaleY), (int)(c.RADIUS * ratio * scaleY))));
            e.Graphics.DrawString($"{username}", f, b, new RectangleF(new Point((int)(circle.LOC.X - (circle.RADIUS / 1.2)),
                (int)(circle.LOC.Y - (circle.RADIUS / 1.2) * scaleY - 50)), new Size(100, 11)));
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
    }
}

