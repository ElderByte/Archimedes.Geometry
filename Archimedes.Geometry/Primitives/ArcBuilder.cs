namespace Archimedes.Geometry.Primitives
{
    public static class ArcBuilder
    {
        /// <summary> 
        /// Create Arc from 3 given Points
        /// </summary>
        /// <param name="startPoint">Start-Point of the Arc</param>
        /// <param name="interPoint">A point anywhere the Arc</param>
        /// <param name="endPoint">End-Point of the Arc</param>
        /// <returns></returns>
        public static Arc FromDescriptorPoints(Vector2 startPoint, Vector2 interPoint, Vector2 endPoint)
        {

            const Direction calcdirection = Direction.RIGHT;

            // Calculate Rays from the 3 given Points
            var rays = RaysFromDescriptorPoints(startPoint, interPoint, endPoint, DirectionUtil.Switch(calcdirection));
            // The two Rays intercept in the Arc's Middlepoint:
            var arcCenter = rays[0].Intersect(rays[1]);
            var arcRadius = new Vector2(startPoint, arcCenter).Length;

            // Take Vectors from these Points
            var middleToStart = new Vector2(arcCenter, startPoint);
            var middleToEnd = new Vector2(arcCenter, endPoint);

            // Calculate base vector
            var vbase = middleToStart.GetOrthogonalVector(Direction.RIGHT)*-1;

            var arcAngle = middleToEnd.AngleSignedTo(middleToStart, true);

            var newArc = new Arc(
                arcRadius,
                arcAngle,
                vbase)
            {
                Location = startPoint,
                Direction = DirectionUtil.Switch(calcdirection)
            };

            return newArc;
        }


        /// <summary> 
        /// Get 2 rays from 3 points. The Rays interception Point is the Middlepoint of the Arc
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="interPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        private static Ray[] RaysFromDescriptorPoints(Vector2 startPoint, Vector2 interPoint, Vector2 endPoint, Direction direction)
        {

            Ray[] rays = new Ray[2];

            Vector2 vRay1 = new Vector2(startPoint, interPoint).GetOrthogonalVector(direction);    //Direction doesn't matter !?
            Vector2 vRay2 = new Vector2(interPoint, endPoint).GetOrthogonalVector(direction);      //Direction doesn't matter !?

            var ray1StartPoint = new LineSegment2(startPoint, interPoint).MiddlePoint;
            var ray2StartPoint = new LineSegment2(interPoint, endPoint).MiddlePoint;

            rays[0] = new Ray(vRay1, ray1StartPoint);
            rays[1] = new Ray(vRay2, ray2StartPoint);
            return rays;
        }



    }
}
