
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : Singleton<MoveManager>
{
    private Dictionary<int, Move> m_moves;

    protected override void InternalInit()
    {
        m_moves = new Dictionary<int, Move>();
    }

    public void LoadMoves()
    {
        TextAsset movesFile = (TextAsset)Resources.Load("Data/moves");
        string[] linesFromFile = movesFile.text.Split('\n');

        for (int i = 1; i < linesFromFile.Length; i++)
        {
            string[] csvContent = linesFromFile[i].Split(',');
            int moveID = Convert.ToInt32(csvContent[0]);
            string name = csvContent[1];
            string description = csvContent[2];
            PocketMonster.Element type = StringToType(csvContent[3]);
            Move.Effect effect = StringToEffect(csvContent[4]);
            float damage = Convert.ToSingle(csvContent[5]);
            float accuracy = StringToAccuracy(csvContent[6]);


            m_moves.Add(moveID, new Move(name, description, type, effect, damage, accuracy));
        }

        foreach (KeyValuePair<int, Move> kvp in m_moves)
        {
            //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            Debug.Log($"Key = {kvp.Key}, Value = {kvp.Value}");
        }
    }

    private PocketMonster.Element StringToType(string type)
    {
        switch (type)
        {
            case "Bug":
                return PocketMonster.Element.Bug;
            case "Dark":
                return PocketMonster.Element.Dark;
            case "Dragon":
                return PocketMonster.Element.Dragon;
            case "Electric":
                return PocketMonster.Element.Electric;
            case "Fighting":
                return PocketMonster.Element.Fighting;
            case "Fire":
                return PocketMonster.Element.Fire;
            case "Flying":
                return PocketMonster.Element.Flying;
            case "Ghost":
                return PocketMonster.Element.Ghost;
            case "Grass":
                return PocketMonster.Element.Grass;
            case "Ground":
                return PocketMonster.Element.Ground;
            case "Ice":
                return PocketMonster.Element.Ice;
            case "Typeless":
            case "Normal":
                return PocketMonster.Element.Normal;
            case "Poison":
                return PocketMonster.Element.Poison;
            case "Psychic":
                return PocketMonster.Element.Psychic;
            case "Rock":
                return PocketMonster.Element.Rock;
            case "Steel":
                return PocketMonster.Element.Steel;
            case "Water":
                return PocketMonster.Element.Water;
            default:
                throw new ArgumentOutOfRangeException(type, "Make sure the type is supported!");
        }
    }

    private float StringToAccuracy(string accuracy)
    {
        // Read up to the percentage sign
        int index = accuracy.IndexOf('%');
        if (index > 0)
        {
            return Convert.ToSingle(accuracy[..index]) / 100f;
        }

        if (accuracy.Length == 0)
        {
            return 1f;
        }

        throw new ArithmeticException($"Unable to convert from {accuracy} to float");
    }

    private Move.Effect StringToEffect(string effect)
    {
        switch (effect)
        {
            case "Physical":
            case "Special":
                return Move.Effect.Damage;
            case "a+":
            case "sa+":
                return Move.Effect.IncreaseAttack;
            case "a-":
            case "sa-":
                return Move.Effect.DecreaseAttack;
            case "acc+":
                return Move.Effect.IncreaseAccuracy;
            case "acc-":
                return Move.Effect.DecreaseAccuracy;
            case "d+":
            case "sd+":
                return Move.Effect.IncreaseDefense;
            case "d-":
            case "sd-":
                return Move.Effect.DecreaseDefense;
            case "s+":
                return Move.Effect.IncreaseSpeed;
            case "s-":
                return Move.Effect.DecreaseSpeed;
            case "Status":
                return Move.Effect.Status;
        }

        throw new ArgumentOutOfRangeException(effect, "Ensure the effect is supported");
    }
}
