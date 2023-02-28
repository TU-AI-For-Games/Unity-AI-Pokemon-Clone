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

    [SerializeField] private BattleUI m_battleHUD;

    private Queue<string> m_battleMessages;

    public enum BattleState
    {
        SelectMove,
        Attack,
        BattleInfo,
        PlayerFainted,
        End
    }

    private BattleState m_battleState;

    public enum BattleType
    {
        None,
        WildPkmn,
        Trainer
    }

    private BattleType m_currentBattleType;

    private bool m_aiChosenThisTurn = false;

    // Start is called before the first frame update
    void Start()
    {
        m_battleMessages = new Queue<string>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_battleState)
        {
            case BattleState.SelectMove:
                {
                    // TODO: For now, the AI is just choosing a random move, of course this will be more sophisticated when Jay has done the decision making
                    if (!m_aiChosenThisTurn && m_currentBattleType == BattleType.WildPkmn)
                    {
                        m_otherPokemon.ChooseRandomMove();
                        m_aiChosenThisTurn = true;
                    }

                    break;
                }
            case BattleState.Attack:
                {
                    // We haven't attacked yet if there are no messages in the queue
                    if (m_battleMessages.Count == 0)
                    {
                        AttackState();
                        m_battleState = BattleState.BattleInfo;
                    }

                    break;
                }
            case BattleState.BattleInfo:
                {
                    // When the messages have all been consumed, we can move to the next state
                    if (m_battleMessages.Count == 0)
                    {
                        m_battleState = BattleState.SelectMove;
                        m_battleHUD.ShowChoiceUI();
                        m_aiChosenThisTurn = false;
                    }

                    break;
                }
            case BattleState.PlayerFainted:
                break;
            case BattleState.End:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(m_battleState), m_battleState, null);
        }
    }

    private void AttackState()
    {
        // Determine which pokemon is going first based on speed
        PocketMonster firstMon;
        PocketMonster secondMon;

        if (m_playerPokemon.GetStats().Speed < m_otherPokemon.GetStats().Speed)
        {
            firstMon = m_otherPokemon;
            secondMon = m_playerPokemon;
        }
        else
        {
            firstMon = m_playerPokemon;
            secondMon = m_otherPokemon;
        }

        HandleMove(firstMon, secondMon);
        HandleMove(secondMon, firstMon);

        m_battleHUD.ShowBattleInfoUI();
    }

    private void HandleMove(PocketMonster attacker, PocketMonster target)
    {
        Move.Outcome outcome = Attack(attacker.GetChosenMove(), attacker, target, out Move.Effectiveness effectiveness);

        m_battleMessages.Enqueue(
            GenerateOutcomeString(
                    attacker.Name,
                    target.Name,
                    attacker.GetChosenMove().Name,
                    outcome
            )
            );

        if (effectiveness != Move.Effectiveness.Neutral)
        {
            m_battleMessages.Enqueue(GenerateEffectivenessString(effectiveness));
        }

        // If the target died this turn, then we want to add that message to the queue
        if (target.GetStats().HP < 0)
        {
            m_battleMessages.Enqueue($"{target.Name} fainted...");

            OnFaint(target);
        }
    }

    private void OnFaint(PocketMonster pokemon)
    {
        Debug.Log($"{pokemon.Name.ToUpper()} FAINTED");
    }

    public void SetBattleState(BattleState state)
    {
        m_battleState = state;
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

    public string ConsumeNextMessage()
    {
        if (m_battleMessages.Count != 0)
        {
            return m_battleMessages.Dequeue();
        }

        return null;
    }

    private Move.Outcome Attack(Move move, PocketMonster attacker, PocketMonster target, out Move.Effectiveness effectiveness)
    {
        switch (move.MoveEffect)
        {
            case Move.Effect.Damage:
                return DealDamage(move, attacker, target, out effectiveness);
            case Move.Effect.Heal:
                break;
            case Move.Effect.IncreaseAttack:
                break;
            case Move.Effect.DecreaseAttack:
                break;
            case Move.Effect.IncreaseAccuracy:
                break;
            case Move.Effect.DecreaseAccuracy:
                break;
            case Move.Effect.IncreaseDefense:
                break;
            case Move.Effect.DecreaseDefense:
                break;
            case Move.Effect.IncreaseSpeed:
                break;
            case Move.Effect.DecreaseSpeed:
                break;
            case Move.Effect.Status:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        effectiveness = Move.Effectiveness.Immune;
        return Move.Outcome.Miss;
    }

    private Move.Outcome DealDamage(Move move, PocketMonster attacker, PocketMonster target, out Move.Effectiveness effectiveness)
    {
        Move.Outcome outcome = Move.Outcome.Hit;

        bool isCrit = IsCriticalHit(attacker);

        if (isCrit)
        {
            outcome = Move.Outcome.CriticalHit;
        }

        bool hit = target.TakeDamage(attacker, move, out effectiveness, isCrit);

        if (!hit)
        {
            outcome = Move.Outcome.Miss;
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

    // TODO: Ideally this would come from a stringtable but I really cba...
    private string GenerateOutcomeString(string attackerName, string targetName, string moveName, Move.Outcome outcome)
    {
        string outcomeString = $"{attackerName} used {moveName} on {targetName}...\n";

        if (outcome == Move.Outcome.CriticalHit)
        {
            outcomeString += "It was a critical hit!";
        }
        else if (outcome == Move.Outcome.Miss)
        {
            outcomeString += "The attack missed";
        }

        return outcomeString;
    }

    private string GenerateEffectivenessString(Move.Effectiveness effectiveness)
    {
        if (effectiveness == Move.Effectiveness.Immune)
        {
            return "It had no effect...";
        }
        if (effectiveness == Move.Effectiveness.NotVeryEffective)
        {
            return "It was not very effective...";
        }
        if (effectiveness == Move.Effectiveness.SuperEffective)
        {
            return "It was super effective!";
        }

        return "";
    }

    public void SetBattleType(BattleType type)
    {
        m_currentBattleType = type;
    }

    public BattleType GetBattleType()
    {
        return m_currentBattleType;
    }
}
