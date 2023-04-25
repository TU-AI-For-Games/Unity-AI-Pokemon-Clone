using UnityEngine;

namespace Learning
{
    public abstract class Learner : Singleton<Learner>
    {
        [SerializeField] protected int m_epochs = 5000;
        [SerializeField] protected float m_learningRate = 0.141f;
        [SerializeField] protected int m_numTests;
        [SerializeField] protected bool m_loadLearnedData;

        protected abstract void LoadTrainingDataFile();
        public abstract void LoadSavedNeuralNetwork();
        public abstract void LearnData();
    }

    public class LearningData
    {
        public float[] Targets;
        public float[] Values;

        public LearningData(float[] targets, float[] values)
        {
            Targets = targets;
            Values = values;
        }
    }
}