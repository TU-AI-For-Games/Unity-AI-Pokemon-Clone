using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        public enum ActivationFunction
        {
            Sigmoid,
            TanH,
            ReLU
        }

        private ActivationFunction m_activationFunction;

        public Layer(int numInputs, int numOutputs, float learningRate, ActivationFunction activationFunction)
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
            m_activationFunction = activationFunction;
        }

        public Layer()
        {
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

        public void BackPropagationOutputLayer(float[] expected)
        {
            for (int i = 0; i < m_numOutputs; i++)
            {
                m_error[i] = Outputs[i] - expected[i];
            }

            for (int i = 0; i < m_numOutputs; i++)
            {
                Gamma[i] = m_error[i] * Derive(Outputs[i]);
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

                Gamma[i] *= Derive(Outputs[i]);
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

                Outputs[i] = Evaluate(Outputs[i]);
            }


            return Outputs;
        }

        private float Evaluate(float value)
        {
            switch (m_activationFunction)
            {
                case ActivationFunction.Sigmoid:
                    return Sigmoid.Evaluate(value);
                case ActivationFunction.TanH:
                    return TanH.Evaluate(value);
                case ActivationFunction.ReLU:
                    return ReLU.Evaluate(value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private float Derive(float value)
        {
            switch (m_activationFunction)
            {
                case ActivationFunction.Sigmoid:
                    return Sigmoid.Derive(value);
                case ActivationFunction.TanH:
                    return TanH.Derive(value);
                case ActivationFunction.ReLU:
                    return ReLU.Derive(value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Save(StreamWriter writer)
        {
            string output = $"{m_numInputs},{m_numOutputs},";

            // numInputs, numOutputs, {input1, input2}, {output1, output2}, {{weight1, weight2}, {weight1, weight2}}, {{weightsDelta}}, {gamma}, {error}, learningRate, activationfunction
            output += FlattenArray1D(m_inputs) + ",";
            output += FlattenArray1D(Outputs) + ",";
            output += FlattenArray2D(Weights) + ",";
            output += FlattenArray2D(m_weightsDelta) + ",";
            output += FlattenArray1D(Gamma) + ",";
            output += FlattenArray1D(m_error) + ",";
            output += $"{m_learningRate},{(int)m_activationFunction}";

            writer.WriteLine(output);
        }

        public void Load(string data)
        {
            Regex regex = new(",(?![^{}^]*\\})");
            string[] substrings = regex.Split(data);

            m_numInputs = int.Parse(substrings[0]);
            m_numOutputs = int.Parse(substrings[1]);
            m_inputs = BuildArray1D(substrings[2]);
            Outputs = BuildArray1D(substrings[3]);
            Gamma = BuildArray1D(substrings[6]);
            m_error = BuildArray1D(substrings[7]);
            m_learningRate = float.Parse(substrings[8]);
            m_activationFunction = (ActivationFunction)int.Parse(substrings[9]);
        }

        private string FlattenArray1D(float[] array)
        {
            return $"{{{string.Join(',', array)}}}";
        }

        private float[] BuildArray1D(string input)
        {
            List<float> floats = new List<float>();

            string[] list = input.Substring(1, input.Length - 2).Split(',');

            foreach (string str in list)
            {
                floats.Add(float.Parse(str));
            }

            return floats.ToArray();
        }

        private string FlattenArray2D(float[,] array)
        {
            string stringArray = "{";
            for (int col = 0; col < array.GetLength(0); col++)
            {
                stringArray += "{";

                for (int row = 0; row < array.GetLength(1); row++)
                {
                    stringArray += array[col, row].ToString() + ",";
                }

                stringArray += "}";
            }

            stringArray += "}";

            return stringArray;
        }

    }
}