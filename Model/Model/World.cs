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
using FileLogger;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Model
{

    /// <summary>
    /// Class representing an agar.io Gameworld
    /// </summary>
    public class World
    {
        public int WORLDSIZE;
       
        /// <summary>
        /// List of the world corners
        /// </summary>
        public Vector2[] CORNERS
        {
            get; private set;
        }

        //Game objects
        public Dictionary<int, Circle> Players;
        public Dictionary<int, Circle> Food;

        //Logging object
        ILogger logger;

        /// <summary>
        /// Base constructor for world object/
        /// </summary>
        public World()
        {
            WORLDSIZE = 5000;
            Players = new Dictionary<int, Circle>();
            Food = new Dictionary<int, Circle>();
        }
        /// <summary>
        /// Overload to create a new world from an existing one.
        /// </summary>
        /// <param name="logger">logging object</param>
        /// <param name="Players">List of existing players</param>
        /// <param name="Food">List of existing food</param>
        public World(ILogger logger, Dictionary<int, Circle> Players, Dictionary<int, Circle> Food)
        {
            this.logger = logger;
            this.Players = Players;
            this.Food = Food;

            //Setup world locations
            WORLDSIZE = 5000;
            CORNERS[0] = new Vector2(0, 0);
            CORNERS[1] = new Vector2(0, 5000);
            CORNERS[2] = new Vector2(5000, 0);
            CORNERS[3] = new Vector2(5000, 5000);

        }
        /// <summary>
        /// Overload to allow worldsize change
        /// </summary>
        /// <param name="logger">logging object</param>
        /// <param name="worldsize">Size of world</param>
        /// <param name="Players">List of existing players</param>
        /// <param name="Food">List of existing food</param>
        public World(ILogger logger, int worldsize, Dictionary<int, Circle> Players, Dictionary<int, Circle> Food)
        {
            this.logger = logger;
            this.Players = Players;
            this.Food = Food;

            //Setup world locations
            WORLDSIZE = worldsize;
            CORNERS[0] = new Vector2(0, 0);
            CORNERS[1] = new Vector2(0, worldsize);
            CORNERS[2] = new Vector2(worldsize, 0);
            CORNERS[3] = new Vector2(worldsize, worldsize);
        }

        /// <summary>
        /// Simple method to count remaining food whose mass>0
        /// </summary>
        /// <returns>The amount of food</returns>
        public int GetFood()
        {
            int availableFood = 0;
            lock (this)
            {
                foreach (Circle c in Food.Values)
                {
                    if (c.MASS > 0)
                    {
                        availableFood++;
                    }
                }
            }
            return availableFood;
        }
    }
}
