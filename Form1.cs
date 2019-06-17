using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _21317118
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            polygon = new Point[10010];
            polygonTemp = new Point[10010];
            g = this.CreateGraphics();
        }

        /// <summary>
        /// 变量设计
        /// </summary>
        Graphics g;
        Pen p = new Pen(Color.Red);
        private Point[] polygon;
        private Point[] polygonTemp;
        private bool isClick = false;
        private int indexOfPolygon = 0;
        private int indexOfPolygonTemp = 0;
        private bool isFinished = false;
        private int CodeLeft = 1;
        private int CodeRight = 2;
        private int CodeButtom = 4;
        private int CodeTop = 8;
        private int left = 50, right = 400, top = 100, buttom = 200;
        private Point P1 = new Point(100, 150), P2 = new Point(200, 200);
        Color BackColor1 = Color.White;
        Color ForeColor1 = Color.Black;

        public int MenuID, PressNum, FirstX, FirstY, OldX, OldY;
        Point[] group = new Point[100];

        public struct EdgeInfo
        {
            int ymax, ymin;//y的上下端点
            float k, xmin;//斜率倒数和x的下端
            //为四个内部变量设置的公共变量，方便外界存取数据
            public int YMax { get { return ymax; } set { ymax = value; } }
            public int YMmin { get { return ymin; } set { ymin = value; } }
            public float XMin { get { return xmin; } set { xmin = value; } }
            public float K { get { return k; } set { k = value; } }
            //构造结构函数，这里用来初始化结构变量
            public EdgeInfo(int x1, int y1, int x2, int y2)//下端点、上端点
            {
                ymax = y2; ymin = y1; xmin = (float)x1; k = (float)(x1 - x2) / (float)(y1 - y2);
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {

            Graphics g = CreateGraphics();
            Pen MyPen = new Pen(Color.Red, 1);
            if (MenuID == 1)
            {
                if (PressNum == 0)
                {
                    FirstX = e.X; FirstY = e.Y;
                    OldX = e.X; OldY = e.Y;
                }
                else
                {
                    DDALine1(FirstX, FirstY, e.X, e.Y);
                }
                PressNum++;
                if (PressNum >= 2) PressNum = 0;
            }
            if (MenuID == 2)
            {
                if (PressNum == 0)
                {
                    FirstX = e.X; FirstY = e.Y;
                    OldX = e.X; OldY = e.Y;
                }
                else
                {
                    MidpointLine(FirstX, FirstY, e.X, e.Y);
                }
                PressNum++;
                if (PressNum >= 2) PressNum = 0;
            }
            if (MenuID == 3)
            {
                if (PressNum == 0)
                {
                    FirstX = e.X; FirstY = e.Y;
                    OldX = e.X; OldY = e.Y;
                }
                else
                {
                    IntegerBresenhamLine(FirstX, FirstY, e.X, e.Y);
                }
                PressNum++;
                if (PressNum >= 2) PressNum = 0;
            }
            if (MenuID == 4)
            {
                if (PressNum == 0)
                {
                    FirstX = e.X; FirstY = e.Y;
                }
                else
                {
                    if (FirstX == e.X && FirstY == e.Y)
                        return;  //
                    IntegerBresenhamLine(FirstX, FirstY, e.X, e.Y);
                    Circle(FirstX, FirstY, e.X, e.Y);
                }
                PressNum++;
                if (PressNum >= 2) PressNum = 0;
            }
            if (MenuID == 5)//扫描线充填
            {
                if (e.Button == MouseButtons.Left)
                {
                    group[PressNum].X = e.X;
                    group[PressNum].Y = e.Y;
                    if (PressNum > 0)//依次画多边形
                    {
                        g.DrawLine(Pens.Red, group[PressNum - 1], group[PressNum]);
                    }
                    PressNum++;//记录多边形顶点数
                }
                if (e.Button == MouseButtons.Right)//如果按右键，结束顶点采集，开始充填
                {
                    g.DrawLine(Pens.Red, group[PressNum - 1], group[0]);//最后一条边
                    ScanLinFill();
                    PressNum = 0;//清零，为绘制下一个图形做准备
                }
            }
            if (MenuID == 6)
            {
                int x = e.X, y = e.Y;

                if (PressNum == 0)
                {
                    FirstX = e.X; FirstY = e.Y;
                    OldX = e.X; OldY = e.Y;
                }
                PressNum++;
                if (PressNum >= 2) PressNum = 0;
                P1.X = FirstX; P1.Y = FirstY;
                P2.X = OldX; P2.Y = OldY;
                Pen p = new Pen(Color.Blue);
                g.DrawLine(p, new Point(left, top), new Point(right, top));
                g.DrawLine(p, new Point(left, top), new Point(left, buttom));
                g.DrawLine(p, new Point(right, top), new Point(right, buttom));
                g.DrawLine(p, new Point(right, buttom), new Point(left, buttom));
                g.DrawLine(new Pen(Color.Gray), P1, P2);
                CohenSutherland(P1.X, P1.Y, P2.X, P2.Y);
            }
            if (MenuID == 7 || MenuID == 8)
            {
                if (PressNum == 0)//保留第一点
                {
                    FirstX = e.X; FirstY = e.Y; PressNum++;
                    // g.DrawRectangle(Pens.Red, FirstX, FirstY, 10, 10);

                }
                else//第二点调用裁剪算法
                {
                    // g.DrawRectangle(Pens.Yellow, FirstX, FirstY, 1, 1);
                    // g.DrawLine(Pens.Red, FirstX, FirstY, e.X, e.Y); 
                    Pen p = new Pen(Color.Blue);
                    g.DrawLine(p, new Point(left, top), new Point(right, top));
                    g.DrawLine(p, new Point(left, top), new Point(left, buttom));
                    g.DrawLine(p, new Point(right, top), new Point(right, buttom));
                    g.DrawLine(p, new Point(right, buttom), new Point(left, buttom));

                    if (MenuID == 7)
                        MidCut1(FirstX, FirstY, e.X, e.Y);
                    if (MenuID == 8)
                        LiangCut1(FirstX, FirstY, e.X, e.Y);
                    PressNum = 0;

                }
            }
            if (MenuID == 9)//扫描线充填
            {
                Graphics gg = CreateGraphics();
                if (e.Button == MouseButtons.Left)
                {

                    group[PressNum].X = e.X;
                    group[PressNum].Y = e.Y;
                    if (PressNum > 0)//依次画多边形
                    {

                        gg.DrawLine(Pens.Red, group[PressNum - 1], group[PressNum]);
                    }
                    PressNum++;//记录多边形顶点数
                }
                if (e.Button == MouseButtons.Right)//如果按右键，结束顶点采集，开始充填
                {
                    Pen p = new Pen(Color.Blue);
                    gg.DrawLine(Pens.Red, group[PressNum - 1], group[0]);//最后一条边

                    PressNum = 0;//清零，为绘制下一个图形做准备
                }
                drawEdge();
                RectangleF rec = new RectangleF(e.Location.X - 1, e.Location.Y - 1, 3, 3);
                g.DrawEllipse(p, rec);
                if (isClick == false)
                {
                    Point pp = new Point();
                    pp.X = e.Location.X;
                    pp.Y = e.Location.Y;
                    polygon[indexOfPolygon++] = pp;
                    isClick = true;
                }
                else
                {
                    Point pp = new Point();
                    pp.X = e.Location.X;
                    pp.Y = e.Location.Y;
                    if (distance(pp, polygon[0]) < 8)
                    {
                        for (int i = 0; i < indexOfPolygon; i++)
                            g.DrawLine(p, polygon[i], polygon[(i + 1) % indexOfPolygon]);
                        isFinished = true;
                    }
                    polygon[indexOfPolygon++] = pp;
                }
                if (e.Button == MouseButtons.Right)
                {
                    if (isClick)
                        SutherlandHodgmanClip();
                    for (int a = 0; a < 10010; a++)
                    {
                        polygon[a].X = 0;
                        polygon[a].Y = 0;
                    }
                    indexOfPolygon = 0;
                }
            }

        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            Graphics g = CreateGraphics();
            Pen Backpen = new Pen(BackColor1, 1);
            Pen Mypen = new Pen(ForeColor1, 1);
            if (MenuID == 1 && PressNum == 1)
            {
                if (!(e.X == OldX && e.Y == OldY))
                {
                    g.DrawLine(Backpen, FirstX, FirstY, OldX, OldY);
                    g.DrawLine(Mypen, FirstX, FirstY, e.X, e.Y);
                    OldX = e.X;
                    OldY = e.Y;
                }
            }
            if (MenuID == 2 && PressNum == 1)
            {
                if (!(e.X == OldX && e.Y == OldY))
                {
                    g.DrawLine(Backpen, FirstX, FirstY, OldX, OldY);
                    g.DrawLine(Mypen, FirstX, FirstY, e.X, e.Y);
                    OldX = e.X;
                    OldY = e.Y;
                }
            }
            if (MenuID == 3 && PressNum == 1)
            {
                if (!(e.X == OldX && e.Y == OldY))
                {
                    g.DrawLine(Backpen, FirstX, FirstY, OldX, OldY);
                    g.DrawLine(Mypen, FirstX, FirstY, e.X, e.Y);
                    OldX = e.X;
                    OldY = e.Y;
                }
            }
            if (MenuID == 4 && PressNum == 1)
            {
                if (!(e.X == OldX && e.Y == OldY))
                {
                    g.DrawLine(Backpen, FirstX, FirstY, OldX, OldY);
                    g.DrawLine(Mypen, FirstX, FirstY, e.X, e.Y);

                    double r = Math.Sqrt((FirstX - OldX) * (FirstX - OldX) + (FirstY - OldY) * (FirstY - OldY));
                    int r1 = (int)(r + 0.5);//取整
                    g.DrawEllipse(Backpen, FirstX - r1, FirstY - r1, 2 * r1, 2 * r1);//擦除旧圆


                    r = Math.Sqrt((FirstX - e.X) * (FirstX - e.X) + (FirstY - e.Y) * (FirstY - e.Y));
                    r1 = (int)(r + 0.5);
                    g.DrawEllipse(Mypen, FirstX - r1, FirstY - r1, 2 * r1, 2 * r1);
                    OldX = e.X;
                    OldY = e.Y;
                }
            }
            if (MenuID == 6 && PressNum == 1)
            {
                if (!(e.X == OldX && e.Y == OldY))
                {
                    g.DrawLine(Backpen, FirstX, FirstY, OldX, OldY);
                    g.DrawLine(Mypen, FirstX, FirstY, e.X, e.Y);
                    OldX = e.X;
                    OldY = e.Y;
                }
            }
            if (MenuID == 7 && PressNum == 1)
            {
                if (!(e.X == OldX && e.Y == OldY))
                {
                    g.DrawLine(Backpen, FirstX, FirstY, OldX, OldY);
                    g.DrawLine(Mypen, FirstX, FirstY, e.X, e.Y);
                    OldX = e.X;
                    OldY = e.Y;
                }
            }
            if (MenuID == 8 && PressNum == 1)
            {
                if (!(e.X == OldX && e.Y == OldY))
                {
                    g.DrawLine(Backpen, FirstX, FirstY, OldX, OldY);
                    g.DrawLine(Mypen, FirstX, FirstY, e.X, e.Y);
                    OldX = e.X;
                    OldY = e.Y;
                }
            }
            if (MenuID == 5 && PressNum != 0)
            {
                if (!(e.X == OldX && e.Y == OldY))
                {
                    g.DrawLine(Backpen, group[PressNum - 1].X, group[PressNum - 1].Y, OldX, OldY);
                    g.DrawLine(Mypen, group[PressNum - 1].X, group[PressNum - 1].Y, e.X, e.Y);
                    OldX = e.X;
                    OldY = e.Y;
                }
            }
            if (MenuID == 9 && PressNum != 0)
            {
                if (!(e.X == OldX && e.Y == OldY))
                {
                    g.DrawLine(Backpen, group[PressNum - 1].X, group[PressNum - 1].Y, OldX, OldY);
                    g.DrawLine(Mypen, group[PressNum - 1].X, group[PressNum - 1].Y, e.X, e.Y);
                    OldX = e.X;
                    OldY = e.Y;
                }
            }
        }

        private void dDAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuID = 1; PressNum = 0;
            Graphics g = CreateGraphics();
            g.Clear(BackColor1);
        }

        private void ScanLinFill()
        {
            Graphics ggg = CreateGraphics();
            EdgeInfo[] edgelist = new EdgeInfo[100];//建立边结构数组
            int j = 0, yu = 0, yd = 1024;//活化边的扫描范围从yu倒yd
            group[PressNum] = group[0];//将第一点复制为数组最后一点
            for (int i = 0; i < PressNum; i++)//建立每一条边的边结构
            {
                if (group[i].Y > yu) yu = group[i].Y;//找出图形最高点

                if (group[i].Y < yd) yd = group[i].Y;//找出图形最低点
                if (group[i].Y != group[i + 1].Y)//只处理非水平边
                {
                    if (group[i].Y > group[i + 1].Y)//下端点在前，上端点在后
                    {
                        edgelist[j++] = new EdgeInfo(group[i + 1].X, group[i + 1].Y, group[i].X, group[i].Y);
                    }
                    else
                    {
                        edgelist[j++] = new EdgeInfo(group[i].X, group[i].Y, group[i + 1].X, group[i + 1].Y);
                    }
                }
            }
            for (int y = yd; y < yu; y++)
            {
                var sorted =
                    from item in edgelist
                    where y < item.YMax && y >= item.YMmin
                    orderby item.XMin, item.K
                    select item;
                int flag = 0;
                foreach (var item in sorted)
                {
                    if (flag == 0)
                    {
                        FirstX = (int)(item.XMin + 0.5); flag++;
                    }
                    else
                    {
                        ggg.DrawLine(Pens.Green, (int)(item.XMin + 0.5), y, FirstX - 1, y);
                        flag = 0;
                    }
                }
                for (int i = 0; i < j; i++)
                {
                    if (y < edgelist[i].YMax - 1 && y > edgelist[i].YMmin)
                    {
                        edgelist[i].XMin += edgelist[i].K;
                    }
                }
            }
        }

        private void LiangCut1(int x1, int y1, int x2, int y2)
        {
            Graphics g = CreateGraphics();
            g.DrawLine(Pens.Red, x1, y1, x2, y2);
            float tsx, tsy, tex, tey;
            if (x1 == x2)
            {
                tsx = 0; tex = 1;
            }
            else if (x1 < x2)
            {
                tsx = (float)(left - x1) / (float)(x2 - x1);
                tex = (float)(right - x1) / (float)(x2 - x1);
            }
            else
            {
                tsx = (float)(right - x1) / (float)(x2 - x1);
                tex = (float)(left - x1) / (float)(x2 - x1);

            }
            if (y1 == y2)
            {
                tsy = 0; tey = 1;

            }
            else if (y1 < y2)
            {
                tsy = (float)(top - y1) / (float)(y2 - y1);
                tey = (float)(buttom - y1) / (float)(y2 - y1);

            }
            else
            {
                tsy = (float)(buttom - y1) / (float)(y2 - y1);
                tey = (float)(top - y1) / (float)(y2 - y1);
            }
            tsx = Math.Max(tsx, tsy);
            tsx = Math.Max(tsx, 0);
            tex = Math.Min(tex, tey);
            tex = Math.Min(tex, 1);
            if (tsx < tex)
            {
                int xx1, yy1, xx2, yy2;
                xx1 = (int)(x1 + (x2 - x1) * tsx);
                yy1 = (int)(y1 + (y2 - y1) * tsx);
                xx2 = (int)(x1 + (x2 - x1) * tex);
                yy2 = (int)(y1 + (y2 - y1) * tex);
                Pen MyPen = new Pen(Color.Yellow, 3);
                g.DrawLine(MyPen, xx1, yy1, xx2, yy2);
            }
        }

        private void MidpointLine(int x0, int y0, int x1, int y1)
        {
            Graphics g = CreateGraphics();
            int a, b, d1, d2, d, x, y;
            float m = (float)(y1 - y0) / (float)(x1 - x0);
            if (x0 < x1)
            {
                if (y0 > y1 && m > -1 && m < 0)
                {
                    a = y1 - y0; b = x1 - x0; d = 2 * a + b;
                    d1 = 2 * a; d2 = 2 * (a + b);
                    x = x0;
                    y = y0;
                    for (x = x0; x < x1; x++)
                    {
                        if (d < 0)
                        { y--; d += d2; g.DrawRectangle(Pens.Red, x, y, 1, 1); }
                        else
                        { d += d1; g.DrawRectangle(Pens.Red, x, y, 1, 1); }
                    }
                }
                if (y0 < y1 && m < 1 && m > 0)
                {
                    a = y0 - y1; b = x1 - x0; d = 2 * a + b;
                    d1 = 2 * a; d2 = 2 * (a + b);
                    x = x0;
                    y = y0;
                    for (x = x0; x < x1; x++)
                    {
                        if (d < 0)
                        { y++; d += d2; g.DrawRectangle(Pens.Red, x, y, 1, 1); }
                        else
                        { d += d1; g.DrawRectangle(Pens.Red, x, y, 1, 1); }
                    }
                }
                if (y0 > y1 && m < -1)
                {
                    a = y0 - y1; b = x0 - x1; d = a + 2 * b;
                    d1 = 2 * b; d2 = 2 * (a + b);
                    x = x0;
                    y = y0;
                    for (y = y0; y > y1; y--)
                    {
                        if (d < 0)
                        { x++; d += d2; g.DrawRectangle(Pens.Red, x, y, 1, 1); }
                        else
                        { d += d1; g.DrawRectangle(Pens.Red, x, y, 1, 1); }
                    }
                }
                if (y0 < y1 && m > 1)
                {
                    a = y1 - y0; b = x0 - x1; d = a + 2 * b;
                    d1 = 2 * b; d2 = 2 * (a + b);
                    x = x0;
                    y = y0;
                    for (y = y0; y < y1; y++)
                    {
                        if (d < 0)
                        { x++; d += d2; g.DrawRectangle(Pens.Red, x, y, 1, 1); }
                        else
                        { d += d1; g.DrawRectangle(Pens.Red, x, y, 1, 1); }
                    }
                }
                if (y0 == y1)
                {
                    y = y0;
                    for (x = x0; x < x1; x++)
                    {
                        g.DrawRectangle(Pens.Red, x, y, 1, 1);
                    }
                }
            }
            if (x0 > x1)
            {
                x = x0; x0 = x1; x1 = x;
                y = y0; y0 = y1; y1 = y;
                if (y0 > y1 && m > -1 && m < 0)
                {
                    a = y1 - y0; b = x1 - x0; d = 2 * a + b;
                    d1 = 2 * a; d2 = 2 * (a + b);
                    x = x0;
                    y = y0;
                    for (x = x0; x < x1; x++)
                    {
                        if (d < 0)
                        { y--; d += d2; g.DrawRectangle(Pens.Red, x, y, 1, 1); }
                        else
                        { d += d1; g.DrawRectangle(Pens.Red, x, y, 1, 1); }
                    }
                }
                if (y0 < y1 && m < 1 && m > 0)
                {
                    a = y0 - y1; b = x1 - x0; d = 2 * a + b;
                    d1 = 2 * a; d2 = 2 * (a + b);
                    x = x0;
                    y = y0;
                    for (x = x0; x < x1; x++)
                    {
                        if (d < 0)
                        { y++; d += d2; g.DrawRectangle(Pens.Red, x, y, 1, 1); }
                        else
                        { d += d1; g.DrawRectangle(Pens.Red, x, y, 1, 1); }
                    }
                }
                if (y0 > y1 && m < -1)
                {
                    a = y0 - y1; b = x0 - x1; d = a + 2 * b;
                    d1 = 2 * b; d2 = 2 * (a + b);
                    x = x0;
                    y = y0;
                    for (y = y0; y > y1; y--)
                    {
                        if (d < 0)
                        { x++; d += d2; g.DrawRectangle(Pens.Red, x, y, 1, 1); }
                        else
                        { d += d1; g.DrawRectangle(Pens.Red, x, y, 1, 1); }
                    }
                }
                if (y0 < y1 && m > 1)
                {
                    a = y1 - y0; b = x0 - x1; d = a + 2 * b;
                    d1 = 2 * b; d2 = 2 * (a + b);
                    x = x0;
                    y = y0;
                    for (y = y0; y < y1; y++)
                    {
                        if (d < 0)
                        { x++; d += d2; g.DrawRectangle(Pens.Red, x, y, 1, 1); }
                        else
                        { d += d1; g.DrawRectangle(Pens.Red, x, y, 1, 1); }
                    }
                }
                if (y0 == y1)
                {
                    y = y0;
                    for (x = x0; x > x1; x--)
                    {
                        g.DrawRectangle(Pens.Red, x, y, 1, 1);
                    }
                }
            }
            if (x0 == x1)
            {
                x = x0;
                if (y0 < y1)
                    for (y = y0; y < y1; y++)
                    {
                        g.DrawRectangle(Pens.Red, x, y, 1, 1);
                    }
                else if (y0 > y1)
                    for (y = y0; y > y1; y--)
                    {
                        g.DrawRectangle(Pens.Red, x, y, 1, 1);
                    }
            }
        }

        private void MidCut1(int x1, int y1, int x2, int y2)
        {
            Graphics g = CreateGraphics();
            g.DrawLine(Pens.Red, x1, y1, x2, y2);
            Point p1, p2;
            if (LineIsOutOfWindow(x1, y1, x2, y2))
                return;
            p1 = FindNearestPoint(x1, y1, x2, y2);
            if (PointIsOutOfWindow(p1.X, p1.Y))
                return;
            p2 = FindNearestPoint(x2, y2, x1, y1);
            Pen Mypen = new Pen(Color.Yellow, 3);
            g.DrawLine(Mypen, p1, p2);

        }

        private Point FindNearestPoint(int x1, int y1, int x2, int y2)
        {
            int x = 0, y = 0;
            Point p = new Point(0, 0);
            if (!PointIsOutOfWindow(x1, y1))
            {
                p.X = x1;
                p.Y = y1;
                return p;

            }
            while (!(Math.Abs(x1 - x2) <= 1 && Math.Abs(y1 - y2) <= 1))
            {
                x = (x1 + x2) / 2; y = (y1 + y2) / 2;
                if (LineIsOutOfWindow(x1, y1, x, y))
                {
                    x1 = x; y1 = y;
                }
                else
                {
                    x2 = x; y2 = y;
                }

            }
            if (PointIsOutOfWindow(x1, y1))
            {
                p.X = x2; p.Y = y2;

            }
            else
            {
                p.X = x1; p.Y = y1;
            }
            return p;
        }

        private bool LineIsOutOfWindow(int x1, int y1, int x2, int y2)
        {
            if (x1 < left && x2 < left)
                return true;
            else if (x1 > right && x2 > right)
                return true;
            else if (y1 > buttom && y2 > buttom)
                return true;
            else if (y1 < top && y2 < top)
                return true;
            else
                return false;
        }

        private bool PointIsOutOfWindow(int x, int y)
        {
            if (x < left)
                return true;
            else if (x > right)
                return true;
            else if (y > buttom)
                return true;
            else if (y < top)
                return true;
            else
                return false;

        }

        private void Circle(int x0, int y0, int x1, int y1)
        {
            int r, d, x, y;
            Graphics g = CreateGraphics();
            r = (int)(Math.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0)) + 0.5);
            x = 0; y = r; d = 3 - 2 * r;
            while (x <= y)
            {
                g.DrawRectangle(Pens.Red, x + x0, y + y0, 1, 1);
                g.DrawRectangle(Pens.Red, -x + x0, y + y0, 1, 1);
                g.DrawRectangle(Pens.Red, x + x0, -y + y0, 1, 1);
                g.DrawRectangle(Pens.Red, -x + x0, -y + y0, 1, 1);

                g.DrawRectangle(Pens.Red, y + x0, x + y0, 1, 1);
                g.DrawRectangle(Pens.Red, -y + x0, x + y0, 1, 1);
                g.DrawRectangle(Pens.Red, y + x0, -x + y0, 1, 1);
                g.DrawRectangle(Pens.Red, -y + x0, -x + y0, 1, 1);
                x = x + 1;
                if (d <= 0)
                {
                    d = d + 4 * x + 6;
                }
                else
                {
                    y = y - 1; d = d + 4 * (x - y) + 10;
                }
            }
        }

        private void bresenhamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuID = 3; PressNum = 0;
            Graphics g = CreateGraphics();
            g.Clear(BackColor1);
        }

        private void drawEdge()
        {
            Pen p = new Pen(Color.Blue);
            g.DrawLine(p, 200, 200, 200, 400);
            g.DrawLine(p, 200, 400, 400, 400);
            g.DrawLine(p, 400, 400, 400, 200);
            g.DrawLine(p, 400, 200, 200, 200);
        }

        private void 中点画线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuID = 2; PressNum = 0;
            Graphics g = CreateGraphics();
            g.Clear(BackColor1);

        }

        private void bresenhamToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            MenuID = 3; PressNum = 0;
            Graphics g = CreateGraphics();
            g.Clear(BackColor1);
        }

        private void 画圆ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuID = 4; PressNum = 0;
            Graphics g = CreateGraphics();
            g.Clear(BackColor1);
        }

        private void 扫描线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("右键结束绘制");
            MenuID = 5; PressNum = 0;
            Graphics g = CreateGraphics();
            g.Clear(BackColor1);
        }

        private void 直线裁剪ToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            MenuID = 6; PressNum = 0;
            Graphics g = CreateGraphics();
            g.Clear(BackColor1);
            Pen p = new Pen(Color.Blue);
            g.DrawLine(p, new Point(left, top), new Point(right, top));
            g.DrawLine(p, new Point(left, top), new Point(left, buttom));
            g.DrawLine(p, new Point(right, top), new Point(right, buttom));
            g.DrawLine(p, new Point(right, buttom), new Point(left, buttom));

        }

        private void 中点裁剪ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuID = 7; PressNum = 0;
            Graphics g = CreateGraphics();
            g.Clear(BackColor1);
            Pen p = new Pen(Color.Blue);
            g.DrawLine(p, new Point(left, top), new Point(right, top));
            g.DrawLine(p, new Point(left, top), new Point(left, buttom));
            g.DrawLine(p, new Point(right, top), new Point(right, buttom));
            g.DrawLine(p, new Point(right, buttom), new Point(left, buttom));

        }

        private void 梁友栋裁剪ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuID = 8; PressNum = 0;
            Graphics g = CreateGraphics();
            g.Clear(BackColor1);
            Pen p = new Pen(Color.Blue);
            g.DrawLine(p, new Point(left, top), new Point(right, top));
            g.DrawLine(p, new Point(left, top), new Point(left, buttom));
            g.DrawLine(p, new Point(right, top), new Point(right, buttom));
            g.DrawLine(p, new Point(right, buttom), new Point(left, buttom));

        }

        private void 多边形裁剪ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuID = 9; PressNum = 0;
            Graphics g = CreateGraphics();
            g.Clear(BackColor1);
            Pen p = new Pen(Color.Blue);
        }

        private bool inSide(int edge, Point pt)
        {
            //左垂直边
            if (edge == 1 && pt.X < 200 || edge == 3 && pt.X > 400 || edge == 2 && pt.Y < 200 || edge == 4 && pt.Y > 400)
                return false;
            return true;
        }

        private void SutherlandHodgmanClip()
        {
            //分四条边裁剪
            clipEdge(1);
            clipEdge(2);
            clipEdge(3);
            clipEdge(4);
            g.Clear(Color.White);
            drawEdge();
            for (int i = 0; i < indexOfPolygon; i++)
                g.DrawLine(p, polygon[i], polygon[(i + 1) % indexOfPolygon]);
        }

        private void clipEdge(int edge)
        {
            indexOfPolygonTemp = 0;
            bool isP1In, isP2In;
            for (int i = 0; i < indexOfPolygon; i++)
            {
                isP1In = inSide(edge, polygon[i]);
                isP2In = inSide(edge, polygon[(i + 1) % indexOfPolygon]);
                if (isP1In && isP2In)
                    polygonTemp[indexOfPolygonTemp++] = polygon[(i + 1) % indexOfPolygon];
                else if (isP1In)
                    polygonTemp[indexOfPolygonTemp++] = findIntersection(edge, polygon[i], polygon[(i + 1) % indexOfPolygon]);
                else if (isP2In)
                {
                    polygonTemp[indexOfPolygonTemp++] = findIntersection(edge, polygon[i], polygon[(i + 1) % indexOfPolygon]);
                    polygonTemp[indexOfPolygonTemp++] = polygon[(i + 1) % indexOfPolygon];
                }
            }
            indexOfPolygon = indexOfPolygonTemp;
            for (int i = 0; i < indexOfPolygon; i++)
                polygon[i] = polygonTemp[i];
        }

        private Point findIntersection(int edge, Point p1, Point p2)
        {
            Point pt = new Point();
            if (edge == 1)
            {
                pt.X = 200;
                pt.Y = (int)(p1.Y - (float)(p1.Y - p2.Y) * (p1.X - 200) / (p1.X - p2.X));
            }
            else if (edge == 3)
            {
                pt.X = 400;
                pt.Y = (int)(p1.Y - (float)(p1.Y - p2.Y) * (p1.X - 400) / (p1.X - p2.X));
            }
            else if (edge == 2)
            {
                pt.Y = 200;
                pt.X = (int)(p1.X - (float)(p1.X - p2.X) * (p1.Y - 200) / (p1.Y - p2.Y));
            }
            else if (edge == 4)
            {
                pt.Y = 400;
                pt.X = (int)(p1.X - (float)(p1.X - p2.X) * (p1.Y - 400) / (p1.Y - p2.Y));
            }
            return pt;
        }

        private double distance(Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        private void IntegerBresenhamLine(int x0, int y0, int x1, int y1)
        {
            int x, y, dx, dy, e, n;
            Graphics g = CreateGraphics();
            if (x0 < x1)
            {
                dx = x1 - x0; dy = y0 - y1; e = -dx;
                x = x0; y = y0;
                float m = (float)dy / (float)dx;
                if (y0 > y1 && m < 1 && m > 0)
                {
                    for (int i = 0; i <= dx; i++)
                    {
                        g.DrawRectangle(Pens.Red, x, y, 1, 1);
                        x++;
                        e = e + 2 * dy;
                        if (e >= 0) { y--; e = e - 2 * dx; }
                    }
                }
                if (y0 > y1 && m > 1)
                {
                    n = x0; x0 = y0; y0 = n;
                    n = x1; x1 = y1; y1 = n; e = -dy;
                    for (int i = 0; i <= dy; i++)
                    {
                        g.DrawRectangle(Pens.Red, x, y, 1, 1);
                        y--;
                        e = e + 2 * dx;
                        if (e >= 0) { x++; e = e - 2 * dy; }
                    }
                    n = x0; x0 = y0; y0 = n;
                    n = x1; x1 = y1; y1 = n;
                }
                if (y0 < y1 && m > -1 && m < 0)
                {
                    dy = -dy;
                    for (int i = 0; i <= dx; i++)
                    {
                        g.DrawRectangle(Pens.Red, x, y, 1, 1);
                        x++;
                        e = e + 2 * dy;
                        if (e >= 0) { y++; e = e - 2 * dx; }
                    }
                }
                if (y0 < y1 && m < -1)
                {
                    dy = -dy;
                    n = x0; x0 = y0; y0 = n;
                    n = x1; x1 = y1; y1 = n; e = -dy;
                    for (int i = 0; i < dy; i++)
                    {
                        g.DrawRectangle(Pens.Red, x, y, 1, 1);
                        y++;
                        e = e + 2 * dx;
                        if (e >= 0) { x++; e = e - 2 * dy; }
                    }
                }
                if (y0 == y1)
                {
                    y = y0;
                    for (x = x0; x < x1; x++)
                    {
                        g.DrawRectangle(Pens.Red, x, y, 1, 1);
                    }
                }
            }
            if (x0 > x1)
            {
                n = x0; x0 = x1; x1 = n;
                n = y0; y0 = y1; y1 = n;
                dx = x1 - x0; dy = y0 - y1; e = -dx;
                x = x0; y = y0;
                float m = (float)dy / (float)dx;
                if (y0 > y1 && m < 1 && m > 0)
                {
                    for (int i = 0; i <= dx; i++)
                    {
                        g.DrawRectangle(Pens.Red, x, y, 1, 1);
                        x++;
                        e = e + 2 * dy;
                        if (e >= 0) { y--; e = e - 2 * dx; }
                    }
                }
                if (y0 > y1 && m > 1)
                {
                    n = x0; x0 = y0; y0 = n;
                    n = x1; x1 = y1; y1 = n; e = -dy;
                    for (int i = 0; i <= dy; i++)
                    {

                        g.DrawRectangle(Pens.Red, x, y, 1, 1);
                        y--;
                        e = e + 2 * dx;
                        if (e >= 0) { x++; e = e - 2 * dy; }
                    }
                }
                if (y0 < y1 && m > -1 && m < 0)
                {
                    dy = -dy;
                    for (int i = 0; i <= dx; i++)
                    {
                        g.DrawRectangle(Pens.Red, x, y, 1, 1);
                        x++;
                        e = e + 2 * dy;
                        if (e >= 0) { y++; e = e - 2 * dx; }
                    }
                }
                if (y0 < y1 && m < -1)
                {
                    dy = -dy;
                    n = x0; x0 = y0; y0 = n;
                    n = x1; x1 = y1; y1 = n; e = -dy;
                    for (int i = 0; i < dy; i++)
                    {
                        g.DrawRectangle(Pens.Red, x, y, 1, 1);
                        y++;
                        e = e + 2 * dx;
                        if (e >= 0) { x++; e = e - 2 * dy; }
                    }
                }
                if (y0 == y1)
                {
                    y = y0;
                    for (x = x1; x > x0; x--)
                    {
                        g.DrawRectangle(Pens.Red, x, y, 1, 1);
                    }
                }
            }
            if (x0 == x1)
            {
                x = x0;
                if (y0 < y1)
                    for (y = y0; y < y1; y++)
                    {
                        g.DrawRectangle(Pens.Red, x, y, 1, 1);
                    }
                else if (y0 > y1)
                    for (y = y0; y > y1; y--)
                    {
                        g.DrawRectangle(Pens.Red, x, y, 1, 1);
                    }
            }
            if (x0 == x1 || y0 == y1) return;
        }

        private void CohenSutherland(int P1x, int P1y, int P2x, int P2y)
        {
            int C1 = Code(P1x, P1y), C2 = Code(P2x, P2y);
            int C;
            int Px = 0, Py = 0;//记录交点
            while (C1 != 0 || C2 != 0)//两个点（P1x,P1y）,（P2x,P2y）不都在矩形框内；都在内部就画出线段
            {
                if ((C1 & C2) != 0)   //两个点在矩形框的同一外侧 → 不可见
                {
                    P1x = 0;
                    P1y = 0;
                    P2x = 0;
                    P2y = 0;
                    break;
                }
                C = C1;
                if (C1 == 0)// 判断P1 P2谁在矩形框内（可能是P1，也可能是P2）
                {
                    C = C2;
                }

                if ((C & CodeLeft) != 0)//用与判断的点在左侧 
                {
                    Px = left;
                    Py = P1y + (int)(Convert.ToDouble(P2y - P1y) / (P2x - P1x) * (left - P1x));
                }
                else if ((C & CodeRight) != 0)//用与判断的点在右侧 
                {
                    Px = right;
                    Py = P1y + (int)(Convert.ToDouble(P2y - P1y) / (P2x - P1x) * (right - P1x));
                }
                else if ((C & CodeTop) != 0)//用与判断的点在上方
                {
                    Py = top;
                    Px = P1x + (int)(Convert.ToDouble(P2x - P1x) / (P2y - P1y) * (top - P1y));
                }
                else if ((C & CodeButtom) != 0)//用与判断的点在下方
                {
                    Py = buttom;
                    Px = P1x + (int)(Convert.ToDouble(P2x - P1x) / (P2y - P1y) * (buttom - P1y));
                }

                if (C == C1) //上面判断使用的是哪个端点就替换该端点为新值
                {
                    P1x = Px;
                    P1y = Py;
                    C1 = Code(P1x, P1y);
                }
                else
                {
                    P2x = Px;
                    P2y = Py;
                    C2 = Code(P2x, P2y);
                }
            }
            Graphics g = CreateGraphics();
            Pen p = new Pen(Color.Red);
            g.DrawLine(p, new Point(P1x, P1y), new Point(P2x, P2y));
        }

        private int Code(int x, int y) //端点编码函数 左右上下分别对应一位
        {
            int c = 0;
            if (x < left)
            {
                c = c | CodeLeft;
            }
            if (x > right)
            {
                c = c | CodeRight;
            }
            if (y < top)
            {
                c = c | CodeTop;
            }
            if (y > buttom)
            {
                c = c | CodeButtom;
            }
            return c;
        }

        private void DDALine1(int x0, int y0, int x1, int y1)
        {
            int x, flag;
            float m, y;
            Graphics g = CreateGraphics();
            if (x0 == x1 && y0 == y1) return;
            if (x0 == x1)
            {
                if (y0 > y1)
                {
                    x = y0; y0 = y1; y1 = x;
                }
                for (x = y0; x < y1; x++)
                {
                    g.DrawRectangle(Pens.Red, x1, x, 1, 1);
                }
                return;
            }
            if (y0 == y1)
            {
                if (x0 > x1)
                {
                    x = x0; x0 = x1; x1 = x;
                }
                for (x = x0; x < x1; x++)
                {
                    g.DrawRectangle(Pens.Red, x, y0, 1, 1);
                }
                return;
            }
            if (x0 > x1)
            {

                x = x0; x0 = x1; x1 = x;
                x = y0; y0 = y1; y1 = x;
            }
            flag = 0;
            if (x1 - x0 > y1 - y0 && y1 - y0 > 0)
                flag = 1;
            if (x1 - x0 > y0 - y1 && y0 - y1 > 0)
            {
                flag = 2; y0 = -y0; y1 = -y1;
            }
            if (y1 - y0 > x1 - x0)
            {
                flag = 3; x = x0; x0 = y0; y0 = x;
                x = x1; x1 = y1; y1 = x;
            }
            if (y0 - y1 > x1 - x0)
            {
                flag = 4; x = x0; x0 = -y0; y0 = x;
                x = x1; x1 = -y1; y1 = x;
            }
            m = (float)(y1 - y0) / (float)(x1 - x0);
            for (x = x0, y = (float)y0; x <= x1; x++, y = y + m)
            {
                if (flag == 1) g.DrawRectangle(Pens.Red, x, (int)(y + 0.5), 1, 1);
                if (flag == 2) g.DrawRectangle(Pens.Red, x, -(int)(y + 0.5), 1, 1);
                if (flag == 3) g.DrawRectangle(Pens.Red, (int)(y + 0.5), x, 1, 1);
                if (flag == 4) g.DrawRectangle(Pens.Red, (int)(y + 0.5), -x, 1, 1);
            }

        }
    }
}
