using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
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

    public bool AllMessagesConsumed = false;
    private bool m_inBattle = false;
    private bool m_aiChosenThisTurn = false;
    private bool m_battleEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        m_battleMessages = new Queue<string>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: This should probably be properly implemented and event-driven, but that's what refactoring is for! 
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
                        if (m_battleState != BattleState.PlayerFainted)
                        {
                            SetBattleState(BattleState.BattleInfo);
                        }
                    }

                    break;
                }
            case BattleState.BattleInfo:
                {
                    // When the messages have all been consumed, we can move to the next state
                    if (m_battleHUD.DisplayedAllMessages())
                    {
                        Debug.Log("Nothing more to show...");
                    }

                    break;
                }
            case BattleState.PlayerFainted:
                {
                    if (m_battleHUD.DisplayedAllMessages())
                    {
                        m_battleHUD.ShowChoosePkmnMenu();
                    }
                }
                break;
            case BattleState.End:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(m_battleState), m_battleState, null);
        }

        // If the battle has finished and we have no more messages to read on screen, go back to the overworld
        if (m_inBattle && m_battleEnded && m_battleHUD.DisplayedAllMessages())
        {
            GameManager.Instance.EndBattle(false);
            m_inBattle = false;
        }
    }


    public void InitialiseBattle(BattleType type)
    {
        m_currentBattleType = type;
        m_aiChosenThisTurn = false;
        m_inBattle = true;
        m_battleEnded = false;
    }

    private void AttackState()
    {
        // Determine which pokemon is going first based on speed
        PocketMonster firstMon;
        PocketMonster secondMon;

        if (m_playerPokemon.GetStats().GetSpeed() < m_otherPokemon.GetStats().GetSpeed())
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

        // If the target died this turn, then we want to add that message to the queue

        if (CheckIfFainted(secondMon))
        {
            OnFaint(secondMon, secondMon == m_playerPokemon);
        }

        HandleMove(secondMon, firstMon);

        if (CheckIfFainted(firstMon))
        {
            OnFaint(firstMon, firstMon == m_playerPokemon);
        }

        m_battleHUD.ShowBattleInfoUI();
    }

    private bool CheckIfFainted(PocketMonster pokemon)
    {
        return pokemon.GetStats().HP < 0;
    }

    private void HandleMove(PocketMonster attacker, PocketMonster target)
    {
        if (attacker.GetStats().HP <= 0)
        {
            // Don't attack if we fainted!
            return;
        }

        Move.Outcome outcome = Move.Outcome.Hit;
        Move.Effectiveness effectiveness = Move.Effectiveness.Neutral;

        Move chosenMove = attacker.GetChosenMove();

        bool statChange = false;

        if (target.GetStats().HP <= 0)
        {
            // Allow the user to use a stat move if the target has fainted and they are using the move on themself
            if (chosenMove.MoveEffect != Move.Effect.Damage && chosenMove.MoveEffect != Move.Effect.Status && chosenMove.AffectedStatChange != Move.StatChangeAffected.Target)
            {
                statChange = HandleStatChange(chosenMove, attacker, target);
            }
            else
            {
                // Miss the target if it has fainted!
                outcome = Move.Outcome.Miss;
            }
        }
        else
        {
            outcome = DealDamage(chosenMove, attacker, target, out effectiveness);

            statChange = HandleStatChange(chosenMove, attacker, target);
        }

        m_battleMessages.Enqueue(
            GenerateOutcomeString(
                    attacker.Name,
                    target.Name,
                    chosenMove.Name,
                    outcome
            )
            );

        if (effectiveness != Move.Effectiveness.Neutral)
        {
            m_battleMessages.Enqueue(GenerateEffectivenessString(effectiveness));
        }

        if(statChange)
        {
            // Enqueue the message saying the stat and whether it increased or decreased
        }
    }

    private void OnFaint(PocketMonster pokemon, bool isPlayerMon)
    {
        m_battleMessages.Enqueue($"{pokemon.Name} fainted...");

        Debug.Log($"{pokemon.Name.ToUpper()} FAINTED");

        if (isPlayerMon)
        {
            // TODO: Make the player select another pokemon to battle
            Debug.Log("PLAYER MON FAINTED!");

            SetBattleState(BattleState.PlayerFainted);
        }
        else
        {
            if (m_currentBattleType == BattleType.WildPkmn)
            {
                // If the wild pokemon fainted then we want to end the battle
                m_battleEnded = true;
            }
            else
            {
                // TODO: Make the trainer AI pick another pokemon to battle
            }
        }
    }

    private bool HandleStatChange(Move move, PocketMonster attacker, PocketMonster target)
    {
        PocketMonster affectedMon = move.AffectedStatChange == Move.StatChangeAffected.User ? attacker : target;

        switch (move.MoveEffect)
        {
            case Move.Effect.Heal:
                break;
            case Move.Effect.IncreaseAttack:
                affectedMon.GetStats().IncreaseAttack();
                break;
            case Move.Effect.DecreaseAttack:
                affectedMon.GetStats().DecreaseAttack();
                break;
            case Move.Effect.IncreaseAccuracy:
                // TODO: Accuracy
                break;
            case Move.Effect.DecreaseAccuracy:
                // TODO: Accuracy
                break;
            case Move.Effect.IncreaseDefense:
                affectedMon.GetStats().IncreaseDefense();
                break;
            case Move.Effect.DecreaseDefense:
                affectedMon.GetStats().DecreaseDefense();
                break;
            case Move.Effect.IncreaseSpeed:
                affectedMon.GetStats().IncreaseSpeed();
                break;
            case Move.Effect.DecreaseSpeed:
                affectedMon.GetStats().DecreaseSpeed();
                break;
            case Move.Effect.RaiseAllStats:
                affectedMon.GetStats().IncreaseAttack();
                affectedMon.GetStats().IncreaseDefense();
                affectedMon.GetStats().IncreaseSpeed();
                break;
        }
    }

    public void SetBattleState(BattleState state)
    {
        m_battleState = state;
    }

    public void SetPlayerPokemon(PocketMonster pokemon)
    {
        GameManager.Instance.SpawnPlayerPokemon();
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
        int baseSpeedOverTwo = mon.GetStats().GetBaseSpeed() / 2;

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

    public void NextTurn()
    {
        m_aiChosenThisTurn = false;

        // Make the player choose a new pokemon if they fainted this turn
        if (m_battleState == BattleState.PlayerFainted)
        {
            m_battleHUD.ShowChoosePkmnMenu();
        }
        else
        {
            SetBattleState(BattleState.SelectMove);
            m_battleHUD.ShowChoiceUI();
        }
    }
}
