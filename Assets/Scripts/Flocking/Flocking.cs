using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private flockingUnit flockBirdsPrefab;
    [SerializeField] private int flockSize;
    [SerializeField] private Vector3 flockBoundary;

    [Header("Set Speed")]
    [Range(0, 10)]
    [SerializeField] private float minimumSpeed;
    public float minSpeed { get { return minimumSpeed; } }
    [Range(0, 10)]
    [SerializeField] private float maximumSpeed;
    public float maxSpeed { get { return maximumSpeed; } }

    [Header("Detection Distances")]
    [Range(0, 10)]
    [SerializeField] private float cohesionDist;
    public float cohesionDistance { get { return cohesionDist; } }

    [Range(0, 10)]
    [SerializeField] private float avoidDist;
    public float avoidDistance { get { return avoidDist; } }

    [Range(0, 10)]
    [SerializeField] private float alignDist;
    public float alignDistance { get { return alignDist; } }

    [Range(0, 10)]
    [SerializeField] private float boundsDist;
    public float boundsDistance { get { return boundsDist; } }

    [Range(0, 10)]
    [SerializeField] private float obstacleDist;
    public float obstacleDistance { get { return obstacleDist; } }

    [Header("Behavoiur Weights")]
    [Range(0, 10)]
    [SerializeField] private float _cohesionWeight;
    public float cohesionWeight { get { return _cohesionWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _avoidWeight;
    public float avoidWeight { get { return _avoidWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _alignWeight;
    public float alignWeight { get { return _alignWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _boundsWeight;
    public float boundsWeight { get { return _boundsWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _obstacleWeight;
    public float obstacleWeight { get { return _obstacleWeight; } }

    [SerializeField] private Vector2 limit;

    public flockingUnit[] allBirds { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        spawnBirds();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < allBirds.Length; i++)
        {
            allBirds[i].moveBirds();

            allBirds[i].transform.position = new Vector3(allBirds[i].transform.position.x, Mathf.Clamp(allBirds[i].transform.position.y, limit.x, limit.y), allBirds[i].transform.position.z);

        }
    }

    void spawnBirds()
    {
        allBirds = new flockingUnit[flockSize];
        
        for (int i = 0; i < flockSize; i++)
        {
            var vector = UnityEngine.Random.insideUnitSphere;
            vector = new Vector3(vector.x * flockBoundary.x, vector.y * flockBoundary.y, vector.z * flockBoundary.z);
            var spawnPos = transform.position + vector;
            var Rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            allBirds[i] = Instantiate(flockBirdsPrefab, spawnPos, Rotation);
            allBirds[i].assignFlock(this);
            allBirds[i].getSpeed(UnityEngine.Random.Range(minimumSpeed, maximumSpeed));

        }
    }
}
