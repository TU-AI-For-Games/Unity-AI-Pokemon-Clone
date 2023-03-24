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

        private readonly float m_lambda;
        private readonly float m_learningRate;

        public enum WeightInitialisationMode
        {
            Random,
            Xavier,
            He
        }

        public enum NeuronActivationMode
        {
            TanH,
            ReLU,
            Sigmoid
        }

        private readonly NeuronActivationMode m_neuronActivationMode;

        public Layer(int numInputs, int numOutputs, float learningRate, float lambda, WeightInitialisationMode initialisationMode, NeuronActivationMode neuronActivationMode)
        {
            m_numInputs = numInputs;
            m_numOutputs = numOutputs;

            m_inputs = new float[numInputs];
            Outputs = new float[m_numOutputs];

            Weights = new float[numOutputs, numInputs];
            m_weightsDelta = new float[numOutputs, numInputs];

            Gamma = new float[m_numOutputs];
            m_error = new float[m_numOutputs];

            m_lambda = lambda;

            m_learningRate = learningRate;

            InitialiseWeights(initialisationMode);
            m_neuronActivationMode = neuronActivationMode;
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

            foreach (float weight in Weights)
            {
                if (float.IsNaN(weight))
                {
                    Debug.Log("Something is very broken...");
                }
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

                    if (float.IsNaN(Weights[i, j]))
                    {
                        Debug.Log("What the actual fuck...");
                    }
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

        public void BackPropagationOutputLayer(float[] expected)
        {
            for (int i = 0; i < m_numOutputs; i++)
            {
                m_error[i] = Outputs[i] - expected[i];
            }

            for (int i = 0; i < m_numOutputs; i++)
            {
                float smoothedOutput = Math.Max(Math.Min(Outputs[i], 1 - 1e-7f), 1e-7f);
                Gamma[i] = m_error[i] / (smoothedOutput * (1 - smoothedOutput)) * DeriveActivationFunction(Outputs[i], m_neuronActivationMode);

                if (float.IsNaN(Gamma[i]))
                {
                    Debug.Log("I will actually shoot myself in the head");
                }
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

                    if (float.IsNaN(Gamma[j]) || float.IsNegativeInfinity(Gamma[j]) || float.IsPositiveInfinity(Gamma[j]))
                    {
                        Gamma[j] = 1e-8f;
                    }
                }

                float gammaBefore = Gamma[i];

                Gamma[i] *= DeriveActivationFunction(Outputs[i], m_neuronActivationMode);

                if (float.IsNaN(Gamma[i]) || float.IsNegativeInfinity(Gamma[i]) || float.IsPositiveInfinity(Gamma[i]))
                {
                    Debug.Log("OMFG");
                }
            }

            UpdateWeightsDelta();
        }

        private void UpdateWeightsDelta()
        {
            for (int i = 0; i < m_numOutputs; i++)
            {
                for (int j = 0; j < m_numInputs; j++)
                {
                    if (float.IsNaN(Gamma[i]) || float.IsNaN(m_inputs[j]))
                    {
                        Debug.Log("Fuck my life");
                    }

                    m_weightsDelta[i, j] = Gamma[i] * m_inputs[j];

                    if (float.IsNaN(m_weightsDelta[i, j]))
                    {
                        Debug.Log("Guess it's time for me to die");
                    }
                }
            }
        }

        public void UpdateWeights()
        {
            for (int i = 0; i < m_numOutputs; i++)
            {
                for (int j = 0; j < m_numInputs; j++)
                {
                    float l2Regularisation = m_lambda * Weights[i, j];

                    if (float.IsNaN(l2Regularisation))
                    {
                        Debug.Log("Breaking here!");
                    }

                    Weights[i, j] -= (m_weightsDelta[i, j] + l2Regularisation) * m_learningRate;

                    if (float.IsNaN(Weights[i, j]))
                    {
                        Debug.Log("I am so done with this shit");
                    }
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

                float outputBeforeActivation = Outputs[i];

                Outputs[i] = Activate(Outputs[i], m_neuronActivationMode);

                if (float.IsNaN(Outputs[i]))
                {
                    Debug.Log("Eek...");
                }
            }


            return Outputs;
        }

        private float Activate(float value, NeuronActivationMode mode)
        {
            return mode switch
            {
                NeuronActivationMode.TanH => (float)Math.Tanh(value),
                NeuronActivationMode.ReLU => Relu(value),
                NeuronActivationMode.Sigmoid => Sigmoid(value),
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        private static float Relu(float value) => MathF.Max(0, value);

        private static float Sigmoid(float value)
        {
            float result = 1f / (1f + MathF.Exp(-value));

            if (float.IsNaN(result))
            {
                Debug.Log("Fuck my life");
            }

            return result;
        }

        private static float DeriveActivationFunction(float value, NeuronActivationMode mode)
        {
            return mode switch
            {
                NeuronActivationMode.TanH => DeriveTanH(value),
                NeuronActivationMode.ReLU => DeriveReLU(value),
                NeuronActivationMode.Sigmoid => DeriveSigmoid(value),
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        private static float DeriveTanH(float value) => 1 - value * value;

        private static float DeriveReLU(float value) => value <= 0f ? 0f : 1f;

        private static float DeriveSigmoid(float value)
        {
            float sigmoid = Sigmoid(value);

            if (float.IsNaN(sigmoid))
            {
                Debug.Log("Fuck me");
            }

            float derivation = sigmoid * (1 - sigmoid);
            if (float.IsNaN(derivation))
            {
                Debug.Log("Guess I'll just kms");
            }

            return derivation;
        }

        public float[] CalculateCrossEntropyLossGradient(float[] expectedOutputs, float[] actualOutputs)
        {
            float[] gradient = new float[expectedOutputs.Length];

            for (int i = 0; i < expectedOutputs.Length; i++)
            {
                gradient[i] = actualOutputs[i] - expectedOutputs[i];
            }

            return gradient;
        }

        private static float CrossEntropyLoss(float t, float y)
        {
            const float epsilon = 1e-8f;
            y = MathF.Max(y, epsilon);
            y = MathF.Min(y, 1 - epsilon);
            float result = -t * MathF.Log(y) - (1 - t) * MathF.Log(1 - y);

            if (float.IsNaN(result))
            {
                Debug.Log("Oops!");
            }

            return result;
        }
    }
}