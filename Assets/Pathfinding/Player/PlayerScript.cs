using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float moveSpeed = 10f;

    [SerializeField]
    public Pathfinding navGrid;

    [SerializeField]
    public GameObject targetObject;

    [SerializeField]
    public float acceptanceRadius;

    private List<Node> path;

    private void Start()
    {
        if (targetObject == null) return;
        FindPathTo(targetObject.transform.position);
    }

    void Update()
    {
        MoveTowards();
    }

    void MoveTowards()
    {

        if (path == null) return;
        if (path.Count == 0) return;

        Vector3 nextPoint = path[0].GetNodeWorldPosition();

        // If distance is less than the acceptanceradius
        if (Vector3.Distance(transform.position, nextPoint) < acceptanceRadius)
        {
            print("Log");
            // Remove point from array
            path.RemoveAt(0);

        }

        transform.position = Vector3.MoveTowards(transform.position, nextPoint, moveSpeed * Time.deltaTime);


    }

    void FindPathTo(Vector3 targetPosition)
    {
        // Get path
        path = navGrid.FindPath(transform.position, targetPosition);

    }
}
