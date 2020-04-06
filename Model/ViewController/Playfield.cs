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
        public int playerID
        {
            get; set;
        }
        public string username
        {
            get; set;
        }

        public Playfield(World world)
        {
            this.world = world;
        }

        public delegate void ObjectDrawer(object o, PaintEventArgs e);
        private static int WorldSpaceToImageSpace(int size, double coord)
        {
            return (int)(coord*((float)804/(float)size));

        }
        private void DrawObjectWithTransform(PaintEventArgs e, object o, int worldSize, double worldX, double worldY, ObjectDrawer objectDrawer)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            int x = WorldSpaceToImageSpace(worldSize, worldX);
            int y = WorldSpaceToImageSpace(worldSize, worldY);
            e.Graphics.TranslateTransform(x, y);

            objectDrawer(o, e);

            // "pop" the transform
            e.Graphics.Transform = oldMatrix;
        }


        protected override void OnPaint(PaintEventArgs e)
        {

            if (!(world.Players is null) && world.WORLDSIZE > 0)
            {
                Dictionary<int, Circle> players = world.Players;
                Circle player;
                players.TryGetValue(playerID, out player);
                Vector2 loc = player.LOC;
                double playerX = loc.X; // ... (the player's world-space X coordinate)
                double playerY = loc.Y;  //... (the player's world-space Y coordinate)
                int wsize = world.WORLDSIZE;
                int imageX = -WorldSpaceToImageSpace((int)wsize, playerX);
                int imageY = -WorldSpaceToImageSpace((int)wsize, playerY);
                float halfRatio = (float)804 / (2 * ((float)804 / (float)wsize));

                double X = playerX + halfRatio;
                double Y = playerY + halfRatio;
                e.Graphics.TranslateTransform(-((float)playerX-imageX-370),-((float)playerY-imageY-402));
                //e.Graphics.TranslateTransform(0, 0);
                DrawObjectWithTransform(e, new Rectangle(new Point(2500, 2500), new Size(5000, 5000)), 5000, 0, 0, DrawBorder);


                foreach (Circle c in world.Players.Values)
                {
                    DrawObjectWithTransform(e, c, 5000, c.LOC.X, c.LOC.Y, DrawPlayers);
                }
                foreach (Circle c in world.Food.Values)
                {
                    DrawObjectWithTransform(e, c, 5000, c.LOC.X, c.LOC.Y, DrawFood);
                }

            }

            base.OnPaint(e);
        }

        private void DrawBorder(object o, PaintEventArgs e)
        {
            using Pen pen = new Pen(Color.Black);
            Brush b = pen.Brush;
            Brush d = Brushes.Black;
            e.Graphics.DrawRectangle(pen, (Rectangle)o);
        }

        public void DrawPlayers(object o, PaintEventArgs e)
        {
            Circle circle = (Circle)o;
            var argb = 0;
            Font f = new Font(FontFamily.GenericSansSerif, 10);
            argb = Convert.ToInt32(circle.ARGB_COLOR);
            Color col = Color.FromArgb(argb);
            using Pen pen = new Pen(col);
            Brush b = pen.Brush;
            Brush d = Brushes.Black;
            e.Graphics.FillEllipse(b, new Rectangle(new Point((int)circle.LOC.X, (int)circle.LOC.Y), new Size((int)circle.RADIUS, (int)circle.RADIUS)));
            e.Graphics.DrawString($"{circle.NAME}: {circle.LOC.X}, {circle.LOC.Y}", f, d, new PointF((float)circle.LOC.X, (float)circle.LOC.Y));
        }
        public void DrawFood(object o, PaintEventArgs e)
        {

            Circle circle = (Circle)o;
            var argb = 0;
            Font f = new Font(FontFamily.GenericSansSerif, 10);
            argb = Convert.ToInt32(circle.ARGB_COLOR);
            Color col = Color.FromArgb(argb);
            using Pen pen = new Pen(col);
            Brush b = pen.Brush;
            Brush d = Brushes.Black;
            e.Graphics.FillEllipse(b, new Rectangle(new Point((int)circle.LOC.X, (int)circle.LOC.Y), new Size((int)circle.RADIUS, (int)circle.RADIUS)));
            e.Graphics.DrawString($" : {circle.LOC.X}, {circle.LOC.Y}", f, d, new PointF((float)circle.LOC.X, (float)circle.LOC.Y));
        }




        public void Zoom()
        {

        }
    }
}

