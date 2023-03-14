using UnityEngine;

namespace Learning
{
    public class Layer
    {
        public float[,] Weights;
        public float[] Biases;
        public float[] Nodes;

        private int m_numNodes;
        private int m_numInputs;

        public Layer(int numInputs, int numNodes)
        {
            m_numInputs = numInputs;
            m_numNodes = numNodes;

            Weights = new float[numNodes, numInputs];
            Biases = new float[numNodes];
            Nodes = new float[numNodes];
        }
    }
}