using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Npgsql;
using System.Drawing;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using System.Data.Odbc;
using System.Runtime.InteropServices;

namespace MyGIS
{
    public class FFeature
    {
        public FSpatial spatialpart;
        public FAttribute attributepart;
        public bool selected = false;

        public FFeature(FSpatial _spatial,FAttribute _attribute)
        {
            spatialpart = _spatial;
            attributepart = _attribute;
        }

        public FSpatial getSpatial()
        {
            return spatialpart;
        }

        public FAttribute getAttribute()
        {
            return attributepart;
        }

        public object getAttributeValue(int index)
        {
            return attributepart.getValue(index);
        }

        public void draw(Graphics graphics,FView view,bool drawAttributeOrNot,int index)
        {
            spatialpart.draw(graphics, view, selected);
            if (drawAttributeOrNot)
                attributepart.draw(graphics, view, spatialpart.getCenter(), index);
        }
    }

    public abstract class FSpatial
    {
        protected FVertex center;
        protected FExtent extent;

        public FVertex getCenter()
        {
            return center;
        }

        public FExtent getExtent()
        {
            return extent;
        }

        public FVertex calculateCenter(List<FVertex> _vertexes)
        {
            if (_vertexes.Count > 0)
            {
                double x = 0;
                double y = 0;
                for (int i = 0; i < _vertexes.Count(); i++)
                {
                    x += _vertexes[i].getX();
                    y += _vertexes[i].getY();
                }
                return new FVertex(x / _vertexes.Count(), y / _vertexes.Count());
            }
            else
                return null;
        }

        public FExtent calculateExtent(List<FVertex> _vertexes)
        {
            if (_vertexes.Count() > 0)
            {
                double minx = Double.MaxValue;
                double miny = Double.MaxValue;
                double maxx = Double.MinValue;
                double maxy = Double.MinValue;
                for (int i = 0; i < _vertexes.Count(); i++)
                {
                    if (_vertexes[i].getX() > maxx)
                        maxx = _vertexes[i].getX();
                    if (_vertexes[i].getX() < minx)
                        minx = _vertexes[i].getX();
                    if (_vertexes[i].getY() > maxy)
                        maxy = _vertexes[i].getY();
                    if (_vertexes[i].getY() < miny)
                        miny = _vertexes[i].getY();
                }
                return new FExtent(new FVertex(maxx, maxy), new FVertex(minx, miny));
            }
            else
                return null;
        }

        public Point[] GetScreenPoints(List<FVertex> _vertexes,FView _view)
        {
            Point[] points = new Point[_vertexes.Count];
            for(int i=0;i<points.Length;i++)
            {
                points[i] = _view.ToScreenPoint(_vertexes[i]);
            }
            return points;
        }

        public abstract void draw(Graphics graphics, FView view, bool selected);
    }

    public class FAttribute
    {
        ArrayList values = new ArrayList();

        public void AddValue(object o)
        {
            values.Add(o);
        }

        public object getValue(int index)
        {
            return values[index];
        }

        public void draw(Graphics graphics,FView view,FVertex location,int index)
        {
            Point screenpoint = view.ToScreenPoint(location);
            graphics.DrawString(values[index].ToString(), new Font("宋体", 20), 
                new SolidBrush(Color.Green), new PointF(screenpoint.X, screenpoint.Y));
        }
    }

    public class FMultiPoint:FSpatial
    {
        public List<FPoint> points;

        public FMultiPoint(List<FPoint> _points)
        {
            points = _points;
            List<FVertex> vertexs = new List<FVertex>();
            FExtent ext = new FExtent(points[0].getExtent());
            foreach (FPoint point in points)
            {
                vertexs.Add(point.location);
                ext.Merge(point.getExtent());
            }
            center = calculateCenter(vertexs);
            extent = ext;
        }

        public double Distance(FVertex vertex)
        {
            double dist = Double.MaxValue;
            foreach (FPoint point in points)
                dist = Math.Min(dist, point.Distance(vertex));
            return dist;
        }

        public override void draw(Graphics graphics, FView view, bool selected)
        {
            foreach (FPoint point in points)
                point.draw(graphics, view, selected);
        }
    }

    public class FPoint : FSpatial
    {
        public FVertex location;

        public FPoint(FVertex _vertex)
        {
            location = _vertex;
            center = _vertex;
            extent = new FExtent(_vertex, _vertex);
        }

        public double Distance(FVertex anothervertex)
        {
            return location.Distance(anothervertex);
        }

        public override void draw(Graphics graphics, FView view, bool selected)
        {
            Point screenpoint = view.ToScreenPoint(location);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.FillEllipse(new SolidBrush(selected ? Color.SkyBlue : Color.OrangeRed),
                selected ? new Rectangle(screenpoint.X - 3, screenpoint.Y - 3, 6, 6):
                new Rectangle(screenpoint.X - 2, screenpoint.Y - 2, 4, 4));
        }

    }

    public class FMultiLine :FSpatial
    {
        public List<FLine> lines;

        public FMultiLine(List<FLine> _lines)
        {
            lines = _lines;
            List<FVertex> centers=new List<FVertex>();
            FExtent ext = new FExtent(lines[0].getExtent());
            foreach(FLine line in lines)
            {
                centers.Add(line.getCenter());
                ext.Merge(line.getExtent());
            }
            center = calculateCenter(centers);
            extent = ext;
        }

        public double Distance(FVertex vertex)
        {
            double dist = Double.MaxValue;
            foreach(FLine line in lines)
                dist = Math.Min(dist, line.Distance(vertex));
            return dist;
        }

        public override void draw(Graphics graphics, FView view, bool selected)
        {
            foreach (FLine line in lines)
                line.draw(graphics, view, selected);
        }
    }

    public class FLine : FSpatial
    {
        public List<FVertex> vertexes;

        public FLine(List<FVertex> _vertexes)
        {
            vertexes = _vertexes;
            center = calculateCenter(vertexes);
            extent = calculateExtent(vertexes);
        }

        public double Length()
        {
            double length= 0;
            for (int i = 0; i < vertexes.Count() - 1; i++)
                length += vertexes[i].Distance(vertexes[i + 1]);
            return length;
        }

        public double Distance(FVertex vertex)
        {
            double distance = Double.MaxValue;
            for (int i=0;i<vertexes.Count-1;i++)
            {
                distance = Math.Min(FVertex.PointToSegment(vertexes[i], vertexes[i + 1], vertex), distance);
            }
            return distance;
        }

        public override void draw(Graphics graphics, FView view, bool selected)
        {
            Point[] points = GetScreenPoints(vertexes, view);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.DrawLines(new Pen(selected ? Color.SkyBlue : Color.Red, selected ? 3 : 1), points);
        }
    }

