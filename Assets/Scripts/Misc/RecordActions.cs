using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class RecordActions : Singleton<RecordActions>
{
    private readonly string m_saveLocation = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\AiPokeClone";
    private string m_fileName;
    private List<string> m_actions;

    private readonly string[] m_csvHeaders = new[]
    {
        "Chosen Action (Attack/Switch/Heal)",
        "Player Pkmn ID",
        "Target Pkmn ID",
        "Player HP",
        "Target HP",
        "Player Element",
        "Target Element",
        "Chosen Move ID",
        "Move Element",
        "STAB?",
        "Move Power",
        "Status Move?",
        "Status Hit?",
        "Status Applied",
        "Affected Status (T/U)",
        "Hit?",
        "Critical?",
        "Effectiveness(0-2)",
        "Stat Change?",
        "Stat Changed",
        "Stat Target(T/U)",
        "Kill?",
        "Outcome Target HP"
    };

    public enum PlayerAction
    {
        Attack,
        Heal,
        Switch
    }

    protected override void InternalInit()
    {
        // Create the folder if it doesn't exist
        if (!Directory.Exists(m_saveLocation))
        {
            Directory.CreateDirectory(m_saveLocation);
        }

        m_actions = new List<string>();
    }

    public void OnStartBattle()
    {
        DateTime now = DateTime.Now;
        m_fileName = $"battle_{now.Day}_{now.Month}_{now.Year}_{now.TimeOfDay.Hours}{now.TimeOfDay.Minutes}{now.TimeOfDay.Seconds}";

        // Create the file
        FileStream outStream = File.Create($"{m_saveLocation}\\{m_fileName}.csv");
    }

    public void RecordAction(PlayerAction chosenAction, PocketMonster playerPokemon, PocketMonster targetPokemon, int playerHpBefore, int targetHpBefore, bool statusApplied, Move.Outcome moveOutcome, bool statChange, bool targetFainted)
    {
        // SET UP VALUES TO BE WRITTEN TO THE FILE
        string playerAction = chosenAction switch
        {
            PlayerAction.Attack => "Attack",
            PlayerAction.Heal => "Heal",
            _ => "Switch"
        };

        string playerPkmnID = playerPokemon.ID.ToString();
        string targetPkmnID = targetPokemon.ID.ToString();

        string playerHP = playerHpBefore.ToString();
        string targetHP = targetHpBefore.ToString();

        string playerElement = ((int)playerPokemon.Type).ToString();
        string targetElement = ((int)targetPokemon.Type).ToString();


        Move move = playerPokemon.GetChosenMove();
        string chosenMoveID = MoveManager.Instance.GetMoveID(move).ToString();
        string chosenMoveElement = ((int)move.Type).ToString();
        string isStab = playerPokemon.Type == move.Type ? "1" : "0";
        string movePower = move.Damage.ToString();

        string statusMove = move.MoveEffect == Move.Effect.Status ? "1" : "0";
        string statusHit = statusApplied ? "1" : "0";
        string appliedStatus = move.Status != Move.StatusEffect.None && statusApplied ? ((int)move.Status).ToString() : "";

        string moveHit = moveOutcome != Move.Outcome.Miss ? "1" : "0";
        string effectiveness = BattleManager.Instance.GetTypeAdvantageMultiplier(move.Type, targetPokemon.Type).ToString();

        string statChangedThisTurn = statChange ? "1" : "0";

        string changedStat = statChange switch
        {
            true => MoveManager.EffectToString(move.MoveEffect),
            _ => ""
        };

        string statChangeTarget = move.AffectedStatChange switch
        {
            Move.StatChangeAffected.Target => "T",
            Move.StatChangeAffected.User => "U",
            _ => ""
        };

        string killThisTurn = targetFainted ? "1" : "0";
        string outcomeTargetHP = MathF.Max(0, targetPokemon.GetStats().HP).ToString();

        // TODO: THERE MUST BE A BETTER WAY OF DOING IT THAN THIS...
        string action = $"{playerAction}," +
                        $"{playerPkmnID}," +
                        $"{targetPkmnID}," +
                        $"{playerHP}," +
                        $"{targetHP}," +
                        $"{playerElement}," +
                        $"{targetElement}," +
                        $"{chosenMoveID}," +
                        $"{chosenMoveElement}," +
                        $"{isStab},{movePower}," +
                        $"{statusMove}," +
                        $"{statusHit}," +
                        $"{appliedStatus}," +
                        $"{moveHit}," +
                        $"{effectiveness}," +
                        $"{statChangedThisTurn}," +
                        $"{changedStat}," +
                        $"{statChangeTarget}," +
                        $"{killThisTurn}," +
                        $"{outcomeTargetHP}";

        m_actions.Add(action);
    }



    private void WriteToFile()
    {
        FileStream outStream = File.OpenWrite($"{m_saveLocation}\\{m_fileName}.csv");

        StreamWriter writer = new StreamWriter(outStream);

        // Write the header
        writer.WriteLine(string.Join(',', m_csvHeaders));

        // Write the contents
        foreach (string action in m_actions)
        {
            writer.WriteLine(action);
        }
    }
}
