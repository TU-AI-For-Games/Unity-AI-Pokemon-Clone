using System.Collections.Generic;
using UnityEngine;

public class WildPocketMonster : MonoBehaviour
{
    public PocketMonster Pokemon { get; private set; }

    [SerializeField] private float m_speed = 10f;

    [SerializeField] private float m_acceptanceRadius;

    private List<Node> m_path;
    private Vector3 m_target;
    private Pathfinding m_navGrid;

    private WildPocketMonsterArea m_parentArea;

    private void Start()
    {
        m_path = new List<Node>();
        m_parentArea = transform.parent.gameObject.GetComponent<WildPocketMonsterArea>();

        SetPathfindingTarget(m_parentArea.GenerateRandomPosition());
    }

    private void Update()
    {
        if (m_path == null)
        {
            SetPathfindingTarget(m_parentArea.GenerateRandomPosition());
        }

        // If the path is empty, we've reached the target... Choose a new one
        if (m_path.Count == 0)
        {
            SetPathfindingTarget(m_parentArea.GenerateRandomPosition());
        }
        else
        {
            MoveTowards();
        }
    }

    private void MoveTowards()
    {
        Vector3 nextPoint = m_path[0].GetNodeWorldPosition();

        // If distance is less than the acceptanceradius
        if (Vector3.Distance(transform.position, nextPoint) < m_acceptanceRadius)
        {
            m_path.RemoveAt(0);
        }

        transform.position = Vector3.MoveTowards(transform.position, nextPoint, m_speed * Time.deltaTime);
    }

    private void FindPathTo(Vector3 targetPosition)
    {
        m_path = m_navGrid.FindPath(transform.position, targetPosition);
    }

    public void SetNavGrid(Pathfinding grid)
    {
        m_navGrid = grid;
    }

    private void SetPathfindingTarget(Vector3 newTarget)
    {
        m_path.Clear();

        m_target = newTarget;
        FindPathTo(m_target);
    }

    public void SetPokemon(PocketMonster mon)
    {
        Pokemon = mon;
    }
}
