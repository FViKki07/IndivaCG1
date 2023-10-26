using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.LinkLabel;

namespace Indiva1
{
    public partial class Form1 : Form
    {
        PictureBox pb;
        private Graphics g;
        private Bitmap bmp;
        List<Point> polygonPoints = new List<Point>();
        Point startPoint;
        
        List<List<Point>> polygons = new List<List<Point>>();
        List<Point> l = new List<Point>();
        public Form1()
        {
            InitializeComponent();
            pb = pictureBox1;
            bmp = new Bitmap(pb.Width, pb.Height);
            pictureBox1.Image = bmp;
            g = Graphics.FromImage(pictureBox1.Image);

            g.Clear(Color.White);

            startPoint = Point.Empty;

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
            polygonPoints.Add(startPoint);
            startPoint = Point.Empty;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            g.Clear(Color.White);
            if (polygonPoints.Count == 1)
            {
                g.DrawRectangle(Pens.Red, polygonPoints[0].X, polygonPoints[0].Y, 2, 2);
            }

            if (polygonPoints.Count >= 2)
            {
                for (int i = 0; i < polygonPoints.Count - 1; i++)
                {
                    g.DrawLine(Pens.Red, polygonPoints[i], polygonPoints[i + 1]);
                }
                g.DrawLine(Pens.Red, polygonPoints[0], polygonPoints[polygonPoints.Count() - 1]);
            }
            
            List<Color> colors = new List<Color> { Color.Red, Color.Blue, Color.Green, Color.HotPink, Color.Chocolate, Color.DarkViolet, Color.Gold};
            for(int i = 0;i< l.Count(); i++)
            {
                g.DrawEllipse(new Pen(Color.Black), l[i].X - 1, l[i].Y - 1, 3, 3);
            }
            for (int i = 0; i < polygons.Count(); i++)
            {
                for(int j =0;j < polygons[i].Count() - 1;j++)
                {
                    g.DrawLine(new Pen(colors[i ]), polygons[i][j], polygons[i][j+1]);
                }
               g.DrawLine(new Pen(colors[i]), polygons[i][0], polygons[i][polygons[i].Count() - 1]);
            }
            pictureBox1.Invalidate();

        }

        private int GetScalarMult(Point a, Point b)
        {
            return a.X * b.X + a.Y * b.Y;
        }


        private Point findPoint(Line l1, Line l2)
        {

            Point a = l1.leftP;
            Point b = l1.rightP;
            Point c = l2.leftP;
            Point d = l2.rightP;


            Point ba = l1.Diff();
            Point dc = l2.Diff();

            Point n = new Point(-dc.Y, dc.X);

            int perp = GetScalarMult(n, ba);
            if (perp != 0)
            {
                Point ac = new Point(a.X - c.X, a.Y - c.Y);
                float t = -1 * GetScalarMult(n, ac) * 1.0f / perp;

                Point k = new Point(-ba.Y, ba.X);
                float u = -1 * GetScalarMult(k, ac) * 1.0f / perp;

                if (u >= 0 && u < 1 && t >= 0 && t <= 1)
                {
                    Point intersection = new Point((int)((b.X - a.X) * t + a.X), (int)(t * (b.Y - a.Y) + a.Y));
                    return intersection;
                }

            }

            return Point.Empty;
        }

        private bool Bending(int i, List<Point> sortedPolygon, ref string side)
        {
            
            int prev = (i - 1) % sortedPolygon.Count;
            int next = (i + 1) % sortedPolygon.Count;
            if (i == 0)
                prev = sortedPolygon.Count - 1;
            if (i == sortedPolygon.Count - 1) next = 0;
            Point p1 = sortedPolygon[prev];
            Point p2 = sortedPolygon[i];
            Point p3 = sortedPolygon[next];
            
            double angle1 = Math.Atan2(p1.Y - p2.Y, p1.X - p2.X);
            double angle2 = Math.Atan2(p3.Y - p2.Y, p3.X - p2.X);
            double angle = (angle1 - angle2) * 180 / Math.PI;

            if (angle < 0)
            {
                angle += 360; 
            }

            if(angle > 180 ){
                if(p1.X < p2.X && p3.X < p2.X)
                {
                    side = "right";
                    return true;
                }
                else if(p1.X > p2.X && p3.X > p2.X)
                {
                    side = "left"; return true;
                }
            }


            return false;

        }

