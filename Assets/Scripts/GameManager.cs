#define RECORD_PLAYER_ACTIONS
using System;
using Cinemachine;
using Learning;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using CameraType = Poke.CameraType;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject m_gameHUD;

    [SerializeField] private PlayerController m_player;
    private PocketMonsterTrainer m_trainer;

    [SerializeField] private PostProcessVolume m_overworldVolume;
    [SerializeField] private PostProcessVolume m_battleVolume;

    [Header("Battle Settings")]
    [SerializeField] private BattleUIScript m_battleHUD;
    [SerializeField] private Transform m_battlePlayerPosition;
    [SerializeField] private Transform m_battlePlayerPkmnPosition;
    [SerializeField] private Transform m_battleTrainerPosition;
    [SerializeField] private Transform m_battleTrainerPkmnPosition;
    [SerializeField] private Transform m_battleWildPkmnPosition;

    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineVirtualCameraBase m_overworldCamera;
    [SerializeField] private CinemachineVirtualCameraBase m_battleCamera;

    private Vector3 m_previousPlayerPosition;
    private Quaternion m_previousPlayerRotation;
    private Vector3 m_previousTrainerPosition;
    private Quaternion m_previousTrainerRotation;

    private TypeLearner m_typeLearner;
    private MoveDecisionLearner m_moveDecisionLearner;

    public enum State
    {
        Overworld,
        Battle
    }

    private State m_state;


    private GameObject m_battler;

    public State CurrentState => m_state;

    // Start is called before the first frame update
    void Start()
    {
        m_typeLearner = new TypeLearner(5000, 0.141f, 20, true, Layer.ActivationFunction.TanH);
        m_moveDecisionLearner = new MoveDecisionLearner(10000, 0.141f, 100, true, Layer.ActivationFunction.TanH);

        m_player.CanMove = true;
        WildPocketMonsterManager.Instance.CanSpawnPokemon = true;

        m_previousPlayerPosition = new Vector3();
        m_previousPlayerRotation = new Quaternion();

        SwitchCamera(CameraType.Overworld);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartBattle(BattleManager.BattleType type, GameObject battler)
    {
        BattleManager.Instance.InitialiseBattle(type);

        Destroy(m_battler);

        m_battler = battler;

        Debug.Log("BATTLE STARTED!");
        m_state = State.Battle;

        m_player.CanMove = false;
        WildPocketMonsterManager.Instance.CanSpawnPokemon = false;

        m_battleHUD.gameObject.SetActive(true);
        m_gameHUD.SetActive(false);

        SwitchCamera(Poke.CameraType.Battle);

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

                // Stop the wildPokemon from walking when in battle!
                WildPocketMonster wild = battler.GetComponent<WildPocketMonster>();
                wild.StopMoving();

                PocketMonster wildMon = wild.Pokemon;

                BattleManager.Instance.SetOtherPokemon(wildMon);

                m_battleHUD.OnOtherSwitchPokemon(wildMon);
                break;
            case BattleManager.BattleType.Trainer:
                m_previousTrainerPosition = new(battler.transform.position.x, battler.transform.position.y, battler.transform.position.z);
                m_previousTrainerRotation = new(battler.transform.rotation.x, battler.transform.rotation.y, battler.transform.rotation.z, battler.transform.rotation.w);

                battler.transform.position = m_battleTrainerPosition.position;
                battler.transform.rotation = m_battleTrainerPosition.rotation;

                m_trainer = battler.GetComponent<PocketMonsterTrainer>();

                // Spawn the trainer's pokemon
                SpawnTrainerPokemon();
                BattleManager.Instance.SetTrainer(m_trainer);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void SpawnPlayerPokemon()
    {
        ShowPokemon(m_battlePlayerPkmnPosition, m_player.GetActivePokemon().ID);
        m_battleHUD.OnPlayerSwitchPokemon();
    }

    public void SpawnTrainerPokemon()
    {
        ShowPokemon(m_battleTrainerPkmnPosition, m_trainer.GetActivePokemon().ID);
        m_battleHUD.OnOtherSwitchPokemon(m_trainer.GetActivePokemon());
    }

    public void ShowPokemon(Transform parent, int pokemonId)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        Instantiate(PocketMonsterManager.Instance.GetPocketMonsterMesh(pokemonId), parent);
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

        SwitchCamera(Poke.CameraType.Overworld);

        m_player.Teleport(m_previousPlayerPosition, m_previousPlayerRotation);

        if (BattleManager.Instance.GetBattleType() == BattleManager.BattleType.WildPkmn)
        {
            WildPocketMonsterManager.Instance.OnPokemonDeath(m_battler);
        }
        else if (BattleManager.Instance.GetBattleType() == BattleManager.BattleType.Trainer)
        {
            // TODO: Destroy the trainer's pokemon but maintain the trainer model
        }

        BattleManager.Instance.SetBattleType(BattleManager.BattleType.None);

#if RECORD_PLAYER_ACTIONS
        RecordActions.Instance.OnEndBattle();
#endif

        // Heal up the player's Pokemon
        foreach (PocketMonster monster in m_player.GetPokemon())
        {
            // monster.HealHealthAndStatus();
            if (monster.HasFainted())
            {
                monster.HealHealthAndStatus();
            }
        }
    }

    private void SwitchCamera(Poke.CameraType cameraType)
    {
        m_overworldVolume.gameObject.SetActive(false);
        m_battleVolume.gameObject.SetActive(false);

        switch (cameraType)
        {
            case Poke.CameraType.Battle:
                m_overworldCamera.Priority = 0;
                m_battleCamera.Priority = 10;

                m_battleVolume.gameObject.SetActive(true);
                break;
            case Poke.CameraType.Overworld:
                m_overworldCamera.Priority = 10;
                m_battleCamera.Priority = 0;

                m_overworldCamera.LookAt = m_player.transform;
                m_overworldCamera.Follow = m_player.transform;

                m_overworldVolume.gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(cameraType), cameraType, null);
        }
    }

    public PlayerController GetPlayerController()
    {
        return m_player;
    }

    public TypeLearner GetTypeLearner()
    {
        return m_typeLearner;
    }

    public MoveDecisionLearner GetMoveDecisionLearner()
    {
        return m_moveDecisionLearner;
    }

    protected override void InternalInit()
    {
    }
}

namespace Poke
{
    public enum CameraType
    {
        Battle,
        Overworld
    }
}
