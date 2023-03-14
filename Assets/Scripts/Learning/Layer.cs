using System;
using UnityEngine;

namespace Learning
{
    public class Layer
    {
        private int m_numInputs;
        private int m_numOutputs;

        private float[] m_inputs;
        public float[] Outputs;

        private float[,] m_weights;
        private float[,] m_weightsDelta;

        private float[] m_gamma;
        private float[] m_error;

        public Layer(int numInputs, int numOutputs)
        {
            m_numInputs = numInputs;
            m_numOutputs = numOutputs;

            m_inputs = new float[numInputs];
            Outputs = new float[m_numOutputs];

            m_weights = new float[numOutputs, numInputs];
            m_weightsDelta = new float[numOutputs, numInputs];

            m_gamma = new float[m_numOutputs];
            m_error = new float[m_numOutputs];
        }

        public float[] FeedForward(float[] input)
        {
            m_inputs = input;

            for (int i = 0; i < m_numOutputs; i++)
            {
                Outputs[i] = 0;

                for (int j = 0; j < m_numInputs; j++)
                {
                    Outputs[i] += m_inputs[j] * m_weights[i, j];
                }

                Outputs[i] = (float)Math.Tanh(Outputs[i]);
            }


            return Outputs;
        }
    }
}