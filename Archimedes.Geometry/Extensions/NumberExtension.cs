using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archimedes.Geometry.Extensions
{
    public static class NumberExtension
    {
        #region double Ext

        public static bool IsSignEqual(this double MyNum, double MyNum2) {
            return (MyNum.IsNegative() == MyNum2.IsNegative());    
        }
        public static bool IsNegative(this double MyNum) {
            return (MyNum < 0);
        }
        public static bool IsPositive(this double MyNum) {
            return !IsNegative(MyNum);
        }

        #endregion

        #region int Ext

        public static bool IsSignEqual(this int MyNum, int MyNum2) {
            return (MyNum.IsNegative() == MyNum2.IsNegative());
        }
        public static bool IsNegative(this int MyNum) {
            return (MyNum < 0);
        }
        public static bool IsPositive(this int MyNum) {
            return !IsNegative(MyNum);
        }
        
        #endregion

        #region float Ext
        public static bool IsSignEqual(this float MyNum, float MyNum2) {
            return (MyNum.IsNegative() == MyNum2.IsNegative());
        }
        public static bool IsNegative(this float MyNum) {
            return (MyNum < 0);
        }
        public static bool IsPositive(this float MyNum) {
            return !IsNegative(MyNum);
        }
        #endregion

    }
}
