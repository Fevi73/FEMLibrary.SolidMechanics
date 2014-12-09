using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FEMLibrary.SolidMechanics.Geometry
{
    [Serializable]
    public class Rectangle:Shape
    {
        private double _height;
        public double Height
        {
            get { return _height; }
            set { setHeight(value); }
        }
        private double _width;
        public double Width
        {
            get { return _width; }
            set { setWidth(value); }
        }

        private void setWidth(double width) 
        {
            _width = width;
            _rightBottomPoint.X = _leftBottomPoint.X +_width;
            _rightTopPoint.X = _leftBottomPoint.X + _width;
        }

        private void setHeight(double height)
        {
            _height = height;
            _leftTopPoint.Y = _leftBottomPoint.Y + _height;
            _rightTopPoint.Y = _leftBottomPoint.Y + _height;
        }

        public void SetBottomY(double y) {
            _leftBottomPoint.Y = y;
            _rightBottomPoint.Y = y;
        }

        private Point _leftBottomPoint;
        private Point _leftTopPoint;
        private Point _rightTopPoint;
        private Point _rightBottomPoint;

        public Rectangle(double x, double y, double height, double width) {
            _height = height;
            _width = width;

            _leftBottomPoint = new Point(x, y);
            _leftTopPoint = new Point(_leftBottomPoint.X, _leftBottomPoint.Y + height);
            _rightTopPoint = new Point(_leftBottomPoint.X + _width, _leftBottomPoint.Y + _height);
            _rightBottomPoint = new Point(_leftBottomPoint.X + _width, _leftBottomPoint.Y);

            Points.Add(_leftTopPoint);
            Points.Add(_rightTopPoint);
            Points.Add(_rightBottomPoint);
            Points.Add(_leftBottomPoint);

            Edges.Add(new Edge(_leftBottomPoint, _leftTopPoint));
            Edges.Add(new Edge(_leftTopPoint, _rightTopPoint));
            Edges.Add(new Edge(_rightTopPoint, _rightBottomPoint));
            Edges.Add(new Edge(_rightBottomPoint, _leftBottomPoint));
        }

        public Rectangle(double height, double width)
            : this(0, 0, height, width)
        {
        }

        public Rectangle()
            : this(0, 0)
        { }

        public void Copy(Rectangle rectangle)
        {
            if (rectangle != null) {
                Height = rectangle.Height;
                Width = rectangle.Width;
                base.Copy(rectangle);
            }
        }

        
    }
}
