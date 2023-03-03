using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Stats
{
    public int HP;
    public readonly int BaseHP;
    private readonly int m_baseAttack;
    private readonly int m_baseDefense;
    private readonly int m_baseSpeed;

    // Stat Modifiers (max of +- 2, reset to 1 per battle)
    private float m_attackModifier;
    private float m_defenseModifier;
    private float m_speedModifier;

    // Reset at the end of every battle
    public float Accuracy;

    public Stats(Stats stats)
    {
        BaseHP = stats.BaseHP;
        HP = stats.HP;
        m_baseAttack = stats.m_baseAttack;
        m_baseDefense = stats.m_baseDefense;
        m_baseSpeed = stats.m_baseSpeed;

        m_attackModifier = 0f;
        m_defenseModifier = 0f;
        m_speedModifier = 0f;
    }

    public Stats(int hp, int attack, int defense, int speed)
    {
        // Calculate the stats at level 50 using the incoming stats
        // Formulae taken from https://www.serebii.net/rb/evtraining.shtml 
        int newHp = CalculateHpStat(hp, 15, Random.Range(1, 256), 50);
        BaseHP = newHp;
        HP = newHp;

        m_baseAttack = CalculateStat(attack, 15, Random.Range(1, 256), 50);
        m_baseDefense = CalculateStat(defense, 15, Random.Range(1, 256), 50);
        m_baseSpeed = CalculateStat(speed, 15, Random.Range(1, 256), 50);

        m_attackModifier = 0f;
        m_defenseModifier = 0f;
        m_speedModifier = 0f;
    }

    public void Print()
    {
        Debug.Log($"HP: {HP}\tATK: {m_baseAttack}\tDEF: {m_baseDefense}\tSPD: {m_baseSpeed}");
    }

    public int GetAttack()
    {
        return GetModifiedStat(m_baseAttack, m_attackModifier);
    }

    public bool IncreaseAttack()
    {
        return IncreaseStat(ref m_attackModifier);
    }

    public bool DecreaseAttack()
    {
        return DecreaseStat(ref m_attackModifier);
    }

    public int GetDefense()
    {
        return GetModifiedStat(m_baseDefense, m_defenseModifier);
    }

    public bool IncreaseDefense()
    {
        return IncreaseStat(ref m_defenseModifier);
    }

    public bool DecreaseDefense()
    {
        return DecreaseStat(ref m_defenseModifier);
    }

    public int GetSpeed()
    {
        return GetModifiedStat(m_baseSpeed, m_speedModifier);
    }

    public int GetBaseSpeed()
    {
        return m_baseSpeed;
    }

    public bool IncreaseSpeed()
    {
        return IncreaseStat(ref m_speedModifier);
    }

    public bool DecreaseSpeed()
    {
        return DecreaseStat(ref m_speedModifier);
    }    

    private int GetModifiedStat(int baseStat, float modifier)
    {
        return baseStat + (int)Mathf.Floor(modifier * baseStat);
    }

    private bool IncreaseStat(ref float modifier)
    {
        if (modifier < 2f)
        {
            modifier += 0.5f;
            return true;
        }

        return false;
    }

    private bool DecreaseStat(ref float modifier)
    {
        if(modifier > -2f)
        {
            modifier -= 0.5f;
            return true;
        }

        return false;
    }

    private int CalculateHpStat(int baseHp, int iv, int ev, int level)
    {
        // HP = floor((((Base Stat + IV) * 2 + (?(EV) / 4))*Level)/ 100)+Level + 10
        return (int)Math.Floor(((baseHp + iv) * 2 + MathF.Sqrt(ev) / 4) * level / 100) + level + 10;
    }

    private int CalculateStat(int baseStat, int iv, int ev, int level)
    {
        // Others = floor((((Base Stat+IV)*2+(?(EV)/4))*Level)/100)+ 5
        return (int)Math.Floor(((baseStat + iv) * 2 + MathF.Sqrt(ev) / 4) * level / 100) + 5;
    }

}
