using UnityEditor;
using UnityEngine;

public class PokemonTest : MonoBehaviour
{
    public PocketMonster CurrentPokemon;

    public void DestroyChildren()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}

[CustomEditor(typeof(PokemonTest))]
public class PokemonTestTool : Editor
{
    [SerializeField] private int m_currentPkdxNo = 1;

    private readonly string[] m_pkmnDetails = new string[10];

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        foreach (var stat in m_pkmnDetails)
        {
            GUILayout.Label(stat);
        }

        if (GUILayout.Button("Previous Pokemon"))
        {
            m_currentPkdxNo--;
            if (m_currentPkdxNo < 1)
            {
                m_currentPkdxNo = PocketMonsterManager.Instance.GetPocketMonsterCount();
            }
            SetPokemonModel();
            UpdateText();
        }

        if (GUILayout.Button("Next Pokemon"))
        {
            m_currentPkdxNo++;
            if (m_currentPkdxNo > PocketMonsterManager.Instance.GetPocketMonsterCount())
            {
                m_currentPkdxNo = 1;
            }

            SetPokemonModel();
            UpdateText();
        }
    }

    private void UpdateText()
    {
        PokemonTest testAsset = (PokemonTest)target;

        m_pkmnDetails[0] = testAsset.CurrentPokemon.Name;
        m_pkmnDetails[1] = PocketMonster.TypeToString(testAsset.CurrentPokemon.Type);
        m_pkmnDetails[2] = testAsset.CurrentPokemon.GetStats().HP.ToString();
        m_pkmnDetails[3] = testAsset.CurrentPokemon.GetStats().GetAttack().ToString();
        m_pkmnDetails[4] = testAsset.CurrentPokemon.GetStats().GetDefense().ToString();
        m_pkmnDetails[5] = testAsset.CurrentPokemon.GetStats().GetSpeed().ToString();

        Move[] moves = testAsset.CurrentPokemon.GetMoves();
        m_pkmnDetails[6] = moves[0].Name;
        m_pkmnDetails[7] = moves[1].Name;
        m_pkmnDetails[8] = moves[2].Name;
        m_pkmnDetails[9] = moves[3].Name;
    }

    private void SetPokemonModel()
    {
        PokemonTest testAsset = (PokemonTest)target;

        testAsset.DestroyChildren();

        testAsset.CurrentPokemon = PocketMonsterManager.Instance.GetPocketMonster(m_currentPkdxNo);

        Instantiate(
            PocketMonsterManager.Instance.GetPocketMonsterMesh(m_currentPkdxNo),
            testAsset.transform
        );
    }
}
