namespace FEMLibrary.SolidMechanics.GUI.Models
{
    public class ResultItem
    {
        public ResultItem()
        {
            Alfa1 = 0;
            /*U1Analitical = 0;
            U3Analitical = 0;*/
            U1Numeric = 0;
            U3Numeric = 0;
        }

        public double Alfa1 { get; set; }
        public double U1Numeric { get; set; }
        public double U3Numeric { get; set; }
        /*public double U1Analitical { get; set; }
        public double U3Analitical { get; set; }*/
    }
}
