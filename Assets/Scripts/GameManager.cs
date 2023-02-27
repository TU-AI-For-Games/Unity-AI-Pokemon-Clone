using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject m_battleHUD;
    [SerializeField] private GameObject m_gameHUD;

    [SerializeField] private Camera m_mainCamera;
    [SerializeField] private Camera m_battleCamera;

    [SerializeField] private PlayerController m_player;


    public enum State
    {
        Overworld,
        Battle
    }

    private State m_state;

    // Start is called before the first frame update
    void Start()
    {
        m_player.CanMove = true;
        WildPocketMonsterManager.Instance.CanSpawnPokemon = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartBattle()
    {
        Debug.Log("BATTLE STARTED!");
        m_state = State.Battle;
        WildPocketMonsterManager.Instance.CanSpawnPokemon = false;
        m_battleHUD.SetActive(true);
        m_gameHUD.SetActive(false);

        m_mainCamera.gameObject.SetActive(false);
        m_battleCamera.gameObject.SetActive(true);

        m_player.CanMove = false;
        m_player.ShowPokemon();
    }

    protected override void InternalInit()
    {
    }
}
