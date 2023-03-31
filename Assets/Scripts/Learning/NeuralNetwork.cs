using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Learning
{
    public enum InitialisationMode
    {
        Random,
        Xavier
    }

    public class NeuralNetwork
    {
        private readonly List<Neuron> m_inputLayer;
        private readonly List<List<Neuron>> m_hiddenLayers;
        private readonly List<Neuron> m_outputLayer;

        public static int Inputs;
        public static int Outputs;
        private readonly int m_iterations;

        private static readonly System.Random _random = new(12345);
        private readonly double m_learningRate;
        private readonly double m_regressionRate;

        public NeuralNetwork(int iterations, int inputSize, int hiddenSize, int numHiddenLayers, int outputSize, double learningRate, double regressionRate)
        {
            m_iterations = iterations;

            Inputs = inputSize;
            Outputs = outputSize;

            m_learningRate = learningRate;
            m_regressionRate = regressionRate;

            m_inputLayer = new List<Neuron>(inputSize);
            m_hiddenLayers = new List<List<Neuron>>(numHiddenLayers);
            m_outputLayer = new List<Neuron>(outputSize);

            for (var i = 0; i < inputSize; i++)
            {
                m_inputLayer.Add(new Neuron());
            }

            for (int i = 0; i < numHiddenLayers; i++)
            {
                m_hiddenLayers.Add(new List<Neuron>());

                for (var j = 0; j < hiddenSize; j++)
                {
                    if (i == 0)
                    {
                        m_hiddenLayers[i].Add(new Neuron(m_inputLayer));
                    }
                    else
                    {
                        m_hiddenLayers[i].Add(new Neuron(m_hiddenLayers[i - 1]));
                    }
                }
            }

            for (var i = 0; i < outputSize; i++)
            {
                m_outputLayer.Add(new Neuron(m_hiddenLayers[numHiddenLayers - 1]));
            }
        }

        public void Train(List<InputData> dataset, double minError)
        {
            double currentError = 1.0d;
            int currentEpochs = 0;

            // Train until the error has been minimised, or until we have reached the max iterations
            while (currentError > minError && currentEpochs < m_iterations)
            {
                List<double> errors = new List<double>();

                foreach (InputData data in dataset)
                {
                    ForwardPropagate(data.Values);
                    BackPropagate(data.Targets);

                    errors.Add(CalculateError(data.Targets));
                }

                currentError = errors.Average();
                currentEpochs++;
            }
        }

        private void ForwardPropagate(double[] inputs)
        {
            int i = 0;

            foreach (Neuron neuron in m_inputLayer)
            {
                neuron.Value = inputs[i++];
            }

            foreach (List<Neuron> layer in m_hiddenLayers)
            {
                foreach (Neuron neuron in layer)
                {
                    neuron.CalculateValue();
                }
            }

            foreach (Neuron neuron in m_outputLayer)
            {
                neuron.CalculateValue();
            }
        }

        private void BackPropagate(double[] targets)
        {
            var i = 0;
            m_outputLayer.ForEach(a => a.CalculateGradient(targets[i++]));
            foreach (var layer in m_hiddenLayers.AsEnumerable<List<Neuron>>().Reverse())
            {
                layer.ForEach(a => a.CalculateGradient());
                layer.ForEach(a => a.UpdateWeights(m_learningRate, m_regressionRate));
            }
            m_outputLayer.ForEach(a => a.UpdateWeights(m_learningRate, m_regressionRate));
        }

        public double[] Compute(double[] inputs)
        {
            ForwardPropagate(inputs);
            return m_outputLayer.Select(neuron => neuron.Value).ToArray();
        }

        private double CalculateError(double[] targets)
        {
            var i = 0;
            return m_outputLayer.Sum(a => a.CalculateError(targets[i++]));
        }


        public static double GetRandomInitialisationValue(InitialisationMode mode)
        {
            switch (mode)
            {
                case InitialisationMode.Random:
                    return GetRandomNumber(-0.5, 0.5);
                case InitialisationMode.Xavier:
                    float distribution = Mathf.Sqrt(2 / (float)(Inputs + Outputs));
                    return GetRandomNumber(-distribution, distribution);
                default:
                    return 0d;
            }
        }

        public static double GetRandomNumber(double minimum, double maximum)
        {
            return _random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
