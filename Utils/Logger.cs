using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MatrixLibrary;

namespace FEMLibrary.SolidMechanics.Utils
{
    public class Logger:ILogger, IDisposable
    {
        private StreamWriter sw;
        public Logger() {
            sw = new StreamWriter("log.txt");
        }
        public void Log(string log)
        {
            sw.WriteLine(log);
        }

        public void LogEigenVector(Vector eigenvector, int nodesOnLine)
        {
            nodesOnLine++;
            nodesOnLine *= 2;
            sw.WriteLine("In first dimenssion\n");

            for (int i = 0; i < eigenvector.Length; i++)
            {
                if (i % 2 == 0)
                {
                    sw.WriteLine(eigenvector[i]);
                }
                if ((i+1) % nodesOnLine == 0)
                {
                    sw.WriteLine();
                }
            }

            sw.WriteLine("\n\nIn second dimenssion\n");

            for (int i = 0; i < eigenvector.Length; i++)
            {
                if (i % 2 == 1)
                {
                    sw.WriteLine(eigenvector[i]);
                }

                if ((i + 1) % nodesOnLine == 0)
                {
                    sw.WriteLine();
                }
            }
        }

        public void Dispose()
        {
            sw.Dispose();
        }
    }
}
