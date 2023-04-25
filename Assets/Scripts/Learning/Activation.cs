using System;

namespace Learning
{
    public static class TanH
    {
        public static float Evaluate(float value)
        {
            return MathF.Tanh(value);
        }

        public static float Derive(float value)
        {
            return 1 - Evaluate(value) * Evaluate(value);
        }
    }

    public static class Sigmoid
    {
        public static float Evaluate(float value)
        {
            return 1f / (1f + MathF.Exp(-value));
        }

        public static float Derive(float value)
        {
            return Evaluate(value) * (1 - Evaluate(value));
        }
    }

    public static class ReLU
    {
        public static float Evaluate(float value)
        {
            return value < 0 ? 0 : value;
        }

        public static float Derive(float value)
        {
            return value < 0 ? 0 : 1;
        }
    }
}