using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FEMLibrary.SolidMechanics.Physics;
using MatrixLibrary;
using FEMLibrary.SolidMechanics.Geometry;
using FEMLibrary.SolidMechanics.NumericalUtils;
using FEMLibrary.SolidMechanics.Meshing;
using FEMLibrary.SolidMechanics.Results;

namespace FEMLibrary.SolidMechanics.Solving
{
    public class StationaryNonlinear2DSolver:Solver
    {
        private int _iterations = 0;
        public int Iterations { get { return _iterations; }
        }

        public StationaryNonlinear2DSolver(Model model, Mesh mesh, double error, int maxIterations)
            : base(model, mesh)
        { 
            _error = error;
            _maxIterations = maxIterations;
        }

        private double _error;
        private double _maxIterations;

        private Matrix ConstMatrix;

        private double M1;
        private double M2;
        private double M3;
        private double G13;

        private IFiniteElement elementCurrent;
        private StaticNumericalResult previousRes;

        public override IEnumerable<INumericalResult> Solve(int resultsCount)
        {
            List<INumericalResult> results = new List<INumericalResult>();
            previousRes = new StaticNumericalResult(_mesh.Elements, null);
            results.Add(previousRes);
            _iterations = 0;

            if (_mesh.IsMeshGenerated)
            {
                GetConstantMatrix();    
                Vector U = new Vector(_mesh.Nodes.Count*2);

                do
                {
                    previousRes.U = U;
                    Matrix StiffnessMatrix = GetStiffnessMatrix();
                    Vector TotalVector = GetTotalVector();
                    AsumeStaticBoundaryConditions(StiffnessMatrix, TotalVector);
                    U = StiffnessMatrix.LUalgorithm(TotalVector);
                    _iterations++;
                } while ((Vector.Norm(previousRes.U - U) > _error*Vector.Norm(U)) && (_iterations < _maxIterations));

                previousRes.U = U;
            }
            return results;
        }

        private Matrix GetConstantMatrix()
        {
            ConstMatrix = new Matrix(4, 4);
            M1 = ConstMatrix[0, 0] = _model.Material.GetE1Modif() * (1 + _model.Material.GetAlfa1());
            M2 = ConstMatrix[1, 0] = ConstMatrix[0, 1] = _model.Material.GetLambda1() * _model.Material.GetE0();
            M3 = ConstMatrix[1, 1] = _model.Material.GetE0();
            G13 = ConstMatrix[2, 2] = ConstMatrix[2, 3] = ConstMatrix[3, 2] = ConstMatrix[3, 3] = _model.Material.GetG13();
            return ConstMatrix;
        }

        #region Boundary conditions
        //static == fixed
        private void AsumeStaticBoundaryConditions(Matrix stiffnessMatrix, Vector totalVector)
        {
            foreach(Edge edge in _model.Shape.Edges)
            {
                BoundaryCondition boundaryCondition = _model.BoundaryConditions[edge];
                if (boundaryCondition.Type == BoundaryConditionsType.Static)
                {
                    IEnumerable<FiniteElementNode> nodes = _mesh.GetNodesOnEdge(edge);
                    foreach (FiniteElementNode node in nodes)
                    {
                        stiffnessMatrix[2 * node.Index, 2 * node.Index] *= 1000000000000;
                        stiffnessMatrix[2 * node.Index + 1, 2 * node.Index + 1] *= 1000000000000;
                    }
                }
                else
                {
                    IEnumerable<Segment> segments = _mesh.GetSegmentsOnEdge(edge);
                    foreach (Segment segment in segments)
                    {
                        Vector localVector = GetLocalTotalVector(segment, boundaryCondition.Value);
                        for (int i = 0; i < segment.Count; i++)
                        {
                            totalVector[2 * segment[i].Index] += localVector[2 * i];
                            totalVector[2 * segment[i].Index + 1] += localVector[2 * i + 1];
                        }
                    }
                }
            }

            foreach (Point point in _model.Shape.Points)
            {
                BoundaryCondition pointCondition = _model.PointConditions[point];
                if (pointCondition.Type == BoundaryConditionsType.Static)
                {
                    FiniteElementNode node = _mesh.GetNodeOnPoint(point);
                    if (node != null)
                    {
                        stiffnessMatrix[2 * node.Index, 2 * node.Index] *= 1000000000000;
                        stiffnessMatrix[2 * node.Index + 1, 2 * node.Index + 1] *= 1000000000000;
                    }
                }
            }
        }
        #endregion

        #region Total Vector
        private Vector GetTotalVector()
        {
            Vector TotalVector = new Vector(_mesh.Nodes.Count * 2);

            foreach (IFiniteElement element in _mesh.Elements)
            {
                Matrix NonlinearLocalTotalVector = GetNonlinearLocalTotalVector(element);

                for (int i = 0; i < element.Count; i++)
                {
                    TotalVector[2 * element[i].Index] -= NonlinearLocalTotalVector[2 * i, 0];
                    TotalVector[2 * element[i].Index + 1] -= NonlinearLocalTotalVector[2 * i + 1, 0];
                }
            }
            return TotalVector;
        }

        #region Local Vector
        private Vector GetLocalTotalVector(Segment segment, Vector load)
        {
            Vector vector = new Vector(4);
            vector[1] = vector[3] = (load[2] * segment.Length()) / 2.0;
            vector[0] = vector[2] = (load[0] * segment.Length()) / 2.0;
            return vector;
        }

        #endregion

        private Matrix GetNonlinearLocalTotalVector(IFiniteElement element)
        {
            elementCurrent = element;

            Matrix NonlinearLocalTotalVector = Integration.GaussianIntegrationMatrix(LocalVectorFunction);

            return NonlinearLocalTotalVector;
        }