        // сверху вниз
        public bool wherePointTop(Line line, Point p)
        {
            // вектор, представляющий направление и длину ребра
            int line_temp_x = line.leftP.X - line.rightP.X;
            int line_temp_Y = line.leftP.Y - line.rightP.Y;

            // вектор от правого конца ребра до точки p
            int user_temp_x = p.X - line.rightP.X;
            int user_temp_y = p.Y - line.rightP.Y;

            int sin = user_temp_y * line_temp_x - user_temp_x * line_temp_Y;
            if (line.leftP != line.rightP)
            {
                if (sin > 0)
                    return true;
                else if (sin < 0)
                    return false;

            }
            return true;
        }

        // снизу вверх true - riht false - left
        public bool wherePointBottom(Line line, Point p)
        {
            int line_temp_x = line.leftP.X - line.rightP.X;
            int line_temp_y = line.rightP.Y - line.leftP.Y; // изменение направления вектора по оси y

            int user_temp_x = p.X - line.rightP.X;
            int user_temp_y = line.rightP.Y - p.Y; // изменение направления вектора по оси y

            int sin = user_temp_y * line_temp_x - user_temp_x * line_temp_y;
            if (line.leftP != line.rightP)
            {
                if (sin > 0)
                   return true;
                else if (sin < 0)
                    return false;
               
            }
            return true;
        }

        static double CalculateDistance(Point p1,Point p2)
        {
            double deltaX = p2.X - p1.X;
            double deltaY = p2.Y - p1.Y;
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            return distance;
        }

        private Point FindNearestLeft(Point bending, Line l1, Line l2)
        {
            Line vertex = new Line(new Point(bending.X, 2), new Point(bending.X, pictureBox1.Height - 2));
            List<Point> points = new List<Point>();
            if (vertex.rightP.Y > vertex.leftP.Y)
            {
                if (!wherePointTop(vertex, l1.rightP))
                    points.Add(l1.rightP);
                if (!wherePointTop(vertex, l1.leftP))
                    points.Add(l1.leftP);

                if (!wherePointTop(vertex, l2.rightP))
                    points.Add(l2.rightP);
                if (!wherePointTop(vertex, l2.leftP))
                    points.Add(l2.leftP);
            }
            else
            {
                if (!wherePointBottom(vertex, l1.rightP))
                    points.Add(l1.rightP);
                if (!wherePointBottom(vertex, l1.leftP))
                    points.Add(l1.leftP);

                if (!wherePointBottom(vertex, l2.rightP))
                    points.Add(l2.rightP);
                if (!wherePointBottom(vertex, l2.leftP))
                    points.Add(l2.leftP);
            }

            Point nearest = Point.Empty;
            double distance = Double.MaxValue;
            for (int i = 0; i < points.Count; i++)
            {
                double d = CalculateDistance(bending, points[i]);
                if (d < distance)
                {
                    distance = d;
                    nearest = points[i];
                }
            }
            return nearest;
        }

        private Point FindNearestRight(Point bending, Line l1, Line l2)
        {
            Line vertex = new Line(new Point(bending.X, 2), new Point(bending.X, pictureBox1.Height - 2));
            List<Point> points = new List<Point>();
            if (vertex.rightP.Y > vertex.leftP.Y)
            {
                if(wherePointTop(vertex, l1.rightP) )
                    points.Add(l1.rightP);
                if (wherePointTop(vertex, l1.leftP) )
                    points.Add(l1.leftP);

                if (wherePointTop(vertex, l2.rightP) )
                    points.Add(l2.rightP);
                if (wherePointTop(vertex, l2.leftP))
                    points.Add(l2.leftP);
            }
            else
            {
                if (wherePointBottom(vertex, l1.rightP) )
                    points.Add(l1.rightP);
                if (wherePointBottom(vertex, l1.leftP) )
                    points.Add(l1.leftP);

                if (wherePointBottom(vertex, l2.rightP))
                    points.Add(l2.rightP);
                if (wherePointBottom(vertex, l2.leftP))
                    points.Add(l2.leftP);
            }

            Point nearest = Point.Empty;
            double distance = Double.MaxValue;
            for(int i =0;i< points.Count; i++)
            {
                double d = CalculateDistance(bending, points[i]);
                if (d < distance)
                {
                    distance = d;
                    nearest = points[i];
                }
            }
            return nearest;
        }


