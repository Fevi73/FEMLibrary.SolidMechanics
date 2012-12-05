using System.Collections.Generic;
using FEMLibrary.SolidMechanics.Geometry;
using MatrixLibrary;
using System;

namespace FEMLibrary.SolidMechanics.Physics
{
    [Serializable]
    public class Model
    {
        public Material Material { get; private set; }
        public Shape Shape { get; private set; }
        public Dictionary<Edge, BoundaryCondition> BoundaryConditions { get; private set; }
        public Dictionary<Point, BoundaryCondition> PointConditions { get; private set; }

        public Model(Material material, Shape shape) {
            Material = material;
            Shape = shape;
            BoundaryConditions = new Dictionary<Edge, BoundaryCondition>();
            foreach(Edge edge in Shape.Edges)
            {
                BoundaryConditions.Add(edge, new BoundaryCondition(BoundaryConditionsType.Kinematic, new Vector(3)));
            }

            PointConditions = new Dictionary<Point, BoundaryCondition>();
            foreach (Point point in Shape.Points)
            {
                PointConditions.Add(point, new BoundaryCondition(BoundaryConditionsType.Kinematic, new Vector(3)));
            }
        }
        public Model(Shape shape):this(new Material(), shape)
        {
        }

        public Model()
            : this(new Rectangle())
        {
        }

        public void Copy(Model model)
        {
            Material.Copy(model.Material);
            Shape.Copy(model.Shape);
            foreach (Edge edge in model.BoundaryConditions.Keys)
            {
                BoundaryConditions[edge].Copy(model.BoundaryConditions[edge]);
            }
            foreach (Point point in model.PointConditions.Keys)
            {
                PointConditions[point].Copy(model.PointConditions[point]);
            }

        }

    }
}
