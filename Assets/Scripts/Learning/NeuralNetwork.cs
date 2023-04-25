using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Learning
{
    public class NeuralNetwork
    {
        private int[] m_networkShape;
        private List<Layer> m_layers;

        // The folder that the ANN will be saved out to when serialised
        private readonly string m_outputFolderName = $"{Application.dataPath}\\Resources\\NeuralNets";

        public NeuralNetwork(int[] networkShape, float learningRate, Layer.ActivationFunction activationFunction)
        {
            m_networkShape = new int[networkShape.Length];

            for (int i = 0; i < networkShape.Length; i++)
            {
                m_networkShape[i] = networkShape[i];
            }

            m_layers = new List<Layer>();

            for (int i = 0; i < networkShape.Length - 1; ++i)
            {
                m_layers.Add(new Layer(networkShape[i], networkShape[i + 1], learningRate, activationFunction));
            }
        }

        public void Train(List<TrainingData> trainingData, int numEpochs)
        {
            for (int i = 0; i < numEpochs; i++)
            {
                foreach (TrainingData data in trainingData)
                {
                    FeedForward(data.Targets);
                    BackPropagation(data.Values);
                }

                System.Diagnostics.Debug.WriteLine($"{i + 1}/{numEpochs} epochs");
            }
        }

        public float[] Compute(float[] input)
        {
            FeedForward(input);
            return m_layers[^1].Outputs;
        }

        private void FeedForward(float[] input)
        {
            m_layers[0].FeedForward(input);

            for (int i = 1; i < m_layers.Count; i++)
            {
                m_layers[i].FeedForward(m_layers[i - 1].Outputs);
            }
        }

        private void BackPropagation(float[] expected)
        {
            for (int i = m_layers.Count - 1; i >= 0; i--)
            {
                if (i == m_layers.Count - 1)
                {
                    m_layers[i].BackPropagationOutputLayer(expected);
                }
                else
                {
                    m_layers[i].BackPropagationHiddenLayer(m_layers[i + 1].Gamma, m_layers[i + 1].Weights);
                }
            }

            // Update the weights of the layers...
            foreach (Layer layer in m_layers)
            {
                layer.UpdateWeights();
            }
        }

        public void Save(string filename)
        {
            string filePath = $"{m_outputFolderName}\\{filename}.NEURALNET";

            File.WriteAllText(filePath, string.Empty);

            FileStream outStream = File.OpenWrite(filePath);

            StreamWriter writer = new StreamWriter(outStream);

            // Network shape
            string networkShape = string.Join(',', m_networkShape);
            writer.WriteLine(networkShape);

            // Each Layer on new line
            foreach (Layer layer in m_layers)
            {
                layer.Save(writer);
            }

            writer.Close();
            outStream.Close();
        }

        public void Load(string filename)
        {
            string filePath = $"{m_outputFolderName}\\{filename}.NEURALNET";

            string text = File.ReadAllText(filePath);
            string[] lines = text.Split(Environment.NewLine);

            m_layers = new List<Layer>();

            for (int i = 0; i < lines.Length; ++i)
            {
                if (lines[i].Length == 0)
                {
                    continue;
                }

                // Network details
                if (i == 0)
                {
                    string[] networkDetails = lines[i].Split(',');

                    m_networkShape = new int[networkDetails.Length];

                    for (int j = 0; j < networkDetails.Length; ++j)
                    {
                        m_networkShape[j] = int.Parse(networkDetails[j]);
                    }
                }
                else
                {
                    // Layer details
                    Layer layer = new Layer();
                    layer.Load(lines[i]);

                    m_layers.Add(layer);
                }
            }
        }
    }
}