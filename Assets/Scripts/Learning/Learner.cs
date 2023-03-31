using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Learning
{
    public class Learner : MonoBehaviour
    {
        private NeuralNetwork m_neuralNet;

        [SerializeField] private int m_numIterations;
        [SerializeField] private double m_learningRate;
        [SerializeField] private double m_l2RegressionRate;

        private List<InputData> m_inputData;

        // Start is called before the first frame update
        void Start()
        {
            m_inputData = GenerateTrainingData();

            m_neuralNet = new NeuralNetwork(
                m_numIterations,
                17,
                34,
                1,
                4,
                m_learningRate,
                m_l2RegressionRate
            );

            m_neuralNet.Train(m_inputData, 0.1);

            // Test the network
            Effectiveness a = new(m_neuralNet.Compute(GenerateTypeMatchup(PocketMonster.Element.Electric, PocketMonster.Element.Flying))); // Should be super-effective

            Effectiveness b = new(m_neuralNet.Compute(GenerateTypeMatchup(PocketMonster.Element.Electric, PocketMonster.Element.Grass))); // Should be neutral

            Effectiveness c = new(m_neuralNet.Compute(GenerateTypeMatchup(PocketMonster.Element.Electric, PocketMonster.Element.Rock))); // Should be not very effective

            Effectiveness d = new(m_neuralNet.Compute(GenerateTypeMatchup(PocketMonster.Element.Electric, PocketMonster.Element.Ground))); // Should be immune
        }

        // Update is called once per frame
        void Update()
        {

        }

        private List<InputData> GenerateTrainingData()
        {
            List<InputData> data = new List<InputData>();

            TextAsset movesFile = (TextAsset)Resources.Load("Data\\AI_Training\\typing");

            string[] linesFromFile = movesFile.text.Split('\n');

            for (int i = 1; i < linesFromFile.Length; ++i)
            {
                string[] lineContents = linesFromFile[i].Split(',');

                double[] inputValues = new double[(int)PocketMonster.Element.Water + 1];
                for (int j = 0; j < (int)PocketMonster.Element.Water + 1; j++)
                {
                    inputValues[j] = double.Parse(lineContents[j]);
                }

                double[] outputValues = new double[4];
                for (int j = 0; j < 4; j++)
                {
                    outputValues[j] = double.Parse(lineContents[(int)PocketMonster.Element.Water + 1 + j]);
                }

                data.Add(new InputData(inputValues, outputValues));
            }


            return data;
        }

        private double[] GenerateTypeMatchup(PocketMonster.Element typeA, PocketMonster.Element typeB)
        {
            double[] matchupTable = new double[17];

            for (int i = 0; i < 17; i++)
            {
                if (i == (int)typeA || i == (int)typeB)
                {
                    matchupTable[i] = 1;
                }
                else
                {
                    matchupTable[i] = 0;
                }
            }

            return matchupTable;
        }

        struct Effectiveness
        {
            public Effectiveness(double[] effectiveness)
            {
                m_immune = effectiveness[0];
                m_notVeryEffective = effectiveness[1];
                m_neutral = effectiveness[2];
                m_superEffective = effectiveness[3];
            }

            private readonly double m_immune;
            private readonly double m_notVeryEffective;
            private readonly double m_neutral;
            private readonly double m_superEffective;
        }
    }
}