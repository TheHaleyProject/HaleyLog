using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Haley.Models;

namespace Haley.Abstractions
{
    public class LogEqualityComparer : IEqualityComparer<LogData>
    {
        public bool Equals(LogData x, LogData y)
        {
            return (x.Id == y.Id);
        }

        public int GetHashCode(LogData obj)
        {
            return obj.GetHashCode();
        }
    }
}
