using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Map_Routing
{
    public partial class Form1 : Form
    {
        List<Map> maps;
        int q;
        MapDrawer md;
        public Form1()
        {
            InitializeComponent();
            maps = new List<Map>();
            q = 0;
        }
        private void Button1_Click(object sender, EventArgs e) // Choose Map FILE - Read from File TO Data Structure
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            textBox1.Text = openFileDialog1.FileName;
            FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            int N = int.Parse(sr.ReadLine());
            List<List<Road>> roads = new List<List<Road>>(N + 2);
            Node[] points = new Node[N + 1];
            for(int i = 0; i < N + 2; i++) //Θ(V)
            {
                roads.Add(new List<Road>());
            }
            for(int i = 0; i < N; i++)
            {
                string[] s = sr.ReadLine().Split(' ');
                points[int.Parse(s[0]) + 1] = new Node(double.Parse(s[1]), double.Parse(s[2]));
            }
            int M = int.Parse(sr.ReadLine());
            for(int i = 0; i < M; i++) 
            {
                string[] s = sr.ReadLine().Split(' ');
                int p1 = int.Parse(s[0]) + 1;
                int p2 = int.Parse(s[1]) + 1;
                roads[p1].Add(new Road(p2, double.Parse(s[2]), double.Parse(s[3])));
                roads[p2].Add(new Road(p1, double.Parse(s[2]), double.Parse(s[3])));
            }
            Map.roads = roads;
            Map.points = points;
            sr.Close();
            fs.Close();
            if(md != null)
                md.DeleteMap();
            md = new MapDrawer(pictureBox1);
            md.DrawMap();
        }

        private void Button2_Click(object sender, EventArgs e) // Choose Query FILE - Read From File To Data Structure
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            textBox2.Text = openFileDialog1.FileName;
            FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            int N = int.Parse(sr.ReadLine());
            maps.Clear();
            for(int i = 0; i < N; i++)
            {
                string[] s = sr.ReadLine().Split(' ');
                Node src = new Node(double.Parse(s[0]), double.Parse(s[1]));
                Node dest = new Node(double.Parse(s[2]), double.Parse(s[3]));
                double R = double.Parse(s[4]);
                Map map = new Map(src, dest, R);
                maps.Add(map);
            }
            q = 0;
            
            md.DrawSrcAndDest(maps[q].GetSrc(), maps[q].GetDest());
            label8.Text = q.ToString();
            sr.Close();
            fs.Close();
        }

        private void button3_Click(object sender, EventArgs e) // Choose Query FILE
        {
            if(q+1 < maps.Count)
            {
                q++;
                md.DeleteSrcAndDest();
                md.DrawSrcAndDest(maps[q].GetSrc(), maps[q].GetDest());
                label8.Text = q.ToString();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(q-1 >=0)
            {
                q--;
                md.DeleteSrcAndDest();
                md.DrawSrcAndDest(maps[q].GetSrc(), maps[q].GetDest());
                label8.Text = q.ToString();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            double t = DateTime.Now.TimeOfDay.TotalMilliseconds;
            double result = Math.Round(maps[q].ShortestTime() * 60, 2);
            int extime = (int)Math.Round(DateTime.Now.TimeOfDay.TotalMilliseconds - t);
            label7.Text = "Execution Time: " + extime.ToString();
            label3.Text = "Shortest Time: "+ result.ToString("0.00");

            List<int> path = maps[q].ShortestPath();
            double totalDistance = Math.Round(maps[q].TotalDistance(),2);
            double walkingDistance = Math.Round(maps[q].WalkingDistance(),2);
            double vehicleDistance = Math.Round(maps[q].VehicleDistance(),2);
            label4.Text = "Total Distance: " + totalDistance.ToString("0.00");
            label5.Text = "Walking Distance: " + walkingDistance.ToString("0.00");
            label6.Text = "Vehicle Distance: " + vehicleDistance.ToString("0.00");

            PathsFlow.FlowDirection = FlowDirection.LeftToRight;
            PathsFlow.WrapContents = false;
            PathsFlow.AutoScroll = true;
            PathsFlow.Controls.Clear();

            for (int i = 0; i <= path.Count-1; i++)
            {
                Label p = new Label();
                if (i == 0)
                    p.Text = "src";
                else if (i == path.Count - 1)
                    p.Text = "dest";
                else
                    p.Text = (path[i]-1).ToString();
                p.Width = 50; 
                PathsFlow.Controls.Add(p);
            }
            md.DrawPath(path, maps[q].GetSrc(), maps[q].GetDest());

        }
        private void button6_Click(object sender, EventArgs e)
        {
            double totalExtime = 0;
            FileStream fs = new FileStream("output.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            for(int i = 0; i < maps.Count; i++)
            {
            
                double t = DateTime.Now.TimeOfDay.TotalMilliseconds;
                double result = Math.Round(maps[i].ShortestTime() * 60, 2);
                double extime = Math.Round(DateTime.Now.TimeOfDay.TotalMilliseconds - t);

                List<int> path = maps[i].ShortestPath();
                double totalDistance = Math.Round(maps[i].TotalDistance(),2);
                double walkingDistance = Math.Round(maps[i].WalkingDistance(),2);
                double vehicleDistance = Math.Round(maps[i].VehicleDistance(),2);
                totalExtime += extime;
                sw.WriteLine(result.ToString("0.00") + " mins");
                sw.WriteLine(totalDistance.ToString("0.00") + " km");
                sw.WriteLine(walkingDistance.ToString("0.00") + " km");
                sw.WriteLine(vehicleDistance.ToString("0.00") + " km");
                for(int j = 1; j < path.Count - 1; j++)
                {
                    sw.Write((path[j] - 1).ToString() + " ");
                }
                sw.WriteLine();
                sw.WriteLine(extime + " ms");
                sw.WriteLine();
            }
            sw.WriteLine(Math.Round(totalExtime) +" ms");
            
            sw.Close();
            fs.Close();

        }
        Point mouseDown;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                mouseDown = e.Location;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mouseNow = e.Location;

                int deltaX = mouseNow.X - mouseDown.X;
                int deltaY = mouseNow.Y - mouseDown.Y;

                int newX = pictureBox1.Location.X + deltaX;
                int newY = pictureBox1.Location.Y + deltaY;

                if (newX < panel1.Width - pictureBox1.Width)
                    newX = panel1.Width - pictureBox1.Width;
                if (newY < panel1.Height - pictureBox1.Height)
                    newY = panel1.Height - pictureBox1.Height;
                if (newX > 0)
                    newX = 0;
                if (newY > 0)
                    newY = 0;

                pictureBox1.Location = new Point(newX, newY);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            int newWidth = pictureBox1.Width;
            int newHeight = pictureBox1.Height;
            int newX = pictureBox1.Location.X;
            int newY = pictureBox1.Location.Y;

            if (e.Delta > 0)
            {
                if(newHeight < md.bmHeight)
                {
                    newWidth = pictureBox1.Width + (pictureBox1.Width / 10);
                    newHeight = pictureBox1.Height + (pictureBox1.Height / 10);
                    newX = pictureBox1.Location.X - ((pictureBox1.Size.Width / 10) / 2);
                    newY = pictureBox1.Location.Y - ((pictureBox1.Size.Height / 10) / 2);
                }
            }
            else if(e.Delta < 0)
            {
                if (newHeight > panel1.Height)
                {
                    newWidth = pictureBox1.Width - (pictureBox1.Width / 10);
                    newHeight = pictureBox1.Height - (pictureBox1.Height / 10);
                    newX = pictureBox1.Location.X + ((pictureBox1.Size.Width / 10) / 2);
                    newY = pictureBox1.Location.Y + ((pictureBox1.Size.Height / 10) / 2);
                }
            }

            if (newX < panel1.Width - pictureBox1.Width)
                newX = panel1.Width - pictureBox1.Width;
            if (newY < panel1.Height - pictureBox1.Height)
                newY = panel1.Height - pictureBox1.Height;
            if (newX > 0)
                newX = 0;
            if (newY > 0)
                newY = 0;

            pictureBox1.Size = new Size(newWidth, newHeight);
            pictureBox1.Location = new Point(newX, newY);
        }

    }  
}
