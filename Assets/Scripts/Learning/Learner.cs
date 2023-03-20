using System.Collections;
using System.Collections.Generic;
using Learning;
using UnityEngine;

public class Learner : MonoBehaviour
{
    [SerializeField] private int m_iterations;
    [SerializeField][Range(0f, 1f)] private float m_learningRate;

    private readonly Dictionary<PocketMonster.Element, Dictionary<PocketMonster.Element, float>> m_typeMatchupTable = new();

    // Start is called before the first frame update
    void Start()
    {
        LoadTypingData();

        NeuralNetwork net = new NeuralNetwork(new[] { 2, 34, 34, 34, 1 }, m_learningRate, Layer.WeightInitialisationMode.Xavier, Layer.NeuronActivationMode.Sigmoid);

        for (int i = 0; i < m_iterations; i++)
        {
            foreach (PocketMonster.Element type in m_typeMatchupTable.Keys)
            {
                foreach (KeyValuePair<PocketMonster.Element, float> matchup in m_typeMatchupTable[type])
                {
                    float aTypeNormalised = normalize((float)type, 0f, 17f);
                    float normaliseTypeB = normalize((float)matchup.Key, 0f, 17f);

                    net.FeedForward(new[] { aTypeNormalised, normaliseTypeB });

                    // Dividing by 2 to normalise (0f-2f to range 0f-1f)...
                    net.BackPropagation(new[] { normalize(matchup.Value, 0f, 2f) }); // Supervised training with the known value
                }
            }
        }

        // Test the training data
        float resultA = net.FeedForward(new[] {
            normalize((float)PocketMonster.Element.Electric, 0f, 17f),
            normalize((float)PocketMonster.Element.Flying, 0f, 17f)
        })[0];

        Debug.Log(resultA);

        float resultB = net.FeedForward(new[] {
            normalize((float)PocketMonster.Element.Electric, 0f, 17f),
            normalize((float)PocketMonster.Element.Fire, 0f, 17f)
        })[0];

        Debug.Log(resultB);

        float resultC = net.FeedForward(new[] {
            normalize((float)PocketMonster.Element.Electric, 0f, 17f),
            normalize((float)PocketMonster.Element.Grass, 0f, 17f)
        })[0];

        Debug.Log(resultC);

        float resultD = net.FeedForward(new[] {
            normalize((float)PocketMonster.Element.Electric, 0f, 17f),
            normalize((float)PocketMonster.Element.Ground, 0f, 17f)
        })[0];

        Debug.Log(resultD);
    }

    private void LoadTypingData()
    {
        TextAsset movesFile = (TextAsset)Resources.Load("Data\\AI_Training\\typing");
        string[] linesFromFile = movesFile.text.Split('\n');

        foreach (string line in linesFromFile)
        {
            string[] lineContents = line.Split(',');

            PocketMonster.Element type = PocketMonster.StringToType(lineContents[0]);
            if (!m_typeMatchupTable.ContainsKey(type))
            {
                m_typeMatchupTable.Add(type, new Dictionary<PocketMonster.Element, float>());
            }

            PocketMonster.Element otherType = PocketMonster.StringToType(lineContents[1]);
            float typeMultiplier = float.Parse(lineContents[2]);

            m_typeMatchupTable[type].Add(otherType, typeMultiplier);
        }
    }

    private float normalize(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
