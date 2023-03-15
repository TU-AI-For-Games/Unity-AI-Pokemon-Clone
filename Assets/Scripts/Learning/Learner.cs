using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Learner : MonoBehaviour
{
    [SerializeField] private int m_iterations;

    // Start is called before the first frame update
    void Start()
    {
        NeuralNetwork net = new NeuralNetwork(new int[]
        {
            3, 25, 25, 1
        });

        // Train the neural network to perform an XOR operation
        // 0 0 0 -> 0
        // 0 0 1 -> 1
        // 0 1 1 -> 0
        // 0 1 0 -> 1
        // 1 1 0 -> 0
        // 1 1 1 -> 0
        // 1 0 1 -> 0
        // 1 0 0 -> 1

        for (int i = 0; i < m_iterations; i++)
        {
            net.FeedForward(new float[] { 0, 0, 0 });
            net.BackPropagation(new float[] { 0 });

            net.FeedForward(new float[] { 0, 0, 1 });
            net.BackPropagation(new float[] { 1 });

            net.FeedForward(new float[] { 0, 1, 1 });
            net.BackPropagation(new float[] { 0 });

            net.FeedForward(new float[] { 0, 1, 0 });
            net.BackPropagation(new float[] { 1 });

            net.FeedForward(new float[] { 1, 1, 0 });
            net.BackPropagation(new float[] { 0 });

            net.FeedForward(new float[] { 1, 1, 1 });
            net.BackPropagation(new float[] { 0 });

            net.FeedForward(new float[] { 1, 0, 1 });
            net.BackPropagation(new float[] { 0 });

            net.FeedForward(new float[] { 1, 0, 0 });
            net.BackPropagation(new float[] { 1 });
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
