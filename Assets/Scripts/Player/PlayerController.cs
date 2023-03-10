#define RECORD_PLAYER_ACTIONS
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Range(0f, 100f)][SerializeField] private float m_speed;
    [Range(0f, 720f)][SerializeField] private float m_rotationSpeed;
    [Range(0f, 720f)][SerializeField] private float m_cameraSensitivity;
    [SerializeField] private Animator m_animator;
    [SerializeField] private CharacterController m_controller;
    [SerializeField] private GameObject m_camera;

    public bool CanMove { get; set; }

    private PocketMonster[] m_pocketMonsters = new PocketMonster[6];

    private int m_activePokemonIndex = 0;

    private Vector2 m_cameraRotation;

    // Start is called before the first frame update
    void Start()
    {
        m_controller = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;

        // For now, set up 6 random pokemon
        for (int i = 0; i < 6; ++i)
        {
            m_pocketMonsters[i] = PocketMonsterManager.Instance.GetPocketMonster(
                Random.Range(
                    1,
                    PocketMonsterManager.Instance.GetPocketMonsterCount()
                )
            );
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!CanMove)
        {
            return;
        }


        float forward = Input.GetAxisRaw("Vertical");
        float sideways = Input.GetAxisRaw("Horizontal");

        Vector3 forwardVector = m_camera.transform.forward.normalized * forward;
        Vector3 sidewaysVector = m_camera.transform.right.normalized * sideways;

        Vector3 gravity = new Vector3(0,-9.81f, 0);

        Vector3 moveDirection = (forwardVector + sidewaysVector).normalized + gravity;
        Vector3 moveVector = moveDirection * (m_speed * Time.deltaTime);
        
        
        print(moveDirection);
        print(moveVector);
        
        m_controller.Move(moveVector);
        
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, m_rotationSpeed * Time.deltaTime);
        
        m_animator.SetFloat("Speed", m_controller.velocity.magnitude);
        
        m_cameraRotation.x += Input.GetAxis("Mouse X") * m_cameraSensitivity;
        m_cameraRotation.y += Input.GetAxis("Mouse Y") * m_cameraSensitivity;

        m_camera.transform.localRotation = Quaternion.Euler(-m_cameraRotation.y, m_cameraRotation.x, 0);
        

        // Set camera location to player location
        m_camera.transform.position = transform.position;


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
