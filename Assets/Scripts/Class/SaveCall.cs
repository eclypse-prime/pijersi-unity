using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

public partial class Save
{
    /// <summary>
    /// Returns an ordered fileInfo list of all valid saves present in the save folder.
    /// </summary>
    public static FileInfo[] GetList()
    {
        IEnumerable<FileInfo> files = new DirectoryInfo(savePath)
            .GetFiles("*.txt", SearchOption.TopDirectoryOnly);

        files = files.Where(f => IsValidFile(f))
                    .OrderByDescending(f => f.LastWriteTime);

        return files.ToArray();

        static bool IsValidFile(FileInfo info)
        {
            string data = info.OpenText().ReadToEnd();
            return IsValidData(data);
        }
    }

    public static bool IsValidData(string data)
    {
        Regex validSyntax = new(validSavePattern, RegexOptions.IgnoreCase);
        return validSyntax.IsMatch(data);
    }

    public void AddTurn() => turns.Add(new(null));
    public void AddAction(ActionType action, Cell start, Cell end) =>
        turns[^1].Add(action, start, end);

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
                text += turn.actions[i].IsStack() ? stackMoveSign : moveSign;
                text += turn.cells[i + 1].name;

                if (turn.actions[i].IsAttack())
                    text += attackSign;
            }

            text += isLastColumn ? "\n" : "\t";
        }

        string[] nameArgs = { savePath, playerTypes[0].ToString(), playerTypes[1].ToString(), date.ToString("dd-MM-yyyy HH-mm-ss") };
        string fileName = string.Format("{0}\\{1} {2} - {3}.txt", nameArgs);

        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        File.WriteAllText(fileName, text);
    }
}