    public class FMultiPolygon:FSpatial
    {
        public List<FPolygon[]> polygons;
        
        public FMultiPolygon(List<FPolygon[]> _polygons)
        {
            polygons = _polygons;
            List<FVertex> vertexes = new List<FVertex>();
            FExtent ext = new FExtent(polygons[0][0].getExtent());
            foreach (FPolygon[] polygon in polygons)
            {
                vertexes.Add(polygon[0].getCenter());
                ext.Merge(polygon[0].getExtent());
            }
            extent = ext;
            center = calculateCenter(vertexes);
        }

        public double Area()
        {
            double area = 0;
            foreach (FPolygon[] polygon in polygons)
            {
                if (polygon[1] != null)
                    area += (polygon[0].Area() - polygon[1].Area());
                else
                    area += polygon[0].Area();
            }
            return area;
        }

        public bool Include(FVertex vertex)
        {
            foreach (FPolygon[] polygon in polygons)
            {
                if (polygon[1] == null)
                {
                    if (polygon[0].Include(vertex))
                        return true;
                }
                else
                {
                    if (polygon[0].Include(vertex) && !polygon[1].Include(vertex))
                        return true;
                }
            }
            return false;
        }

        public override void draw(Graphics graphics, FView view, bool selected)
        {
            foreach(FPolygon[] polygon in polygons)
            {
                polygon[0].draw(graphics, view, selected);
                if (polygon[1] != null)
                    polygon[1].drawInterioRing(graphics, view, selected);
            }
        }

    }
    

    public class FPolygon : FSpatial
    {
        public List<FVertex> vertexes;

        public FPolygon(List<FVertex> _vertexes)
        {
            vertexes = _vertexes;
            center = calculateCenter(_vertexes);
            extent = calculateExtent(_vertexes);
        }

        public double Area()
        {
            double area=0;
            for (int i = 0; i < vertexes.Count - 1; i++)
            {
                area += vertexes[i].getX() * vertexes[i + 1].getY() - vertexes[i].getY() * vertexes[i + 1].getX();
            }
            area += vertexes[vertexes.Count - 1].getX() * vertexes[0].getY() - vertexes[vertexes.Count - 1].getY() * vertexes[0].getX();
            return Math.Abs(area * 0.5);
        }

        public bool Include(FVertex vertex)
        {
            int count = 0;
            int sum = 0;
            for (int i=0;i<vertexes.Count-1;i++)
            {
                count = FVertex.IntersactCount(vertexes[i], vertexes[i + 1], vertex);
                if (count < 0)
                    return true;
                sum += count;
            }
            count = FVertex.IntersactCount(vertexes[vertexes.Count - 1], vertexes[0], vertex);
            if (count < 0)
                return true;
            sum += count;
            return sum % 2 != 0;
        }

        public override void draw(Graphics graphics, FView view, bool selected)
        {
            Point[] points = GetScreenPoints(vertexes, view);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.FillPolygon(new SolidBrush(Color.Wheat), points);
            graphics.DrawPolygon(new Pen(selected ? Color.SkyBlue : Color.Black, selected ? 3 : 1), points);
        }

        /// <summary>
        /// 绘制内环，仅在FMultiPolygons中调用
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="view"></param>
        /// <param name="selected"></param>
        public void drawInterioRing(Graphics graphics, FView view, bool selected)
        {
            Point[] points = GetScreenPoints(vertexes, view);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.FillPolygon(new SolidBrush(Color.AliceBlue), points);
            graphics.DrawPolygon(new Pen(selected ? Color.SkyBlue : Color.Black, selected ? 3 : 1), points);
        }
    }

    public class FExtent
    {
        FVertex upright;
        FVertex bottomleft;
        public FVertex mapcenter;

        public double minX;
        public double minY;
        public double maxX;
        public double maxY;
        public double width;
        public double height;

        public FExtent(FVertex ur, FVertex bl)
        {
            setValue(ur, bl);
        }

        public FExtent(FExtent _extent)
        {
            setValue(_extent);
        }

        public void setValue(FVertex ur, FVertex bl)
        {
            upright = ur;
            bottomleft = bl;
            minX = bl.getX();
            minY = bl.getY();
            maxX = ur.getX();
            maxY = ur.getY();
            mapcenter = new FVertex(0.5 * (minX + maxX), 0.5 * (minY + maxY));
            height = maxY - minY;
            width = maxX - minX;
        }

        public void setValue(FExtent _extent)
        {
            setValue(new FVertex(_extent.maxX, _extent.maxY),new FVertex(_extent.minX, _extent.minY));
        }

        public void CopyFrom(FExtent _extent)
        {
            setValue(_extent);
        }

        public void ZoomIn()
        {
            double newminx = minX + width / 16;
            double newmaxx = maxX - width / 16;
            double newminy = minY + height / 16;
            double newmaxy = maxY - height / 16;
            setValue(new FVertex(newmaxx, newmaxy), new FVertex(newminx, newminy));
        }

        public void ZoomOut()
        {
            double newminx = minX - width / 8;
            double newmaxx = maxX + width / 8;
            double newminy = minY - height / 8;
            double newmaxy = maxY + height / 8;
            setValue(new FVertex(newmaxx, newmaxy), new FVertex(newminx, newminy));
        }

        public void MoveUp()
        {
            double newminy = minY - height / 8;
            double newmaxy = maxY - height / 8;
            setValue( new FVertex(maxX, newmaxy), new FVertex(minX, newminy));
        }

        public void MoveDown()
        {
            double newminy = minY + height / 8;
            double newmaxy = maxY + height / 8;
            setValue( new FVertex(maxX, newmaxy), new FVertex(minX, newminy));
        }

        public void MoveRight()
        {
            double newminx = minX - width / 8;
            double newmaxx = maxX - width / 8;
            setValue( new FVertex(newmaxx, maxY), new FVertex(newminx, minY));
        }

        public void MoveLeft()
        {
            double newminx = minX + width / 8;
            double newmaxx = maxX + width / 8;
            setValue( new FVertex(newmaxx, maxY), new FVertex(newminx, minY));
        }

        /// <summary>
        /// 判断点是否在Extent范围内
        /// </summary>
        /// <param name="_vertex"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        public bool WithInDistance(FVertex _vertex,double dist)
        {
            return (((_vertex.getX() + dist) > minX) && ((_vertex.getX() - dist) < maxX) &&
                ((_vertex.getY() + dist) > minY) && ((_vertex.getY() - dist < maxY)));
        }

