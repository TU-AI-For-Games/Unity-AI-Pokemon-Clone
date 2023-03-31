using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum State
{
    ROAMING,
    STOPPED
}


public class NPCBrain : MonoBehaviour
{
    [SerializeField]
    float m_movementSpeed = 5;

    [SerializeField]
    float m_movementRadius = 30;

    [SerializeField]
    float m_acceptanceRadius = 3;

    [SerializeField]
    float m_minTimeBetweenMoves = 2;

    [SerializeField]
    float m_maxTimeBetweenMoves = 4;

    [SerializeField]
    bool m_drawTargetPoint = false;


    private Pathfinding m_navGrid;
    private List<Node> m_path;
    private Vector3 m_target;
    private State m_state = State.ROAMING;

    // Timer
    float m_timeRemaining = 0f;
    bool m_countingDown = false;


    private void Start()
    {
        StartMoving();
        //m_mesh = GetComponent<MeshFilter>();
        //height = m_mesh.mesh.bounds.max.y * transform.localScale.y;
        //heightOffset = new Vector3(0, height, 0);
    }



    private void Update()
    {


        MoveTowards();

        if (m_countingDown)
        {
            if (m_timeRemaining > 0f)
            {
                m_timeRemaining -= Time.deltaTime;
            }
            else 
            {
                m_timeRemaining = 0f;
                TimerDone();
            }

        }
    }

    private void TimerDone()
    {
        // When the timer is done, we start moving towards a new location, and stop the timer from ticking
       

        m_countingDown = false;
        StartMoving();

    }


    public void SetNavGrid(Pathfinding newGrid)
    {
        m_navGrid = newGrid;
    }


    private void StartTimer(float time)
    {
        // When we start the timer, we wait for a random amount of time
        m_timeRemaining = time;
        m_countingDown = true;;
    }


    private Vector3 GetRandomPointInRadius()
    {
        Vector3 randomPos = transform.position + (Random.insideUnitSphere * m_movementRadius);

        // Trace to find the walkable ground
        bool hit = Physics.Linecast(randomPos + new Vector3(0, 100, 0), randomPos + new Vector3(0, -100, 0), out RaycastHit rayHit, m_navGrid.m_walkableMask);

        if (hit)
        {
            return rayHit.point;
        }

        StartTimer(0.5f);
        return transform.position;


    }

    private void StartMoving()
    {

        m_target = GetRandomPointInRadius();
        m_path = m_navGrid.FindPath(transform.position, m_target);
        if (m_path == null)
        {
            StartTimer(0.5f);
        }

    }

    private void MoveTowards()
    {
        if (m_state != State.ROAMING) return;
        if (m_path == null || m_path.Count < 1) return;

        Vector3 nextPoint = m_path[0].GetNodeWorldPosition();

        // If distance is less than the acceptanceradius
        if (Vector3.Distance(transform.position, nextPoint) < m_acceptanceRadius)
        {
            m_path.RemoveAt(0);

            if (m_path.Count == 0)
            {
                StartTimer(Random.Range(m_minTimeBetweenMoves, m_maxTimeBetweenMoves));
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, nextPoint, m_movementSpeed * Time.deltaTime);
    }


    public void SetNPCState(State newState)
    {
        m_state = newState;
    }


    private void OnDrawGizmos()
    {
        if (!m_drawTargetPoint) return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(m_target, 2);

        
        if (m_navGrid.m_nodeGrid.Count <= 0) return;




        // Draw debug points to show the nodes

        foreach (Node node in m_navGrid.m_nodeGrid.Values)
        {
            float radius = node.GetNodeRadius() * 0.9f;
            bool selected = node.IsSelected();
            Gizmos.color = Color.blue;
            if(selected)
            {
                Gizmos.DrawSphere(node.GetNodeWorldPosition(), radius / 4);
            }
            
        }
    }
}
