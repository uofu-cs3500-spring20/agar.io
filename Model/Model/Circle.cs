using System;
using System.Drawing;
using System.Numerics;

namespace Agario
{
    public interface Circle
    {
        public string NAME
        {
            get; set;
        }

        public int ID
        {
            get; set;
        }

        public Vector2 POSITIONS
        {
            get; set;
        }

        public int MASS
        {
            get; set;
        }

        public Color COLOR
        {
            get; set;
        }

        public int TYPE
        {
            get; set;
        }
        public float RADIUS
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
