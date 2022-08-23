using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Save
{
    private const char moveSign = '-';
    private const char stackMoveSign = '=';
    private const char attackSign = '!';

    public struct Turn
    {
        public List<Cell> cells;
        public List<ActionType> actions;
        public List<bool> isStackMoves;

        public Turn(object nope = null)
        {
            cells = new List<Cell>();
            actions = new List<ActionType>();
            isStackMoves = new List<bool>();
        }

        public void Add(ActionType action, Cell start, Cell end)
        {
            if (cells.Count == 0)
                cells.Add(start);
            cells.Add(end);
            actions.Add(action);
            isStackMoves.Add(end.isFull);
        }
    }

    public PlayerType[] gameType;
    public List<Turn> turns;

    private string date;

    public Save(PlayerType[] gameType)
    {
        this.gameType = gameType;
        turns = new List<Turn>();
        date = DateTime.Now.ToString("yyyy'-'MM'-'dd HH'-'mm");
    }

    public void AddTurn()
    {
        turns.Add(new Turn(null));
    }

    public void AddAction(ActionType action, Cell start, Cell end)
    {
        turns[turns.Count -1].Add(action, start, end);
    }

    public void Write()
    {

        bool isLastColumn = true;
        string text = "";
        foreach (Turn turn in turns)
        {
            isLastColumn = !isLastColumn;
            text += turn.cells[0].name;

            for (int i = 0; i < turn.actions.Count; i++)
            {
                if (turn.actions[i] == ActionType.Move || turn.actions[i] == ActionType.Attack)
                    text += turn.isStackMoves[i] ? stackMoveSign : moveSign;
                else
                    text += moveSign;

                text += turn.cells[i + 1].name;

                if (turn.actions[i] == ActionType.Attack)
                    text += attackSign;
            }

            text += isLastColumn ? "\n" : "\t";
        }

        string path = $"{Application.dataPath}\\Saves";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        File.WriteAllText($"{path}\\{date}.txt", text);
    }
}
