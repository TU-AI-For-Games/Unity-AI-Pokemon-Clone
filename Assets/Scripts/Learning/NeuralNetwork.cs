using System.Collections.Generic;
using Learning;
using UnityEngine;

public class NeuralNetwork
{
    private int[] m_networkShape;
    private Layer[] m_layers;


    public NeuralNetwork(int[] networkShape)
    {
        m_networkShape = new int[networkShape.Length];

        for (int i = 0; i < networkShape.Length; i++)
        {
            m_networkShape[i] = networkShape[i];
        }

        m_layers = new Layer[networkShape.Length - 1];

        for (int i = 0; i < m_layers.Length; ++i)
        {
            m_layers[i] = new Layer(networkShape[i], networkShape[i + 1]);
        }
    }

    public float[] FeedForward(float[] input)
    {
        m_layers[0].FeedForward(input);

        for (int i = 1; i < m_layers.Length; i++)
        {
            m_layers[i].FeedForward(m_layers[i - 1].Outputs);
        }

        return m_layers[^1].Outputs;
    }
}
