using System.Collections.Generic;
using Learning;
using UnityEngine;
using UnityEngine.Experimental.AI;

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
}
