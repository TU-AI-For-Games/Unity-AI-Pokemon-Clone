
namespace Learning
{
    public class InputData
    {
        public double[] Values;
        public double[] Targets;

        public InputData(double[] values, double[] targets)
        {
            Values = values;
            Targets = targets;
        }
    }
}