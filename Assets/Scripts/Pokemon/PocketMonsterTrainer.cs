using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocketMonsterTrainer : MonoBehaviour
{
    [SerializeField] private int[] m_pokedexIds;
    private readonly List<PocketMonster> m_pokemon = new(6);
    private int m_activePokemonIndex = 0;

    private void Awake()
    {
        foreach (var dexNo in m_pokedexIds)
        {
            m_pokemon.Add(PocketMonsterManager.Instance.GetPocketMonster(dexNo));
        }
    }

    public PocketMonster GetActivePokemon()
    {
        return m_pokemon[m_activePokemonIndex];
    }

    public MoveDecisionLearner.Action ChooseAction(PocketMonster playerMon)
    {
        // Make a decision about whether to attack, heal or switch
        float healthPercentage = GetActivePokemon().GetStats().HP / (float)GetActivePokemon().GetStats().BaseHP;
        float playerHealthPercentage = playerMon.GetStats().HP / (float)playerMon.GetStats().BaseHP;

        return GameManager.Instance.GetMoveDecisionLearner().GetLearnedMoveOutcome(
            healthPercentage,
            GetActivePokemon().Type,
            playerHealthPercentage,
            playerMon.Type
        );
    }

    public void SwitchPokemon(PocketMonster playerPokemon)
    {

    }

    public void HealPokemon()
    {
    }
}
