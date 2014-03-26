using FEMLibrary.SolidMechanics.Geometry;
using FEMLibrary.SolidMechanics.Meshing;
using FEMLibrary.SolidMechanics.Physics;
using System.IO;
using System.Xml.Serialization;
using System;
using System.Runtime.Serialization.Formatters.Binary;

namespace FEMLibrary.SolidMechanics.GUI.Models
{
    [Serializable]
    public class SolidMechanicsModel2D : SolidMechanicsModel
    {
        public int VerticalElements { get; set; }

        public SolidMechanicsModel2D(Shape shape) : base(shape)
        {
        }

        public SolidMechanicsModel2D():base()
        {
        }

        public void Copy(SolidMechanicsModel2D model) 
        {
            VerticalElements = model.VerticalElements;
            base.Copy(model);
        }

        
    }
}
