using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Archimedes.Geometry.Units
{
    public static class AngleParser
    {
        public static readonly string UnitValuePattern = string.Format(@"^(?: *)(?<Value>{0}) *(?<Unit>.+) *$", Parser.DoublePattern);

        public static Angle Parse(string s)
        {
            Match match = Regex.Match(s, UnitValuePattern);
            double d = Parser.ParseDouble(match.Groups["Value"]);
            var unit = ParseUnit(match.Groups["Unit"].Value);
            return new Angle(d, unit);
        }

        public static AngleUnit ParseUnit(string s)
        {
            var trim = s.Trim();
            switch (trim)
            {
                case "°":
                    return AngleUnit.Degrees;
                case "rad":
                    return AngleUnit.Radians;
                default:
                    throw new NotSupportedException("The unit text '" + trim + "' was not recocnized as valid angle unit!");
            }
        }
    }
}
