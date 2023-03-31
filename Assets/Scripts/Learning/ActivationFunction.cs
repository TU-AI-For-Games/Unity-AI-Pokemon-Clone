using System;

namespace Learning
{
    public interface IActivationFunction
    {
        public double Compute(double value);
        public double Derive(double value);
    }

    public class Sigmoid : IActivationFunction
    {
        public double Compute(double value)
        {
            return 1d / (1 + MathF.Pow(MathF.E, (float)-value));
        }

        public double Derive(double value)
        {
            return value * (1 - value);
        }
    }
}