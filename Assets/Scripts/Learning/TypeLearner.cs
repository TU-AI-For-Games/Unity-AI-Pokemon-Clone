using System;
using System.Collections.Generic;
using System.Linq;
using Learning;
using UnityEngine;

public class TypeLearner : Singleton<TypeLearner>
{
    private Dictionary<PocketMonster.Element, NeuralNetwork> m_typeNeuralNetworks = new();

    [SerializeField] private int m_epochs = 5000;
    [SerializeField] private float m_learningRate = 0.141f;
    [SerializeField] private int m_numTests;
    [SerializeField] private bool m_loadLearnedData;

    protected override void InternalInit()
    {
        if (m_loadLearnedData)
        {
            LoadLearnedData();
        }
        else
        {
            LearnData();
        }
    }

    public void LearnData()
    {
        m_typeNeuralNetworks = new Dictionary<PocketMonster.Element, NeuralNetwork>();

        Dictionary<PocketMonster.Element, List<LearningData>> data = ReadTypeTrainingDataFromFiles();

        for (int i = 0; i < (int)PocketMonster.Element.Water + 1; ++i)
        {
            NeuralNetwork neuralNetwork = new NeuralNetwork(
                new[] { 17, 34, 34, 4 },
                m_learningRate,
                Layer.ActivationFunction.TanH
            );


            neuralNetwork.Train(data[(PocketMonster.Element)i], m_epochs);

            neuralNetwork.Save(PocketMonster.TypeToString((PocketMonster.Element)i) + ".NEURALNET");

            m_typeNeuralNetworks.Add((PocketMonster.Element)i, neuralNetwork);
        }

        // Pick random pairings to test the ANN
        for (int i = 0; i < 17; i++)
        {
            for (int j = 0; j < 17; j++)
            {
                PocketMonster.Element typeA = (PocketMonster.Element)i;
                PocketMonster.Element typeB = (PocketMonster.Element)j;

                Effectiveness effectiveness = new(m_typeNeuralNetworks[typeA].Compute(GenerateInputType(typeB)));

                effectiveness.DebugPrint(typeA, typeB);
            }
        }
    }

    public void LoadLearnedData()
    {
        m_typeNeuralNetworks = new Dictionary<PocketMonster.Element, NeuralNetwork>();

        for (int i = 0; i < (int)PocketMonster.Element.Water + 1; ++i)
        {
            NeuralNetwork neuralNetwork = new NeuralNetwork(
                new[] { 17, 34, 34, 4 },
                m_learningRate,
                Layer.ActivationFunction.TanH
            );

            neuralNetwork.Load(PocketMonster.TypeToString((PocketMonster.Element)i) + ".NEURALNET");

            m_typeNeuralNetworks.Add((PocketMonster.Element)i, neuralNetwork);
        }
    }


    private float[] GenerateInputType(PocketMonster.Element inType)
    {
        float[] types = new float[17];
        for (int i = 0; i < (int)PocketMonster.Element.Water + 1; ++i)
        {
            types[i] = (PocketMonster.Element)i == inType ? 1f : 0f;
        }

        return types;
    }

    private Dictionary<PocketMonster.Element, List<LearningData>> ReadTypeTrainingDataFromFiles()
    {
        Dictionary<PocketMonster.Element, List<LearningData>> typeTrainingData = new Dictionary<PocketMonster.Element, List<LearningData>>();

        for (int i = 0; i < (int)PocketMonster.Element.Water + 1; ++i)
        {
            // Open the file
            string fileName = $"{PocketMonster.TypeToString((PocketMonster.Element)i).ToUpper()}_typeMatchUp";

            string[] linesFromFile = ((TextAsset)Resources.Load($"Data\\AI_Training\\{fileName}")).text.Split('\n');

            // Build the LearningData list
            List<LearningData> data = new List<LearningData>((int)PocketMonster.Element.Water + 1);

            for (int j = 1; j < linesFromFile.Length; ++j)
            {
                string[] lineContents = linesFromFile[j].Split(',');

                float[] inputValues = new float[(int)PocketMonster.Element.Water + 1];

                for (int k = 0; k < (int)PocketMonster.Element.Water + 1; k++)
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
            typeTrainingData[(PocketMonster.Element)i] = data;
        }

        return typeTrainingData;
    }

    struct Effectiveness
    {
        public Effectiveness(float[] effectiveness)
        {
            Immune = effectiveness[0];
            NotVeryEffective = effectiveness[1];
            Neutral = effectiveness[2];
            SuperEffective = effectiveness[3];
        }

        public void DebugPrint(PocketMonster.Element aType, PocketMonster.Element bType)
        {
            float[] effectivenessArray =
            {
                Immune,
                NotVeryEffective,
                Neutral,
                SuperEffective
            };

            int maxIndex = Array.IndexOf(effectivenessArray, effectivenessArray.Max());

            Dictionary<int, string> effectiveString = new Dictionary<int, string>
            {
                { 0, "Immune"},
                { 1, "Not Very Effective"},
                { 2, "Neutral"},
                { 3, "Super Effective"}
            };

            Debug.Log($"{aType} is {effectiveString[maxIndex]} against {bType}");

        }

        public readonly float Immune;
        public readonly float NotVeryEffective;
        public readonly float Neutral;
        public readonly float SuperEffective;
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
