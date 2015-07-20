using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archimedes.Geometry
{
    public enum Direction : short { LEFT = 0, RIGHT = 1 }



    public static class DirectionUtil
    {
        /// <summary>
        /// Switches a direction
        /// </summary>
        /// <param name="uDirection"></param>
        /// <returns></returns>
        public static Direction Switch(Direction uDirection)
        {
            return (uDirection == Direction.LEFT) ? Direction.RIGHT : Direction.LEFT;
        }

    }
}
