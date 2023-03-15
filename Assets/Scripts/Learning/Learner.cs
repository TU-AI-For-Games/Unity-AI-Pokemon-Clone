using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Learner : MonoBehaviour
{
    [SerializeField] private int m_iterations;
    [SerializeField][Range(0f, 1f)] private float m_learningRate;

    // Start is called before the first frame update
    void Start()
    {
        NeuralNetwork net = new NeuralNetwork(new[] { 3, 25, 25, 1 }, m_learningRate);

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

        // Test the training data
        float resultA = net.FeedForward(new float[] { 0, 0, 0 })[0];
        Debug.Log(resultA);

        float resultB = net.FeedForward(new float[] { 0, 0, 1 })[0];
        Debug.Log(resultB);

        float resultC = net.FeedForward(new float[] { 0, 1, 1 })[0];
        Debug.Log(resultC);

        float resultD = net.FeedForward(new float[] { 0, 1, 0 })[0];
        Debug.Log(resultD);

        float resultE = net.FeedForward(new float[] { 1, 1, 0 })[0];
        Debug.Log(resultE);

        float resultF = net.FeedForward(new float[] { 1, 1, 1 })[0];
        Debug.Log(resultF);

        float resultG = net.FeedForward(new float[] { 1, 0, 1 })[0];
        Debug.Log(resultG);

        float resultH = net.FeedForward(new float[] { 1, 0, 0 })[0];
        Debug.Log(resultH);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
