using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Learner : MonoBehaviour
{
    // TODO: Neural network for every type, trained against the type matchups for it
    private NeuralNetwork m_neuralNetwork;

    [SerializeField] private int m_epochs = 5000;
    [SerializeField] private float m_learningRate = 0.141f;

    private Dictionary<PocketMonster.Element, List<LearningData>> m_data;

    // Start is called before the first frame update
    void Start()
    {
        ReadDataFromFiles();

        m_neuralNetwork = new NeuralNetwork(
            new[] { 17, 34, 34, 34, 4 },
            m_learningRate
        );

        for (int i = 0; i < m_epochs; i++)
        {
            foreach (LearningData data in m_data[PocketMonster.Element.Fire])
            {
                m_neuralNetwork.FeedForward(data.Targets);
                m_neuralNetwork.BackPropagation(data.Values);
            }
        }

        Effectiveness a = new(m_neuralNetwork.FeedForward(GenerateInputType(PocketMonster.Element.Grass))); // Should be super-effective

        Effectiveness b = new(m_neuralNetwork.FeedForward(GenerateInputType(PocketMonster.Element.Dark))); // Should be neutral

        Effectiveness c = new(m_neuralNetwork.FeedForward(GenerateInputType(PocketMonster.Element.Rock))); // Should be not very effective

        // Effectiveness d = new(m_neuralNetwork.Compute(GenerateInputType(PocketMonster.Element.Ground))); // Should be immune
    }

    // Update is called once per frame
    void Update()
    {

    }

    private float[] GenerateInputType(PocketMonster.Element inType)
    {
        float[] types = new float[17];
        for (int i = 0; i < (int)PocketMonster.Element.Water; ++i)
        {
            types[i] = (PocketMonster.Element)i == inType ? 1f : 0f;
        }

        return types;
    }

    private void ReadDataFromFiles()
    {
        m_data = new Dictionary<PocketMonster.Element, List<LearningData>>();

        for (int i = 0; i < (int)PocketMonster.Element.Water; ++i)
        {
            // Open the file
            string fileName = $"{PocketMonster.TypeToString((PocketMonster.Element)i).ToUpper()}_typeMatchUp";

            string[] linesFromFile = ((TextAsset)Resources.Load($"Data\\AI_Training\\{fileName}")).text.Split('\n');

            // Build the LearningData list
            List<LearningData> data = new List<LearningData>((int)PocketMonster.Element.Water);

            for (int j = 1; j < linesFromFile.Length; ++j)
            {
                string[] lineContents = linesFromFile[j].Split(',');

                float[] inputValues = new float[(int)PocketMonster.Element.Water + 1];

                for (int k = 0; k < (int)PocketMonster.Element.Water; k++)
                {
                    inputValues[k] = float.Parse(lineContents[k]);
                }

                float[] outputValues = new float[4];
                for (int k = 0; k < 4; k++)
                {
                    outputValues[k] = float.Parse(lineContents[(int)PocketMonster.Element.Water + 1 + k]);
                }

                data.Add(new LearningData(inputValues, outputValues));
            }

            // Add to the dictionary for the type
            m_data[(PocketMonster.Element)i] = data;
        }
    }

    struct Effectiveness
    {
        public Effectiveness(float[] effectiveness)
        {
            m_immune = effectiveness[0];
            m_notVeryEffective = effectiveness[1];
            m_neutral = effectiveness[2];
            m_superEffective = effectiveness[3];
        }

        private readonly float m_immune;
        private readonly float m_notVeryEffective;
        private readonly float m_neutral;
        private readonly float m_superEffective;
    }
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
