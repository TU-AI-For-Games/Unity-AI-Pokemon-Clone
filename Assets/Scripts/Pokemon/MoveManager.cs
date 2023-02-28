
using System;
using System.Collections;
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
            string name = csvContent[1];
            string description = csvContent[2];
            PocketMonster.Element type = PocketMonster.StringToType(csvContent[3]);
            Move.MoveEffect moveEffect = StringToEffect(csvContent[4]);
            int damage = Convert.ToInt32(csvContent[5]);
            int accuracy = StringToAccuracy(csvContent[6]);


            m_moves.Add(moveID, new Move(name, description, type, moveEffect, damage, accuracy));
        }
    }

    public Move GetMove(int moveID)
    {
        return m_moves[moveID];
    }

    private int StringToAccuracy(string accuracy)
    {
        // Read up to the percentage sign
        int index = accuracy.IndexOf('%');
        if (index > 0)
        {
            return Convert.ToInt32(accuracy[..index]);
        }

        if (accuracy.Length == 0)
        {
            return 100;
        }

        throw new ArithmeticException($"Unable to convert from {accuracy} to float");
    }

    private Move.MoveEffect StringToEffect(string effect)
    {
        switch (effect)
        {
            case "Physical":
            case "Special":
                return Move.MoveEffect.Damage;
            case "a+":
            case "sa+":
                return Move.MoveEffect.IncreaseAttack;
            case "a-":
            case "sa-":
                return Move.MoveEffect.DecreaseAttack;
            case "acc+":
                return Move.MoveEffect.IncreaseAccuracy;
            case "acc-":
                return Move.MoveEffect.DecreaseAccuracy;
            case "d+":
            case "sd+":
                return Move.MoveEffect.IncreaseDefense;
            case "d-":
            case "sd-":
                return Move.MoveEffect.DecreaseDefense;
            case "s+":
                return Move.MoveEffect.IncreaseSpeed;
            case "s-":
                return Move.MoveEffect.DecreaseSpeed;
            case "Status":
                return Move.MoveEffect.Status;
        }

        throw new ArgumentOutOfRangeException(effect, "Ensure the moveEffect is supported");
    }
}
