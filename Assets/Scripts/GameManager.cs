using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject m_gameHUD;

    [SerializeField] private Camera m_mainCamera;

    [SerializeField] private PlayerController m_player;

    [Header("Battle Settings")]
    [SerializeField] private Camera m_battleCamera;
    [SerializeField] private BattleUI m_battleHUD;
    [SerializeField] private Transform m_battlePlayerPosition;
    [SerializeField] private Transform m_battlePlayerPkmnPosition;
    [SerializeField] private Transform m_battleTrainerPosition;
    [SerializeField] private Transform m_battleTrainerPkmnPosition;
    [SerializeField] private Transform m_battleWildPkmnPosition;


    private Transform m_previousPlayerPosition;

    public enum State
    {
        Overworld,
        Battle
    }

    private State m_state;

    public enum BattleType
    {
        WildPkmn,
        Trainer
    }

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

    public void StartBattle(BattleType type, GameObject battler)
    {
        Debug.Log("BATTLE STARTED!");
        m_state = State.Battle;
        WildPocketMonsterManager.Instance.CanSpawnPokemon = false;
        m_battleHUD.gameObject.SetActive(true);
        m_gameHUD.SetActive(false);

        m_mainCamera.gameObject.SetActive(false);
        m_battleCamera.gameObject.SetActive(true);

        m_previousPlayerPosition = m_player.transform;

        m_player.CanMove = false;

        m_player.transform.position = m_battlePlayerPosition.position;
        m_player.transform.rotation = m_battlePlayerPosition.rotation;
        m_player.ShowPokemon(m_battlePlayerPkmnPosition);

        switch (type)
        {
            case BattleType.WildPkmn:
                battler.transform.position = m_battleWildPkmnPosition.position;
                battler.transform.rotation = m_battleWildPkmnPosition.rotation;

                m_battleHUD.OnOtherSwitchPokemon(battler.GetComponent<WildPocketMonster>().Pokemon);
                break;
            case BattleType.Trainer:
                // TODO: SET UP TRAINER BATTLES
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    protected override void InternalInit()
    {
    }
}
