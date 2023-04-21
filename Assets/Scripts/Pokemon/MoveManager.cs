
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MoveManager : Singleton<MoveManager>
{
    private Dictionary<int, Move> m_moves;

    protected override void InternalInit()
    {
        m_moves = new Dictionary<int, Move>();

        LoadMoves();
    }

    private void LoadMoves()
    {
        TextAsset movesFile = (TextAsset)Resources.Load("Data\\moves");
        string[] linesFromFile = movesFile.text.Split('\n');

        for (int i = 1; i < linesFromFile.Length; i++)
        {
            string[] csvContent = linesFromFile[i].Split(',');

            int moveID = Convert.ToInt32(csvContent[0]);

            // Reordered to be in the same order as the Move() constructor... I know it should probably consistent with the CSV but hey - Tom
            string moveName = csvContent[1];

            string description = csvContent[2];

            PocketMonster.Element type = PocketMonster.StringToType(csvContent[3]);

            int damage = Convert.ToInt32(csvContent[5]);
            int accuracy = PercentToInt(csvContent[6]);

            Move.Effect effect = StringToEffect(csvContent[4]);

            // If the move was a status move, we want to work out which status it is
            Move.StatusEffect statusEffect = Move.StatusEffect.None;
            int statusChance = 0;
            if (effect == Move.Effect.Status)
            {
                statusEffect = StringToStatusEffect(csvContent[7]);
                statusChance = PercentToInt(csvContent[8]);
            }


            Move.StatChangeAffected statChange = Move.StatChangeAffected.None;
            int statChangeChance = 0;

            // If it's not a damaging move only, it will affect a stat in some way
            if (effect != Move.Effect.Damage && effect != Move.Effect.Status)
            {
                // Work out who the stat will affect
                statChange = StringToStatChangeAffected(csvContent[9]);
                statChangeChance = PercentToInt(csvContent[10]);
            }


            m_moves.Add(moveID, new Move(moveName, description, type, effect, statusEffect, statusChance, damage, accuracy, statChange, statChangeChance));
        }
    }

    public Move GetMove(int moveID)
    {
        return m_moves[moveID];
    }

    public int GetMoveID(Move move)
    {
        foreach (KeyValuePair<int, Move> pair in m_moves)
        {
            if (move == pair.Value)
            {
                return pair.Key;
            }
        }

        return -1;
    }

    private int PercentToInt(string percentString)
    {
        if (percentString is null or "\r" or "\n")
        {
            return 0;
        }

        // Read up to the percentage sign
        int index = percentString.IndexOf('%');
        if (index > 0)
        {
            return Convert.ToInt32(percentString[..index]);
        }

        if (percentString.Length == 0)
        {
            return 100;
        }

        throw new ArithmeticException($"Unable to convert from {percentString} to int");
    }

    public static Move.Effect StringToEffect(string effect)
    {
        return effect switch
        {
            "Physical" => Move.Effect.Damage,
            "Special" => Move.Effect.Damage,
            "a+" => Move.Effect.IncreaseAttack,
            "sa+" => Move.Effect.IncreaseAttack,
            "a-" => Move.Effect.DecreaseAttack,
            "sa-" => Move.Effect.DecreaseAttack,
            "acc+" => Move.Effect.IncreaseAccuracy,
            "acc-" => Move.Effect.DecreaseAccuracy,
            "all+" => Move.Effect.RaiseAllStats,
            "d+" => Move.Effect.IncreaseDefense,
            "sd+" => Move.Effect.IncreaseDefense,
            "d-" => Move.Effect.DecreaseDefense,
            "sd-" => Move.Effect.DecreaseDefense,
            "hp+" => Move.Effect.Heal,
            "s+" => Move.Effect.IncreaseSpeed,
            "s-" => Move.Effect.DecreaseSpeed,
            "Status" => Move.Effect.Status,
            _ => throw new ArgumentOutOfRangeException(effect, "Ensure the moveEffect is supported")
        };
    }

    public static string EffectToString(Move.Effect effect)
    {
        return effect switch
        {
            Move.Effect.Damage => "DMG",
            Move.Effect.Heal => "HP+",
            Move.Effect.IncreaseAttack => "ATTK+",
            Move.Effect.DecreaseAttack => "ATTK-",
            Move.Effect.IncreaseAccuracy => "ACC+",
            Move.Effect.DecreaseAccuracy => "ACC-",
            Move.Effect.IncreaseDefense => "DEF+",
            Move.Effect.DecreaseDefense => "DEF-",
            Move.Effect.IncreaseSpeed => "SPD+",
            Move.Effect.DecreaseSpeed => "SPD-",
            Move.Effect.Status => "STATUS",
            Move.Effect.RaiseAllStats => "ALL+",
            _ => throw new ArgumentOutOfRangeException(nameof(effect), effect, null)
        };
    }

    public static Move.StatusEffect StringToStatusEffect(string statusEffect)
    {
        return statusEffect switch
        {
            "BURN" => Move.StatusEffect.Burn,
            "CONFUSE" => Move.StatusEffect.Confuse,
            "FLINCH" => Move.StatusEffect.Flinch,
            "FREEZE" => Move.StatusEffect.Freeze,
            "PARALYZE" => Move.StatusEffect.Paralyze,
            "POISON" => Move.StatusEffect.Poison,
            "SLEEP" => Move.StatusEffect.Sleep,
            "TRIATTACK" => Move.StatusEffect.TriAttack,
            _ => Move.StatusEffect.None
        };
    }

    public static Move.StatChangeAffected StringToStatChangeAffected(string affectedPerson)
    {
        return affectedPerson switch
        {
            "U" => Move.StatChangeAffected.User,
            "T" => Move.StatChangeAffected.Target,
            _ => Move.StatChangeAffected.None
        };
    }
}
