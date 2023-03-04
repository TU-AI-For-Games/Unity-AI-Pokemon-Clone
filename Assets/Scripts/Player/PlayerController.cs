using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Range(0f, 100f)][SerializeField] private float m_speed;
    [Range(0f, 720f)][SerializeField] private float m_rotationSpeed;

    public bool CanMove { get; set; }

    private Rigidbody m_rigidBody;

    private PocketMonster[] m_pocketMonsters = new PocketMonster[6];

    private int m_activePokemonIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();

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

        transform.Translate(Vector3.forward * Input.GetAxisRaw(StringConstants.FORWARD) * m_speed * Time.deltaTime);

        transform.Rotate(Vector3.up * m_rotationSpeed * Time.deltaTime * Input.GetAxisRaw(StringConstants.ROTATE));
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
        m_activePokemonIndex = index;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StringConstants.WILD_POKEMON_TAG))
        {
            // The trigger collider is on a child of the pokemon, passing the parent
            GameManager.Instance.StartBattle(BattleManager.BattleType.WildPkmn, other.transform.parent.gameObject);
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
