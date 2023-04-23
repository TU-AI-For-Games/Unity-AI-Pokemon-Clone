using System.Collections.Generic;
using Learning;
using UnityEngine;

public class NeuralNetwork
{
    private int[] m_networkShape;
    private Layer[] m_layers;

    public NeuralNetwork(int[] networkShape, float learningRate, Layer.ActivationFunction activationFunction)
    {
        m_networkShape = new int[networkShape.Length];

        for (int i = 0; i < networkShape.Length; i++)
        {
            m_networkShape[i] = networkShape[i];
        }

        m_layers = new Layer[networkShape.Length - 1];

        for (int i = 0; i < m_layers.Length; ++i)
        {
            m_layers[i] = new Layer(networkShape[i], networkShape[i + 1], learningRate, activationFunction);
        }
    }

    public void Train(List<LearningData> trainingData, int numEpochs)
    {
        for (int i = 0; i < numEpochs; i++)
        {
            foreach (LearningData data in trainingData)
            {
                FeedForward(data.Targets);
                BackPropagation(data.Values);
            }
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

        for (int i = 1; i < m_layers.Length; i++)
        {
            m_layers[i].FeedForward(m_layers[i - 1].Outputs);
        }
    }

    private void BackPropagation(float[] expected)
    {
        for (int i = m_layers.Length - 1; i >= 0; i--)
        {
            if (i == m_layers.Length - 1)
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
}