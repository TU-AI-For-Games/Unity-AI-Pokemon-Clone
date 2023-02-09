using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Node
{
    public float radius;
    public Vector3 position;
    public bool walkable = true;
    private LayerMask unwalkableMask;

    public Node(float radius, Vector3 position, LayerMask unwalkableMask)
    {
        this.radius = radius;
        this.position = position;
        this.unwalkableMask = unwalkableMask;
        CheckWalkable();
    }

    private void CheckWalkable()
    {
        walkable = !Physics.CheckSphere(position, radius, unwalkableMask);
    }
    
    
}
