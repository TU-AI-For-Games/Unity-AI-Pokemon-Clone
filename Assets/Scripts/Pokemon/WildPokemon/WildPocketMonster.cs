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


    void Update()
    {
        if (m_path == null || m_path.Count == 0)
        {
            return;
        }
        MoveTowards();
    }

    void MoveTowards()
    {
        Vector3 nextPoint = m_path[0].GetNodeWorldPosition();

        // If distance is less than the acceptanceradius
        if (Vector3.Distance(transform.position, nextPoint) < m_acceptanceRadius)
        {
            m_path.RemoveAt(0);
        }

        transform.position = Vector3.MoveTowards(transform.position, nextPoint, m_speed * Time.deltaTime);
    }

    void FindPathTo(Vector3 targetPosition)
    {
        m_path = m_navGrid.FindPath(transform.position, targetPosition);
    }

    public void SetPathfindingTarget(Vector3 newTarget)
    {
        m_target = newTarget;
        FindPathTo(m_target);
    }

    public void SetPokemon(PocketMonster mon)
    {
        Pokemon = mon;
    }
}