        private Matrix LocalVectorFunction(double ksi, double eta)
        {
            Matrix derivativeMatrix = GetLocalDerivativeMatrix(elementCurrent, ksi, eta);

            JacobianRectangular J = new JacobianRectangular();
            J.Element = elementCurrent;
            Matrix VariableVectorOnElement = GetVariableVectorOnElement(elementCurrent, ksi, eta);

            return derivativeMatrix * VariableVectorOnElement * J.GetJacobianDeterminant(ksi, eta);
        }

        private Matrix GetVariableVectorOnElement(IFiniteElement element, double ksi, double eta)
        {
            Matrix VariableVector = new Matrix(4, 1);
            Vector du = previousRes.DU(ksi, eta, element);
            double d1u1 = du[0];
            double d3u3 = du[1];
            double d3u1 = du[2];
            double d1u3 = du[3];


            VariableVector[0, 0] = 0.5 * M1 * d1u1 * d1u1 + 0.5 * M1 * d1u3 * d1u3 + 0.5 * M2 * d3u1 * d3u1 + 0.5 * M2 * d3u3 * d3u3;
            VariableVector[1, 0] = 0.5 * M2 * d1u1 * d1u1 + 0.5 * M2 * d1u3 * d1u3 + 0.5 * M3 * d3u1 * d3u1 + 0.5 * M3 * d3u3 * d3u3;
            VariableVector[2, 0] = G13 * (d1u1 * d3u1 + d1u3 * d3u3);
            VariableVector[3, 0] = G13 * (d1u1 * d3u1 + d1u3 * d3u3);

            return VariableVector;
        }
        
        #endregion

        #region StiffnessMatrix

        private Matrix GetStiffnessMatrix()
        {
            Matrix StiffnessMatrix = new Matrix(_mesh.Nodes.Count * 2, _mesh.Nodes.Count * 2);
            foreach (IFiniteElement element in _mesh.Elements)
            {
                Matrix localStiffnessMatrix = GetLocalStiffnessMatrix(element);
                
                for (int i = 0; i < element.Count; i++)
                {
                    for (int j = 0; j < element.Count; j++)
                    {
                        StiffnessMatrix[2 * element[i].Index, 2 * element[j].Index] += localStiffnessMatrix[2 * i, 2 * j];
                        StiffnessMatrix[2 * element[i].Index + 1, 2 * element[j].Index] += localStiffnessMatrix[2 * i + 1, 2 * j];
                        StiffnessMatrix[2 * element[i].Index, 2 * element[j].Index + 1] += localStiffnessMatrix[2 * i , 2 * j + 1];
                        StiffnessMatrix[2 * element[i].Index + 1, 2 * element[j].Index + 1] += localStiffnessMatrix[2 * i + 1, 2 * j + 1];
                    }    
                }
            }

            return StiffnessMatrix;
        }



        #region Local Matrix

        private Matrix GetLocalStiffnessMatrix(IFiniteElement element)
        {
            elementCurrent = element;
            
            Matrix localStiffnessMatrix = Integration.GaussianIntegrationMatrix(LocalStiffnessMatrixFunction);
            
            return localStiffnessMatrix;
        }

        private Matrix LocalStiffnessMatrixFunction(double ksi, double eta)
        {
            Matrix derivativeMatrix = GetLocalDerivativeMatrix(elementCurrent, ksi, eta);

            JacobianRectangular J = new JacobianRectangular();
            J.Element = elementCurrent;

            return derivativeMatrix * ConstMatrix * Matrix.Transpose(derivativeMatrix) * J.GetJacobianDeterminant(ksi, eta);
        }

        private Matrix GetLocalDerivativeMatrix(IFiniteElement element, double ksi, double eta)
        {
            Matrix LocalDerivativeMatrix = new Matrix(8, 4);

            Matrix gradNksieta = new Matrix(2,4);

            gradNksieta[0, 0] = (eta - 1) * 0.25;
            gradNksieta[1, 0] = (ksi - 1) * 0.25;
            gradNksieta[0, 1] = (1 - eta) * 0.25;
            gradNksieta[1, 1] = (-ksi - 1) * 0.25;
            gradNksieta[0, 2] = (eta + 1) * 0.25;
            gradNksieta[1, 2] = (ksi + 1) * 0.25;
            gradNksieta[0, 3] = (-eta - 1) * 0.25;
            gradNksieta[1, 3] = (1 - ksi) * 0.25;

            JacobianRectangular J = new JacobianRectangular();
            J.Element = element;

            Matrix gradN = J.GetInverseJacobian(ksi, eta) * gradNksieta;

            LocalDerivativeMatrix[0, 0] = LocalDerivativeMatrix[1, 3] = gradN[0, 0];
            LocalDerivativeMatrix[2, 0] = LocalDerivativeMatrix[3, 3] = gradN[0, 1];
            LocalDerivativeMatrix[4, 0] = LocalDerivativeMatrix[5, 3] = gradN[0, 2];
            LocalDerivativeMatrix[6, 0] = LocalDerivativeMatrix[7, 3] = gradN[0, 3];

            LocalDerivativeMatrix[1, 1] = LocalDerivativeMatrix[0, 2] = gradN[1, 0];
            LocalDerivativeMatrix[3, 1] = LocalDerivativeMatrix[2, 2] = gradN[1, 1];
            LocalDerivativeMatrix[5, 1] = LocalDerivativeMatrix[4, 2] = gradN[1, 2];
            LocalDerivativeMatrix[7, 1] = LocalDerivativeMatrix[6, 2] = gradN[1, 3];

            return LocalDerivativeMatrix;
        }
        #endregion

        #endregion

        
    }
}
