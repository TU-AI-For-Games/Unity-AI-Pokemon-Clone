#define RECORD_PLAYER_ACTIONS
using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject m_gameHUD;

    [SerializeField] private Camera m_mainCamera;

    [SerializeField] private PlayerController m_player;

    [Header("Battle Settings")]
    [SerializeField] private Camera m_battleCamera;
    [SerializeField] private BattleUIScript m_battleHUD;
    [SerializeField] private Transform m_battlePlayerPosition;
    [SerializeField] private Transform m_battlePlayerPkmnPosition;
    [SerializeField] private Transform m_battleTrainerPosition;
    [SerializeField] private Transform m_battleTrainerPkmnPosition;
    [SerializeField] private Transform m_battleWildPkmnPosition;


    private Vector3 m_previousPlayerPosition;
    private Quaternion m_previousPlayerRotation;

    public enum State
    {
        Overworld,
        Battle
    }

    private State m_state;


    private GameObject m_battler;

    // Start is called before the first frame update
    void Start()
    {
        m_player.CanMove = true;
        WildPocketMonsterManager.Instance.CanSpawnPokemon = true;

        m_previousPlayerPosition = new Vector3();
        m_previousPlayerRotation = new Quaternion();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartBattle(BattleManager.BattleType type, GameObject battler)
    {
        BattleManager.Instance.InitialiseBattle(type);

        m_battler = battler;

        Debug.Log("BATTLE STARTED!");
        m_state = State.Battle;

        m_player.CanMove = false;
        WildPocketMonsterManager.Instance.CanSpawnPokemon = false;

        m_battleHUD.gameObject.SetActive(true);
        m_gameHUD.SetActive(false);

        m_mainCamera.gameObject.SetActive(false);
        m_battleCamera.gameObject.SetActive(true);

        m_previousPlayerPosition = new(m_player.transform.position.x, m_player.transform.position.y, m_player.transform.position.z);
        m_previousPlayerRotation = new(m_player.transform.rotation.x, m_player.transform.rotation.y, m_player.transform.rotation.z, m_player.transform.rotation.w);

        m_player.Teleport(m_battlePlayerPosition.position, m_battlePlayerPosition.rotation);

        SpawnPlayerPokemon();

        BattleManager.Instance.SetPlayerPokemon(m_player.GetActivePokemon());

        switch (type)
        {
            case BattleManager.BattleType.WildPkmn:
                battler.transform.position = m_battleWildPkmnPosition.position;
                battler.transform.rotation = m_battleWildPkmnPosition.rotation;

                PocketMonster wildMon = battler.GetComponent<WildPocketMonster>().Pokemon;

                BattleManager.Instance.SetOtherPokemon(wildMon);

                m_battleHUD.OnOtherSwitchPokemon(wildMon);
                break;
            case BattleManager.BattleType.Trainer:
                // TODO: SET UP TRAINER BATTLES
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void SpawnPlayerPokemon()
    {
        m_player.ShowPokemon(m_battlePlayerPkmnPosition);
        m_battleHUD.OnPlayerSwitchPokemon();
    }

    public void EndBattle(bool runAway)
    {
        if (runAway && BattleManager.Instance.GetBattleType() != BattleManager.BattleType.WildPkmn)
        {
            return;
        }

        Debug.Log("BATTLE ENDED!");
        m_state = State.Overworld;

        m_player.CanMove = true;
        WildPocketMonsterManager.Instance.CanSpawnPokemon = true;

        m_battleHUD.gameObject.SetActive(false);
        m_gameHUD.SetActive(true);

        m_mainCamera.gameObject.SetActive(true);
        m_battleCamera.gameObject.SetActive(false);

        m_player.Teleport(m_previousPlayerPosition, m_previousPlayerRotation);

        if (BattleManager.Instance.GetBattleType() == BattleManager.BattleType.WildPkmn)
        {
            Destroy(m_battler);
        }
        else if (BattleManager.Instance.GetBattleType() == BattleManager.BattleType.Trainer)
        {
            // TODO: Destroy the trainer's pokemon but maintain the trainer model
        }

        BattleManager.Instance.SetBattleType(BattleManager.BattleType.None);

#if RECORD_PLAYER_ACTIONS
        RecordActions.Instance.OnEndBattle();
#endif
    }

    public PlayerController GetPlayerController()
    {
        return m_player;
    }

    protected override void InternalInit()
    {
    }
}
