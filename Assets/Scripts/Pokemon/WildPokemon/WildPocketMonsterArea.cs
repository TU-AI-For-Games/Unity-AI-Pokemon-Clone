using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildPocketMonsterArea : MonoBehaviour
{
    public bool DrawDebug;

    [SerializeField] private Transform m_bottomLeft;
    [SerializeField] private Transform m_topRight;
    [SerializeField] private int m_maxPokemon;

    [SerializeField] private List<SpawnChances> m_spawnChances;

    private List<GameObject> m_activePokemon;

    // Start is called before the first frame update
    void Start()
    {
        m_activePokemon = new List<GameObject>(m_maxPokemon);
    }

    public bool IsFull()
    {
        return m_activePokemon.Count == m_maxPokemon;
    }

    public void AddPokemon(GameObject pokemonGameObject)
    {
        m_activePokemon.Add(pokemonGameObject);

        pokemonGameObject.transform.position = GenerateRandomPosition();
    }

    public void RemovePokemon(GameObject pokemonObject)
    {
        m_activePokemon.Remove(pokemonObject);
    }

    public int GetNextPokedexNumber()
    {
        int spawnChance = Random.Range(1, 65);
        bool selectedMon = false;
        int selectedMonIndex = -1;

        while (!selectedMon)
        {
            // Grab a random pokemon from the SpawnChances list
            SpawnChances spawnChances = m_spawnChances[Random.Range(0, m_spawnChances.Count)];
            if (spawnChances.SpawnChance > spawnChance)
            {
                selectedMonIndex = spawnChances.PokedexNumber;
                selectedMon = true;
            }
        }

        return selectedMonIndex;
    }

    [System.Serializable]
    public class SpawnChances
    {
        public int PokedexNumber;
        [Range(0, 100)] public float SpawnChance;
    }

    private Vector3 GenerateRandomPosition()
    {
        return new Vector3(
            Random.Range(m_bottomLeft.position.x, m_topRight.position.x),
            Random.Range(m_bottomLeft.position.y, m_topRight.position.y),
            Random.Range(m_bottomLeft.position.z, m_topRight.position.z)
        );
    }

    private void OnDrawGizmos()
    {
        if (DrawDebug)
        {
            Gizmos.color = Color.magenta;

            float averageHeight = (m_bottomLeft.position.y + m_topRight.position.y) / 2;

            Gizmos.DrawLine(m_bottomLeft.position, new Vector3(m_bottomLeft.position.x, averageHeight, m_topRight.position.z));
            Gizmos.DrawLine(m_bottomLeft.position, new Vector3(m_topRight.position.x, averageHeight, m_bottomLeft.position.z));

            Gizmos.DrawLine(m_topRight.position, new Vector3(m_bottomLeft.position.x, averageHeight, m_topRight.position.z));
            Gizmos.DrawLine(m_topRight.position, new Vector3(m_topRight.position.x, averageHeight, m_bottomLeft.position.z));
        }
    }
}
