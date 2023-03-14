using System;
using UnityEngine;
using Random = UnityEngine.Random;

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

        private float m_learningRate = 0.0003f;

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

            InitialiseWeights();
        }

        private void InitialiseWeights()
        {
            for (int i = 0; i < m_numOutputs; i++)
            {
                for (int j = 0; j < m_numInputs; j++)
                {
                    m_weights[i, j] = Random.Range(-0.5f, 0.5f);
                }
            }
        }

        private static float DeriveTanH(float value)
        {
            return 1 - value * value;
        }

        private void BackPropagationOutputLayer(float[] expected)
        {
            for (int i = 0; i < m_numOutputs; i++)
            {
                m_error[i] = Outputs[i] - expected[i];
            }

            for (int i = 0; i < m_numOutputs; i++)
            {
                m_gamma[i] = m_error[i] * DeriveTanH(Outputs[i]);
            }

            for (int i = 0; i < m_numOutputs; i++)
            {
                for (int j = 0; j < m_numInputs; j++)
                {
                    m_weightsDelta[i, j] = m_gamma[i] * m_inputs[j];
                }
            }
        }

        private void UpdateWeights()
        {
            for (int i = 0; i < m_numOutputs; i++)
            {
                for (int j = 0; j < m_numInputs; j++)
                {
                    m_weights[i, j] -= m_weightsDelta[i, j] * m_learningRate;
                }
            }
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