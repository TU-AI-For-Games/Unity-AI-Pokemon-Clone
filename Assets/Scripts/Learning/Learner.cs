using UnityEngine;

namespace Learning
{
    public abstract class Learner
    {
        protected int m_epochs = 5000;
        protected float m_learningRate = 0.141f;
        protected int m_numTests;
        protected bool m_loadLearnedData;
        protected Layer.ActivationFunction m_activationFunction;

        protected abstract void LoadTrainingDataFile();
        public abstract void LoadSavedNeuralNetwork();
        public abstract void LearnData();
    }

    public class TrainingData
    {
        public float[] Targets;
        public float[] Values;

        public TrainingData(float[] targets, float[] values)
        {
            Targets = targets;
            Values = values;
        }
    }
}