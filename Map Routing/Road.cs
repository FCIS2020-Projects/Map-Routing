using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Map_Routing
{
    class Road
    {
        int To;
        double Length;
        double Speed;
        double Time;

        public class RoadsComparer : IComparer<Road>
        {
            public int Compare(Road x, Road y)
            {
                int q = x.Time.CompareTo(y.Time);
                if (q == 0)
                {
                    q = x.To.CompareTo(y.To);
                }

                return q;
            }
        }
        public Road(int to, double length, double speed)
        {
            To = to;
            Length = length;
            Speed = speed;
            Time = length / speed;
        }
        public Road(int to, double time)
        {
            To = to;
            Time = time;
        }
        public int GetTo()
        { return To; }
        public double GetLength()
        { return Length; }
        public double GetSpeed()
        { return Speed; }
        public double GetTime()
        { return Time; }
    }
}
