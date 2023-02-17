using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

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
    public int iterations = 10;

    [SerializeField] public GameObject startingObject;
    [SerializeField] public GameObject targetObject;

    [SerializeField] 
    public LayerMask unWalkableMask;
    [SerializeField] 
    public LayerMask walkableMask;
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
                var offset = location + new Vector3(x * nodeRadius, 0, z * nodeRadius);


                // Trace to find the walkable ground
                var hit = Physics.Linecast(offset + new Vector3(0, 100, 0), offset + new Vector3(0, -100, 0), out var rayHit, walkableMask);
                
                
                nodes[x, z] = MakeNode(offset, new Vector2Int(x,z));

                if (hit) 
                {

                    // If the angle of the terrain is more than the max walkable angle, it will be un-walkable
                    if(Vector3.Angle(Vector3.up, rayHit.normal) > maxWalkableAngle)
                    {
                        nodes[x, z].SetWalkable(false);
                    }

                    // Set the Y position of the node to the terrain
                    var OldPos = nodes[x, z].GetNodeWorldPosition();
                    nodes[x, z].SetNodeWorldPosition(new Vector3(OldPos.x, rayHit.point.y, OldPos.z));
                }
                else
                {
                    // This means the ray did not hit anything, making an un-walkable node
                    nodes[x, z].SetWalkable(false);
                }

            }
        }
    }


    bool FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        
        // Get the nodes
        var startNode = GetNodeFromPosition(startPosition);
        var endNode = GetNodeFromPosition(targetPosition);

        // If one of them is not valid, cancel
        if (startNode == null || endNode == null) return false;
        
        // Select the nodes
        startNode.SetSelected(true);
        endNode.SetSelected(true);
        
        // Make the Open and Closed lists
        List<Node> OpenNodes = new List<Node>();
        List<Node> ClosedNodes = new List<Node>();
        
        OpenNodes.Add(startNode);
        
        
        var currentNode = startNode;
    
        // While we have not yet reached the target node

        for (int i = 0; i < iterations; i++)
        {
            currentNode = GetLowestCost(OpenNodes, endNode);
            OpenNodes.Remove(currentNode);
            ClosedNodes.Add(currentNode);

            // If the path is found, return
            if (currentNode == endNode)
            {
              print("Path Found!");
              return true;
            }

            var neighbors = GetNeighboringNodes(currentNode);

            foreach (var neighbor in neighbors)
            {
                // Check if the neighbor is valid, if not, continue to next
                if (!neighbor.IsWalkable() || ClosedNodes.Contains(neighbor)) continue;
                
                
                
            }

        }
        
       

        return false;

    }


    private void Update()
    {
        MakeGrid(transform.position);
        FindPath(startingObject.transform.position, targetObject.transform.position);

    }

    private Node MakeNode(Vector3 worldPosition, Vector2Int gridPosition)
    {
        return new Node(nodeRadius, worldPosition, gridPosition, unWalkableMask);
    }

    private Node GetLowestCost(List<Node> openNodes, Node endNode)
    {
        Node currentNext = null;

        int index = 1;
        foreach (var node in openNodes)
        {
            if (currentNext == null)
            {
                currentNext = node;
                continue;
            }
            
            var newGCost = index % 2 == 0 ? straightCost : diagonalCost;
            var newHCost = Vector2Int.Distance(node.GetNodeGridPosition(), endNode.GetNodeGridPosition());

            node.gCost = newGCost + node.GetParent().gCost;
            node.hCost = Mathf.RoundToInt(newHCost);

            if (node.fCost < currentNext.fCost)
            {
                currentNext = node;
            }

            index++;

        }
        return currentNext;
        
    }
    
    

    private Node GetNodeFromPosition(Vector3 position)
    {
        // Subtract world position to get local node position
        position -= transform.position;
        
        // Round it to work with the scale of the grid
        Vector3Int gridPosition = Vector3Int.RoundToInt(position / nodeRadius);
        
        // Make Coordinates from the position
        Vector2Int nodeCoords = new Vector2Int(gridPosition.x, gridPosition.z);

        // Return the node if the coordinates is valid
        return NodeExists(nodeCoords) ? nodes[nodeCoords.x, nodeCoords.y] : null;
    }

    private bool NodeExists(Vector2Int nodeCoords)
    {
        // Node Coordinates are outside the grid
        if (nodeCoords.x > nodes.GetLength(0) || nodeCoords.y > nodes.GetLength(1)) return false;

        // Node Coordinates are negative
        if (nodeCoords.x < 0 || nodeCoords.y < 0) return false;

        
        // Node Coords are valid
        return true;
    }


    // Lookup table for neighbor Nodes
    private readonly Vector2Int[] _nodeNeighbors =
    {

        new (-1, -1),
        new (0, -1),
        new (1, -1),
        new (1, 0),
        new (1, 1),
        new (0, 1),
        new (-1, 1),
        new (-1, 0)

    };

    private List<Node> GetNeighboringNodes(Node parentNode)
    {

        var neighbors = new List<Node>();

        
        // Loop through all the eight neighbors and add them to the neighbors list
        
        for (var i = 0; i < 8; i++)
        {
            var offset = parentNode.GetNodeGridPosition() + _nodeNeighbors[i];
            if (!NodeExists(offset)) continue;
            
            var newNode = nodes[offset.x, offset.y];
            neighbors.Add(newNode);
        }

        return neighbors;
    }
    
    private void OnDrawGizmos()
    {

        if (!showDebug) return;
        if (nodes == null) return;

        // Draw debug points to show the nodes

        foreach (var node in nodes)
        {
        
            var radius = node.GetNodeRadius() * 0.9f;
            Gizmos.color = node.IsSelected() ? Color.blue : node.IsWalkable() ? Color.green : Color.red;
            Gizmos.DrawSphere(node.GetNodeWorldPosition(), radius/4);
            Handles.Label(node.GetNodeWorldPosition() + new Vector3(0,1,0), node.fCost.ToString());

        }
    }
}
