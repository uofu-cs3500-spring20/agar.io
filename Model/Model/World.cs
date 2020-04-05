using Agario;
using FileLogger;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Model
{

    public class World
    {
        public int WORLDSIZE;
        public Vector2[] CORNERS
        {
            get; private set;

            
        }

        public Dictionary<int, Circle> Players;
        public Dictionary<int, Circle> Food;
        public Dictionary<int, Heartbeat> Heartbeat;
        public Dictionary<int, Circle> circles;

        ILogger logger;
        public int playerID;

        public World() { }
        public World(ILogger logger, Dictionary<int, Circle> Players,Dictionary<int,Circle> Food)
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
        public World(ILogger logger, int worldsize, Dictionary<int, Circle> Players, Dictionary<int,Circle> Food)
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


    }
}
