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
            SpawnPokemon();
        }
    }

    public void SpawnPokemon()
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

            GameObject pokemonWrapper = Instantiate(m_wildPocketMonsterTemplate, pocketMonsterArea.transform);

            pocketMonsterArea.AddPokemon(pokemonWrapper);

            int pokedexNumber = pocketMonsterArea.GetNextPokedexNumber();

            pokemonWrapper.name = $"WILD_{PocketMonsterManager.Instance.GetPocketMonster(pokedexNumber).Name}";

            // Spawn the pokemon mesh
            Instantiate(PocketMonsterManager.Instance.GetPocketMonsterMesh(pokedexNumber), pokemonWrapper.transform);

            // Set up the WildPocketMonster reference
            pokemonWrapper.GetComponent<WildPocketMonster>().SetPokemon(PocketMonsterManager.Instance.GetPocketMonster(pokedexNumber));
        }
    }
}
