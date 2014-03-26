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
    public class SolidMechanicsModel
    {
        public Model Model { get; private set; }
        public double MaxAmplitude { get; set; }
        public int HorizontalElements { get; set; }

        public SolidMechanicsModel(Shape shape)
        {
            Model = new Model(shape);
        }

        public SolidMechanicsModel()
        {
            Model = new Model();
        }

        public virtual void Copy(SolidMechanicsModel model) 
        {
            HorizontalElements = model.HorizontalElements;
            MaxAmplitude = model.MaxAmplitude;
            Model.Copy(model.Model);
        }

        public void Save(string filename)
        {
            BinaryFormatter serializator = new BinaryFormatter();
            using (FileStream sw = new FileStream(filename, FileMode.Create))
            {
                serializator.Serialize(sw, this);
            }
        }

        public static SolidMechanicsModel Load(string filename)
        {
            SolidMechanicsModel model = null;
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream sw = new FileStream(filename, FileMode.Open))
            {
                model = formatter.Deserialize(sw) as SolidMechanicsModel;
            }
            return model;
        }
    }
}
