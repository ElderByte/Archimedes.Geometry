using System.Collections.Generic;
using System.Linq;
using Archimedes.Geometry.DataStructures;
using Archimedes.Geometry.Primitives;

namespace Archimedes.Geometry.Algorithms
{
    /// <summary>
    /// 
    /// Find the convex hull of a point set in the plane
    /// 
    /// An implementation of Graham's (1972) point elimination algorithm,
    /// as modified by Andrew (1979) to find lower and upper hull separately.
    /// 
    /// This implementation correctly handles duplicate points, and
    /// multiple points with the same x-coordinate.
    /// 
    /// 1. Sort the points lexicographically by increasing (x,y), thus 
    ///    finding also a leftmost point L and a rightmost point R.
    /// 2. Partition the point set into two lists, upper and lower, according as
    ///    point is above or below the segment LR.  The upper list begins with 
    ///    L and ends with R; the lower list begins with R and ends with L.
    /// 3. Traverse the point lists clockwise, eliminating all but the extreme
    ///    points (thus eliminating also duplicate points).
    /// 4. Eliminate L from lower and R from upper, if necessary.
    /// 5. Join the point lists (in clockwise order) in an array.
    /// 
    ///
    /// Code bases upon Peter Sestoft (sestoft@itu.dk) reference implementation
    /// * Java 2000-10-07
    /// 
    /// Further modifications and tuning by P. Büttiker 2010-2012
    /// 
    /// </summary>
    public class ConvexHullBuilder
    {
        public static Polygon2 Convexhull(IEnumerable<Vector2> vertices) {
            var pts = vertices.ToArray();

            if (!pts.Any())
                return new Polygon2();

            

            // Sort points lexicographically by increasing (x, y)
            int n = pts.Length;
            Polysort.Quicksort(pts);


            var left = pts[0];
            var right = pts[n - 1];

            // Partition into lower hull and upper hull
            var lower = new CircularDoublyLinkedList<Vector2>(left);
            var upper = new CircularDoublyLinkedList<Vector2>(left);

            for (int i = 0; i < n; i++) {
                double det = Vector2.Area2(left, right, pts[i]);
                if (det > 0)
                    upper = upper.Append(new CircularDoublyLinkedList<Vector2>(pts[i]));
                else if (det < 0)
                    lower = lower.Prepend(new CircularDoublyLinkedList<Vector2>(pts[i]));
            }
            lower = lower.Prepend(new CircularDoublyLinkedList<Vector2>(right));
            upper = upper.Append(new CircularDoublyLinkedList<Vector2>(right)).Next;
            // Eliminate points not on the hull
            Eliminate(lower);
            Eliminate(upper);
            // Eliminate duplicate endpoints
            if (lower.Prev.Value.Equals(upper.Value))
                lower.Prev.Delete();
            if (upper.Prev.Value.Equals(lower.Value))
                upper.Prev.Delete();
            // Join the lower and upper hull
            var res = new Vector2[lower.Size() + upper.Size()];
            lower.CopyInto(res, 0);
            upper.CopyInto(res, lower.Size());
            return new Polygon2(res);
        }


        // Graham's scan
        private static void Eliminate(CircularDoublyLinkedList<Vector2> start)
        {
            CircularDoublyLinkedList<Vector2> v = start, w = start.Prev;
            bool fwd = false;
            while (v.Next != start || !fwd) {
                if (v.Next == w)
                    fwd = true;
                if (Vector2.Area2(v.Value, v.Next.Value, v.Next.Next.Value) < 0) // right turn
                    v = v.Next;
                else {                                       // left turn or straight
                    v.Next.Delete();
                    v = v.Prev;
                }
            }
        }
    }

    

    #region Polysort

    static class Polysort
    {
        private static void Swap<T>(T[] arr, int s, int t) {
            var tmp = arr[s]; arr[s] = arr[t]; arr[t] = tmp;
        }

        private static void Qsort<T>(T[] arr, int a, int b) 
            where T : IOrdered<T>{
            // sort arr[a..b]
            if (a < b) {
                int i = a, j = b;
                var x = arr[(i + j) / 2];
                do {
                    while (arr[i].Less(x)) i++;
                    while (x.Less(arr[j])) j--;
                    if (i <= j) {
                        Swap(arr, i, j);
                        i++; j--;
                    }
                } while (i <= j);
                Qsort<T>(arr, a, j);
                Qsort<T>(arr, i, b);
            }
        }

        /// <summary>
        /// Quicksort implementation 
        /// Hoare/Wirth
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        public static void Quicksort<T>(T[] arr) 
            where T : IOrdered<T>
        {
            Qsort<T>(arr, 0, arr.Length - 1);
        }
    }

    #endregion
}
