using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public partial class Save
{
    private const char moveSign = '-';
    private const char stackMoveSign = '=';
    private const char attackSign = '!';
    private const string validNamePattern = ".* .* - .*";
    private const string validPattern = "[a-g][1-7]((-|=)[a-g][1-7]!?){1,2}";
    private static readonly string validSavePattern = $"([^a-z0-9]*{validPattern}){{3,}}";
    private static readonly string savePath = $"{Application.dataPath}\\..\\Saves\\";

    public PlayerType[] playerTypes;
    public List<Turn> turns;
    public DateTime date;

    public struct Turn
    {
        public List<Cell> cells;
        public List<ActionType> actions;

        public Turn(object _ = null)
        {
            cells = new();
            actions = new();
        }

        public Turn(List<Cell> cells, List<ActionType> actions)
        {
            this.cells = new(cells);
            this.actions = new(actions);
        }

        public readonly void Add(ActionType action, Cell start, Cell end)
        {
            if (cells.Count == 0)
                cells.Add(start);
            cells.Add(end);
            actions.Add(action);
        }
    }

    public Save(Save save)
    {
        playerTypes = save.playerTypes;
        turns = save.turns.ConvertAll(turn => new Turn(turn.cells, turn.actions));
        date = new DateTime(save.date.Ticks);
    }

    public Save(PlayerType[] playerTypes)
    {
        this.playerTypes = playerTypes;
        turns = new();
        date = DateTime.Now;
    }

    public Save(Board board, string saveName)
    {

        LoadPlayerTypes(saveName);
        turns = new();
        date = DateTime.Now;
        LoadSaveData(board, saveName);

    }

    private void LoadPlayerTypes(string saveName)
    {
        playerTypes = new PlayerType[] { PlayerType.Human, PlayerType.Human };
        Regex nameSyntax = new(validNamePattern, RegexOptions.IgnoreCase);
        if (nameSyntax.IsMatch(saveName))
        {
            string[] matches = Regex.Split(saveName, " - ")[0].Split(' ');
            if (matches.Length == 2)
            {
                Enum.TryParse(matches[0], out playerTypes[0]);
                Enum.TryParse(matches[1], out playerTypes[1]);
            }
        }
    }

    private void LoadSaveData(Board board, string saveName)
    {
        string data = new StreamReader(savePath + saveName).ReadToEnd();
        MatchCollection turnMatches = Regex.Matches(data, validPattern, RegexOptions.IgnoreCase);
        foreach (Match turnMatch in turnMatches)
        {
            if (turnMatch.Length == 0) continue;

            string turnData = turnMatch.Value;

            Turn turn = new(null);
            turn.cells.Add(board.Cells[board.CoordsToIndex(turnData[0], turnData[1])]);
            turn.cells.Add(board.Cells[board.CoordsToIndex(turnData[3], turnData[4])]);

            int offset = FirstAction(turnData, ref turn);
            SecondAction(turnData, ref turn, offset);

            turns.Add(turn);
        }
        board.ResetBoard();

        int FirstAction(string turnData, ref Turn turn)
        {
            bool isAttackAction = turnData.Length > 5 && turnData[5] == attackSign;
            if (isAttackAction)
            {
                if (turnData[2] == moveSign)
                    turn.actions.Add(turn.cells[0].IsFull ? ActionType.UnstackAttack : ActionType.Attack);
                else
                    turn.actions.Add(ActionType.StackAttack);
                SimulateAction(turn.actions[0], turn.cells[0], turn.cells[1]);

                return 1;
            }

            if (!turn.cells[1].IsEmpty)
                turn.actions.Add(ActionType.Stack);
            else if (turnData[2] == moveSign)
                turn.actions.Add(turn.cells[0].IsFull ? ActionType.Unstack : ActionType.Move);
            else
                turn.actions.Add(ActionType.StackMove);
            SimulateAction(turn.actions[0], turn.cells[0], turn.cells[1]);

            return 0;
        }

        void SecondAction(string turnData, ref Turn turn, int offset)
        {
            if (turnData.Length <= 6) return;

            turn.cells.Add(board.Cells[board.CoordsToIndex(turnData[6 + offset], turnData[7 + offset])]);

            bool isAttackAction = turnData.Length == 9 + offset;
            if (isAttackAction)
            {
                if (turnData[5 + offset] == moveSign)
                    turn.actions.Add(turn.cells[1].IsFull ? ActionType.UnstackAttack : ActionType.Attack);
                else
                    turn.actions.Add(ActionType.StackAttack);
                SimulateAction(turn.actions[1], turn.cells[1], turn.cells[2]);

                return;
            }

            if (!turn.cells[2].IsEmpty)
                turn.actions.Add(ActionType.Stack);
            else if (turnData[5 + offset] == moveSign)
                turn.actions.Add(turn.cells[1].IsFull ? ActionType.Unstack : ActionType.Move);
            else
                turn.actions.Add(ActionType.StackMove);
            SimulateAction(turn.actions[1], turn.cells[1], turn.cells[2]);
        }

        void SimulateAction(ActionType action, Cell start, Cell end)
        {
            switch (action)
            {
                case ActionType.Move:
                case ActionType.StackMove:
                case ActionType.Attack:
                case ActionType.StackAttack:
                    board.Move(start, end);
                    break;
                case ActionType.Stack:
                    board.Stack(start, end);
                    break;
                case ActionType.Unstack:
                case ActionType.UnstackAttack:
                    board.Unstack(start, end);
                    break;
                default:
                    Debug.LogError("Bad ActionType");
                    break;
            }
        }
    }
}
