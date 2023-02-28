using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleManager : Singleton<BattleManager>
{
    // The type matchup table, 2 if super-effective, 0.5 if not-very effective, 0 if immune, 1 if normal
    private Dictionary<PocketMonster.Element, Dictionary<PocketMonster.Element, float>> m_typeMatchupTable;

    private PocketMonster m_playerPokemon;
    private PocketMonster m_otherPokemon;

    public enum MoveOutcome
    {
        Hit,
        CriticalHit,
        Miss
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPlayerPokemon(PocketMonster pokemon)
    {
        m_playerPokemon = pokemon;
        m_playerPokemon.ResetAccuracy();
    }

    public void SetOtherPokemon(PocketMonster pokemon)
    {
        m_otherPokemon = pokemon;
        m_otherPokemon.ResetAccuracy();
    }

    public MoveOutcome PlayerAttack(int moveID)
    {
        Move move = m_playerPokemon.GetMoves()[moveID];

        return Attack(move, m_playerPokemon, m_otherPokemon);
    }

    private MoveOutcome Attack(Move move, PocketMonster attacker, PocketMonster target)
    {
        switch (move.Effect)
        {
            case Move.MoveEffect.Damage:
                return DealDamage(move, attacker, target);
            case Move.MoveEffect.Heal:
                break;
            case Move.MoveEffect.IncreaseAttack:
                break;
            case Move.MoveEffect.DecreaseAttack:
                break;
            case Move.MoveEffect.IncreaseAccuracy:
                break;
            case Move.MoveEffect.DecreaseAccuracy:
                break;
            case Move.MoveEffect.IncreaseDefense:
                break;
            case Move.MoveEffect.DecreaseDefense:
                break;
            case Move.MoveEffect.IncreaseSpeed:
                break;
            case Move.MoveEffect.DecreaseSpeed:
                break;
            case Move.MoveEffect.Status:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return MoveOutcome.Miss;
    }

    private MoveOutcome DealDamage(Move move, PocketMonster attacker, PocketMonster target)
    {
        MoveOutcome outcome = MoveOutcome.Hit;

        bool isCrit = IsCriticalHit(attacker);

        if (isCrit)
        {
            outcome = MoveOutcome.CriticalHit;
        }

        bool hit = target.TakeDamage(attacker, move, isCrit);

        if (!hit)
        {
            outcome = MoveOutcome.Miss;
        }

        return outcome;
    }

    private bool IsCriticalHit(PocketMonster mon)
    {
        // In Pokemon, it is a critical hit if the random number generated between 1 and 255 is bigger than base speed / 2 rounded down
        int baseSpeedOverTwo = mon.GetStats().Speed / 2;

        int randomNumber = Random.Range(1, 255);

        return baseSpeedOverTwo > randomNumber;
    }

    public float GetTypeAdvantageMultiplier(PocketMonster.Element moveType, PocketMonster.Element targetType)
    {
        // If the types aren't in the table, the multiplier is 1
        if (!m_typeMatchupTable[moveType].ContainsKey(targetType))
        {
            return 1f;
        }

        // Else, we wanna just return the value from the table
        return m_typeMatchupTable[moveType][targetType];
    }

    protected override void InternalInit()
    {
        m_typeMatchupTable = new Dictionary<PocketMonster.Element, Dictionary<PocketMonster.Element, float>>();

        TextAsset movesFile = (TextAsset)Resources.Load("Data\\typeAdvantages");
        string[] linesFromFile = movesFile.text.Split('\n');

        for (int i = 1; i < linesFromFile.Length; i++)
        {
            string[] lineContents = linesFromFile[i].Split(',');

            PocketMonster.Element type = PocketMonster.StringToType(lineContents[1]);
            if (!m_typeMatchupTable.ContainsKey(type))
            {
                m_typeMatchupTable.Add(type, new Dictionary<PocketMonster.Element, float>());
            }

            PocketMonster.Element otherType = PocketMonster.StringToType(lineContents[2]);
            float typeMultiplier = float.Parse(lineContents[3]);

            m_typeMatchupTable[type].Add(otherType, typeMultiplier);
        }
    }

    public PocketMonster GetPlayerPokemon()
    {
        return m_playerPokemon;
    }

    public PocketMonster GetOtherPokemon()
    {
        return m_otherPokemon;
    }
}
