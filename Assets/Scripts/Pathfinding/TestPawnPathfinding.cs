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
    
    private void OnDrawGizmos()
    {
        List<Node> path = m_pathfinding.FindPath(transform.position, m_target.transform.position);

    }
}
