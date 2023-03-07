using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class RecordActions : Singleton<RecordActions>
{
    private readonly string m_saveLocation = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\AiPokeClone";
    private string m_fileName;
    private List<string> m_actions;

    private bool m_isRecording;

    public int PreviousPokemonID;

    private readonly string[] m_csvHeaders = new[]
    {
        "Chosen Action (Attack/Switch/Heal)",
        "Player Pkmn ID",
        "Target Pkmn ID",
        "Player HP",
        "Target HP",
        "Player Element",
        "Target Element",
        "Previous Pkmn ID",
        "Previous Pkmn Element",
        "Chosen Move ID",
        "Move Element",
        "STAB?",
        "Move Power",
        "User Status",
        "Status Move?",
        "Status Hit?",
        "Status Applied",
        "Hit?",
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
        m_isRecording = false;

        // Create the folder if it doesn't exist
        if (!Directory.Exists(m_saveLocation))
        {
            Directory.CreateDirectory(m_saveLocation);
        }

        m_actions = new List<string>();
    }

    public void OnStartBattle()
    {
        m_actions.Clear();

        PreviousPokemonID = 0;

        DateTime now = DateTime.Now;
        m_fileName = $"battle_{now.Day}_{now.Month}_{now.Year}_{now.TimeOfDay.Hours}{now.TimeOfDay.Minutes}{now.TimeOfDay.Seconds}";

        // Create the file
        FileStream outStream = File.Create($"{m_saveLocation}\\{m_fileName}.csv");
        outStream.Close();

        m_isRecording = true;
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

        string previousPokemonID = "";
        string previousPokemonElement = "";

        if (chosenAction == PlayerAction.Switch)
        {
            previousPokemonID = PreviousPokemonID.ToString();
            previousPokemonElement = ((int)PocketMonsterManager.Instance.GetPocketMonster(PreviousPokemonID).Type).ToString();
        }

        // move may be null if the player switched in!
        Move move = playerPokemon.GetChosenMove();

        string chosenMoveID = "";
        string chosenMoveElement = "";
        string isStab = "";
        string movePower = "";
        string statusMove = "";
        string appliedStatus = "";
        string effectiveness = "";
        string changedStat = "";
        string statChangeTarget = "";


        if (move != null)
        {
            chosenMoveID = MoveManager.Instance.GetMoveID(move).ToString();

            chosenMoveElement = ((int)move.Type).ToString();

            isStab = playerPokemon.Type == move.Type ? "1" : "0";

            movePower = move.Damage.ToString();

            statusMove = move.MoveEffect == Move.Effect.Status ? "1" : "0";

            appliedStatus = move.Status != Move.StatusEffect.None && statusApplied ? ((int)move.Status).ToString() : "";

            effectiveness = BattleManager.Instance.GetTypeAdvantageMultiplier(move.Type, targetPokemon.Type) switch
            {
                0f => "Immune",
                0.5f => "Not very effective",
                1f => "Neutral",
                _ => "Super effective"
            };

            changedStat = statChange switch
            {
                true => MoveManager.EffectToString(move.MoveEffect),
                _ => ""
            };

            statChangeTarget = move.AffectedStatChange switch
            {
                Move.StatChangeAffected.Target => "T",
                Move.StatChangeAffected.User => "U",
                _ => ""
            };
        }


        string playerStatus = ((int)playerPokemon.GetStatusEffect()).ToString();

        string statusHit = statusApplied ? "1" : "0";

        string moveHit = moveOutcome switch
        {
            Move.Outcome.Miss => "Miss",
            Move.Outcome.Hit => "Hit",
            Move.Outcome.CriticalHit => "Critical Hit",
            _ => ""
        };

        string statChangedThisTurn = statChange ? "1" : "0";

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
                        $"{previousPokemonID}," +
                        $"{previousPokemonElement}," +
                        $"{chosenMoveID}," +
                        $"{chosenMoveElement}," +
                        $"{isStab}," +
                        $"{movePower}," +
                        $"{playerStatus}," +
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

    public void OnEndBattle()
    {
        WriteToFile();
        m_isRecording = false;
        Debug.Log($"Written to {m_saveLocation}\\{m_fileName}.csv");
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

        writer.Close();
        outStream.Close();
    }

    void OnApplicationQuit()
    {
        if (m_isRecording)
        {
            WriteToFile();
        }
    }
}
