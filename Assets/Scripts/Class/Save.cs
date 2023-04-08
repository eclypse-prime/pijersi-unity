using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;

public class Save
{
    private const char moveSign = '-';
    private const char stackMoveSign = '=';
    private const char attackSign = '!';
    private const string validPattern = "[a-g][1-7]((-|=)[a-g][1-7]!?){1,2}";
    private static string validSavePattern = $"([^a-z0-9]*{validPattern}){{3,}}";
    private static string savePath = $"{Application.dataPath}\\Saves\\";

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

        public Turn(List<Cell> cells, List<ActionType> actions, List<bool> isStackMoves)
        {
            this.cells = new List<Cell>(cells);
            this.actions = new List<ActionType>(actions);
            this.isStackMoves = new List<bool>(isStackMoves);
        }

        public void Add(ActionType action, Cell start, Cell end)
        {
            if (cells.Count == 0)
                cells.Add(start);
            cells.Add(end);
            actions.Add(action);
            if ((action == ActionType.Move || action == ActionType.Attack) && end.isFull)
            {
                isStackMoves.Add(true);
                return;
            }
            isStackMoves.Add(false);
        }
    }

    public PlayerType[] playerTypes;
    public List<Turn> turns;
    public DateTime date;

    public Save(Save save)
    {
        playerTypes = save.playerTypes;
        turns = save.turns.ConvertAll(turn => new Turn(turn.cells, turn.actions, turn.isStackMoves));
        date = new DateTime(save.date.Ticks);
    }

    public Save(PlayerType[] playerTypes)
    {
        this.playerTypes = playerTypes;
        turns = new List<Turn>();
        date = DateTime.Now;
    }

    public Save(Board board, string saveName)
    {
        playerTypes = new PlayerType[] { PlayerType.Human, PlayerType.Human };
        turns = new List<Turn>();
        date = DateTime.Now;

        // load save data
        string data = new StreamReader(savePath + saveName + ".txt").ReadToEnd();

        MatchCollection turnMatches = Regex.Matches(data, validPattern, RegexOptions.IgnoreCase);
        foreach (Match turnMatch in turnMatches)
        {
            if (turnMatch.Length == 0) continue;

            string turnData = turnMatch.Value;

            Turn turn = new Turn(null);
            turn.cells.Add(board.cells[board.CoordsToIndex(turnData[0], turnData[1])]);
            turn.cells.Add(board.cells[board.CoordsToIndex(turnData[3], turnData[4])]);

            int offset = 0;
            // if the first action is an attack
            if (turnData.Length > 5 && turnData[5] == attackSign)
            {
                if (turnData[2] == moveSign)
                    turn.actions.Add(turn.cells[0].isFull ? ActionType.UnstackAttack : ActionType.Attack);
                else
                    turn.actions.Add(ActionType.StackAttack);

                offset++;
            }
            else
            {
                if (!turn.cells[1].isEmpty)
                    turn.actions.Add(ActionType.Stack);
                else if (turnData[2] == moveSign)
                    turn.actions.Add(turn.cells[0].isFull ? ActionType.Unstack : ActionType.Move);
                else
                    turn.actions.Add(ActionType.StackMove);
            }

            SimulateAction(turn.actions[0], turn.cells[0], turn.cells[1]);

            // second action
            if (turnData.Length > 6)
            {
                turn.cells.Add(board.cells[board.CoordsToIndex(turnData[6 + offset], turnData[7 + offset])]);

                // if the second action is an attack
                if (turnData.Length == 9 + offset)
                {
                    if (turnData[5 + offset] == moveSign)
                        turn.actions.Add(turn.cells[1].isFull ? ActionType.UnstackAttack : ActionType.Attack);
                    else
                        turn.actions.Add(ActionType.StackAttack);
                }
                else
                {
                    if (!turn.cells[2].isEmpty)
                        turn.actions.Add(ActionType.Stack);
                    else if (turnData[5 + offset] == moveSign)
                        turn.actions.Add(turn.cells[1].isFull ? ActionType.Unstack : ActionType.Move);
                    else
                        turn.actions.Add(ActionType.StackMove);
                }

                SimulateAction(turn.actions[1], turn.cells[1], turn.cells[2]);
            }

            turns.Add(turn);
        }


        board.ResetBoard();

        // simulate an Action on the board
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

    public void AddTurn()
    {
        turns.Add(new Turn(null));
    }

    public void AddAction(ActionType action, Cell start, Cell end)
    {
        turns[turns.Count -1].Add(action, start, end);
    }

    /// <summary>
    /// Creates a save file.
    /// </summary>
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
                text += turn.isStackMoves[i] ? stackMoveSign : moveSign;
                text += turn.cells[i + 1].name;

                if (turn.actions[i] == ActionType.Attack)
                    text += attackSign;
            }

            text += isLastColumn ? "\n" : "\t";
        }

        string path = $"{Application.dataPath}\\Saves";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        File.WriteAllText($"{path}\\{date.ToString("dd-MM-yyyy HH-mm-ss")}.txt", text);
    }

    /// <summary>
    /// Returns an ordered fileInfo list of all valid saves present in the save folder.
    /// </summary>
    public static FileInfo[] GetList()
    {
        // get .txt files from save folder
        IEnumerable<FileInfo> files = new DirectoryInfo(savePath)
            .GetFiles("*.txt", SearchOption.TopDirectoryOnly);

        // sort valid files by date of the last modification (from newest to oldest)
        files = files.Where(f => IsValideData(f))
                    .OrderByDescending(f => f.LastWriteTime);

        return files.ToArray();
    
        // check if the file data is valid
        static bool IsValideData(FileInfo info)
        {
            string data = info.OpenText().ReadToEnd();
            Regex validSyntax = new Regex(validSavePattern, RegexOptions.IgnoreCase);

            return validSyntax.IsMatch(data);
        }
    }
}
