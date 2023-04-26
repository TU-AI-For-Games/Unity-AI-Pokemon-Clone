using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Learning;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoveDecisionLearner : Learner
{
    private NeuralNetwork m_neuralNetwork;
    private List<TrainingData> m_trainingData;

    public enum Action
    {
        Attack, Switch, Heal
    }

    public MoveDecisionLearner(int epochs, float learningRate, int numTests, bool loadLearnedData, Layer.ActivationFunction activationFunction)
    {
        m_epochs = epochs;
        m_learningRate = learningRate;
        m_numTests = numTests;
        m_loadLearnedData = loadLearnedData;
        m_activationFunction = activationFunction;

        if (m_loadLearnedData)
        {
            LoadTrainingDataFile();
            LoadSavedNeuralNetwork();
        }
        else
        {
            LearnData();
        }

        // TestData();
    }

    public Action GetLearnedMoveOutcome(float trainerHp, PocketMonster.Element trainerType, float targetHp, PocketMonster.Element targetType)
    {
        return new MoveOutcome(m_neuralNetwork.Compute(GenerateInputData(trainerHp, trainerType, targetHp, targetType))).GetAction();
    }

    private float[] GenerateInputData(float trainerHp, PocketMonster.Element trainerType, float targetHp, PocketMonster.Element targetType)
    {
        float[] inputData = new float[4];
        inputData[0] = trainerHp;
        inputData[1] = (float)trainerType / 17f;
        inputData[2] = targetHp;
        inputData[3] = (float)targetType / 17f;
        return inputData;
    }

    private void TestData()
    {
        for (int i = 0; i < m_numTests; ++i)
        {
            // Choose a random sample from the training data
            TrainingData data = m_trainingData[Random.Range(0, m_trainingData.Count)];

            MoveOutcome outcome = new MoveOutcome(m_neuralNetwork.Compute(data.Targets));


            Debug.Log("PREDICTED...");
            PocketMonster.Element playerElement = (PocketMonster.Element)(data.Targets[2] * 17);
            PocketMonster.Element targetElement = (PocketMonster.Element)(data.Targets[3] * 17);

            outcome.DebugPrint(playerElement, data.Targets[0], targetElement, data.Targets[1]);

            Debug.Log("ACTUAL");
            MoveOutcome actualOutcome = new MoveOutcome(data.Values);
            actualOutcome.DebugPrint(playerElement, data.Targets[0], targetElement, data.Targets[1]);
        }
    }

    public sealed override void LearnData()
    {
        LoadTrainingDataFile();

        m_neuralNetwork = new NeuralNetwork(
            new[] { 4, 40, 40, 40, 3 },
            m_learningRate,
            m_activationFunction
        );

        m_neuralNetwork.Train(m_trainingData, m_epochs);
        m_neuralNetwork.Save("MoveDecision");
    }

    protected sealed override void LoadTrainingDataFile()
    {
        m_trainingData = new List<TrainingData>();

        string fileName = "decisionMakingData";

        string[] linesFromFile = ((TextAsset)Resources.Load($"Data\\AI_Training\\{fileName}")).text.Split('\n');

        for (int i = 1; i < linesFromFile.Length; i++)
        {
            string[] lineContents = linesFromFile[i].Split(',');

            float[] inputs = new float[4];

            for (int j = 0; j < 4; ++j)
            {
                inputs[j] = float.Parse(lineContents[j + 2]);
            }

            float[] outputs = new float[3];
            for (int j = 0; j < 3; ++j)
            {
                outputs[j] = float.Parse(lineContents[j + 6]);
            }

            m_trainingData.Add(new TrainingData(inputs, outputs));
        }

    }

    public sealed override void LoadSavedNeuralNetwork()
    {
        m_neuralNetwork = new NeuralNetwork(
            new[] { 4, 40, 40, 40, 3 },
            m_learningRate,
            m_activationFunction
        );

        m_neuralNetwork.Load("MoveDecision");
    }

    public struct MoveOutcome
    {
        public MoveOutcome(float[] outcome)
        {
            m_attack = outcome[0];
            m_switch = outcome[1];
            m_heal = outcome[2];
        }

        public void DebugPrint(PocketMonster.Element playerType, float playerHP, PocketMonster.Element targetType, float targetHP)
        {
            float[] actions =
            {
                m_attack,
                m_switch,
                m_heal
            };

            int maxIndex = Array.IndexOf(actions, actions.Max());

            playerHP *= 100;
            targetHP *= 100;

            Dictionary<int, string> actionStrings = new Dictionary<int, string>
            {
                { 0, "Attack"},
                { 1, "Switch"},
                { 2, "Heal"},
            };

            Debug.Log($"Player Pokemon {playerType} with {playerHP}% health against {targetType} with {targetHP}% health should {actionStrings[maxIndex]} ");

        }

        public Action GetAction()
        {
            float[] actions =
            {
                m_attack,
                m_switch,
                m_heal
            };

            return (Action)Array.IndexOf(actions, actions.Max());
        }

        private readonly float m_attack;
        private readonly float m_switch;
        private readonly float m_heal;
    }

}
