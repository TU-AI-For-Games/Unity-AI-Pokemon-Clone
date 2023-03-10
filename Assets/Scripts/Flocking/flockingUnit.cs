using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flockingUnit : MonoBehaviour
{
    [SerializeField] private float angleFOV;
    [SerializeField] private float smoothDamp;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private Vector3[] directionsToCheck;

    private List<flockingUnit> cohesionNeighbours = new List<flockingUnit>();
    private List<flockingUnit> avoidNeighbours = new List<flockingUnit>();
    private List<flockingUnit> alignNeighbours = new List<flockingUnit>();
    private Flocking assignedFlock;
    private Vector3 currentVelocity;
    private Vector3 currentObstacleAvoidVec;
    
    [SerializeField] private float speed;

    private GameObject birb;
    private GameObject rat;
    bool isBird;

    public Transform birdTransform { get; set; }

    private void Awake()
    {
        birdTransform = transform;
    }

    public void assignFlock (Flocking flock)
    {
        assignedFlock = flock;
    }

    public void getSpeed(float speed)
    {
        this.speed = speed;
    }

    private void findPrefab()
    {
        birb = GameObject.Find("Assets/Prefabs/Pidgey");
        rat = GameObject.Find("Assets/Prefabs/Rat");
    }

    public void moveBirds()
    {
        findNeighbours();
        calcSpeed();

        var cohesionVec = calcCohesionVec() * assignedFlock.cohesionWeight;
        var avoidVec = calcAvoidVec() * assignedFlock.avoidWeight;
        var alignVec = calcAlignVec() * assignedFlock.alignWeight;
        var boundsVec = calcBoundsVec() * assignedFlock.boundsWeight;
        var obstacleVec = calcObstacleVec() * assignedFlock.obstacleWeight;

        var moveVec = cohesionVec + avoidVec + alignVec + boundsVec + obstacleVec;
        moveVec = Vector3.SmoothDamp(birdTransform.forward, moveVec, ref currentVelocity, smoothDamp);
        moveVec = moveVec.normalized * speed;
        birdTransform.forward = moveVec;
        birdTransform.position += moveVec * Time.deltaTime;
        //use an IF statement to check if it is birb or rat
        birdTransform.rotation = new Quaternion(0, birdTransform.rotation.y, 0, birdTransform.rotation.w);
    }

    private void findNeighbours()
    {
        cohesionNeighbours.Clear();
        avoidNeighbours.Clear();
        alignNeighbours.Clear();
        var allBirds = assignedFlock.allBirds;
        for (int i = 0; i < allBirds.Length; i++)
        {
            var currentBird = allBirds[i];
            if (currentBird != this)
            {
                float sqrNeighbourDist = Vector3.SqrMagnitude(currentBird.birdTransform.position - birdTransform.position);
                if (sqrNeighbourDist <= assignedFlock.cohesionDistance * assignedFlock.cohesionDistance)
                {
                    cohesionNeighbours.Add(currentBird);
                }
                if (sqrNeighbourDist <= assignedFlock.avoidDistance * assignedFlock.avoidDistance)
                {
                    avoidNeighbours.Add(currentBird);
                }
                if (sqrNeighbourDist <= assignedFlock.alignDistance * assignedFlock.alignDistance)
                {
                    alignNeighbours.Add(currentBird);
                }
            }
        }
    }

    private void calcSpeed()
    {
        if (cohesionNeighbours.Count == 0)
            return;
        speed = 0;
        for(int i = 0; i < cohesionNeighbours.Count; i++)
        {
            speed += cohesionNeighbours[i].speed;
        }
        speed /= cohesionNeighbours.Count;
        speed = Mathf.Clamp(speed, assignedFlock.minSpeed, assignedFlock.maxSpeed);
    }

    private Vector3 calcCohesionVec()
    {
        var cohesionVec = Vector3.zero;
        if (cohesionNeighbours.Count == 0)
            return cohesionVec;
        int neighboursSeen = 0;
        for (int i = 0; i < cohesionNeighbours.Count; i++)
        {
            if (isSeen(cohesionNeighbours[i].birdTransform.position))
            {
                neighboursSeen++;
                cohesionVec = cohesionNeighbours[i].birdTransform.position;
            }
        }
        
        cohesionVec /= neighboursSeen;
        cohesionVec -= birdTransform.position;
        cohesionVec = cohesionVec.normalized;
        return cohesionVec;
    }

    private Vector3 calcAvoidVec()
    {
        var avoidVec = Vector3.zero;
        if (avoidNeighbours.Count == 0)
            return Vector3.zero;
        int neighboursSeen = 0;
        for (int i = 0; i < avoidNeighbours.Count; i++)
        {
            if (isSeen(avoidNeighbours[i].birdTransform.position))
            {
                neighboursSeen++;
                avoidVec += (birdTransform.position - avoidNeighbours[i].birdTransform.position);
            }
        }
       
        avoidVec /= neighboursSeen;
        avoidVec = avoidVec.normalized;
        return avoidVec;
    }

    private Vector3 calcAlignVec()
    {
        var alignVec = birdTransform.forward;
        if (alignNeighbours.Count == 0)
            return birdTransform.forward;
        int neighboursSeen = 0;
        for (int i = 0; i < alignNeighbours.Count; i++)
        {
            if (isSeen(alignNeighbours[i].birdTransform.position))
            {
                neighboursSeen++;
                alignVec = alignNeighbours[i].birdTransform.forward;
            }
        }
      
        alignVec /= neighboursSeen;
        alignVec = alignVec.normalized;
        return alignVec;
    }

    private Vector3 calcBoundsVec()
    {
        var offsetToCenter = assignedFlock.transform.position - birdTransform.position;
        bool nearCenter = (offsetToCenter.magnitude >= assignedFlock.boundsDistance * 0.9f);
        return nearCenter ? offsetToCenter.normalized : Vector3.zero;
    }

    private Vector3 calcObstacleVec()
    {
        var obstacleVec = Vector3.zero;
        RaycastHit Hit;
        if (Physics.Raycast(birdTransform.position, birdTransform.forward, out Hit, assignedFlock.obstacleDistance, obstacleMask))
        {
            obstacleVec = bestDirectionToAvoid();
        }
        else
        {
            currentObstacleAvoidVec = Vector3.zero;
        }
        return obstacleVec;
    }

    private Vector3 bestDirectionToAvoid()
    {
        if (currentObstacleAvoidVec != Vector3.zero)
        {
            RaycastHit Hit;
            if (Physics.Raycast(birdTransform.position, birdTransform.forward, out Hit, assignedFlock.obstacleDistance, obstacleMask))
            {
                return currentObstacleAvoidVec;
            }

        }

        float maxDistance = int.MinValue;
        var chosenDirection = Vector3.zero;
        for (int i = 0; i < directionsToCheck.Length; i++)
        {
            RaycastHit Hit;
            var currentDirection = birdTransform.TransformDirection(directionsToCheck[i].normalized);
            if (Physics.Raycast(birdTransform.position, currentDirection, out Hit, assignedFlock.obstacleDistance, obstacleMask))
            {
                float currentDistance = (Hit.point - birdTransform.position).sqrMagnitude;
                if (currentDistance > maxDistance)
                {
                    maxDistance = currentDistance;
                    chosenDirection = currentDirection;
                }
            }
            else
            {
                chosenDirection = currentDirection;
                currentObstacleAvoidVec = currentDirection.normalized;
                return chosenDirection.normalized;
            }
        }
        return chosenDirection.normalized;
    }

    private bool isSeen(Vector3 position)
    {
        return Vector3.Angle(birdTransform.forward, position - birdTransform.position) <= angleFOV;
    }

    
    /*private bool isBird()
    {
        if (birb )
        return birdTransform.position.sqrMagnitude > angleFOV;
    }*/
}


