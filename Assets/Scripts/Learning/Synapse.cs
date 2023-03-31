
namespace Learning
{
    public class Synapse
    {
        public Neuron Input { get; set; }
        public Neuron Output { get; set; }
        public double Weight { get; set; }
        public double DeltaWeight { get; set; }

        public Synapse(Neuron input, Neuron output)
        {
            Input = input;
            Output = output;
            Weight = NeuralNetwork.GetRandomInitialisationValue(InitialisationMode.Xavier);
        }
    }
}