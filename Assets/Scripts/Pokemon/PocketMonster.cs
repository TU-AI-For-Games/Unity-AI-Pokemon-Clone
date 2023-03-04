using System;
using UnityEngine;
using static Move;
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

    public enum StatusType
    {
        None,
        Asleep,
        Burned,
        Frozen,
        Paralyzed,
        Poisoned
    }

    public Element Type { get; }

    public string Name { get; }

    public int ID { get; }

    //private Ability m_ability;

    private Stats m_stats;

    private Move[] m_moves;

    private GameObject m_model;

    private Move m_chosenMoveThisTurn;

    private StatusType m_status = StatusType.None;
    private int m_statusTurnCount;

    private bool m_isConfused;
    private int m_confusedTurnCount;

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

    public void HandleStatus()
    {
        if (m_status == StatusType.None)
        {
            m_statusTurnCount = 0;
            return;
        }

        // Info from https://bulbapedia.bulbagarden.net/wiki/Status_condition
        m_statusTurnCount++;

        if (m_status == StatusType.Asleep)
        {
            switch (m_statusTurnCount)
            {
                // You wake up after a max of 5 turns
                case 5:
                // Or randomly if we've had more than 3 turns asleep
                case < 3 when Random.Range(1, 8) == m_statusTurnCount:
                    BattleManager.Instance.OnEndStatusMessage(this, m_status);
                    m_status = StatusType.None;
                    break;
            }
        }
        else if (m_status == StatusType.Burned)
        {
            {
                // Burn does 1/16 of your total health each turn
                int damage = m_stats.BaseHP / 16;
                m_stats.HP -= damage;

                BattleManager.Instance.OnEndStatusMessage(this, m_status);
            }
        }
        else if (m_status == StatusType.Frozen)
        {
            // There's a 10% chance of thawing out each turn
            int random = Random.Range(0, 100);
            if (random < 10)
            {
                BattleManager.Instance.OnEndStatusMessage(this, m_status);
                m_status = StatusType.None;
            }
        }
        else if (m_status == StatusType.Poisoned)
        {
            {
                // Poison takes 1/8 of the health each turn
                int damage = m_stats.BaseHP / 8;
                m_stats.HP -= damage;

                BattleManager.Instance.OnEndStatusMessage(this, m_status);
            }
        }

        CheckIfFainted();

        if (!m_isConfused)
        {
            return;
        }

        m_confusedTurnCount++;

        if (m_confusedTurnCount >= 3 || m_confusedTurnCount == Random.Range(1, 4))
        {
            BattleManager.Instance.OnSnapOut(this);
            m_isConfused = false;
        }
    }

    private void CheckIfFainted()
    {
        if (m_stats.HP <= 0)
        {
            BattleManager.Instance.OnFaint(this);
        }
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

        if (m_stats.HP <= 0 || move.Status == StatusEffect.None)
        {
            return true;
        }

        // If we are still alive and there is a status condition, apply it here
        if (m_status == StatusType.None)
        {
            int randomChance = Random.Range(0, 100);
            if (randomChance < move.StatusEffectChance)
            {
                ApplyStatus(move.Status);
            }
        }

        if (!m_isConfused && move.Status == Move.StatusEffect.Confuse)
        {
            m_isConfused = true;
        }

        return true;
    }

    private void ApplyStatus(StatusEffect effect)
    {
        if (effect == StatusEffect.Burn)
        {
            m_status = StatusType.Burned;
        }
        else if (effect == StatusEffect.Freeze)
        {
            m_status = StatusType.Frozen;
        }
        else if (effect == StatusEffect.Paralyze)
        {
            m_status = StatusType.Paralyzed;
        }
        else if (effect == StatusEffect.Poison)
        {
            m_status = StatusType.Poisoned;
        }
        else if (effect == StatusEffect.Sleep)
        {
            m_status = StatusType.Asleep;
        }
        else if (effect == StatusEffect.TriAttack)
        {
            StatusType[] options =
            {
                StatusType.Burned, StatusType.Frozen, StatusType.Paralyzed
            };

            m_status = options[Random.Range(0, options.Length)];
        }

        BattleManager.Instance.OnApplyStatusMessage(this, m_status);
    }

    public void ChooseRandomMove()
    {
        m_chosenMoveThisTurn = m_moves[Random.Range(0, 4)];
    }

    public StatusType GetStatusEffect()
    {
        return m_status;
    }

    public bool IsConfused()
    {
        return m_isConfused;
    }

    public void DealConfusionDamage()
    {
        // Deal 1/8 of total health
        int damage = m_stats.BaseHP / 8;
        m_stats.HP -= damage;
    }

    public bool HasFainted()
    {
        return m_stats.HP < 0;
    }

    public void ResetStats()
    {
        m_stats.ResetStats();
        m_isConfused = false;
    }
}
