using System.Collections.Generic;
using Learning;
using UnityEngine;

public class NeuralNetwork
{
    private readonly int[] m_networkShape;
    private readonly Layer[] m_layers;

    private readonly Layer.NeuronActivationMode m_neuronActivationMode;

    public NeuralNetwork(int[] networkShape, float learningRate, Layer.WeightInitialisationMode layerWeightInitialisationMode, Layer.NeuronActivationMode neuronActivationMode)
    {
        m_networkShape = new int[networkShape.Length];

        for (int i = 0; i < networkShape.Length; i++)
        {
            m_networkShape[i] = networkShape[i];
        }

        m_layers = new Layer[networkShape.Length - 1];

        for (int i = 0; i < m_layers.Length; ++i)
        {
            m_layers[i] = new Layer(networkShape[i], networkShape[i + 1], learningRate, layerWeightInitialisationMode);
        }
    }

    public float[] FeedForward(float[] input)
    {
        m_layers[0].FeedForward(input, m_neuronActivationMode);

        for (int i = 1; i < m_layers.Length; i++)
        {
            m_layers[i].FeedForward(m_layers[i - 1].Outputs, m_neuronActivationMode);
        }

        return m_layers[^1].Outputs;
    }

    public void BackPropagation(float[] expected)
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
