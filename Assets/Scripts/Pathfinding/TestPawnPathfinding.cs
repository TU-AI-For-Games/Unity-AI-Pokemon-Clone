using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TestPawnPathfinding : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject m_target;
    [SerializeField] private Pathfinding m_pathfinding;
    [SerializeField] private float m_acceptanceRadius = 3;
    [SerializeField] private float m_speed = 5;


    private List<Node> path;
    
    private void MakePath()
    {
        path = m_pathfinding.FindPath(transform.position, m_target.transform.position);
    }

    private void Start()
    {
        MakePath();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        
        MakePath();
    }

    private void Update()
    {
        if (!Application.isPlaying) return;

        if (path == null) return;

        if (path.Count == 0) return;
 
        MoveTowards();
  
    }

    private void MoveTowards()
    {
        if (path == null || path.Count < 1) return;

        Vector3 nextPoint = path[0].GetNodeWorldPosition();

        // If distance is less than the acceptanceradius, remove it and go to next point
        if (Vector3.Distance(transform.position, nextPoint) < m_acceptanceRadius)
        {
            path.RemoveAt(0);
        }

        transform.position = Vector3.MoveTowards(transform.position, nextPoint, m_speed * Time.deltaTime);
    }


}
