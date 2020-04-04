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
        public Circle(string name, int id, Vector2 loc, double mass, float argb_color, float belongs_to, int type)
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
        public float BELONGS_TO
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
        public string NAME { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Vector2 POSITIONS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MASS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ISPLAYER { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Color COLOR { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int TYPE { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float RADIUS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
    public class Player : Circle
    {
        public string NAME { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Vector2 POSITIONS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MASS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ISPLAYER { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Color COLOR { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int TYPE { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float RADIUS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    public class Heartbeat : Circle
    {
        public string NAME { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Vector2 POSITIONS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MASS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Color COLOR { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int TYPE { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float RADIUS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ISPLAYER { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
