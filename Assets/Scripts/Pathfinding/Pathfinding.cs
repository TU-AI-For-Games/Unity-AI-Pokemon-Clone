using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class Pathfinding : MonoBehaviour
{
    public int StraightCost = 10;
    public int DiagonalCost = 14;
    [Range(0.0f, 180.0f)]
    public float MaxWalkableAngle = 30;
    public bool ShowDebug = false;
    public bool ShowPath = false;

    public LayerMask UnWalkableMask;
    public LayerMask WalkableMask;
    public float NodeRadius = 1;
    public int GridSize = 10;

    private Node[,] m_nodes;

    private void Awake()
    {
        MakeGrid(transform.position);
    }


    void MakeGrid(Vector3 location)
    {
        m_nodes = new Node[GridSize, GridSize];

        // Make a 2D grid of nodes
        for (int x = 0; x < GridSize; x++)
        {
            for (int z = 0; z < GridSize; z++)
            {
                var offset = location + new Vector3(x * NodeRadius, 0, z * NodeRadius);


                // Trace to find the walkable ground
                var hit = Physics.Linecast(offset + new Vector3(0, 100, 0), offset + new Vector3(0, -100, 0), out var rayHit);
                

                m_nodes[x, z] = MakeNode(offset, new Vector2Int(x, z));

                if (hit)
                {
                    // If the angle of the terrain is more than the max walkable angle, it will be un-walkable
                    if (Vector3.Angle(Vector3.up, rayHit.normal) > MaxWalkableAngle)
                    {
                        m_nodes[x, z].SetWalkable(false);
                    }

                    // Set the Y position of the node to the terrain
                    var OldPos = m_nodes[x, z].GetNodeWorldPosition();
                    m_nodes[x, z].SetNodeWorldPosition(new Vector3(OldPos.x, rayHit.point.y, OldPos.z));
                }
                else
                {
                    // This means the ray did not hit anything, making an un-walkable node
                    m_nodes[x, z].SetWalkable(false);
                }

            }
        }
    }


    public List<Node> FindPath(Vector3 startPosition, Vector3 targetPosition)
    {


        // Get the nodes
        var startNode = GetNodeFromPosition(startPosition);
        var endNode = GetNodeFromPosition(targetPosition);

        // If one of them is not valid, cancel
        if (startNode == null || endNode == null) return null;

        // Select the nodes
        startNode.SetSelected(true);
        endNode.SetSelected(true);

        // Make the Open and Closed lists
        List<Node> openNodes = new List<Node>();
        List<Node> closedNodes = new List<Node>();

        openNodes.Add(startNode);


        // While we have not yet reached the target node

        while (openNodes.Count > 0)
        {
            var currentNode = openNodes[0];

            currentNode = GetLowestCost(openNodes, currentNode);

            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            // If the path is found, return
            if (currentNode == endNode)
            {
                return RetracePath(startNode, endNode);
            }

            var neighbors = GetNeighboringNodes(currentNode);

            foreach (var neighbor in neighbors)
            {
                // Check if the neighbor is valid, if not, continue to next
                if (!neighbor.IsWalkable() || closedNodes.Contains(neighbor)) continue;

                int newMovementCostToNeighbor = currentNode.gCost + GetDistanceBetweenNodes(currentNode, neighbor);

                // Check if path to this one is shorter, or not in Open list
                if (newMovementCostToNeighbor < neighbor.gCost || !openNodes.Contains(neighbor))
                {
                    // Calculate the costs for this new node
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistanceBetweenNodes(neighbor, endNode);
                    neighbor.SetParent(currentNode);

                    // If the openNodes doesn't already contain this node, add it
                    if (!openNodes.Contains(neighbor))
                    {
                        openNodes.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();

        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode.SetSelected(true);
            currentNode = currentNode.GetParent();
        }

        path.Reverse();

        return path;


    }

    private void Update()
    {
        MakeGrid(transform.position);
    }

    private Node MakeNode(Vector3 worldPosition, Vector2Int gridPosition)
    {
        return new Node(NodeRadius, worldPosition, gridPosition, UnWalkableMask);
    }

    private Node GetLowestCost(List<Node> openNodes, Node currentNode)
    {
        var returnNode = currentNode;

        foreach (var node in openNodes.Where(node => node.fCost < returnNode.fCost || node.fCost == returnNode.fCost && node.hCost < returnNode.hCost))
        {
            returnNode = node;
        }

        return returnNode;

    }



    private Node GetNodeFromPosition(Vector3 position)
    {
        // Subtract world position to get local node position
        position -= transform.position;

        // Round it to work with the scale of the grid
        Vector3Int gridPosition = Vector3Int.RoundToInt(position / NodeRadius);

        // Make Coordinates from the position
        Vector2Int nodeCoords = new Vector2Int(gridPosition.x, gridPosition.z);

        // Return the node if the coordinates is valid
        return NodeExists(nodeCoords) ? m_nodes[nodeCoords.x, nodeCoords.y] : null;
    }

    private bool NodeExists(Vector2Int nodeCoords)
    {
        // Node Coordinates are outside the grid
        if (nodeCoords.x > GridSize - 1 || nodeCoords.y > GridSize - 1) return false;

        // Node Coordinates are negative
        if (nodeCoords.x < 0 || nodeCoords.y < 0) return false;


        // Node Coords are valid
        return true;
    }

    int GetDistanceBetweenNodes(Node A, Node B)
    {
        int distanceX = Mathf.Abs(A.GetNodeGridPosition().x - B.GetNodeGridPosition().x);
        int distanceY = Mathf.Abs(A.GetNodeGridPosition().y - B.GetNodeGridPosition().y);

        if (distanceX > distanceY)
            return DiagonalCost * distanceY + StraightCost * (distanceX - distanceY);
        return DiagonalCost * distanceX + StraightCost * (distanceY - distanceX);

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

            var newNode = m_nodes[offset.x, offset.y];
            neighbors.Add(newNode);
        }

        return neighbors;
    }

    private void OnDrawGizmos()
    {

        if (m_nodes == null) return;


        // Draw debug points to show the nodes

        foreach (var node in m_nodes)
        {
            var radius = node.GetNodeRadius() * 0.9f;
            bool selected = node.IsSelected();
            Gizmos.color = selected ? Color.blue : node.IsWalkable() ? Color.green : Color.red;


            if (ShowDebug || (ShowPath && selected))
            {
                Gizmos.DrawSphere(node.GetNodeWorldPosition(), radius / 4);

            }
        }
    }
}
