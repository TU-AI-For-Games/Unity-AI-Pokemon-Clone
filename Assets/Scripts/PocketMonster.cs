using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PocketMonster : MonoBehaviour
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

    public Element Type { get; private set; }
    [SerializeField] private Element m_type;

    [System.Serializable]
    public struct Stats
    {
        public float m_attack;
        public float m_defense;
        public float m_hp;
        public float m_speed;
    }

    private Ability m_ability;

    [SerializeField] private Stats m_stats;

    [SerializeField] private Move[] m_moves = new Move[4];


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
