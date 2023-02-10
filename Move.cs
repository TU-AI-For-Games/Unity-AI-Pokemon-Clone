using System;


[System.Serializable]
public abstract class Move
{
    protected string m_name;
    protected string m_description;
    protected PocketMonster.Element m_type;
    protected float m_damage;
    protected float m_accuracy;

    public enum OutcomeType
    {
        Hit,
        CriticalHit,
        Miss
    }

    protected abstract OutcomeType Outcome(PocketMonster enemyMon);

}
