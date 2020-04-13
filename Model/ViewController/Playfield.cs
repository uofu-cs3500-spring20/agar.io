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
    /// <summary>
    /// Class representing where all of our agar.io objects will be drawn
    /// </summary>
    public partial class Playfield : Panel
    {

        //Administrative stuff
        public string username;
        public int playerID
        {
            get; set;
        }
        Size imageSize;
        private MethodInvoker m;
        private float oldMass = 50f;
        public World world;
        /// <summary>
        /// Property to count frames rendered.
        /// </summary>
        public int rendered
        {
            get; set;
        }
        //Death events
        public delegate void DeathEvent();
        public event DeathEvent Death;
        private bool deathPlayed = false;

        //Sound stuff
        private SoundPlayer eatSound;
        private SoundPlayer deathSound;
        //Initial scale size
        public float scaleX = 5;
        public float scaleY = 5;


        /// <summary>
        /// Constructor for the playfield
        /// </summary>
        /// <param name="world">The world to build</param>
        public Playfield(World world)
        {
            rendered = 1;
            this.DoubleBuffered = true;
            this.world = world;
            eatSound = new SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "Minecraft-eat3.wav");
            deathSound = new SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "Roblox-death-sound.wav");
           
            deathSound.Load();
            eatSound.Load();
        }


        /// <summary>
        /// Where the magic happens V2
        /// 
        /// Method to handle redrawing our playfield upon invalidation
        /// </summary>
        /// <param name="e">The paint event</param>
        protected override void OnPaint(PaintEventArgs e)
        {
           

            rendered++;
            imageSize = this.Size;

            if (!(world.Players is null) && world.WORLDSIZE > 0)
            {
                Dictionary<int, Circle> players;
                Dictionary<int, Circle> food;

                lock (this.world)
                {
                    food = world.Food;
                    players = world.Players;
                }

                //First handle centering player view
                if (players.TryGetValue(playerID, out Circle player))
                {
                    double playerY;
                    double playerX;
                    double ratio = (double)this.Size.Width / (double)world.WORLDSIZE;
                    playerX = player.LOC.X * ratio * scaleX;
                    playerY = player.LOC.Y * ratio * scaleY;
                    if (player.NAME == "Admin")
                    {
                        playerX = 2000 * ratio * scaleX;
                        playerY = 2000 * ratio * scaleY;
                    }

                    float playerOriginX = -(float)(playerX) + (imageSize.Width / 2);
                    float playerOriginY = -(float)(playerY) + (imageSize.Height / 2);

                    e.Graphics.TranslateTransform(playerOriginX, playerOriginY);
                    //End centering view

                    //Draw grid
                    DrawGridLines(e);

                  //Handle eat sounds
                  //  if (player.MASS > oldMass)
                  //  {
                  // m = new MethodInvoker(() => eatSound.Play());
                  // Invoke(m);
                  //  oldMass = (float)player.MASS;
                  //  }

                    //Handle death
                    if (player.MASS == 0)
                    {
                        if (!deathPlayed)
                        {
                            deathSound.PlaySync();
                            deathPlayed = true;
                            Death();
                        }
                        DialogResult deathDialog = MessageBox.Show($"You lost!\nConnect to the server to try again!\n" +
                            "Final Stats:\n" +
                            $"Mass: {oldMass}", "agar.io"
                            , MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    //Begin drawing player and food objects
                    //We only draw what's needed for our player's view to improve performance
                    lock (this.world)
                    {
                        foreach (Circle c in food.Values)
                        {
                            //Optimization: only draw what we need
                            if (player.NAME == "Admin" || (c.MASS > 0 && (c.LOC.X * ratio > (player.LOC.X - (world.WORLDSIZE / scaleX)) * ratio)
                                && (c.LOC.X * ratio < (player.LOC.X + (world.WORLDSIZE / scaleX)) * ratio)
                                && (c.LOC.Y * ratio > (player.LOC.Y - (world.WORLDSIZE / scaleY)) * ratio)
                                && (c.LOC.Y * ratio > (player.LOC.Y - (world.WORLDSIZE / scaleY)) * ratio)))
                                DrawFood(c, e);
                        }
                    }
                    lock (this.world)
                    {

                        foreach (Circle c in players.Values)
                        {
                            //Optimization: only draw what we need
                            if (player.NAME == "Admin" || (c.MASS > 0 && (c.LOC.X * ratio > (player.LOC.X - (world.WORLDSIZE / scaleX)) * ratio)
                                && (c.LOC.X * ratio < (player.LOC.X + (world.WORLDSIZE / scaleX)) * ratio)
                                && (c.LOC.Y * ratio > (player.LOC.Y - (world.WORLDSIZE / scaleY)) * ratio)
                                && (c.LOC.Y * ratio > (player.LOC.Y - (world.WORLDSIZE / scaleY)) * ratio)))
                                DrawPlayers(c, e);
                        }
                    }

                    //Always be in a state of invalidation to improve fps
                    Invalidate(true);
                }
            }
        }

       /// <summary>
       /// Helper method to handle drawing the grid lines
       /// </summary>
       /// <param name="e"></param>
       private void DrawGridLines(PaintEventArgs e)
        {
            using Pen pen = new Pen(Color.FromArgb(218, 218, 218), 1f);
            double ratio = (double)this.Size.Width / (double)world.WORLDSIZE;
            for (int i = 0; i < world.WORLDSIZE; i += 100)
            {
                e.Graphics.DrawLine(pen, (int)(i * ratio * scaleX), 0, (int)(i * ratio * scaleX), (int)(world.WORLDSIZE * ratio * scaleX));
                e.Graphics.DrawLine(pen, 0, (int)(i * ratio * scaleY), (int)(world.WORLDSIZE * ratio * scaleY), (int)(i * ratio * scaleY));

            }
            e.Graphics.DrawLine(pen, (int)(world.WORLDSIZE * ratio * scaleX), 0, (int)(world.WORLDSIZE * ratio * scaleX), (int)(world.WORLDSIZE * ratio * scaleX));
            e.Graphics.DrawLine(pen, 0, (int)(world.WORLDSIZE * ratio * scaleX), (int)(world.WORLDSIZE * ratio * scaleX), (int)(world.WORLDSIZE * ratio * scaleX));

        }


        /// <summary>
        /// Helper method to handle drawing player objects
        /// </summary>
        /// <param name="c">Player to draw</param>
        /// <param name="e"></param>
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
                //NOTE: player name centering is under development...
                e.Graphics.DrawString($"{c.NAME}", f, b, new Rectangle(new Point((int)((c.LOC.X - (c.RADIUS / 2)) * ratio * scaleX), (int)((c.LOC.Y - (c.RADIUS / 2)) * ratio * scaleY) - (int)(c.RADIUS / 2) - 10), new Size((int)(c.NAME.Length * 12 * ratio * scaleY), (int)(20 * ratio * scaleY))));           
                
                e.Graphics.DrawEllipse(new Pen(Color.Black, 2.2f), new Rectangle(new Point((int)((c.LOC.X - (c.RADIUS / 2)) * ratio * scaleX), (int)((c.LOC.Y - (c.RADIUS / 2)) * ratio * scaleY)), new Size((int)(c.RADIUS * ratio * scaleY), (int)(c.RADIUS * ratio * scaleY))));
                e.Graphics.FillEllipse(b, new Rectangle(new Point((int)((c.LOC.X - (c.RADIUS / 2)) * ratio * scaleX), (int)((c.LOC.Y - (c.RADIUS / 2)) * ratio * scaleY)), new Size((int)(c.RADIUS * ratio * scaleY), (int)(c.RADIUS * ratio * scaleY))));
            }
        }
        /// <summary>
        /// Helper method to handle drawing food objects
        /// </summary>
        /// <param name="c">Food to draw</param>
        /// <param name="e"></param>
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


        /// <summary>
        /// Helper method to handle calculating zoom upon mouse wheel movement
        /// </summary>
        /// <param name="sX">X zoom</param>
        /// <param name="sY">Y zoom</param>
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

        /// <summary>
        /// Simple event signifying a player death
        /// </summary>
        /// <param name="death">Death event</param>
        public void DeathArose(DeathEvent death)
        {
            Death += death;
        }

    }
}

