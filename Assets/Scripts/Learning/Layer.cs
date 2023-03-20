using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Learning
{
    public class Layer
    {
        private readonly int m_numInputs;
        private readonly int m_numOutputs;

        private float[] m_inputs;
        public float[] Outputs;

        public float[,] Weights;
        private readonly float[,] m_weightsDelta;

        public float[] Gamma;
        private readonly float[] m_error;

        private readonly float m_learningRate;

        public enum WeightInitialisationMode
        {
            Random,
            Xavier,
            He
        }


        public Layer(int numInputs, int numOutputs, float learningRate, WeightInitialisationMode initialisationMode)
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

            InitialiseWeights(initialisationMode);
        }

        private void InitialiseWeights(WeightInitialisationMode mode)
        {
            switch (mode)
            {
                case WeightInitialisationMode.Random:
                    InitialiseWeightsRandom();
                    break;
                case WeightInitialisationMode.Xavier:
                    InitialiseWeightsXavier();
                    break;
                case WeightInitialisationMode.He:
                    InitialiseWeightsHe();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

        }

        private void InitialiseWeightsRandom()
        {
            for (int i = 0; i < m_numOutputs; i++)
            {
                for (int j = 0; j < m_numInputs; j++)
                {
                    Weights[i, j] = Random.Range(-0.5f, 0.5f);
                }
            }
        }

        private void InitialiseWeightsXavier()
        {
            float scale = Mathf.Sqrt(2.0f / (m_numInputs + m_numOutputs));

            for (int i = 0; i < m_numOutputs; i++)
            {
                for (int j = 0; j < m_numInputs; j++)
                {
                    Weights[i, j] = Random.Range(-scale, scale);
                }
            }
        }

        private void InitialiseWeightsHe()
        {
            float scale = Mathf.Sqrt(2f / m_numInputs);

            for (int i = 0; i < m_numOutputs; i++)
            {
                for (int j = 0; j < m_numInputs; j++)
                {
                    Weights[i, j] = Random.Range(-scale, scale);
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

        {
            m_inputs = input;

            for (int i = 0; i < m_numOutputs; i++)
            {
                Outputs[i] = 0;

                for (int j = 0; j < m_numInputs; j++)
                {
                    Outputs[i] += m_inputs[j] * Weights[i, j];
                }

            }


            return Outputs;
        }
    }
}