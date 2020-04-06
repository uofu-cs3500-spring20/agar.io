using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Numerics;

namespace Agario
{
    public class Circle
    {
        public Circle() { }
        [JsonConstructor]
        public Circle(string name, int id, Vector2 loc, double mass, float argb_color, int belongs_to, int type)
        {
            NAME = name;
            ID = id;
            LOC = loc;
            MASS = mass;
            ARGB_COLOR = argb_color;
            BELONGS_TO = belongs_to;
            TYPE = type;
            RADIUS = (float)MASS / 2;
        }

        public float RADIUS
        {
            get; set;
        }
        public string NAME
        {
            get; set;
        }

        public int ID
        {
            get; set;
        }

        public Vector2 LOC
        {
            get; set;
        }

        public double MASS
        {
            get; set;
        }

        public float ARGB_COLOR
        {

            get; set;
        }
        public int BELONGS_TO
        {
            get; set;
        }
        public int TYPE
        {
            get; set;
        }
        public bool ISPLAYER
        {
            get; set;
        }



    }
    public class Food : Circle
    {
       public Food() { }
    }
    public class Player : Circle
    {
        public Player() { ISPLAYER = true; }
        
    }

    public class Heartbeat : Circle
    {
       public Heartbeat() { }
    }
}
