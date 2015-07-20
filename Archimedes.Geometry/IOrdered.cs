using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archimedes.Geometry
{
    /// <summary>
    /// Implementors are lexically orderable
    /// </summary>
    public interface IOrdered<T>
    {
        /// <summary>
        /// Lexically check if the given Vector is less than this one
        /// </summary>
        /// <param name="that">The element to check against</param>
        /// <returns></returns>
        bool Less(IOrdered<T> that);
    }
}