        /// <summary>
        /// 另一个extent是否完全在此范围之外
        /// </summary>
        /// <param name="_extent"></param>
        /// <returns></returns>
        public bool Outside(FExtent _extent)
        {
            return (maxX < _extent.minX || minX > _extent.maxX
                || maxY < _extent.minY || minY > _extent.maxY);
        }

        /// <summary>
        /// 另一个extent是否完全包含在此范围内
        /// </summary>
        /// <param name="_extent"></param>
        /// <returns></returns>
        public bool Include(FExtent _extent)
        {
            return (minX < _extent.minX && maxX > _extent.maxX
                && minY < _extent.minY && maxY > _extent.maxY);
        }

        /// <summary>
        /// 判断线是否与此范围相交
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool LineOverlay(FLine line)
        {
            if (!Outside(line.getExtent()))
            {
                if (Include(line.getExtent()))
                    return true;
                for (int i = 0; i < line.vertexes.Count - 1; i++)
                {
                    if (FVertex.LinesIntersect(line.vertexes[i], line.vertexes[i + 1], new FVertex(minX, minY), new FVertex(minX, maxY)))
                        return true;
                    if (FVertex.LinesIntersect(line.vertexes[i], line.vertexes[i + 1], new FVertex(minX, maxY), new FVertex(maxX, maxY)))
                        return true;
                    if (FVertex.LinesIntersect(line.vertexes[i], line.vertexes[i + 1], new FVertex(maxX, maxY), new FVertex(maxX, minY)))
                        return true;
                    if (FVertex.LinesIntersect(line.vertexes[i], line.vertexes[i + 1], new FVertex(minX, minY), new FVertex(maxX, minY)))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断多边形是否与此范围相交
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public bool PolygonOverlay(FPolygon polygon)
        {
            if(!Outside(polygon.getExtent()))
            {
                if (Include(polygon.getExtent()))
                    return true;
                for(int i=0;i<polygon.vertexes.Count-1;i++)
                {
                    if (FVertex.LinesIntersect(polygon.vertexes[i], polygon.vertexes[i + 1], new FVertex(minX, minY), new FVertex(minX, maxY)))
                        return true;
                    if (FVertex.LinesIntersect(polygon.vertexes[i], polygon.vertexes[i + 1], new FVertex(minX, maxY), new FVertex(maxX, maxY)))
                        return true;
                    if (FVertex.LinesIntersect(polygon.vertexes[i], polygon.vertexes[i + 1], new FVertex(maxX, maxY), new FVertex(minX, maxY)))
                        return true;
                    if (FVertex.LinesIntersect(polygon.vertexes[i], polygon.vertexes[i + 1], new FVertex(minX, maxY), new FVertex(minX, minY)))
                        return true;
                }
                if (FVertex.LinesIntersect(polygon.vertexes[0], polygon.vertexes[polygon.vertexes.Count - 1], new FVertex(minX, minY), new FVertex(minX, maxY)))
                    return true;
                if (FVertex.LinesIntersect(polygon.vertexes[0], polygon.vertexes[polygon.vertexes.Count - 1], new FVertex(minX, maxY), new FVertex(maxX, maxY)))
                    return true;
                if (FVertex.LinesIntersect(polygon.vertexes[0], polygon.vertexes[polygon.vertexes.Count - 1], new FVertex(maxX, maxY), new FVertex(minX, maxY)))
                    return true;
                if (FVertex.LinesIntersect(polygon.vertexes[0], polygon.vertexes[polygon.vertexes.Count - 1], new FVertex(minX, maxY), new FVertex(minX, minY)))
                    return true;
            }
            return false;
        }

        public void SetMapCenter(FVertex v)
        {
            setValue(new FVertex(v.getX() + width / 2, v.getY() + height / 2),
                new FVertex(v.getX() - width / 2, v.getY() - height / 2));
        }

        /// <summary>
        /// 与新的extent范围融合取并
        /// </summary>
        /// <param name="_extent"></param>
        public void Merge(FExtent _extent)
        {
            double newminx = Math.Min(minX, _extent.minX);
            double newminy = Math.Min(minY, _extent.minY);
            double newmaxx = Math.Max(maxX, _extent.maxX);
            double newmaxy = Math.Max(maxY, _extent.maxY);
            setValue(new FVertex(newmaxx, newmaxy), new FVertex(newminx, newminy));
        }
    }

    public class FVertex
    {
        double x;
        double y;

        public FVertex(double _x, double _y)
        {
            setX(_x);
            setY(_y);
        }

        public void setX(double _x)
        {
            x = _x;
        }

        public void setY(double _y)
        {
            y = _y;
        }

        public double getX()
        {
            return x;
        }

        public double getY()
        {
            return y;
        }

        public double Distance(FVertex anothervertex)
        {
            return Math.Sqrt((anothervertex.x - x) * (anothervertex.x - x) + 
                (anothervertex.y - y) * (anothervertex.y - y));
        }

        private static double DotProduct(FVertex A,FVertex B,FVertex c)//AB BC点积
        {
            FVertex AB = new FVertex(B.x - A.x, B.y - A.y);
            FVertex BC = new FVertex(c.x - B.x, c.y - B.y);
            return AB.x * BC.x + AB.y + BC.y;
        }

        private static double CrossProduct(FVertex A,FVertex  B,FVertex C)//AB AC叉积
        {
            FVertex AB = new FVertex(B.x - A.x, B.y - A.y);
            FVertex AC = new FVertex(C.x - B.x, C.y - B.y);
            return AB.x * AC.y - AB.y * AC.x;
        }

        private static double Distance(FVertex A,FVertex B)
        {
            double d1 = A.x - B.x;
            double d2 = A.y - B.y;
            return Math.Sqrt(d1 * d1 + d2 * d2);
        }

        /// <summary>
        /// C到AB的距离
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public static double PointToSegment(FVertex A,FVertex B,FVertex C)
        {
            double dot1 = DotProduct(A, B, C);
            if (dot1 > 0) return Distance(B, C);
            double dot2 = DotProduct(B, A, C);
            if (dot2 > 0) return Distance(A, C);
            double dist = CrossProduct(A, B, C) / Distance(A, B);
            return Math.Abs(dist);
        }

        /// <summary>
        /// 判断射线与线段的关系：相交返回1，不相交返回0，点为线段的节点或在线段上返回-1      
        /// /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="mp"></param>
        /// <returns></returns>
        public static int IntersactCount(FVertex p1, FVertex p2, FVertex mp)
        {
            double minX = Math.Min(p1.x, p2.x);
            double minY = Math.Min(p1.y, p2.y);
            double maxX = Math.Max(p1.x, p2.x);
            double maxY = Math.Max(p1.y, p2.y);
            if (mp.x > maxX || mp.y > maxY || mp.y < minY)
                return 0;
            if (p1.y == p2.y)
            {
                if (mp.x > minX && mp.x < maxX)//水平线段，则点在线段上返回一1，否则返回0
                    return -1;
                else
                    return 0;
            }
            double X0 = p1.x + (mp.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y);
            if (X0 < mp.x)//交点在射线起点左侧(射线与线段不相交) 
                return 0;
            else if (X0 == mp.x)//交点与射线起点是同一点在线段上，返回一1
                return -1;
            else if (mp.y == minY)//如果射线穿过线段下端点则不计数
                return 0;
            else
                return 1;//交点在边的中间点或者上端点，计数
        }

        /// <summary>
        /// 两条线段p1p2,q1q2是否相交
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <returns></returns>
        public static bool LinesIntersect(FVertex p1, FVertex p2, FVertex q1, FVertex q2)
        {
            if (Math.Min(p1.x, p2.x) <= Math.Max(q1.x, q2.x) && Math.Min(q1.x, q2.x) <= Math.Max(p1.x, p2.x) &&
                Math.Min(p1.y, p2.y) <= Math.Max(q2.y, q2.y) && Math.Min(q1.y, q2.y) <= Math.Max(p1.y, p2.y))
            {
                if (CrossProduct(q1, p1, q2) * CrossProduct(q1, q2, p2) >= 0 && CrossProduct(p1, q1, p2) * CrossProduct(p1, p2, q2) >= 0)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
    }

    public class FView
    {
        public FExtent MyMapExtent;
        public Rectangle myWindowSize;
        double scale;

        public FView(FExtent _extent, Rectangle _window)
        {
            SetValue(_extent, _window);
        }

        public void SetValue(FExtent _extent, Rectangle _window)
        {
            double scaleX;
            double scaleY;
            MyMapExtent = _extent;
            myWindowSize = _window;
            scaleX = MyMapExtent.width / myWindowSize.Width;
            scaleY = MyMapExtent.height / myWindowSize.Height;
            scale = Math.Max(scaleX, scaleY);
        }

        public Point ToScreenPoint(FVertex onevertex)
        {
            double screenX = (onevertex.getX() - MyMapExtent.mapcenter.getX()) / scale + 0.5 * myWindowSize.Width;
            double screenY = 0.5 * myWindowSize.Height - (onevertex.getY() - MyMapExtent.mapcenter.getY()) / scale;
            return new Point((int)screenX, (int)screenY);
        }

        public FVertex ToMapVertex(Point onepoint)
        {
            double MapX = MyMapExtent.mapcenter.getX() + (onepoint.X - 0.5 * myWindowSize.Width) * scale;
            double MapY = MyMapExtent.mapcenter.getY() + (0.5 * myWindowSize.Height - onepoint.Y) * scale;
            return new FVertex(MapX, MapY);
        }

        public double ToMapDistance(int screendistance)
        {
            return screendistance * scale;
        }

        /// <summary>
        /// 将屏幕范围转化成新的地图范围实例
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public FExtent RectToExtent(Rectangle rect)
        {
            FVertex bottomleft = ToMapVertex(new Point(rect.X, rect.Y + rect.Height));
            FVertex upright = ToMapVertex(new Point(rect.X + rect.Width, rect.Y));
            return new FExtent(upright, bottomleft);
        }
    }

    public enum SHAPETYPE
    {
        POINT,LINE,POLYGON,MULTIPOINTS,MULTILINES,MULTIPOLYGONS
    };

    public enum MOUSECOMMAND
    {
        Unused,Select,ZoomIn,ZoomOut,Pan
    };

    public class FField
    {
        public Type type;
        public string name;

        public FField(Type _type,string _name)
        {
            type = _type;
            name = _name;
        }
    }

    public class FLayer
    {
        public string Name;
        public FExtent Extent;
        public List<FFeature> Features = new List<FFeature>();
        public bool DrawAttributeOrNot;
        public SHAPETYPE ShapeType;
        public List<FField> Fields;
        public static int MINSCRDIST = 8;

        public FLayer(string _name,SHAPETYPE _st,FExtent _extent,List<FField> _fields)
        {
            Name = _name;
            ShapeType = _st;
            Extent = _extent;
            Fields = _fields;
        }

        public void draw(Graphics graphics,FView view,FExtent extent)
        {
            for(int i=0;i<Features.Count;i++)
            {
                if (Features[i].spatialpart.getExtent().Outside(extent) == false)
                    Features[i].draw(graphics, view, DrawAttributeOrNot, 0);
            }
            for(int i=0;i<Features.Count;i++)
            {
                if (Features[i].spatialpart.getExtent().Outside(extent) == false)
                    if (Features[i].selected)
                        Features[i].draw(graphics, view, DrawAttributeOrNot, 0);
            }
        }
        
        /// <summary>
        /// 返回点选的点对象
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="mindist"></param>
        /// <returns></returns>
        public FFeature SelectPointByVertex(FVertex vertex,double mindist)
        {
            double distance = Double.MaxValue;
            int id = -1;
            for(int i=0;i<Features.Count;i++)
            {
                if (Features[i].spatialpart.getExtent().WithInDistance(vertex, mindist) == false)
                    continue;
                FPoint point = (FPoint)(Features[i].spatialpart);
                double dist = point.Distance(vertex);
                if(dist<distance)
                {
                    distance = dist;
                    id = i;
                }
            }
            return (distance < mindist) ? Features[id] : null;
        }

        /// <summary>
        /// 返回点选复杂点对象
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="mindist"></param>
        /// <returns></returns>
        public FFeature SelectMulitiPointByVertex(FVertex vertex,double mindist)
        {
            double distance = Double.MaxValue;
            int id = -1;
            for (int i = 0; i < Features.Count; i++)
            {
                FMultiPoint mpt = (FMultiPoint)Features[i].spatialpart;
                for(int j=0;j<mpt.points.Count;j++)
                {
                    if (mpt.points[j].getExtent().WithInDistance(vertex, mindist) == false)
                        continue;
                    double dist = mpt.points[j].Distance(vertex);
                    if (dist < distance)
                    {
                        distance = dist;
                        id = i;
                    }
                }
            }
            return (distance < mindist) ? Features[id] : null;
        }

        /// <summary>
        /// 返回点选的线对象
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="mindist"></param>
        /// <returns></returns>
        public FFeature SelectLineByVertex(FVertex vertex,double mindist)
        {
            double distance = Double.MaxValue;
            int id = -1;
            for (int i=0;i<Features.Count;i++)
            {
                if (Features[i].spatialpart.getExtent().WithInDistance(vertex, mindist) == false)
                    continue;
                FLine line = (FLine)(Features[i].spatialpart);
                double dist = line.Distance(vertex);
                if(dist<distance)
                {
                    distance = dist;
                    id = i;
                }
            }
            return (distance < mindist) ? Features[id] : null;
        }

        /// <summary>
        /// 返回点选复杂线对象
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="mindist"></param>
        /// <returns></returns>
        public FFeature SelectMultiLineByVertex(FVertex vertex, double mindist)
        {
            double distance = Double.MaxValue;
            int id = -1;
            for (int i = 0; i < Features.Count; i++)
            {
                FMultiLine mpl = (FMultiLine)Features[i].spatialpart;
                for (int j = 0; j < mpl.lines.Count; j++)
                {
                    if (mpl.lines[j].getExtent().WithInDistance(vertex, mindist) == false)
                        continue;
                    double dist = mpl.lines[j].Distance(vertex);
                    if (dist < distance)
                    {
                        distance = dist;
                        id = i;
                    }
                }
            }
            return (distance < mindist) ? Features[id] : null;
        }
        
        /// <summary>
        /// 返回点选的多边形实体
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="mindist"></param>
        /// <returns></returns>
        public FFeature SelectPolygonByVertex(FVertex vertex,double mindist)
        {
            for(int i=0;i<Features.Count;i++)
            {
                if (Features[i].spatialpart.getExtent().WithInDistance(vertex, mindist) == false)
                    continue;
                FPolygon polygon = (FPolygon)(Features[i].spatialpart);
                if (polygon.Include(vertex))
                    return Features[i];
            }
            return null;
        }

        /// <summary>
        /// 返回点选的复杂多边形
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="mindist"></param>
        /// <returns></returns>
        public FFeature SelectMultiPolygonByVertex(FVertex vertex, double mindist)
        {
            for (int i = 0; i < Features.Count; i++)
            {
                FMultiPolygon mpp = (FMultiPolygon)Features[i].spatialpart;
                for (int j = 0; j < mpp.polygons.Count; j++)
                {
                    if (mpp.polygons[j][0].getExtent().WithInDistance(vertex, mindist) == false)
                        continue;
                    if (mpp.Include(vertex))
                    {
                        return Features[i];
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 鼠标点选返回对象实例
        /// </summary>
        /// <param name="mousepoint"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public FFeature SelectByClick(Point mousepoint,FView view)
        {
            FFeature feature = null;
            FVertex vertex = view.ToMapVertex(mousepoint);
            double mindist = view.ToMapDistance(MINSCRDIST);
            if (ShapeType == SHAPETYPE.POINT)
                feature = SelectPointByVertex(vertex, mindist);
            else if (ShapeType == SHAPETYPE.LINE)
                feature = SelectLineByVertex(vertex, mindist);
            else if (ShapeType == SHAPETYPE.POLYGON)
                feature = SelectPolygonByVertex(vertex, mindist);
            else if (ShapeType == SHAPETYPE.MULTIPOINTS)
                feature = SelectMulitiPointByVertex(vertex, mindist);
            else if (ShapeType == SHAPETYPE.MULTILINES)
                feature = SelectMultiLineByVertex(vertex, mindist);
            else if (ShapeType == SHAPETYPE.MULTIPOLYGONS)
                feature = SelectMultiPolygonByVertex(vertex, mindist);
            return feature;
        }

        /// <summary>
        /// 返回框选选中的对象集合
        /// </summary>
        /// <param name="extent"></param>
        /// <returns></returns>
        public List<FFeature> SelectByExtent(FExtent extent)
        {
            List<FFeature> selectfeatures = new List<FFeature>();
            if (ShapeType == SHAPETYPE.POINT)
            {
                for (int i = 0; i < Features.Count; i++)
                    if (extent.Include(Features[i].spatialpart.getExtent()))
                        selectfeatures.Add(Features[i]);
            }
            else if (ShapeType == SHAPETYPE.MULTIPOINTS)
            {
                FMultiPoint mpt;
                for (int i = 0; i < Features.Count; i++)
                {
                    mpt = (FMultiPoint)Features[i].spatialpart;
                    for (int j = 0; j < mpt.points.Count; j++)
                        if (extent.Include(mpt.points[j].getExtent()))
                        {
                            selectfeatures.Add(Features[i]);
                            break;
                        }
                }
            }
            else if (ShapeType == SHAPETYPE.LINE)
            {
                for (int i = 0; i < Features.Count; i++)
                    if (extent.LineOverlay((FLine)Features[i].spatialpart))
                        selectfeatures.Add(Features[i]);
            }
            else if (ShapeType == SHAPETYPE.MULTILINES)
            {
                FMultiLine mpl;
                for (int i = 0; i < Features.Count; i++)
                {
                    mpl = (FMultiLine)Features[i].spatialpart;
                    for (int j = 0; j < mpl.lines.Count; j++)
                        if (extent.LineOverlay(mpl.lines[j]))
                        {
                            selectfeatures.Add(Features[i]);
                            break;
                        }
                }
            }
            else if(ShapeType==SHAPETYPE.POLYGON)
            {
                for (int i = 0; i < Features.Count; i++)
                    if (extent.PolygonOverlay((FPolygon)Features[i].spatialpart))
                        selectfeatures.Add(Features[i]);
            }
            else if (ShapeType == SHAPETYPE.MULTIPOLYGONS)
            {
                FMultiPolygon mpp;
                for (int i = 0; i < Features.Count; i++)
                {
                    mpp = (FMultiPolygon)Features[i].spatialpart;
                    for (int j = 0; j < mpp.polygons.Count; j++)
                        if (extent.PolygonOverlay(mpp.polygons[j][0]))
                        {
                            selectfeatures.Add(Features[i]);
                            break;
                        }
                }
            }
            return selectfeatures;
        }

        public void ClearSelection()
        {
            for (int i = 0; i < Features.Count; i++)
                Features[i].selected = false;
        }
    }

    public class FPostGIS
    {
        string strConn;

        public FPostGIS(string constring)
        {
            SetConnectionString(constring);
        }

        /// <summary>
        /// 返回整个属性表
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string tablename)
        {
            NpgsqlConnection dbcon;
            dbcon = new NpgsqlConnection(strConn);
            dbcon.Open();
            NpgsqlCommand command = new NpgsqlCommand("select * from " + tablename, dbcon);
            DataTable table = new DataTable();
            table.Load(command.ExecuteReader());
            dbcon.Close();
            return table;
        }

        public bool ConnnectOrNot()
        {
            NpgsqlConnection dbcon;
            dbcon = new NpgsqlConnection(strConn);
            try
            {
                dbcon.Open();
                dbcon.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public List<string> GetTablesFromDB()
        {
            List<string> tables = new List<string>();
            NpgsqlConnection dbcon;
            dbcon = new NpgsqlConnection(strConn);
            dbcon.Open();
            NpgsqlCommand command = new NpgsqlCommand(
                "select tablename from pg_tables where schemaname = 'public'", dbcon);
            DataTable table = new DataTable();
            table.Load(command.ExecuteReader());
            dbcon.Close();
            for (int i = 0; i < table.Rows.Count; i++)
                tables.Add(table.Rows[i][0].ToString());
            return tables;

        }

        FAttribute ReadAttribute(DataTable table,int RowIndex)
        {
            FAttribute attribute = new FAttribute();
            DataRow row = table.Rows[RowIndex];
            for (int i = 0; i < table.Columns.Count-1; i++)
                attribute.AddValue(row[i]);
            return attribute;
        }

        List<FField> ReadField(DataTable table)
        {
            List<FField> fields = new List<FField>();
            foreach (DataColumn column in table.Columns)
                fields.Add(new FField(column.DataType, column.ColumnName));
            fields.RemoveAt(fields.Count - 1);
            return fields;
        }

        /// <summary>
        /// 返回WKT字符串列表
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        string[] GetGeometryString(string tablename)
        {
            NpgsqlConnection dbcon;
            dbcon = new NpgsqlConnection(strConn);
            dbcon.Open();
            NpgsqlCommand command = new NpgsqlCommand("select st_astext(geom) from " + tablename, dbcon);
            DataTable table = new DataTable();
            table.Load(command.ExecuteReader());
            dbcon.Close();
            int RowCount = table.Rows.Count;
            string[] test = new string[RowCount];
            for(int i=0;i<RowCount;i++)
                test[i]= table.Rows[i][0].ToString();
            return test;
        }

        /// <summary>
        /// 返回对象图层
        /// </summary>
        /// <param name="sql"> sql语句是对Geometry的选择</param>
        /// <returns></returns>
        public FLayer GetDataLayer(string tablename)
        {
            List<FFeature> features = new List<FFeature>();
            string[] geometries = GetGeometryString(tablename);
            DataTable table = GetDataTable(tablename);
            List<FField> fields = ReadField(table);
            string type = geometries[0].Substring(0, geometries[0].IndexOf('('));
            if(type.ToUpper() == "POINT")
            {
                List<FPoint> points = GetPointsFromStringSet(geometries);
                FExtent extent = new FExtent(points[0].getExtent());
                for (int i = 0; i < points.Count; i++)
                {
                    features.Add(new FFeature(points[i], ReadAttribute(table, i)));
                    extent.Merge(features[i].spatialpart.getExtent());
                }
                FLayer layer = new FLayer(tablename, SHAPETYPE.POINT, extent,fields);
                for (int i = 0; i < features.Count; i++)
                    layer.Features.Add(features[i]);
                return layer;
            }
            else if(type.ToUpper() == "MULTIPOINT")
            {
                List<FMultiPoint> points = GetMultiPointsFromStringSet(geometries);
                FExtent extent = new FExtent(points[0].getExtent());
                for (int i = 0; i < points.Count; i++)
                {
                    features.Add(new FFeature(points[i], ReadAttribute(table, i)));
                    extent.Merge(features[i].spatialpart.getExtent());
                }
                FLayer layer = new FLayer(tablename, SHAPETYPE.MULTIPOINTS, extent, fields);
                for (int i = 0; i < features.Count; i++)
                    layer.Features.Add(features[i]);
                return layer;
            }
            else if (type.ToUpper() == "LINESTRING")
            {
                List<FLine> lines = GetLinesFromStringSet(geometries);
                FExtent extent = new FExtent(lines[0].getExtent());
                for (int i = 0; i < lines.Count; i++)
                {
                    features.Add(new FFeature(lines[i], ReadAttribute(table, i)));
                    extent.Merge(features[i].spatialpart.getExtent());
                }
                FLayer layer = new FLayer(tablename, SHAPETYPE.LINE, extent, fields);
                for (int i = 0; i < features.Count; i++)
                    layer.Features.Add(features[i]);
                return layer;
            }
            else if (type.ToUpper() == "MULTILINESTRING")
            {
                List<FMultiLine> lines = GetMultiLinesFromtringSet(geometries);
                FExtent extent = new FExtent(lines[0].getExtent());
                for (int i = 0; i < lines.Count; i++)
                {
                    features.Add(new FFeature(lines[i], ReadAttribute(table, i)));
                    extent.Merge(features[i].spatialpart.getExtent());
                }
                FLayer layer = new FLayer(tablename, SHAPETYPE.MULTILINES, extent, fields);
                for (int i = 0; i < features.Count; i++)
                    layer.Features.Add(features[i]);
                return layer;
            }
            else if (type.ToUpper() == "POLYGON" )
            {
                List<FPolygon> polygons = GetPolygonsFromStringSet(geometries);
                FExtent extent = new FExtent(polygons[0].getExtent());
                for (int i = 0; i < polygons.Count; i++)
                {
                    features.Add(new FFeature(polygons[i], ReadAttribute(table, i)));
                    extent.Merge(features[i].spatialpart.getExtent());
                }
                FLayer layer = new FLayer(tablename, SHAPETYPE.POLYGON, extent, fields);
                for (int i = 0; i < features.Count; i++)
                    layer.Features.Add(features[i]);
                return layer;
            }
            else if (type.ToUpper() == "MULTIPOLYGON")
            {
                List<FMultiPolygon> polygons = GetMultiPolygonsFromtringSet(geometries);
                FExtent extent = new FExtent(polygons[0].getExtent());
                for (int i = 0; i < polygons.Count; i++)
                {
                    features.Add(new FFeature(polygons[i], ReadAttribute(table, i)));
                    extent.Merge(features[i].spatialpart.getExtent());
                }
                FLayer layer = new FLayer(tablename, SHAPETYPE.MULTIPOLYGONS, extent, fields);
                for (int i = 0; i < features.Count; i++)
                    layer.Features.Add(features[i]);
                return layer;
            }
            return null;
        }

        List<FLine> GetLinesFromStringSet(string[] geometries)//for simple line only
        {
            List<FLine> lines = new List<FLine>();
            for (int i = 0; i < geometries.Length; i++)
            {
                string s = geometries[i].Substring(geometries[i].IndexOf('(')+1).Trim('(').Trim(')');
                string[] points = s.Split(',');
                List<FVertex> vertexs = new List<FVertex>();
                for (int j = 0; j < points.Length; j++)
                    vertexs.Add(new FVertex(Double.Parse(points[j].Split(' ')[0]),
                        Double.Parse(points[j].Split(' ')[1])));
                lines.Add(new FLine(vertexs));
            }
            return lines;
        }

        List<FMultiLine> GetMultiLinesFromtringSet(string[] geometries)
        {
            List<FMultiLine> lines = new List<FMultiLine>();
            for (int i = 0; i < geometries.Length; i++)
            {
                string s = geometries[i].Substring(geometries[i].IndexOf('(') + 1);
                string[] ls;
                if (s.Contains("),("))
                    ls = Regex.Split(s, @"\D{3}");//非数字字符3位 "),("
                else
                {
                    ls = new string[1];
                    ls[0] = s;
                }
                List<FLine> splines = new List<FLine>();
                for(int j=0;j<ls.Length;j++)
                {
                    string[] ps = ls[j].Trim(')').Trim('(').Split(',');
                    List<FVertex> vertexes = new List<FVertex>();
                    for(int k=0;k<ps.Length;k++)
                    {
                        vertexes.Add(new FVertex(Double.Parse(ps[k].Trim(')').Trim('(').Split(' ')[0]),
                           Double.Parse(ps[k].Trim(')').Trim('(').Split(' ')[1])));
                    }
                    splines.Add(new FLine(vertexes));
                }
                lines.Add(new FMultiLine(splines));
            }
            return lines;
        }

        List<FPolygon> GetPolygonsFromStringSet(string[] geometries)//for simple polygon only
        {
            List<FPolygon> polygons = new List<FPolygon>();
            for (int i = 0; i < geometries.Length; i++)
            {
                string s = geometries[i].Substring(geometries[i].IndexOf('(') + 1).Trim('(').Trim(')');
                string[] points = s.Split(',');
                List<FVertex> vertexs = new List<FVertex>();
                for (int j = 0; j < points.Length-1; j++)
                    vertexs.Add(new FVertex(Double.Parse(points[j].Trim('(').Trim(')').Split(' ')[0]),
                        Double.Parse(points[j].Trim('(').Trim(')').Split(' ')[1])));
                polygons.Add(new FPolygon(vertexs));
            }
            return polygons;
        }

        List<FMultiPolygon> GetMultiPolygonsFromtringSet(string[] geometries)
        {
            List<FMultiPolygon> polygons = new List<FMultiPolygon>();//一个图层的多边形集合
            for (int i = 0; i < geometries.Length; i++)//i代表图层有多少对象
            {
                string s = geometries[i].Substring(geometries[i].IndexOf('(') + 1);
                string[] ls;//多多边形对象字符串集合
                if (s.Contains(")),(("))
                    ls = Regex.Split(s, @"\D{5}");//非数字字符5位 ")),(("
                else
                {
                    ls = new string[1];
                    ls[0] = s;
                }
                List<FPolygon[]> sppolygon = new List<FPolygon[]>();//每个多多边形对象（包含多个简单多边形）
                
                for (int j = 0; j < ls.Length; j++)//j代表每个对象包含多少简单多边形
                {
                    string[] ps;//每个简单多边形集合 是否包含内环 ps[0] ps[1]
                    FPolygon[] spolygon = new FPolygon[2];//每个简单多边形（可能含有内边界）
                    if (ls[j].Contains("),("))
                        ps = Regex.Split(ls[j].Trim(')').Trim('('), @"\D{3}");//非数字字符3位 "),("
                    else
                    {
                        ps = new string[1];
                        ps[0] = ls[j];
                    }
                    string[] pps;//点集合
                    
                    for (int k = 0; k < ps.Length; k++)//k代表一个简单多边形是否有内环 1 2 
                    {
                        List<FVertex> vertexes = new List<FVertex>();
                        pps = ps[k].Trim(')').Trim('(').Split(',');
                        for (int t = 0; t < pps.Length - 1; t++)//t代表第几个点
                        {
                            vertexes.Add(new FVertex(Double.Parse(pps[t].Trim('(').Trim(')').Split(' ')[0]),
                        Double.Parse(pps[t].Trim('(').Trim(')').Split(' ')[1])));
                        }
                        spolygon[k] = new FPolygon(vertexes);
                    }
                    sppolygon.Add(spolygon);
                }
                polygons.Add(new FMultiPolygon(sppolygon));
            }
            return polygons;
        }

        List<FPoint> GetPointsFromStringSet(string[] geometries)//for simple point only
        {
            List<FPoint> points = new List<FPoint>();
            for (int i = 0; i < geometries.Length; i++)
            {
                string s = geometries[i].Substring(geometries[i].IndexOf('(') + 1).Trim('(').Trim(')');
                points.Add(new FPoint(new FVertex(Double.Parse(s.Split(' ')[0]),
                    Double.Parse(s.Split(' ')[1]))));
            }
            return points;
        }

        List<FMultiPoint> GetMultiPointsFromStringSet(string[] geometries)
        {
            List<FMultiPoint> points = new List<FMultiPoint>();
            for (int i = 0; i < geometries.Length; i++)
            {
                string s = geometries[i].Substring(geometries[i].IndexOf('(') + 1);
                string[] ps = s.Split(',');
                List<FPoint> psl = new List<FPoint>();
                for (int j = 0; j < ps.Length; j++)
                {
                    psl.Add(new FPoint(new FVertex(Double.Parse(ps[j].Trim('(').Trim(')').Split(' ')[0]),
                        Double.Parse(ps[j].Trim('(').Trim(')').Split(' ')[1]))));
                }
                points.Add(new FMultiPoint(psl));
            }
            return points;
        }

        public void SetConnectionString(string connectionString)
        {
            strConn = connectionString;
        }
    }

    public class FShapefile
    {
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct ShapeFileHeader
        {
            public int Unused1, Unused2, Unused3, Unused4;
            public int Unused5, Unused6, Unused7, Unused8;
            public int ShapeType;
            public double Xmin;
            public double Ymin;
            public double Xmax;
            public double Ymax;
            public double Unused9, Unused10, Unused11, Unused12;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct RecordHeader
        {
            public int RecordNumber;
            public int RecordLength;
            public int ShapeType;
        };

        static DataTable ReadDBF(string dbffilename)
        {
            OdbcConnection conn = new OdbcConnection(@"Driver={Microsoft Visual FoxPro Driver};SourceType=DBF;SourceDB=" +
                              dbffilename + ";Exclusive=No;NULL=NO; Collate=Machine; BACKGROUNDFETCH=NO; DELETED=NO");
            conn.Open();
            OdbcCommand command = new OdbcCommand("select * from " + dbffilename, conn);
            DataTable table = new DataTable();
            table.Load(command.ExecuteReader());
            conn.Close();
            return table;
        }

        static FAttribute ReadAttribute(DataTable table, int RowIndex)
        {
            FAttribute attribute = new FAttribute();
            DataRow row = table.Rows[RowIndex];
            for (int i = 0; i < table.Columns.Count; i++)
            {
                attribute.AddValue(row[i]);
            }
            return attribute;
        }

        static List<FField> ReadFields(DataTable table)
        {
            List<FField> fields = new List<FField>();
            foreach (DataColumn column in table.Columns)
            {
                fields.Add(new FField(column.DataType, column.ColumnName));
            }
            return fields;
        }

        static ShapeFileHeader ReadFileHeader(BinaryReader br)
        {
            byte[] buff = br.ReadBytes(Marshal.SizeOf(typeof(ShapeFileHeader)));
            GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
            ShapeFileHeader header = (ShapeFileHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(ShapeFileHeader));
            handle.Free();
            return header;
        }

        static RecordHeader ReadRecordHeader(BinaryReader br)
        {
            byte[] buff = br.ReadBytes(Marshal.SizeOf(typeof(RecordHeader)));
            GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
            RecordHeader header = (RecordHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(RecordHeader));
            handle.Free();
            return header;
        }

        public static FLayer ReadShapeFile(string shpfilename)
        {
            //打开shp文件
            FileStream fsr = new FileStream(shpfilename, FileMode.Open);
            BinaryReader br = new BinaryReader(fsr);
            //读shp文件头
            ShapeFileHeader sfh = ReadFileHeader(br);
            //确定空间数据类型
            SHAPETYPE ShapeType = SHAPETYPE.POINT;
            if (sfh.ShapeType == 1) ShapeType = SHAPETYPE.POINT;
            else if (sfh.ShapeType == 3) ShapeType = SHAPETYPE.LINE;
            else if (sfh.ShapeType == 5) ShapeType = SHAPETYPE.POLYGON;
            else return null;
            //确定图层地理范围
            FExtent extent = new FExtent( new FVertex(sfh.Xmax, sfh.Ymax), new FVertex(sfh.Xmin, sfh.Ymin));
            //读属性数据
            DataTable table = ReadDBF(shpfilename.Replace(".shp", ".dbf"));
            //初始化图层
            FLayer layer = new FLayer(shpfilename, ShapeType, extent, ReadFields(table));
            int RowIndex = 0;
            while (br.PeekChar() != -1)
            {
                RecordHeader rh = ReadRecordHeader(br);
                int RecordLength = FromBigToLittle(rh.RecordLength);
                byte[] RecordContent = br.ReadBytes(RecordLength * 2 - 4);
                if (ShapeType == SHAPETYPE.POINT)
                {
                    FPoint onepoint = ReadPoint(RecordContent);
                    FFeature onefeature = new FFeature(onepoint, ReadAttribute(table, RowIndex));
                    layer.Features.Add(onefeature);
                }
                if (ShapeType == SHAPETYPE.LINE)
                {
                    List<FLine> lines = ReadLines(RecordContent);
                    for (int i = 0; i < lines.Count; i++)
                    {
                        FFeature onefeature = new FFeature(lines[i], ReadAttribute(table, RowIndex));
                        layer.Features.Add(onefeature);
                    }
                }
                if (ShapeType == SHAPETYPE.POLYGON)
                {
                    List<FPolygon> polygons = ReadPolygons(RecordContent);
                    for (int i = 0; i < polygons.Count; i++)
                    {
                        FFeature onefeature = new FFeature(polygons[i], ReadAttribute(table, RowIndex));
                        layer.Features.Add(onefeature);
                    }
                }
                RowIndex++;
                //其它代码,用于处理RecordContent
            }
            br.Close();
            fsr.Close();
            return layer;
        }

        static FPoint ReadPoint(byte[] RecordContent)
        {
            double x = BitConverter.ToDouble(RecordContent, 0);
            double y = BitConverter.ToDouble(RecordContent, 8);
            return new FPoint(new FVertex(x, y));
        }
        static List<FPolygon> ReadPolygons(byte[] RecordContent)
        {
            int N = BitConverter.ToInt32(RecordContent, 32);
            int M = BitConverter.ToInt32(RecordContent, 36);
            int[] parts = new int[N + 1];

            for (int i = 0; i < N; i++)
            {
                parts[i] = BitConverter.ToInt32(RecordContent, 40 + i * 4);
            }
            parts[N] = M;
            List<FPolygon> polygons = new List<FPolygon>();
            for (int i = 0; i < N; i++)
            {
                List<FVertex> vertexs = new List<FVertex>();
                for (int j = parts[i]; j < parts[i + 1]; j++)
                {
                    double x = BitConverter.ToDouble(RecordContent, 40 + N * 4 + j * 16);
                    double y = BitConverter.ToDouble(RecordContent, 40 + N * 4 + j * 16 + 8);
                    vertexs.Add(new FVertex(x, y));
                }
                polygons.Add(new FPolygon(vertexs));
            }
            return polygons;
        }

        static List<FLine> ReadLines(byte[] RecordContent)
        {
            int N = BitConverter.ToInt32(RecordContent, 32);
            int M = BitConverter.ToInt32(RecordContent, 36);
            int[] parts = new int[N + 1];

            for (int i = 0; i < N; i++)
            {
                parts[i] = BitConverter.ToInt32(RecordContent, 40 + i * 4);
            }
            parts[N] = M;
            List<FLine> lines = new List<FLine>();
            for (int i = 0; i < N; i++)
            {
                List<FVertex> vertexs = new List<FVertex>();
                for (int j = parts[i]; j < parts[i + 1]; j++)
                {
                    double x = BitConverter.ToDouble(RecordContent, 40 + N * 4 + j * 16);
                    double y = BitConverter.ToDouble(RecordContent, 40 + N * 4 + j * 16 + 8);
                    vertexs.Add(new FVertex(x, y));
                }
                lines.Add(new FLine(vertexs));
            }
            return lines;
        }

        static int FromBigToLittle(int bigvalue)
        {
            byte[] bigbytes = new byte[4];
            GCHandle handle = GCHandle.Alloc(bigbytes, GCHandleType.Pinned);
            Marshal.StructureToPtr(bigvalue, handle.AddrOfPinnedObject(), false);
            handle.Free();
            byte b2 = bigbytes[2];
            byte b3 = bigbytes[3];
            bigbytes[3] = bigbytes[0];
            bigbytes[2] = bigbytes[1];
            bigbytes[1] = b2;
            bigbytes[0] = b3;
            return BitConverter.ToInt32(bigbytes, 0);
        }
    }
}
