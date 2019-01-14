using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Map_Routing
{
    class Map
    {
        public static List<List<Road>> roads;
        public static Node[] points;
        Node src;
        Node dest;
        double R;

       static int[] prev;
       static double[] dist;
       static List<int> path;

        public Map(Node src, Node dest, double R)
        {
            this.src = src;
            this.dest = dest;
            this.R = R / 1000;
        }
        public Node GetSrc()
        {
            return src;
        }
        public Node GetDest()
        {
            return dest;
        }
        public double ShortestTime() //O
        {
            List<List<Road>> graph = roads; // O(1)
            
            int n = points.Length + 1;

            double ysrc = src.GetY();
            double ydest = dest.GetY();
            double xsrc = src.GetX();
            double xdest = dest.GetX();
            for (int i = 1; i < points.Length; i++) //O(V) : V = NUM OF points
            {
                double x = points[i].GetX();
                double y = points[i].GetY();
                double distFromSrc = Math.Sqrt((xsrc - x) * (xsrc - x) + (ysrc - y) * (ysrc - y));
                double distToDest = Math.Sqrt((xdest - x) * (xdest - x) + (ydest - y) * (ydest - y));

                if (distFromSrc <= R)
                {
                    graph[0].Add(new Road(i, distFromSrc, 5));
                    graph[i].Add(new Road(0, distFromSrc, 5));
                }
                if (distToDest <= R)
                {
                    graph[n - 1].Add(new Road(i, distToDest, 5));
                    graph[i].Add(new Road(n - 1, distToDest, 5));
                }
            }

            double[] time = new double[n];
            bool[] vis = new bool[n];
            dist = new double[n];
            prev = new int[n];
            for (int i = 0; i < n; i++) // O(V)
            {
                time[i] = float.MaxValue;
                vis[i] = false;
                prev[i] = -1;
            }
            SortedSet<Road> set = new SortedSet<Road>(new Road.RoadsComparer());
            time[0] = 0;
            vis[0] = true;
            set.Add(new Road(0, 0)); // O(log V)

            while (set.Count != 0) //O(E log V) : E = S + R
            {
                if (vis[n - 1])
                   break;
                Road top = set.Min; //O(1)
                vis[top.GetTo()] = true; //O(1)
                set.Remove(set.Min); //O(log V)

                for (int i = 0; i < graph[top.GetTo()].Count; i++)  //O(V)
                {
                    Road NR = graph[top.GetTo()][i];

                    if (vis[NR.GetTo()])
                        continue;

                    if (time[NR.GetTo()] > time[top.GetTo()] + NR.GetTime())
                    {
                        time[NR.GetTo()] = time[top.GetTo()] + NR.GetTime();
                        dist[NR.GetTo()] = dist[top.GetTo()] + NR.GetLength();
                        prev[NR.GetTo()] = top.GetTo();
                        set.Add(new Road(NR.GetTo(),time[NR.GetTo()])); // O(log V)
                    }
                }
            }
            for (int i = 0; i < roads[0].Count; i++)
            {
                int r = roads[0][i].GetTo();
                roads[r].Remove(roads[r].Last());
            }
            roads[0].Clear();
            for (int i = 0; i < roads[n-1].Count; i++)
            {
                int r = roads[n - 1][i].GetTo();
                roads[r].Remove(roads[r].Last());
            }
            roads[n - 1].Clear();

            return time[n-1];
        }
        public List<int> ShortestPath()
        {
            path = new List<int>(); // O(1)
            path.Add(prev.Count() - 1); //O(1)
            int i = prev.Count() - 1; //O(1)
            while (prev[i] != -1) //O(V)
            {
                path.Add(prev[i]);
                i = prev[i];
            }
            path.Reverse(); //O(V)
            return path;
        }
        public double TotalDistance()
        {
            return dist[dist.Count() - 1]; //O(1)
        }
        public double VehicleDistance()
        {
            return dist[prev[dist.Count()-1]] - dist[path[1]]; //O(1)
        }
        public double WalkingDistance()
        {
            return TotalDistance() - VehicleDistance(); //O(1)
        }
    }
}
