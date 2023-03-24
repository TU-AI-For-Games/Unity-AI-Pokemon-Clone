using System.Collections;
using System.Collections.Generic;
using Learning;
using UnityEngine;

public class Learner : MonoBehaviour
{
    [SerializeField] private int m_iterations;
    [SerializeField][Range(0f, 0.2f)] private float m_learningRate;
    [SerializeField][Range(0f, 0.1f)] private float m_l2RegularisationRate;

    [SerializeField] private Layer.WeightInitialisationMode m_initialisationMode;
    [SerializeField] private Layer.NeuronActivationMode m_activationMode;

    private readonly Dictionary<int, Dictionary<int, float[]>> m_typeMatchupTable = new();

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

    // Start is called before the first frame update
    void Start()
    {
        LoadTypingData();

        NeuralNetwork net = new NeuralNetwork(new[] { 2, 6, 6, 6, 6, 4 }, m_learningRate, m_l2RegularisationRate, m_initialisationMode, m_activationMode);

        for (int i = 0; i < m_iterations; i++)
        {
            foreach (int type in m_typeMatchupTable.Keys)
            {
                foreach (KeyValuePair<int, float[]> matchup in m_typeMatchupTable[type])
                {
                    float aTypeNormalised = Normalize(type, 0f, 17f);
                    float normaliseTypeB = Normalize(matchup.Key, 0f, 17f);

                    net.FeedForward(new[]
                    {
                        aTypeNormalised, normaliseTypeB
                    });

                    net.BackPropagation(matchup.Value);
                }
            }
        }

        // Test the training data
        Effectiveness resultA = new(net.FeedForward(new[] {
            Normalize((float)PocketMonster.Element.Electric, 0f, 17f),
            Normalize((float)PocketMonster.Element.Flying, 0f, 17f)
        }));

        Debug.Log(resultA);

        Effectiveness resultB = new(net.FeedForward(new[] {
            Normalize((float)PocketMonster.Element.Electric, 0f, 17f),
            Normalize((float)PocketMonster.Element.Fire, 0f, 17f)
        }));

        Debug.Log(resultB);

        Effectiveness resultC = new(net.FeedForward(new[] {
            Normalize((float)PocketMonster.Element.Electric, 0f, 17f),
            Normalize((float)PocketMonster.Element.Grass, 0f, 17f)
        }));

        Debug.Log(resultC);

        Effectiveness resultD = new(net.FeedForward(new[] {
            Normalize((float)PocketMonster.Element.Electric, 0f, 17f),
            Normalize((float)PocketMonster.Element.Ground, 0f, 17f)
        }));

        Debug.Log(resultD);
    }

    private void LoadTypingData()
    {
        TextAsset movesFile = (TextAsset)Resources.Load("Data\\AI_Training\\typing");
        string[] linesFromFile = movesFile.text.Split('\n');

        for (int i = 1; i < linesFromFile.Length; ++i)
        {
            string[] lineContents = linesFromFile[i].Split(',');

            int type = (int)PocketMonster.StringToType(lineContents[0]);
            if (!m_typeMatchupTable.ContainsKey(type))
            {
                m_typeMatchupTable.Add(type, new Dictionary<int, float[]>());
            }

            int otherType = (int)PocketMonster.StringToType(lineContents[1]);

            float[] effectiveness = {
                float.Parse(lineContents[2]),
                float.Parse(lineContents[3]),
                float.Parse(lineContents[4]),
                float.Parse(lineContents[5])
            };

            m_typeMatchupTable[type].Add(otherType, effectiveness);
        }
    }

    private static float Normalize(float value, float min, float max) => (value - min) / (max - min);

    // Update is called once per frame
    void Update()
    {

    }
}
