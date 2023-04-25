#define RECORD_PLAYER_ACTIONS
#undef INFINITE_BATTLE
using System.Collections;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerController : MonoBehaviour
{
    [Range(0f, 100f)][SerializeField] private float m_speed;
    [Range(0f, 720f)][SerializeField] private float m_rotationSpeed;
    [Range(0f, 720f)][SerializeField] private float m_cameraSensitivity;

    private Animator m_animator;
    private CharacterController m_controller;

    [SerializeField] private Transform m_cameraFocus;
    [SerializeField] private CinemachineFreeLook m_freeLookCamera;

    public bool CanMove
    {
        get => m_canMove;
        set
        {
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            m_canMove = value;
        }
    }

    private bool m_canMove;

    [SerializeField] private int[] m_pokemonIds;
    private readonly PocketMonster[] m_pocketMonsters = new PocketMonster[6];

    private int m_activePokemonIndex = 0;

    private Vector2 m_cameraRotation;

    private Camera m_mainCamera;
    private static readonly int Speed = Animator.StringToHash("Speed");

    // Start is called before the first frame update
    private void Start()
    {
        m_controller = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();

        m_mainCamera = Camera.main;

        for (int i = 0; i < 6; ++i)
        {
            m_pocketMonsters[i] = PocketMonsterManager.Instance.GetPocketMonster(m_pokemonIds[i]);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!CanMove || m_controller.enabled == false)
        {
            return;
        }

#if INFINITE_BATTLE
        if (GameManager.Instance.CurrentState != GameManager.State.Battle)
        {
            GameManager.Instance.StartBattle(BattleManager.BattleType.WildPkmn, WildPocketMonsterManager.Instance.SpawnPokemon());
        }
#endif


        float forward = Input.GetAxisRaw(StringConstants.FORWARD);
        float sideways = Input.GetAxisRaw(StringConstants.ROTATE);

        Transform cameraTransform = m_mainCamera.transform;
        Vector3 forwardVector = cameraTransform.forward.normalized * forward;
        Vector3 sidewaysVector = cameraTransform.right.normalized * sideways;

        Vector3 gravity = new Vector3(0, -9.81f, 0);

        Vector3 moveDirection = (forwardVector + sidewaysVector).normalized + gravity;
        Vector3 moveVector = moveDirection * (m_speed * Time.deltaTime);


        m_controller.Move(moveVector);
        Vector3 lookDirection = new Vector3(moveDirection.x, 0, moveDirection.z);

        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, m_rotationSpeed * Time.deltaTime);
        }


        m_animator.SetFloat(Speed, m_controller.velocity.magnitude);
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        m_freeLookCamera.Follow = null;
        m_freeLookCamera.LookAt = null;

        m_freeLookCamera.enabled = false;

        m_controller.enabled = false;

        transform.position = position;
        transform.rotation = rotation;

        m_freeLookCamera.PreviousStateIsValid = false;

        StartCoroutine(UpdateCameraOneFrameLater());
    }

    private IEnumerator UpdateCameraOneFrameLater()
    {
        yield return null;

        m_freeLookCamera.enabled = true;

        m_freeLookCamera.Follow = m_cameraFocus;
        m_freeLookCamera.LookAt = m_cameraFocus;

        m_controller.enabled = true;
    }

    public void ShowPokemon(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        Instantiate(PocketMonsterManager.Instance.GetPocketMonsterMesh(GetActivePokemon().ID), parent);
    }

    public PocketMonster GetActivePokemon()
    {
        return m_pocketMonsters[m_activePokemonIndex];
    }

    public void SetActivePokemonIndex(int index)
    {
#if RECORD_PLAYER_ACTIONS
        RecordActions.Instance.PreviousPokemonID = m_pocketMonsters[m_activePokemonIndex].ID;
#endif

        m_activePokemonIndex = index;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StringConstants.WILD_POKEMON_TAG))
        {
            // The trigger collider is on a child of the pokemon, passing the parent
            GameManager.Instance.StartBattle(BattleManager.BattleType.WildPkmn, other.transform.parent.gameObject);
        }

        if (other.CompareTag(StringConstants.POKEMON_CENTRE_TAG))
        {
            HealPokemon();
        }
    }

    private void HealPokemon()
    {
        foreach (PocketMonster pocketMonster in m_pocketMonsters)
        {
            pocketMonster.GetStats().HP = pocketMonster.GetStats().BaseHP;
            pocketMonster.HealStatus();
            pocketMonster.ResetStats();
        }
    }

    public bool HasUsablePokemon()
    {
        return m_pocketMonsters.Any(
            pokemon => pokemon.GetStats().HP > 0
        );
    }

    public PocketMonster[] GetPokemon()
    {
        return m_pocketMonsters;
    }
}
