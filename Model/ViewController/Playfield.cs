using Agario;
using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace ViewController
{
    public partial class Playfield : Panel
    {

        private World world;

        public string username;
        public int playerID
        {
            get; set;
        }
        public float scaleX = 1;
        public float scaleY = 1;

        Size imageSize;
        public Playfield(World world)
        {
            this.DoubleBuffered = true;
            this.world = world;
         

        }
        protected override void OnPaint(PaintEventArgs e)
        {
            
            // Invalidate(true);
            imageSize = this.Size;
            if (!(world.Players is null) && world.WORLDSIZE > 0)
            {


                Circle player;
                Dictionary<int, Circle> food = world.Food;
                Dictionary<int, Circle> players = world.Players;

                if (players.TryGetValue(playerID, out player))
                {

                    double playerX = player.LOC.X;
                    double playerY = player.LOC.Y;
                   

                    e.Graphics.TranslateTransform(-(float)(playerX) + (imageSize.Width/2) - (player.RADIUS / 2), -(float)(playerY) + (imageSize.Height/2) - (player.RADIUS / 2));
                    DrawBorder(e, world.WORLDSIZE + 20);

                    if (player.MASS == 0)
                    {

                        Font f = new Font(FontFamily.GenericSansSerif, 80);

                        Color col = Color.Black;
                        using Pen pen = new Pen(col);
                        Brush b = pen.Brush;
                        Brush d = Brushes.Black;
                        e.Graphics.DrawString("GAME OVER", f, b, new PointF((float)playerX - (imageSize.Width / 2), (float)playerY));


                    }

                    lock (world)
                    {
                        try
                        {
                            foreach (Circle c in food.Values)
                            {
                                if (c.MASS > 0 && c.LOC.X > playerX - 500 && c.LOC.X < playerX + 500 && c.LOC.Y > playerY - 500 && c.LOC.Y < playerY + 500)
                                    DrawFood(c, e);
                            }


                            foreach (Circle c in players.Values)
                            {
                                if (c.MASS > 0 && c.LOC.X > playerX - 500 && c.LOC.X < playerX + 500 && c.LOC.Y > playerY - 500 && c.LOC.Y < playerY + 500)
                                    DrawPlayers(c, e);
                            }
                        }
                        catch { }
                    }


                }
            }

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

            Circle circle = c;
            var argb = 0;
            Font f = new Font(FontFamily.GenericSansSerif, 10);
            argb = Convert.ToInt32(circle.ARGB_COLOR);
            Color col = Color.FromArgb(argb);
            using Pen pen = new Pen(col);
            Brush b = pen.Brush;
            Brush d = Brushes.Black;
            e.Graphics.FillEllipse(b, new Rectangle(new Point((int)(circle.LOC.X - (circle.RADIUS / 1.2)), (int)(circle.LOC.Y - (circle.RADIUS / 1.2))),
                new Size((int)(circle.RADIUS * 2 * scaleX), (int)(circle.RADIUS * 2 * scaleX))));
            e.Graphics.DrawString($"{username}", f, b, new RectangleF(new Point((int)(circle.LOC.X - (circle.RADIUS / 1.2)*scaleX),
                (int)(circle.LOC.Y - (circle.RADIUS / 1.2)*scaleY - 50)), new Size(100, 11)));
        }
        public void DrawFood(Circle c, PaintEventArgs e)
        {
            Circle circle = c;
            var argb = 0;
            Font f = new Font(FontFamily.GenericSansSerif, 10);
            argb = Convert.ToInt32(circle.ARGB_COLOR);
            Color col = Color.FromArgb(argb);
            using Pen pen = new Pen(col);
            Brush b = pen.Brush;
            Brush d = Brushes.Black;
            e.Graphics.FillEllipse(b, new Rectangle(new Point((int)(circle.LOC.X - (circle.RADIUS / 4)*scaleX), (int)(circle.LOC.Y - (circle.RADIUS / 4)*scaleY )),
                new Size((int)(circle.RADIUS * scaleX), (int)(circle.RADIUS * scaleX))));
            // e.Graphics.DrawString($" : {circle.LOC.X}, {circle.LOC.Y}", f, d, new PointF((float)(circle.LOC.X), (float)(circle.LOC.Y)));
        }




        public void Zoom(int sX, int sY)
        {
          

            if(scaleX-sX/100>=1)
            this.scaleX -= (int)sX/100;
            if(scaleY-sY/100>=1)
            this.scaleY -= (int)sY/100;
        }
    }
}

