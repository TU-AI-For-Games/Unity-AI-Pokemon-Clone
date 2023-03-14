using System.Collections.Generic;
using Learning;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    [SerializeField][Range(1, 1000)] private int m_numHiddenLayers = 2;
    private List<Layer> m_hiddenLayers;
    private Layer m_outputLayer;

    void Awake()
    {
        m_hiddenLayers = new List<Layer>(m_numHiddenLayers)
        {
            new(2, 4)
        };

        for (int i = 0; i < m_numHiddenLayers - 1; ++i)
        {
            m_hiddenLayers.Add(new Layer(4, 4));
        }

        m_outputLayer = new Layer(4, 2);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
