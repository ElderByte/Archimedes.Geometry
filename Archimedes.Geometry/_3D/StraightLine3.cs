using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archimedes.Geometry._3D
{
    /// <summary>
    /// Represents a straight line in 3D space
    /// </summary>
    public struct StraightLine3
    {
        private Vector3 _start;
        private Vector3 _direction;


        /// <summary>
        /// Represents a start point of this straight line
        /// </summary>
        public Vector3 Start 
        {
            get { return _start; }
            set { _start = value; }
        }

        /// <summary>
        /// Represents the direction of this line
        /// </summary>
        public Vector3 Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }




        public StraightLine3(Vector3 start, Vector3 direction) : this()
        {
            Start = start;
            Direction = direction;
        }



        public bool Intersects(StraightLine3 other, out Vector3 intersection)
        {
            bool intersetcts = false;
            intersection = Vector3.Zero;





            return intersetcts;
        }




    }
}
