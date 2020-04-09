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
            if (!(world.Players is null) && world.WORLDSIZE>0)
            {


                Circle player;
                Dictionary<int, Circle> food = world.Food;
                Dictionary<int, Circle> players = world.Players;

                if (players.TryGetValue(playerID, out player))
                {

                    double playerX = player.LOC.X;
                    double playerY = player.LOC.Y;
                  
                    e.Graphics.TranslateTransform(-(float)(playerX) + (imageSize.Width / 2) - (player.RADIUS / 2), (float)-(playerY) + (imageSize.Width / 2) - (player.RADIUS / 2));

                    DrawBorder(e, world.WORLDSIZE);


                    try
                    {
                        foreach (Circle c in food.Values)
                        {
                            if (c.MASS > 0 && c.LOC.X > playerX - 500 && c.LOC.X < playerX + 500 && c.LOC.Y > playerY - 500 && c.LOC.Y < playerY + 500)
                                DrawFood(c, e);
                        }
                    }
                    catch { }
                    foreach (Circle c in players.Values)
                    {
                        if (c.MASS > 0 && c.LOC.X > playerX - 500 && c.LOC.X < playerX + 500 && c.LOC.Y > playerY - 500 && c.LOC.Y < playerY + 500)
                            DrawPlayers(c, e);
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
            e.Graphics.DrawRectangle(pen, new Rectangle(new Point(0,0),new Size((int)size,(int)size) ));
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
            e.Graphics.FillEllipse(b, new Rectangle(new Point((int)(circle.LOC.X), (int)(circle.LOC.Y)), new Size((int)circle.RADIUS, (int)circle.RADIUS)));
            e.Graphics.DrawString($"{circle.NAME}: {circle.LOC.X}, {circle.LOC.Y}", f, d, new PointF((float)(circle.LOC.X), (float)(circle.LOC.Y)));
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
            e.Graphics.FillEllipse(b, new Rectangle(new Point((int)(circle.LOC.X), (int)(circle.LOC.Y)), new Size((int)circle.RADIUS, (int)circle.RADIUS)));
            e.Graphics.DrawString($" : {circle.LOC.X}, {circle.LOC.Y}", f, d, new PointF((float)(circle.LOC.X), (float)(circle.LOC.Y)));
        }




        public void Zoom()
        {

        }
    }
}