        private List<List<Point>> CutPolygon(List<Point> pP)
        {
            List<Line> linePolygon = new List<Line>();
            Point start = pP.OrderBy(p => p.X).ThenBy(p => p.Y).First();
            int index1 = pP.IndexOf(start);
            List<Point> sortedPolygon = new List<Point>();
            int sum = 0;
            List<List<Point>> new_polygons = new List<List<Point>>();

            for (int i = 0; i < pP.Count; i++)
            {
                Point point1 = pP[i];
                Point point2 = pP[(i + 1) % pP.Count]; 

                sum += (point2.X - point1.X) * (point2.Y + point1.Y);
            }
            if (sum <= 0)
            {
                for (int i = 0; i < pP.Count; i++)
                {
                    sortedPolygon.Add(pP[(i + index1) % pP.Count]);
                    linePolygon.Add(new Line(sortedPolygon[i], pP[(i + 1 + index1) % pP.Count]));
                }
            }
            else
            {
                for (int i = 0; i < pP.Count; i++)
                {

                    int ind = (index1 - i);
                    int next_ind = ind - 1;
                    if(next_ind < 0) next_ind = pP.Count + next_ind;
                    if (ind < 0 ) ind = pP.Count + ind;
                    sortedPolygon.Add(pP[ind]);
                    linePolygon.Add(new Line(sortedPolygon[i], pP[next_ind]));
                }
            }

            
            for(int i =0;i< sortedPolygon.Count ;i++)
            {
                string fl = "";
                if (Bending(i, sortedPolygon, ref fl))
                {
                    var bending = sortedPolygon[i];
                    l.Add(sortedPolygon[i]);


                    Line vertex = new Line(new Point(bending.X, 0), new Point(bending.X, pictureBox1.Height));
                    Point up = bending, low = bending;
                    Line upL = vertex, lowL = vertex;
                    low.Y = pictureBox1.Height;
                    up.Y = 0;
                    for(int j =0; j < linePolygon.Count; j++)//находим пересечения 
                    {
                        Point new_point = findPoint(vertex, linePolygon[j]);
                        if (!new_point.IsEmpty)
                        {
                            int y = bending.Y - new_point.Y;
                            if (y > 0 && new_point.Y > up.Y)
                            {
                                up = new_point;
                                upL = linePolygon[j];
                            }
                            else if (y < 0 && new_point.Y < low.Y)
                            {
                                low = new_point;
                                lowL = linePolygon[j];
                            }
                        }
                    }
                    Point p = Point.Empty;
                    if (fl.Equals("right"))
                        p = FindNearestRight(bending, upL, lowL);
                    else if (fl.Equals("left"))
                        p = FindNearestLeft(bending, upL, lowL);
                    if (!p.IsEmpty)
                    {
                        int start_index = i;
                        int end = sortedPolygon.IndexOf(p);
                        List<Point> new_pol1 = new List<Point>();
                        while (start_index != end)
                        {
                            new_pol1.Add(sortedPolygon[start_index]);
                            start_index =(start_index + 1) % sortedPolygon.Count;
                        }
                        new_pol1.Add(sortedPolygon[start_index]);
                        List<Point> new_pol2 = new List<Point>();
                        start_index = i;
                        while (start_index != end)
                        {
                            new_pol2.Add(sortedPolygon[end]);
                            end = (end + 1 )% sortedPolygon.Count;
                        }
                        new_pol2.Add(sortedPolygon[end]);

                        //polygons.Add(new_pol1);
                        var pol = CutPolygon(new_pol1);
                        for(int k =0;k<pol.Count;k++)
                            new_polygons.Add(pol[k]);
                        if (pol.Count == 0)
                            new_polygons.Add(new_pol1);


                        pol = CutPolygon(new_pol2);
                        for (int k = 0; k < pol.Count; k++)
                            new_polygons.Add(pol[k]);
                        if (pol.Count == 0)
                            new_polygons.Add(new_pol2);
                        break;
                        // polygons.Add(new_pol2);
                        //l.Add(p);
                    }
                }


            }
            return new_polygons;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            polygonPoints.Clear();
            textBox1.Text = "";
            //sortedPolygon.Clear();
            l.Clear();
            polygons.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            polygons = CutPolygon(polygonPoints);
        }
    }



}