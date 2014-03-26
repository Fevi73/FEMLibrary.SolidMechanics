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
    public class CylindricalPlate:Rectangle
    {
        private double _curvature;
        public double Curvature
        {
            get { return _curvature; }
            set { _curvature = value; }
        }
        

        public CylindricalPlate(double x, double y, double height, double width, double curvature):base(x,y,height,width) {
            Curvature = curvature;
        }

        public CylindricalPlate(double height, double width, double curvature)
            : this(0, 0, height, width, curvature)
        {
        }

        public CylindricalPlate(double height, double width)
            : this(0, 0, height, width, 0)
        {
        }

        public CylindricalPlate()
            : this(0, 0)
        { }

        public void Copy(CylindricalPlate plate)
        {
            if (plate != null)
            {
                base.Copy(plate);
                this.Curvature = plate.Curvature;
            }
        }

        
    }
}
