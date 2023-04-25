using System;
using System.Collections.Generic;
using System.Linq;
using Learning;
using UnityEngine;

public class TypeLearner : Learner
{
    private Dictionary<PocketMonster.Element, NeuralNetwork> m_typeNeuralNetworks = new();

    private Dictionary<PocketMonster.Element, List<TrainingData>> m_trainingData;

    protected override void InternalInit()
    {
        if (m_loadLearnedData)
        {
            LoadSavedNeuralNetwork();
        }
        else
        {
            LearnData();
        }

        PrintTypePairings();
    }

    public override void LearnData()
    {
        m_typeNeuralNetworks = new Dictionary<PocketMonster.Element, NeuralNetwork>();

        LoadTrainingDataFile();

        for (int i = 0; i < (int)PocketMonster.Element.Water + 1; ++i)
        {
            NeuralNetwork neuralNetwork = new NeuralNetwork(
                new[] { 17, 34, 34, 4 },
                m_learningRate,
                Layer.ActivationFunction.TanH
            );


            neuralNetwork.Train(m_trainingData[(PocketMonster.Element)i], m_epochs);

            neuralNetwork.Save(PocketMonster.TypeToString((PocketMonster.Element)i) + ".NEURALNET");

            m_typeNeuralNetworks.Add((PocketMonster.Element)i, neuralNetwork);
        }
    }

    public override void LoadSavedNeuralNetwork()
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

    private void PrintTypePairings()
    {
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

    private float[] GenerateInputType(PocketMonster.Element inType)
    {
        float[] types = new float[17];
        for (int i = 0; i < (int)PocketMonster.Element.Water + 1; ++i)
        {
            types[i] = (PocketMonster.Element)i == inType ? 1f : 0f;
        }

        return types;
    }

    protected override void LoadTrainingDataFile()
    {
        m_trainingData = new Dictionary<PocketMonster.Element, List<TrainingData>>();

        for (int i = 0; i < (int)PocketMonster.Element.Water + 1; ++i)
        {
            // Open the file
            string fileName = $"{PocketMonster.TypeToString((PocketMonster.Element)i).ToUpper()}_typeMatchUp";

            string[] linesFromFile = ((TextAsset)Resources.Load($"Data\\AI_Training\\{fileName}")).text.Split('\n');

            // Build the TrainingData list
            List<TrainingData> data = new List<TrainingData>((int)PocketMonster.Element.Water + 1);

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

                data.Add(new TrainingData(inputValues, outputValues));
            }

            // Add to the dictionary for the type
            m_trainingData[(PocketMonster.Element)i] = data;
        }
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

        private readonly float Immune;
        private readonly float NotVeryEffective;
        private readonly float Neutral;
        private readonly float SuperEffective;
    }
}
