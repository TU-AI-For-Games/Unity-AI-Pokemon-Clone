using UnityEngine;

namespace Learning
{
    public class Layer
    {
        private int m_numInputs;
        private int m_numOutputs;

        private float[] m_inputs;
        private float[] m_outputs;

        private float[,] m_weights;
        private float[,] m_weightsDelta;

        private float[] m_gamma;
        private float[] m_error;

        public Layer(int numInputs, int numOutputs)
        {
            m_numInputs = numInputs;
            m_numOutputs = numOutputs;

            m_inputs = new float[numInputs];
            m_outputs = new float[m_numOutputs];

            m_weights = new float[numOutputs, numInputs];
            m_weightsDelta = new float[numOutputs, numInputs];

            m_gamma = new float[m_numOutputs];
            m_error = new float[m_numOutputs];
        }
    }
}