using MatrixLibrary;
using FEMLibrary.SolidMechanics.Geometry;
using System;

namespace FEMLibrary.SolidMechanics.Physics
{
    public enum BoundaryConditionsType 
    {
        Static, Kinematic
    }
    [Serializable]
    public class BoundaryCondition
    {
        public BoundaryConditionsType Type { get; set; }
        public Vector Value { get; private set; }

        public BoundaryCondition(BoundaryConditionsType type, Vector value)
        {
            Type = type;
            Value = value;
        }

        public void Copy(BoundaryCondition condition) 
        {
            Type = condition.Type;
            Value.Copy(condition.Value);
        }
    }
}
