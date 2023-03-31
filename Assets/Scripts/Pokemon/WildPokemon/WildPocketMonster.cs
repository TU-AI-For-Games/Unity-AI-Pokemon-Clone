using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WildPocketMonster : MonoBehaviour
{
    public PocketMonster Pokemon { get; private set; }


    private WildPocketMonsterArea m_parentArea;

    private void Start()
    {
        m_parentArea = transform.parent.gameObject.GetComponent<WildPocketMonsterArea>();

    }


    public void SetPokemon(PocketMonster mon)
    {
        Pokemon = mon;
    }
}
