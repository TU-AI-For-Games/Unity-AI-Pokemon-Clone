using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Range(0f, 100f)][SerializeField] private float m_speed;
    [Range(0f, 720f)][SerializeField] private float m_rotationSpeed;

    [SerializeField] private Transform m_activePokemonTransform;

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

    public void ShowPokemon()
    {
        if (m_activePokemonTransform.childCount > 0)
        {
            Destroy(m_activePokemonTransform.GetChild(0).gameObject);
        }

        Instantiate(PocketMonsterManager.Instance.GetPocketMonsterMesh(GetActivePokemon().ID),
            m_activePokemonTransform);
    }

    public PocketMonster GetActivePokemon()
    {
        return m_pocketMonsters[m_activePokemonIndex];
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StringConstants.WILD_POKEMON_TAG))
        {
            GameManager.Instance.StartBattle();
        }
    }
}
