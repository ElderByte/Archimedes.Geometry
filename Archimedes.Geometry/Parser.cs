using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Archimedes.Geometry
{
    internal static class Parser
    {
        public static readonly string DoublePattern = @"[+-]?\d*(?:[.,]\d+)?(?:[eE][+-]?\d+)?";
        public const string SeparatorPattern = @" *[,;] *";
        public static readonly string Vector3DPattern = string.Format(@"^ *\(?(?<x>{0}){1}(?<y>{0}){1}(?<z>{0})\)? *$", DoublePattern, SeparatorPattern);
        public static readonly string Vector2DPattern = string.Format(@"^ *\(?(?<x>{0}){1}(?<y>{0})?\)? *$", DoublePattern, SeparatorPattern);
        private static string _item3DPattern = Vector3DPattern.Trim('^', '$');
        public static readonly string PlanePointVectorPattern = string.Format(@"^ *p: *{{(?<p>{0})}} *v: *{{(?<v>{0})}} *$", _item3DPattern);
        public static readonly string PlaneAbcdPattern = string.Format(@"^ *\(?(?<a>{0}){1}(?<b>{0}){1}(?<c>{0}){1}(?<d>{0})\)? *$", DoublePattern, SeparatorPattern);

        /// <summary>
        /// Expects a string like (2, 3),(3,5),(4,5)
        /// </summary>
        /// <param name="verticesString"></param>
        /// <returns></returns>
        internal static List<double[]> ParseItems2D(string verticesString)
        {
            var items = new List<double[]>();

            var chars = verticesString.ToCharArray();
            int pos = 0;
            for (int i = 0; i < verticesString.Length; i++)
            {
                if (chars[i] == '(')
                {
                    pos = i;
                }
                if (chars[i] == ')')
                {
                    var contentStr = verticesString.Substring(pos, i - pos);
                    var item = ParseItem2D(contentStr);
                    items.Add(item);
                }
            }

            return items;
        }


        internal static double[] ParseItem2D(string vectorString)
        {
            var match = Regex.Match(vectorString, Vector2DPattern);
            Group[] ss = { match.Groups["x"], match.Groups["y"] };
            double[] ds = ss.Select(ParseDouble).ToArray();
            return ds;
        }

        internal static double[] ParseItem3D(string vectorString)
        {
            var match = Regex.Match(vectorString, Vector3DPattern);
            Group[] ss = { match.Groups["x"], match.Groups["y"], match.Groups["z"] };
            double[] ds = ss.Select(x => double.Parse(x.Value.Replace(',', '.'), CultureInfo.InvariantCulture)).ToArray();
            return ds;
        }

        public static double ParseDouble(Group @group)
        {
            if (@group.Captures.Count != 1)
            {
                throw new ArgumentException("Expected single capture");
            }

            return ParseDouble(@group.Value);
        }

        public static double ParseDouble(string s)
        {
            return double.Parse(s.Replace(',', '.'), CultureInfo.InvariantCulture);
        }

    }
}
