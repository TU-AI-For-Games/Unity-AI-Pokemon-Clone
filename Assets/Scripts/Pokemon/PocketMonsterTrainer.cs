using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PocketMonsterTrainer : MonoBehaviour
{
    [SerializeField] private int[] m_pokedexIds;
    private readonly List<PocketMonster> m_pokemon = new(6);
    private int m_activePokemonIndex = 0;

    [SerializeField] private float m_battleCoolDown;
    private float m_battleCountDown;

    private void Awake()
    {
        foreach (var dexNo in m_pokedexIds)
        {
            m_pokemon.Add(PocketMonsterManager.Instance.GetPocketMonster(dexNo));
        }
    }

    private void Update()
    {
        m_battleCountDown -= Time.deltaTime;
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

    public bool SwitchPokemon(PocketMonster playerPokemon)
    {
        int chosenIndex = m_activePokemonIndex;
        Move.Effectiveness chosenEffectiveness = Move.Effectiveness.Immune;
        float chosenHealthPercentage = 0f;

        // Swap to the pokemon who is the most effective with the highest health
        for (int i = 0; i < m_pokemon.Count; ++i)
        {
            if (m_pokemon[i].HasFainted())
            {
                continue;
            }

            Move.Effectiveness effectiveness = GameManager.Instance.GetTypeLearner().GetLearnedEffectiveness(m_pokemon[i].Type, playerPokemon.Type).GetEffectiveness();

            if ((int)effectiveness <= (int)chosenEffectiveness)
            {
                continue;
            }

            float healthPercentage = m_pokemon[i].GetStats().HP / (float)m_pokemon[i].GetStats().BaseHP;

            if (healthPercentage >= chosenHealthPercentage)
            {
                chosenIndex = i;
                chosenEffectiveness = effectiveness;
                chosenHealthPercentage = healthPercentage;
            }
        }

        if (chosenIndex != m_activePokemonIndex)
        {
            m_activePokemonIndex = chosenIndex;
            return true;
        }

        return false;
    }

    public BattleManager.Item HealPokemon()
    {
        float healthPercentage = GetActivePokemon().GetStats().HP / (float)GetActivePokemon().GetStats().BaseHP;
        bool hasStatus = GetActivePokemon().GetStatusEffect() != PocketMonster.StatusType.None;

        if (healthPercentage < 0.5f)
        {
            return BattleManager.Item.HealHP;
        }
        else if (hasStatus)
        {
            return BattleManager.Item.HealStatus;
        }

        return BattleManager.Item.None;
    }

    public void ChooseMove(PocketMonster playerPokemon)
    {
        // TODO: Think about using status or stat change moves if available...
        Move[] pokemonMoves = GetActivePokemon().GetMoves();

        Move chosenMove = pokemonMoves[0];
        Move.Effectiveness chosenMoveEffectiveness = Move.Effectiveness.Immune;

        // Find the highest damaging, most effective move
        foreach (Move move in pokemonMoves)
        {
            Move.Effectiveness effectiveness = GameManager.Instance.GetTypeLearner().GetLearnedEffectiveness(move.Type, playerPokemon.Type).GetEffectiveness();

            if ((int)effectiveness > (int)chosenMoveEffectiveness)
            {
                chosenMove = move;
                chosenMoveEffectiveness = effectiveness;
            }
        }

        Debug.Log($"The most effective move chosen is {chosenMove.Name}... It is {chosenMoveEffectiveness} against {playerPokemon.Name}");

        GetActivePokemon().SetChosenMove(chosenMove);
    }

    public bool CanStartBattle()
    {
        return m_battleCountDown < 0f;
    }

    public bool CanStillBattle()
    {
        return m_pokemon.Any(mon => !mon.HasFainted());
    }

    public void StartCoolDown()
    {
        m_battleCountDown = m_battleCoolDown;

        foreach (PocketMonster monster in m_pokemon)
        {
            monster.HealHealthAndStatus();
        }
    }
}
