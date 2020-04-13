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
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Numerics;

namespace Agario
{
    /// <summary>
    /// Class representing an agar.io game object
    /// </summary>
    public class Circle
    {
        /// <summary>
        /// Base constructor
        /// </summary>
        public Circle() { }

        /// <summary>
        /// Overload, main constructor to use.
        /// </summary>
        /// <param name="name">Name of circle</param>
        /// <param name="id">Id of circle</param>
        /// <param name="loc">Location of circle</param>
        /// <param name="mass">Mass of the circle</param>
        /// <param name="argb_color">Color of the circle</param>
        /// <param name="belongs_to">Used for splitting, tracks what circle has been shot from who.</param>
        /// <param name="type">Type of circle</param>
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
            RADIUS = (float)Math.Sqrt((MASS*Math.PI));
        }

        //MASS = PI * R *R
        //RADIUS = SQRT(MASS*PI)
        [JsonIgnore]
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
        [JsonIgnore]
        public bool ISPLAYER
        {
            get; set;
        }



    }
}
