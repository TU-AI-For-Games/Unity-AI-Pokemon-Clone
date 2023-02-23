using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PocketMonsterManager : Singleton<PocketMonsterManager>
{
    [Header("Please make sure this is in the order of the pokedex!")]
    [SerializeField] private List<GameObject> m_models;

    private Dictionary<int, PocketMonster> m_pocketMonsters;

    protected override void InternalInit()
    {
        m_pocketMonsters = new Dictionary<int, PocketMonster>();

        LoadPkmn();

        // Set up the pokemon's models
        foreach (KeyValuePair<int, PocketMonster> dictPair in m_pocketMonsters)
        {
            dictPair.Value.SetMesh(m_models[dictPair.Key - 1]);
        }
    }

    public PocketMonster GetPocketMonster(int id)
    {
        return m_pocketMonsters[id];
    }

    public GameObject GetPocketMonsterMesh(int id)
    {
        return m_models[id - 1];
    }

    private void LoadPkmn()
    {
        TextAsset movesFile = (TextAsset)Resources.Load("Data\\pokemon");
        string[] linesFromFile = movesFile.text.Split('\n');

        for (int i = 1; i < linesFromFile.Length; i++)
        {
            // Read the line and build a PocketMonster object
            string[] data = linesFromFile[i].Split(',');

            int nationalDexNo = int.Parse(data[0]);

            string pkmnName = data[1];
            PocketMonster.Element type = PocketMonster.StringToType(data[2]);

            PocketMonster.Stats stats = new PocketMonster.Stats(
                float.Parse(data[3]),
                float.Parse(data[4]),
                float.Parse(data[5]),
                float.Parse(data[6])
            );

            Move[] moves =
            {
                MoveManager.Instance.GetMove(int.Parse(data[7])),
                MoveManager.Instance.GetMove(int.Parse(data[8])),
                MoveManager.Instance.GetMove(int.Parse(data[9])),
                MoveManager.Instance.GetMove(int.Parse(data[10]))
            };

            m_pocketMonsters.Add(nationalDexNo, new PocketMonster(pkmnName, type, stats, moves));
        }

    }

    public void PrintPokemon(int pkdexNo)
    {
        m_pocketMonsters[pkdexNo].Print();
    }

    public int GetPocketMonsterCount()
    {
        return m_pocketMonsters.Count;
    }
}
