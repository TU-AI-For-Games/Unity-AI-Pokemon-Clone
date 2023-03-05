using System;
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

    public bool AllMessagesConsumed = false;
    private bool m_inBattle = false;
    private bool m_aiChosenThisTurn = false;
    private bool m_battleEnded = false;
    private bool m_playerSwitchedOutThisTurn;

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
                    m_playerSwitchedOutThisTurn = false;

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

        // If the player switched out, then they shouldn't be able to move
        if (!(m_playerSwitchedOutThisTurn && firstMon == m_playerPokemon))
        {
            TakeTurn(firstMon, secondMon);
        }

        if (!(m_playerSwitchedOutThisTurn && secondMon == m_playerPokemon))
        {
            TakeTurn(secondMon, firstMon);
        }

        firstMon.HandleStatus();
        secondMon.HandleStatus();

        m_battleHUD.ShowBattleInfoUI();
    }

    private void TakeTurn(PocketMonster attacker, PocketMonster target)
    {
        HandleMove(attacker, target);

        // If the target died this turn, then we want to add that message to the queue
        if (target.HasFainted())
        {
            OnFaint(target);
        }
    }

    private void HandleMove(PocketMonster attacker, PocketMonster target)
    {
        if (attacker.GetStats().HP <= 0)
        {
            // Don't attack if we fainted!
            return;
        }

        if (attacker.GetStatusEffect() == PocketMonster.StatusType.Asleep)
        {
            m_battleMessages.Enqueue($"{attacker} is asleep...");

            attacker.HandleStatus();

            if (attacker.GetStatusEffect() == PocketMonster.StatusType.Asleep)
            {
                return;
            }
        }

        Move.Outcome outcome = Move.Outcome.Hit;
        Move.Effectiveness effectiveness = Move.Effectiveness.Neutral;

        Move chosenMove = attacker.GetChosenMove();

        bool statChange = false;

        bool paralyzedThisTurn = attacker.GetStatusEffect() == PocketMonster.StatusType.Paralyzed && Random.Range(0, 100) < 25;

        if (paralyzedThisTurn)
        {
            m_battleMessages.Enqueue($"{attacker.Name} was paralyzed and couldn't move!");
        }
        else
        {
            if (target.GetStats().HP <= 0)
            {
                // Allow the user to use a stat move if the target has fainted and they are using the move on themself
                if (chosenMove.MoveEffect is not (Move.Effect.Damage or Move.Effect.Status) &&
                    chosenMove.AffectedStatChange == Move.StatChangeAffected.User)
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
                // If we're confused, there's a 50% chance of us dealing damage to ourselves 
                if (attacker.IsConfused() && Random.Range(0, 100) < 50)
                {
                    m_battleMessages.Enqueue($"{attacker.Name} hurt itself in its confusion!");
                    attacker.DealConfusionDamage();
                }
                else
                {
                    outcome = DealDamage(chosenMove, attacker, target, out effectiveness);

                    statChange = HandleStatChange(chosenMove, attacker, target);
                }
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

            if (statChange)
            {
                // Enqueue the message saying the stat and whether it increased or decreased
                m_battleMessages.Enqueue(GenerateStatChangeString(
                        chosenMove,
                        attacker,
                        target
                    )
                );
            }
        }

        if (attacker.GetStatusEffect() != PocketMonster.StatusType.Asleep)
        {
            attacker.HandleStatus();
        }
    }

    public void OnFaint(PocketMonster pokemon)
    {
        m_battleMessages.Enqueue($"{pokemon.Name} fainted...");

        Debug.Log($"{pokemon.Name.ToUpper()} FAINTED");

        if (pokemon == m_playerPokemon)
        {
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
        if (move.AffectedStatChange == Move.StatChangeAffected.None)
        {
            return false;
        }

        PocketMonster affectedMon = move.AffectedStatChange == Move.StatChangeAffected.User ? attacker : target;

        int randomChance = Random.Range(0, 100);

        if (move.StatChangeChance < randomChance)
        {
            return false;
        }

        // If we have passed the probability, then we want to apply the stat change
        switch (move.MoveEffect)
        {
            case Move.Effect.Heal:
                break;
            case Move.Effect.IncreaseAttack:
                return affectedMon.GetStats().IncreaseAttack();
            case Move.Effect.DecreaseAttack:
                return affectedMon.GetStats().DecreaseAttack();
            case Move.Effect.IncreaseAccuracy:
                return affectedMon.GetStats().IncreaseAccuracy();
            case Move.Effect.DecreaseAccuracy:
                return affectedMon.GetStats().DecreaseAccuracy();
            case Move.Effect.IncreaseDefense:
                return affectedMon.GetStats().IncreaseDefense();
            case Move.Effect.DecreaseDefense:
                return affectedMon.GetStats().DecreaseDefense();
            case Move.Effect.IncreaseSpeed:
                return affectedMon.GetStats().IncreaseSpeed();
            case Move.Effect.DecreaseSpeed:
                return affectedMon.GetStats().DecreaseSpeed();
            case Move.Effect.RaiseAllStats:
                {
                    return affectedMon.GetStats().IncreaseAttack() ||
                           affectedMon.GetStats().IncreaseDefense() ||
                           affectedMon.GetStats().IncreaseSpeed(); ;
                }
        }

        return false;
    }

    public void SetBattleState(BattleState state)
    {
        m_battleState = state;
    }

    public void SetPlayerPokemon(PocketMonster pokemon)
    {
        GameManager.Instance.SpawnPlayerPokemon();
        m_playerPokemon = pokemon;
        m_playerPokemon.ResetStats();
    }

    public void SetOtherPokemon(PocketMonster pokemon)
    {
        m_otherPokemon = pokemon;
        pokemon.ResetStats();
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
        if (m_typeMatchupTable[moveType].ContainsKey(targetType))
        {
            return m_typeMatchupTable[moveType][targetType];
        }

        // If the types aren't in the table, the multiplier is 1
        return 1f;
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

    private string GenerateStatChangeString(Move move, PocketMonster attacker, PocketMonster target)
    {
        string affected = move.AffectedStatChange == Move.StatChangeAffected.User ? attacker.Name : target.Name;

        switch (move.MoveEffect)
        {
            case Move.Effect.IncreaseAttack:
                return $"{affected}'s Attack Increased";
            case Move.Effect.DecreaseAttack:
                return $"{affected}'s Attack Decreased";
            case Move.Effect.IncreaseAccuracy:
                return $"{affected}'s Accuracy Increased";
            case Move.Effect.DecreaseAccuracy:
                return $"{affected}'s Accuracy Decreased";
            case Move.Effect.IncreaseDefense:
                return $"{affected}'s Defense Increased";
            case Move.Effect.DecreaseDefense:
                return $"{affected}'s Defense Decreased";
            case Move.Effect.IncreaseSpeed:
                return $"{affected}'s Speed Increased";
            case Move.Effect.DecreaseSpeed:
                return $"{affected}'s Speed Decreased";
            case Move.Effect.RaiseAllStats:
                return $"{affected}'s Defense Increased";
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

    public void OnEndStatusMessage(PocketMonster mon, PocketMonster.StatusType status)
    {
        switch (status)
        {
            case PocketMonster.StatusType.Asleep:
                m_battleMessages.Enqueue($"{mon.Name} woke up!");
                break;
            case PocketMonster.StatusType.Burned:
                m_battleMessages.Enqueue($"{mon.Name} was hurt by its burn!");
                break;
            case PocketMonster.StatusType.Frozen:
                m_battleMessages.Enqueue($"{mon.Name} thawed out!");
                break;
            case PocketMonster.StatusType.Paralyzed:
                break;
            case PocketMonster.StatusType.Poisoned:
                m_battleMessages.Enqueue($"{mon.Name} was hurt by its poison!");
                break;
        }
    }

    public void OnApplyStatusMessage(PocketMonster mon, PocketMonster.StatusType status)
    {
        switch (status)
        {
            case PocketMonster.StatusType.Asleep:
                m_battleMessages.Enqueue($"{mon.Name} fell asleep!");
                break;
            case PocketMonster.StatusType.Burned:
                m_battleMessages.Enqueue($"{mon.Name} was burned!");
                break;
            case PocketMonster.StatusType.Frozen:
                m_battleMessages.Enqueue($"{mon.Name} is frozen solid!");
                break;
            case PocketMonster.StatusType.Paralyzed:
                m_battleMessages.Enqueue($"{mon.Name} was paralyzed!");
                break;
            case PocketMonster.StatusType.Poisoned:
                m_battleMessages.Enqueue($"{mon.Name} was badly poisoned!");
                break;
        }
    }

    public void OnSnapOut(PocketMonster mon)
    {
        m_battleMessages.Enqueue($"{mon.Name} snapped out of its confusion!");
    }

    public void PlayerSwitchedOut()
    {
        if (m_battleState == BattleState.PlayerFainted)
        {
            SetBattleState(BattleState.SelectMove);
        }
        else
        {
            m_playerSwitchedOutThisTurn = true;
            SetBattleState(BattleState.Attack);
        }
    }
}
