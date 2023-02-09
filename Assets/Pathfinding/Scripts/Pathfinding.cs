using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Pathfinding : MonoBehaviour
{
    
    [SerializeField]
    public float straightCost = 10;
    [SerializeField]
    public float diagonalCost = 14;

    [SerializeField] public float maxSlope = 30;

    [SerializeField] public LayerMask unwalkableMask;
    [SerializeField]
    public float nodeRadius = 1;
    [SerializeField]
    private int gridSize = 10;

    private Node[,] nodes;

    

    void MakeGrid(Vector3 location)
    {

        nodes = new Node[gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {

                RaycastHit rayHit;
                Vector3 offset = location + new Vector3(x * nodeRadius, 0, z * nodeRadius);
                bool hit = Physics.Linecast(offset + new Vector3(0, 0, 90), offset + new Vector3(0, 0, -10), out rayHit);

                if (hit)
                {
                    offset.y = rayHit.transform.position.y;
                }
                nodes[x, z] = MakeNode(offset);
            }
        }
    }

    private void Update()
    {
        MakeGrid(transform.position);
    }

    Node MakeNode(Vector3 position)
    {
        return new Node(nodeRadius, position, unwalkableMask);
    }

    private void OnDrawGizmos()
    {
        if (nodes == null) return;

        
        foreach (var node in nodes)
        {

            var radius = node.radius * 0.9f;
            Gizmos.color = node.walkable ? Color.green : Color.red;
            Gizmos.DrawCube(node.position, new Vector3(radius,radius,radius));
            
        }
    }
}
