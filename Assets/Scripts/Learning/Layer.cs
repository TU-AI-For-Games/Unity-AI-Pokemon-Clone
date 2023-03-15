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

        public float[,] Weights;
        private float[,] m_weightsDelta;

        public float[] Gamma;
        private float[] m_error;

        private float m_learningRate;

        public Layer(int numInputs, int numOutputs, float learningRate)
        {
            m_numInputs = numInputs;
            m_numOutputs = numOutputs;

            m_inputs = new float[numInputs];
            Outputs = new float[m_numOutputs];

            Weights = new float[numOutputs, numInputs];
            m_weightsDelta = new float[numOutputs, numInputs];

            Gamma = new float[m_numOutputs];
            m_error = new float[m_numOutputs];

            m_learningRate = learningRate;

            InitialiseWeights();
        }

        private void InitialiseWeights()
        {
            for (int i = 0; i < m_numOutputs; i++)
            {
                for (int j = 0; j < m_numInputs; j++)
                {
                    Weights[i, j] = Random.Range(-0.5f, 0.5f);
                }
            }
        }

        private static float DeriveTanH(float value)
        {
            return 1 - value * value;
        }

        public void BackPropagationOutputLayer(float[] expected)
        {
            for (int i = 0; i < m_numOutputs; i++)
            {
                m_error[i] = Outputs[i] - expected[i];
            }

            for (int i = 0; i < m_numOutputs; i++)
            {
                Gamma[i] = m_error[i] * DeriveTanH(Outputs[i]);
            }

            UpdateWeightsDelta();
        }

        public void BackPropagationHiddenLayer(float[] gammaForward, float[,] forwardWeights)
        {
            for (int i = 0; i < m_numOutputs; i++)
            {
                Gamma[i] = 0;

                for (int j = 0; j < gammaForward.Length; j++)
                {
                    Gamma[j] += gammaForward[j] * forwardWeights[j, i];
                }

                Gamma[i] *= DeriveTanH(Outputs[i]);
            }

            UpdateWeightsDelta();
        }

        private void UpdateWeightsDelta()
        {
            for (int i = 0; i < m_numOutputs; i++)
            {
                for (int j = 0; j < m_numInputs; j++)
                {
                    m_weightsDelta[i, j] = Gamma[i] * m_inputs[j];
                }
            }
        }

        public void UpdateWeights()
        {
            for (int i = 0; i < m_numOutputs; i++)
            {
                for (int j = 0; j < m_numInputs; j++)
                {
                    Weights[i, j] -= m_weightsDelta[i, j] * m_learningRate;
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
                    Outputs[i] += m_inputs[j] * Weights[i, j];
                }

                Outputs[i] = (float)Math.Tanh(Outputs[i]);
            }


            return Outputs;
        }
    }
}