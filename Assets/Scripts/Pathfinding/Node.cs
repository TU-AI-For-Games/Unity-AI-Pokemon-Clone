using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Node
{
    
    // G cost is the cost from start to this node
    public int gCost;
    // H cost is the cost from this to the end
    public int hCost;
    // F cost is the combined cost
    public int fCost => gCost + hCost;

    private float m_radius;
    private Vector3 m_worldPosition;
    private Vector2Int m_gridPosition;
    private bool m_walkable = true;
    private LayerMask m_unWalkableMask;
    private bool m_selected;
    private Node m_parentNode = null;

    public Node(float radius, Vector3 worldPosition, Vector2Int gridPosition, LayerMask unwalkableMask)
    {
        m_radius = radius;
        m_worldPosition = worldPosition;
        m_gridPosition = gridPosition;
        m_unWalkableMask = unwalkableMask;
        CheckWalkable();
    }

    private void CheckWalkable()
    {
        m_walkable = !Physics.CheckSphere(m_worldPosition, m_radius, m_unWalkableMask);
    }

    public void SetParent(Node newParent)
    {
        m_parentNode = newParent;
    }
    
    public Node GetParent()
    {
        return m_parentNode;
    }

    public bool IsWalkable()
    {
        return m_walkable;
    }
    
    public void SetWalkable(bool isWalkable)
    {
        m_walkable = isWalkable;
    }

    public Vector3 GetNodeWorldPosition()
    {
        return m_worldPosition;
    }
    
    public Vector2Int GetNodeGridPosition()
    {
        return m_gridPosition;
    }
    
    public void SetNodeWorldPosition(Vector3 newWorldLocation)
    {
        m_worldPosition = newWorldLocation;
    }

    public float GetNodeRadius()
    {
        return m_radius;
    }
    
    public void SetSelected(bool selected)
    {
        m_selected = selected;
    }
    
    public bool IsSelected()
    {
        return m_selected;
    }
    
    
    
}
