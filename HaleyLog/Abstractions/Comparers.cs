using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Haley.Abstractions
{
    public class LogEqualityComparer : IEqualityComparer<ILogData>
    {
        public bool Equals(ILogData x, ILogData y)
        {
            return (x.Id == y.Id);
        }

        public int GetHashCode(ILogData obj)
        {
            return obj.GetHashCode();
        }
    }
}
