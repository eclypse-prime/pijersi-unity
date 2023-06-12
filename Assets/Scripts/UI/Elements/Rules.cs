using UnityEngine;

public class Rules : MonoBehaviour
{
    private const string url = "https://github.com/LucasBorboleta/pijersi/blob/main/README.md#pijersi--the-rules-en--les-règles-fr";

    public static void OpenUrl() => Application.OpenURL(url);
}
