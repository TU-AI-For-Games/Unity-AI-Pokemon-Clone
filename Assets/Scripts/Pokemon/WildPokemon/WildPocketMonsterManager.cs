using System.Collections.Generic;
using UnityEngine;

public class WildPocketMonsterManager : Singleton<WildPocketMonsterManager>
{
    [SerializeField] private GameObject m_wildPocketMonsterTemplate;

    [SerializeField] private float m_spawnTime = 3f;
    private float m_spawnCountdown;

    [SerializeField] private Transform m_bottomLeft;
    [SerializeField] private Transform m_topRight;

    public bool CanSpawnPokemon { get; set; }

    [SerializeField] private List<WildPocketMonsterArea> m_spawnAreas;

    [SerializeField] private Pathfinding m_navGrid;

    protected override void InternalInit()
    {
    }

    private void Start()
    {
        m_spawnCountdown = m_spawnTime;
    }

    private void Update()
    {
        m_spawnCountdown -= Time.deltaTime;

        if (m_spawnCountdown < 0f)
        {
            m_spawnCountdown = m_spawnTime;
            SpawnPokemonInArea();
        }
    }

    public void SpawnPokemonInArea()
    {
        if (!CanSpawnPokemon)
        {
            return;
        }

        foreach (WildPocketMonsterArea pocketMonsterArea in m_spawnAreas)
        {
            if (pocketMonsterArea.IsFull())
            {
                continue;
            }

            int pokedexNumber = pocketMonsterArea.GetNextPokedexNumber();
            GameObject pokemon = SpawnPokemon(pocketMonsterArea.transform, pokedexNumber);

            pocketMonsterArea.AddPokemon(pokemon);
        }
    }

    public GameObject SpawnPokemon(Transform parent = null, int pokedexNumber = -1)
    {
        if (pokedexNumber == -1)
        {
            pokedexNumber = Random.Range(0, 152);
        }

        GameObject pokemonWrapper = parent == null ? Instantiate(m_wildPocketMonsterTemplate) : Instantiate(m_wildPocketMonsterTemplate, parent);

        pokemonWrapper.name = $"WILD_{PocketMonsterManager.Instance.GetPocketMonster(pokedexNumber).Name}";

        // Spawn the pokemon mesh
        Instantiate(PocketMonsterManager.Instance.GetPocketMonsterMesh(pokedexNumber), pokemonWrapper.transform);

        // Set up the WildPocketMonster reference
        WildPocketMonster wildMon = pokemonWrapper.GetComponent<WildPocketMonster>();
        wildMon.SetPokemon(PocketMonsterManager.Instance.GetPocketMonster(pokedexNumber));
        wildMon.SetNavGrid(m_navGrid);

        return pokemonWrapper;
    }

    public void OnPokemonDeath(GameObject pokemonObject)
    {
        if (pokemonObject.transform.parent == null)
        {
            return;
        }

        // RemovePokemon the gameobject from the area
        WildPocketMonsterArea area = pokemonObject.transform.parent.gameObject.GetComponent<WildPocketMonsterArea>();
        area.RemovePokemon(pokemonObject);
    }
}
