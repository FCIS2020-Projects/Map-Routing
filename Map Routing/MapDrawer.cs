using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Map_Routing
{
    class MapDrawer
    {
        double xMax;
        double yMax;
        Pen axesColor = new Pen(Color.Black, 3);
        Pen roadColor = new Pen(Color.White, 5);
        Pen pathColor = new Pen(Color.Yellow, 5);
        Font f;
        Brush fontColor = Brushes.Black;
        Brush nodeColor = Brushes.SkyBlue;
        Brush srcNodeColor = Brushes.DarkGreen;
        Brush destNodeColor = Brushes.DarkRed;
        Graphics bm;
        Bitmap map;
        PictureBox pictureBox;
        int sx, sy, ex, ey;
        public int bmWidth { get; set; }
        public int bmHeight { get; set; }

        Bitmap emptyMap;

        public MapDrawer(PictureBox pictureBox)
        {
            double maxx = int.MinValue;
            double maxy = int.MinValue;
            for (int i = 1; i < Map.points.Length; i++)
            {                
                maxx = Math.Max(maxx, Map.points[i].GetX());
                maxy = Math.Max(maxy, Map.points[i].GetY());
            }
            xMax = maxx;
            yMax = maxy;

            if (xMax < 10)
                bmWidth = (int)(xMax * 1000);
            else if (xMax < 100)
                bmWidth = (int)(xMax * 100);
            else if (xMax < 1000)
                bmWidth = (int)(xMax * 10);
            else
                bmWidth = (int)(xMax / 2);

            if (yMax < 10)
                bmHeight = (int)(yMax * 1000);
            else if (yMax < 100)
                bmHeight = (int)(yMax * 100);
            else if (yMax < 1000)
                bmHeight = (int)(yMax * 10);
            else
                bmHeight = (int)(yMax / 2);

            sx = bmWidth / 20 ; sy = bmHeight / 20; ex = bmWidth - sx; ey = bmHeight - sy;
            this.pictureBox = pictureBox;
            pictureBox.Width = bmWidth;
            pictureBox.Height = bmHeight;
            pictureBox.Location = new Point(0, 0);

            f = new Font("Microsoft Sans Serif", bmWidth / 100);
            map = new Bitmap(bmWidth, bmHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            bm = Graphics.FromImage(map);
            bm.Clear(Color.FromArgb(0xff, 0xe8, 0xe8, 0xe8));
        }


        public void DrawMap()
        {
            bm.DrawLine(axesColor, sx, sy, sx, ey);
            bm.DrawLine(axesColor, sx, ey, ex, ey);

            for (int i = 0; i <= 10; i++)
            {
                float x = (float)Math.Round(i * (xMax / 10), 2);
                bm.DrawString(x.ToString(), f, fontColor, sx + i * ((ex - sx) / 10) - 20, bmHeight - f.Height);

                float y = (float)Math.Round(i * (yMax / 10), 2);
                bm.DrawString(y.ToString(), f, fontColor, 0, ey - i * ((ey - sy) / 10) - f.Height);
            }
            for (int i = 1; i < Map.roads.Count; i++)
            {
                for (int j = 0; j < Map.roads[i].Count; j++)
                {
                    int p1 = i;
                    float x1 = ConvertX(Map.points[p1].GetX());
                    float y1 = ConvertY(Map.points[p1].GetY());

                    int p2 = Map.roads[i][j].GetTo();
                    float x2 = ConvertX(Map.points[p2].GetX());
                    float y2 = ConvertY(Map.points[p2].GetY());

                    bm.DrawLine(roadColor, x1, y1, x2, y2);
                }
            }
            for (int i = 1; i < Map.points.Length; i++)
            {
                float x = ConvertX(Map.points[i].GetX());
                float y = ConvertY(Map.points[i].GetY());
                int r = 20000 / bmWidth;
                bm.FillEllipse(nodeColor, x - r/2, y - r/2, r, r);
                //bm.DrawString(i.ToString(), f, fontColor, x - 2, y - 2);
            }
            emptyMap = new Bitmap(map);
            pictureBox.Image = map;
        }
        public void DeleteMap()
        {
            emptyMap.Dispose();
            map.Dispose();
            bm.Dispose();
        }

        public void DrawSrcAndDest(Node src, Node dest)
        {
            if (src != null && dest != null)
            {
                float x1 = ConvertX(src.GetX());
                float y1 = ConvertY(src.GetY());
                bm.FillEllipse(srcNodeColor, x1 - 10, y1 - 10, 20, 20);

                float x2 = ConvertX(dest.GetX());
                float y2 = ConvertY(dest.GetY());
                bm.FillEllipse(destNodeColor, x2 - 10, y2 - 10, 20, 20);
            }
            pictureBox.Image = map;
        }
        public void DeleteSrcAndDest()
        {
            map.Dispose();
            map = new Bitmap(emptyMap);
            bm.Dispose();
            bm = Graphics.FromImage(map);
            pictureBox.Image = map;
        }

        public void DrawPath(List<int> path, Node src, Node dest)
        {
            int p1;
            int p2 = path[1];
            float x1 = ConvertX(src.GetX());
            float y1 = ConvertY(src.GetY());

            float x2 = ConvertX(Map.points[p2].GetX());
            float y2 = ConvertY(Map.points[p2].GetY());
            bm.DrawLine(pathColor, x1, y1, x2, y2);
            for (int i = 1; i < path.Count - 2; i++)
            {
                p1 = path[i];
                p2 = path[i + 1];

                x1 = ConvertX(Map.points[p1].GetX());
                y1 = ConvertY(Map.points[p1].GetY());

                x2 = ConvertX(Map.points[p2].GetX());
                y2 = ConvertY(Map.points[p2].GetY());
                bm.DrawLine(pathColor, x1, y1, x2, y2);
            }
            p1 = path[path.Count - 2];

            x1 = ConvertX(Map.points[p1].GetX());
            y1 = ConvertY(Map.points[p1].GetY());

            x2 = ConvertX(dest.GetX());
            y2 = ConvertY(dest.GetY());
            bm.DrawLine(pathColor, x1, y1, x2, y2);
            pictureBox.Image = map;
        }
        private float ConvertX(double x)
        {
            return sx + (float)x * (ex - sx) / (float)xMax;
        }
        private float ConvertY(double y)
        {
            return map.Size.Height - (sy + (float)y * (ey - sy) / (float)yMax);
        }
    }
}
