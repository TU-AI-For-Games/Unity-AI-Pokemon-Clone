using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ratHerdTest : MonoBehaviour
{
    /*[Header("Spawning")]
    [SerializeField] private flockingUnit herdRatPrefab;
    [SerializeField] private int herdSize;
    [SerializeField] private Vector3 herdBoundary;*/

    [Header("Set Speed")]
    [Range(0, 10)]
    [SerializeField] private float minSpeed;
    public float _minSpeed { get { return minSpeed; } }
    [Range(0, 10)]
    [SerializeField] private float maxSpeed;
    public float _maxSpeed { get { return maxSpeed; } }

    [Range(0, 20)]
    [SerializeField] private float rotationSpeed;
    public float rotSpeed { get { return rotationSpeed; } }

    private bool wandering = false;
    private bool counterClockRot = false;
    private bool clockwiseRot = false;
    private bool moving = false;

   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*IEnumerator Herding()
    {

    }*/
  
}

/*[Header("Detection Distances")]
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
    public float obstacleWeight { get { return _obstacleWeight; } }*/

/* public flockingUnit[] allRats { get; set; }*/