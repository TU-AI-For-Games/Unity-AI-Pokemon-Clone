using System;

namespace Learning
{
    public interface IActivationFunction
    {
        public double Compute(double value);
        public double Derive(double value);
    }

}