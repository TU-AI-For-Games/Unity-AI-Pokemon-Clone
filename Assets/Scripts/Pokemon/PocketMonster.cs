using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class PocketMonster
{
    public enum Element
    {
        Bug,
        Dark,
        Dragon,
        Electric,
        Fire,
        Fighting,
        Flying,
        Ghost,
        Grass,
        Ground,
        Ice,
        Normal,
        Poison,
        Psychic,
        Rock,
        Steel,
        Water
    }

    public Element Type { get; }

    public string Name { get; }

    public int ID { get; }

    //private Ability m_ability;

    private Stats m_stats;

    private Move[] m_moves;

    private GameObject m_model;

    private Move m_chosenMoveThisTurn;

    public PocketMonster(PocketMonster monster)
    {
        ID = monster.ID;
        Name = monster.Name;
        Type = monster.Type;
        m_stats = new Stats(monster.m_stats);
        m_moves = monster.m_moves;
    }

    public PocketMonster(int id, string name, Element type, /*Ability ability, */ Stats stats, Move[] moves)
    {
        ID = id;
        Name = name;
        Type = type;
        //m_ability = ability;
        m_stats = stats;
        m_moves = moves;
    }

    public void SetMesh(GameObject model)
    {
        m_model = model;
    }

    public static Element StringToType(string type)
    {
        return type switch
        {
            "Bug" => Element.Bug,
            "Dark" => Element.Dark,
            "Dragon" => Element.Dragon,
            "Electric" => Element.Electric,
            "Fighting" => Element.Fighting,
            "Fire" => Element.Fire,
            "Flying" => Element.Flying,
            "Ghost" => Element.Ghost,
            "Grass" => Element.Grass,
            "Ground" => Element.Ground,
            "Ice" => Element.Ice,
            "Typeless" or "Normal" => Element.Normal,
            "Poison" => Element.Poison,
            "Psychic" => Element.Psychic,
            "Rock" => Element.Rock,
            "Steel" => Element.Steel,
            "Water" => Element.Water,
            _ => throw new ArgumentOutOfRangeException(type, "Make sure the type is supported!")
        };
    }

    public static string TypeToString(Element type)
    {
        return type switch
        {
            Element.Bug => "Bug",
            Element.Dark => "Dark",
            Element.Dragon => "Dragon",
            Element.Electric => "Electric",
            Element.Fire => "Fire",
            Element.Fighting => "Fighting",
            Element.Flying => "Flying",
            Element.Ghost => "Ghost",
            Element.Grass => "Grass",
            Element.Ground => "Ground",
            Element.Ice => "Ice",
            Element.Normal => "Normal",
            Element.Poison => "Poison",
            Element.Psychic => "Psychic",
            Element.Rock => "Rock",
            Element.Steel => "Steel",
            Element.Water => "Water",
            _ => throw new ArgumentOutOfRangeException(type.ToString(), "Make sure the type is supported!")
        };
    }

    public void Print()
    {
        Debug.Log($"Name: {Name}");
        Debug.Log($"Type: {TypeToString(Type)}");
        m_stats.Print();
        foreach (Move move in m_moves)
        {
            move.Print();
        }
    }

    public Stats GetStats()
    {
        return m_stats;
    }

    // Whenever a pokemon is switched in or a battle starts, the accuracy is set to 1
    public void ResetAccuracy()
    {
        m_stats.Accuracy = 1f;
    }

    public Move[] GetMoves()
    {
        return m_moves;
    }

    public void SetChosenMove(Move move)
    {
        m_chosenMoveThisTurn = move;
    }

    public Move GetChosenMove()
    {
        return m_chosenMoveThisTurn;
    }


    // Returns true if the attack hits, false if not
    public bool TakeDamage(PocketMonster attacker, Move move, out Move.Effectiveness effectiveness, bool isCrit)
    {
        // According to https://bulbapedia.bulbagarden.net/wiki/Accuracy#Generation_I_and_II a move misses if the accuracy formula is more than the random number
        int accuracy = (int)(move.Accuracy * attacker.m_stats.Accuracy);
        int randomNum = Random.Range(1, 100);

        // if R is strictly less than A, the move hits, otherwise it misses
        if (randomNum > accuracy)
        {
            Debug.Log("MISS!");
            effectiveness = Move.Effectiveness.Neutral;
            return false;
        }

        // Calculate the damage using the formula from the attacker according to https://bulbapedia.bulbagarden.net/wiki/Damage 
        int criticalMod = isCrit ? 2 : 1;

        // TODO: Maybe introduce levels?
        // Assuming every pokemon is level 50 for now for ease of use...
        int level = 50;
        int levelCritical = (2 * level * criticalMod / 5) + 2;
        float attackDefRatio = attacker.GetStats().GetAttack() / (float)m_stats.GetDefense();
        float fraction = (levelCritical * move.Damage * attackDefRatio / 50) + 2;

        float sameTypeAttackBonus = move.Type == attacker.Type ? 1.5f : 1f;

        float typeMultiplier = BattleManager.Instance.GetTypeAdvantageMultiplier(move.Type, Type);

        effectiveness = typeMultiplier switch
        {
            2f => Move.Effectiveness.SuperEffective,
            0.5f => Move.Effectiveness.NotVeryEffective,
            0f => Move.Effectiveness.Immune,
            _ => Move.Effectiveness.Neutral
        };

        // random is realized as a multiplication by a random uniformly distributed integer between 217 and 255 (inclusive), followed by division by 255. If the calculated damage thus far is 1, random is always 1
        float damageSoFar = fraction * sameTypeAttackBonus * typeMultiplier;

        float random = (int)damageSoFar == 1 ? 1f : Random.Range(217, 255) / 255f;

        Debug.Log($"DEALING {(int)damageSoFar * random} DAMAGE");

        m_stats.HP -= (int)(damageSoFar * random);

        return true;
    }

    public void ChooseRandomMove()
    {
        m_chosenMoveThisTurn = m_moves[Random.Range(0, 4)];
    }
}
