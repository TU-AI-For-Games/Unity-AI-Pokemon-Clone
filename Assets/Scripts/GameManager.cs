using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject m_battleHUD;
    [SerializeField] private GameObject m_gameHUD;

    public enum State
    {
        Overworld,
        Battle
    }

    private State m_state;

    // Start is called before the first frame update
    void Start()
    {
        PocketMonsterManager.Instance.PrintPokemon(151);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartBattle()
    {
        // TODO: Set up camera
        // TODO: Show the HUD
        Debug.Log("BATTLE STARTED!");
        m_state = State.Battle;
    }
}
