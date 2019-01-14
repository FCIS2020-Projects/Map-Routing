using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Map_Routing
{
    class Node
    {
        double X;
        double Y;

        public Node(double x, double y)
        {
            X = x;
            Y = y;
        }
        public double GetX()
        {
            return X;
        }
        public double GetY()
        {
            return Y;
        }
    }
}
