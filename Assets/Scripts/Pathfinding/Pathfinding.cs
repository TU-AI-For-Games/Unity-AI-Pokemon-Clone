using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;


    internal enum PathfindingMethod
    {
        BreadthFirstSearch,
        BestFirstSearch,
        DepthFirstSearch,
        Dijkstra,
        AStar
    }


    [ExecuteInEditMode]
    public class Pathfinding : MonoBehaviour
    {
        [SerializeField] private bool liveDebug = false;
        [SerializeField] public int m_straightCost = 10;
        [SerializeField] public int m_diagonalCost = 14;
        [SerializeField] [Range(0.0f, 180.0f)] public float m_maxWalkableAngle = 30;
        [SerializeField] public bool m_showGrid = false;
        [SerializeField] public bool m_showPath = false;
        [SerializeField] public bool m_showSearchedNodes = false;

        [SerializeField] private PathfindingMethod m_pathfindingMethod;
        [SerializeField] public LayerMask m_unWalkableMask;
        [SerializeField] public LayerMask m_walkableMask;
        [SerializeField] public bool m_allowDiagonal = true;
        [SerializeField] public float m_nodeRadius = 1;
        [SerializeField] public int m_gridSize = 10;
        

        public readonly IDictionary<Vector2, Node> m_nodeGrid = new Dictionary<Vector2, Node>();

        private void Awake()
        {
            MakeGrid(transform.position);
            
        }

        


        private void MakeGrid(Vector3 location)
        {
            m_nodeGrid.Clear();

            // Make a 2D grid of nodes
            for (int x = 0; x < m_gridSize; x++)
            {
                for (int z = 0; z < m_gridSize; z++)
                {
                    Vector3 offset = location + new Vector3(x * m_nodeRadius, 0, z * m_nodeRadius);


                    // Trace to find the walkable ground
                    bool hit = Physics.Linecast(offset + new Vector3(0, 100, 0), offset + new Vector3(0, -100, 0), out RaycastHit rayHit);

                    Vector2Int nodePos = new(x, z);
                    Node node =  MakeNode(offset, nodePos);
                    
                    if (hit)
                    {
                        // If the angle of the terrain is more than the max walkable angle, it will be un-walkable
                        if (Vector3.Angle(Vector3.up, rayHit.normal) > m_maxWalkableAngle)
                        {
                            node.SetWalkable(false);
                        }

                        // Set the Y position of the node to the terrain
                        Vector3 oldPos = node.GetNodeWorldPosition();
                        node.SetNodeWorldPosition(new Vector3(oldPos.x, rayHit.point.y, oldPos.z));
                    }
                    else
                    {
                        // This means the ray did not hit anything, making an un-walkable node
                        node.SetWalkable(false);
                    }
                    
                    m_nodeGrid.Add(nodePos, node);

                }
            }
        }


        public List<Node> FindPath(Vector3 startPosition, Vector3 targetPosition)
        {
            return m_pathfindingMethod switch
            {
                PathfindingMethod.AStar => AStarPathfinding(startPosition, targetPosition),
                PathfindingMethod.BreadthFirstSearch => BreadthFSPathfinding(startPosition, targetPosition),
                PathfindingMethod.BestFirstSearch => BestFSPathfinding(startPosition, targetPosition),
                PathfindingMethod.DepthFirstSearch => DepthFSPathfinding(startPosition, targetPosition),
                PathfindingMethod.Dijkstra => DijkstraPathfinding(startPosition, targetPosition),
                _ => AStarPathfinding(startPosition, targetPosition)
            };
        }

        private List<Node> DepthFSPathfinding(Vector3 startPosition, Vector3 targetPosition)
        {
            // Depth First Search never finds the shortest path.
            // It checks each of its neighboring nodes in layers, in all directions from the start node
            // Checks the last node to enter the stack, which means the closer nodes are ignored until later.
            // Continues to expand until it finds the end node

            HashSet<Node> exploredNodes = new();
            
            // The Depth First Search uses a stack, where the top object is always popped first.
            // LIFO - Last one(Newest Member) in is the first one out
            Stack<Node> nodeQueue = new();

            Node startNode = GetNodeFromPosition(startPosition);
            Node endNode = GetNodeFromPosition(targetPosition);
            
            nodeQueue.Push(startNode);
            
            // While there are nodes in our queue
            while (nodeQueue.Count != 0)
            {
                // Dequeue the next node to current node
                Node currentNode = nodeQueue.Pop();
                
                // If this is our goal node, we have found the path, retrace and return.
                if (currentNode == endNode)
                {
                    return RetracePath(startNode, endNode);
                }
                
                // For debug purposes
                if (m_showSearchedNodes)
                {
                    currentNode.SetSelected(true);
                }
         
                
                // This is not the finished node, so get its neighbors
                List<Node> neighbors = GetNeighboringNodes(currentNode);

                // Loop through all of the once we have not explored yet
                foreach (Node neighbor in neighbors.Where(neighbor => !exploredNodes.Contains(neighbor)))
                {
                    // Mark it as explored
                    exploredNodes.Add(neighbor);
                    
                    // If the node is un walkable, continue
                    if (!neighbor.IsWalkable()) continue;
                    
                    // Set the parent of this node to our Current Node
                    neighbor.SetParent(currentNode);
                    
                    // Add it to the queue so it will be checked next time
                    nodeQueue.Push(neighbor);
                    
                    // For debug purposes
                    if (m_showSearchedNodes)
                    {
                        neighbor.SetSelected(true);
                    }
                    
                }

            }

            return null;

        }
        
        private List<Node> BreadthFSPathfinding(Vector3 startPosition, Vector3 targetPosition)
        {
            
            // Breadth First Search always finds the shortest path but does not consider movement cost.
            // It checks each of its neighboring nodes in layers, in all directions from the start node
            // Checks the next node in a queue, which means the closer nodes are checked before newly added nodes.
            // Continues to expand until it finds the end node

            HashSet<Node> exploredNodes = new();
            
            // The Breadth First Search uses a queue, where the oldest queue object is always dequeued first.
            // FIFO - First one(Oldest Member) in is the first one out
            Queue<Node> nodeQueue = new();

            Node startNode = GetNodeFromPosition(startPosition);
            Node endNode = GetNodeFromPosition(targetPosition);
            
            nodeQueue.Enqueue(startNode);
            
            // While there are nodes in our queue
            while (nodeQueue.Count != 0)
            {
                // Dequeue the next node to current node
                Node currentNode = nodeQueue.Dequeue();
                
                // If this is our goal node, we have found the path, retrace and return.
                if (currentNode == endNode)
                {
                    return RetracePath(startNode, endNode);
                }


                // This is not the finished node, so get its neighbors
                List<Node> neighbors = GetNeighboringNodes(currentNode);

                // Loop through all of the once we have not explored yet
                foreach (Node neighbor in neighbors.Where(node => !exploredNodes.Contains(node)))
                {
                    // Mark it as explored
                    exploredNodes.Add(neighbor);
                    
                    // If the node is un walkable, continue
                    if (!neighbor.IsWalkable()) continue;
                    
                    // Set the parent of this node to our Current Node
                    neighbor.SetParent(currentNode);
                    
                    // Add it to the queue so it will be checked next time
                    nodeQueue.Enqueue(neighbor);
                    
                    // For debug purposes
                    if (m_showSearchedNodes)
                    {
                        neighbor.SetSelected(true);
                    }
                    
                }

            }

            return null;

        }

        private List<Node> DijkstraPathfinding(Vector3 startPosition, Vector3 targetPosition)
        {
            // Dijkstra's algorithm always finds the shortest path and takes into account movement cost.
            // It checks each of its neighboring nodes in layers, in all directions from the start node
            // Checks the next node in a queue, which means the closer nodes are checked before newly added nodes.
            // Continues to expand until it finds the end node

            HashSet<Node> unExploredNodes = new();
            IDictionary<Node, int> distances = new Dictionary<Node, int>();

            Node startNode = GetNodeFromPosition(startPosition);
            Node endNode = GetNodeFromPosition(targetPosition);
            
            // Initialize all the distances
            foreach (var node in m_nodeGrid)
            {
                int dist = int.MaxValue;
                distances.Add(node.Value, dist);
                unExploredNodes.Add(node.Value);
            }

            distances[startNode] = 0;

            // While there are nodes in our queue
            while (unExploredNodes.Count > 0)
            {
                // Find the next node, which should be the closest one to the start
                Node currentNode = distances.Where(x => unExploredNodes.Contains(x.Key))
                    .OrderBy(x => x.Value).First().Key;

                // If this is our goal node, we have found the path, retrace and return.
                if (currentNode == endNode)
                {
                    return RetracePath(startNode, endNode);
                }

                // We have now explored this node
                unExploredNodes.Remove(currentNode);

                // Get all valid neighbors of the closest node
                List<Node> neighbors = GetNeighboringNodes(currentNode);
                
                // Loop through all the nodes which are walkable
                foreach (Node neighbor in neighbors.Where(node => node.IsWalkable()))
                {

                    // Get the cost of this node
                    int distance = distances[currentNode] + GetDistanceBetweenNodes(startNode, currentNode) + neighbor.weight;

                    // If the cost is less than the distance to this neighbor node
                    if (distance < distances[neighbor])
                    {
                        // Update the distance of this neighbor to the new shorter distance
                        distances[neighbor] = distance;
                        
                        // And set the parent of this node to our Current Node
                        neighbor.SetParent(currentNode);
                        
                        // For debug purposes
                        if (m_showSearchedNodes)
                        {
                            neighbor.SetSelected(true);
                        }
                    }

                }

            }

            return null;
        }
        
        private List<Node> BestFSPathfinding(Vector3 startPosition, Vector3 targetPosition)
        {   
            // Best First Search does not always find a path
            // It uses only the distance from end node to find the next most optimal node.
            // Continues until it finds the end node
            
            // Get the nodes
            Node startNode = GetNodeFromPosition(startPosition);
            Node endNode = GetNodeFromPosition(targetPosition);

            // If one of them is not valid, cancel
            if (startNode == null || endNode == null) return null;

            // Select the nodes
            startNode.SetSelected(true);
            endNode.SetSelected(true);

            // Make the Open and Closed lists
            List<Node> openNodes = new();
            List<Node> closedNodes = new();

            openNodes.Add(startNode);


            // While we have not yet reached the target node

            while (openNodes.Count > 0)
            {
                Node currentNode = openNodes[0];
                
                // Get the next node with lowest hCost
                foreach (Node node in openNodes.Where(node => node.hCost < currentNode.hCost))
                {
                    currentNode = node;
                }

                openNodes.Remove(currentNode);
                closedNodes.Add(currentNode);

                // If the path is found, return
                if (currentNode == endNode)
                {
                    return RetracePath(startNode, endNode);
                }
                
                
    
                List<Node> neighbors = GetNeighboringNodes(currentNode);

                foreach (Node neighbor in neighbors)
                {
                    // Check if the neighbor is valid, if not, continue to next
                    if (!neighbor.IsWalkable() || closedNodes.Contains(neighbor)) continue;

                    
                    
                    neighbor.hCost = GetDistanceBetweenNodes(neighbor, endNode) + neighbor.weight;
                    
                    // Check if this neighbor has a higher hCost, or not in Open list
                    if (currentNode.hCost <= neighbor.hCost && openNodes.Contains(neighbor)) continue;
                    
                    neighbor.SetParent(currentNode);

                    // If the openNodes doesn't already contain this node, add it
                    if (!openNodes.Contains(neighbor))
                    {
                        openNodes.Add(neighbor);
                    }
                    
                    // For debug purposes
                    if (m_showSearchedNodes)
                    {
                        neighbor.SetSelected(true);
                    }
                }
            }

            return null;
        }
        
        private List<Node> AStarPathfinding(Vector3 startPosition, Vector3 targetPosition)
        {
            // A star always finds the shortest path
            // It combines movement cost and distance from end node to find the next most optimal node.
            // Continues until it finds the end node
            
            // Get the nodes
            Node startNode = GetNodeFromPosition(startPosition);
            Node endNode = GetNodeFromPosition(targetPosition);

            // If one of them is not valid, cancel
            if (startNode == null || endNode == null) return null;

            // Select the nodes
            startNode.SetSelected(true);
            endNode.SetSelected(true);

            // Make the Open and Closed lists
            List<Node> openNodes = new();
            List<Node> closedNodes = new();

            openNodes.Add(startNode);


            // While we have not yet reached the target node

            while (openNodes.Count > 0)
            {
                Node currentNode = openNodes[0];

                currentNode = GetLowestCost(openNodes, currentNode);

                openNodes.Remove(currentNode);
                closedNodes.Add(currentNode);

                // If the path is found, return
                if (currentNode == endNode)
                {
                    return RetracePath(startNode, endNode);
                }
                
                

                List<Node> neighbors = GetNeighboringNodes(currentNode);

                foreach (Node neighbor in neighbors)
                {
                    // Check if the neighbor is valid, if not, continue to next
                    if (!neighbor.IsWalkable() || closedNodes.Contains(neighbor)) continue;

                    int newMovementCostToNeighbor = currentNode.gCost + GetDistanceBetweenNodes(currentNode, neighbor);

                    // Check if path to this one is longer, and if the open list contains it already
                    if (newMovementCostToNeighbor >= neighbor.gCost && openNodes.Contains(neighbor)) continue;
                    // Calculate the costs for this new node
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistanceBetweenNodes(neighbor, endNode) + neighbor.weight;
                    neighbor.SetParent(currentNode);

                    // If the openNodes doesn't already contain this node, add it
                    if (!openNodes.Contains(neighbor))
                    {
                        openNodes.Add(neighbor);
                    }
                    
                    // For debug purposes
                    if (m_showSearchedNodes)
                    {
                        neighbor.SetSelected(true);
                    }
                }
            }

            return null;
        }

        private List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new();

            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                
                if (m_showPath) currentNode.SetSelected(true);
                currentNode = currentNode.GetParent();
                path.Add(currentNode);
            }

            path.Reverse();

            return path;


        }

        private Node FindClosestNode(Node startNode, List<Node> checkNodes)
        {
            Node closestNode = null;
            
            foreach (Node node in checkNodes)
            {
                if (!node.IsWalkable()) continue;

                if (closestNode == null)
                {
                    closestNode = node;
                    continue;
                }
                
                float dist = GetDistanceBetweenNodes(startNode, node);
                node.gCost += (int)dist;
                
                if (node.gCost < closestNode.gCost)
                {
                    closestNode = node;
                }
            }

            return closestNode;
        }

        private void Update()
        {
            if(liveDebug)
            {
                MakeGrid(transform.position);
            }
        }

        private Node MakeNode(Vector3 worldPosition, Vector2Int gridPosition)
        {
            return new Node(m_nodeRadius, worldPosition, gridPosition, m_unWalkableMask);
        }

        private Node GetLowestCost(List<Node> openNodes, Node currentNode)
        {
            Node returnNode = currentNode;

            foreach (Node node in openNodes.Where(node => node.fCost < returnNode.fCost || node.fCost == returnNode.fCost && node.hCost < returnNode.hCost))
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
            Vector3Int gridPosition = Vector3Int.RoundToInt(position / m_nodeRadius);

            // Make Coordinates from the position
            Vector2Int nodeCoords = new Vector2Int(gridPosition.x, gridPosition.z);

            // Return the node if the coordinates is valid
            return NodeExists(nodeCoords) ? m_nodeGrid[nodeCoords] : null;
        }

        private bool NodeExists(Vector2Int nodeCoords)
        {
            // Node Coordinates are outside the grid
            if (nodeCoords.x > m_gridSize - 1 || nodeCoords.y > m_gridSize - 1) return false;

            // Node Coordinates are negative
            if (nodeCoords.x < 0 || nodeCoords.y < 0) return false;


            // Node Coords are valid
            return true;
        }

        private int GetDistanceBetweenNodes(Node a, Node b)
        {
            int distanceX = Mathf.Abs(a.GetNodeGridPosition().x - b.GetNodeGridPosition().x);
            int distanceY = Mathf.Abs(a.GetNodeGridPosition().y - b.GetNodeGridPosition().y);

            if (distanceX > distanceY)
                return m_diagonalCost * distanceY + m_straightCost * (distanceX - distanceY);
            return m_diagonalCost * distanceX + m_straightCost * (distanceY - distanceX);

        }

        // Lookup table for neighbor Nodes
        private readonly Vector2Int[] m_nodeNeighbors =
        {
            
            // Straight
            new (0, -1),
            new (1, 0),
            new (0, 1),
            new (-1, 0),
            
            // Diagonal
            new (-1, -1),
            new (1, -1),
            new (1, 1),
            new (-1, 1)

        };

        private List<Node> GetNeighboringNodes(Node parentNode)
        {

            List<Node> neighbors = new List<Node>();

            int numNeighbors = m_allowDiagonal ? 8 : 4;

       
            // Loop through all the neighbors and add them to the neighbors list

            for (int i = 0; i < numNeighbors; i++)
            {
                Vector2Int offset = parentNode.GetNodeGridPosition() + m_nodeNeighbors[i];
                if (!NodeExists(offset)) continue;

                Node newNode = m_nodeGrid[offset];
                neighbors.Add(newNode);
            }
            
            return neighbors;
        }

        private void OnDrawGizmos()
        {
            
            if (m_nodeGrid.Count <= 0) return;


            // Draw debug points to show the nodes

            foreach (Node node in m_nodeGrid.Values)
            {
                float radius = node.GetNodeRadius() * 0.9f;
                bool selected = node.IsSelected();
            
            Gizmos.color = node.IsWalkable() ? Color.green : Color.red;
            float weightLerp = node.weight / 100f;
            if (node.weight > 0) Gizmos.color = Color.Lerp(Color.green, Color.red, weightLerp);
            if(selected) Gizmos.color = Color.blue;
            


                if (m_showGrid || ((m_showPath || m_showSearchedNodes) && selected))
                {
                    Gizmos.DrawSphere(node.GetNodeWorldPosition(), radius / 4);
                }
            }
        }
    }