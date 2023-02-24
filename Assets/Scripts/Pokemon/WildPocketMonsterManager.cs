using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildPocketMonsterManager : Singleton<WildPocketMonsterManager>
{
    [SerializeField] private GameObject m_wildPocketMonsterTemplate;

    [SerializeField] private float m_spawnTime = 3f;
    private float m_spawnCountdown;

    [SerializeField] private Transform m_bottomLeft;
    [SerializeField] private Transform m_topRight;

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

        if(m_spawnCountdown < 0f)
        {
            m_spawnCountdown = m_spawnTime;
            SpawnPokemon();
        }
    }

    public void SpawnPokemon()
    {
        // TODO: Use an object pooling system
        GameObject pokemonWrapper = Instantiate(m_wildPocketMonsterTemplate, GenerateRandomPosition(), Quaternion.identity);

        int randomId = Random.Range(1, PocketMonsterManager.Instance.GetPocketMonsterCount());

        // Spawn the pokemon
        Instantiate(PocketMonsterManager.Instance.GetPocketMonsterMesh(randomId), pokemonWrapper.transform);
    }

    private Vector3 GenerateRandomPosition()
    {
        return new Vector3(
            Random.Range(m_bottomLeft.position.x, m_topRight.position.x),
            Random.Range(m_bottomLeft.position.y, m_topRight.position.y),
            Random.Range(m_bottomLeft.position.z, m_topRight.position.z)
        );
    }
}
