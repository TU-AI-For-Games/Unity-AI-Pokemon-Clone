
using System.Collections.Generic;

namespace Learning
{
    public class Neuron
    {
        private readonly List<Synapse> m_inputs;
        private readonly List<Synapse> m_outputs;

        public double Bias;
        public double DeltaBias;
        public double Gradient;
        public double Value;

        public Neuron()
        {
            m_inputs = new List<Synapse>();
            m_outputs = new List<Synapse>();
            Bias = NeuralNetwork.GetRandomNumber(-1, 1);
        }

        public Neuron(List<Neuron> inputs)
        {
            m_inputs = new List<Synapse>();
            m_outputs = new List<Synapse>();

            foreach (Neuron inputNeuron in inputs)
            {
                Synapse synapse = new Synapse(inputNeuron, this);

                inputNeuron.m_outputs.Add(synapse);
                m_inputs.Add(synapse);
            }
        }

        public double CalculateValue()
        {
            double sum = 0;
            foreach (Synapse synapse in m_inputs)
            {
                sum += synapse.Weight * synapse.Input.Value;
            }

            return new Sigmoid().Compute(sum + Bias);
        }

        public double CalculateGradient(double target)
        {
            Gradient = CalculateError(target) * new Sigmoid().Derive(Value);

            return Gradient;
        }

        public double CalculateGradient()
        {
            double sum = 0;
            foreach (Synapse synapse in m_outputs)
            {
                sum += synapse.Output.Gradient * synapse.Weight;
            }

            Gradient = sum * new Sigmoid().Derive(Value);

            return Gradient;
        }

        public void UpdateWeights(double learningRate, double regressionRate)
        {
            double previousDelta = DeltaBias;

            DeltaBias = learningRate * Gradient;
            Bias += DeltaBias + regressionRate * previousDelta;

            foreach (Synapse synapse in m_inputs)
            {
                previousDelta = synapse.DeltaWeight;

                synapse.DeltaWeight = learningRate * Gradient * synapse.Input.Value;

                synapse.Weight += synapse.DeltaWeight + regressionRate * previousDelta;
            }
        }

        public double CalculateError(double target)
        {
            return target - Value;
        }
    }
}