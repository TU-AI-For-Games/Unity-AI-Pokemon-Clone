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
    [Range(0.0f, 180.0f)]
    public float maxWalkableAngle = 30;
    [SerializeField]
    public bool showDebug = false;

    [SerializeField] 
    public LayerMask unwalkableMask;
    [SerializeField]
    public float nodeRadius = 1;
    [SerializeField]
    private int gridSize = 10;

    private Node[,] nodes;

    

    void MakeGrid(Vector3 location)
    {

        nodes = new Node[gridSize, gridSize];

        // Make a 2D grid of nodes
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {

                RaycastHit rayHit;
                Vector3 offset = location + new Vector3(x * nodeRadius, 0, z * nodeRadius);


                // Trace to find the ground
                bool hit = Physics.Linecast(offset + new Vector3(0, 100, 0), offset + new Vector3(0, -100, 0), out rayHit);
                
                
                nodes[x, z] = MakeNode(offset);

                if (hit) 
                {

                    // If the angle of the terrain is more than the max walkable angle, it will be unwalkable
                    if(Vector3.Angle(Vector3.up, rayHit.normal) > maxWalkableAngle)
                    {
                        nodes[x, z].walkable = false;
                    }

                    // Set the Y position of the node to the terrain
                    nodes[x, z].position.y = rayHit.point.y;
                }
                else
                {
                    // This means the ray did not hit anything, making an unwalkable node
                    nodes[x, z].walkable = false;
                }

            }
        }
    }


    bool FindPath(Vector3 startPosition, Vector3 targetPosition)
    {





        return false;
    }


    private void Update()
    {
        MakeGrid(transform.position);
    }

    Node MakeNode(Vector3 position)
    {
        return new Node(nodeRadius, position, unwalkableMask);
    }


    Node GetNodeFromPosition(Vector3 position)
    {

        float gridSize = 1f;
        Vector3 gridPosition = Vector3Int.FloorToInt(position / gridSize);
        var coords = gridPosition * gridSize;





        return null;


    }

    private void OnDrawGizmos()
    {

        if (!showDebug) return;
        if (nodes == null) return;

        // Draw debug points to show the nodes

        foreach (var node in nodes)
        {

            var radius = node.radius * 0.9f;
            Gizmos.color = node.walkable ? Color.green : Color.red;
            Gizmos.DrawSphere(node.position, radius/4);
            
        }
    }
}
