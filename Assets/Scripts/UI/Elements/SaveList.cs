using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class SaveList : MonoBehaviour
{
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Text counter;

    private new Transform transform;
    private RectTransform rect;
    private List<SaveButton> lines;
    private float lineHeight;

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        DoUpdate();
    }

    private void Init()
    {
        transform = base.transform;
        rect = GetComponent<RectTransform>();

        linePrefab.SetActive(false);
        lines = new();
        lineHeight = linePrefab.GetComponent<RectTransform>().rect.height;
        lines.Add(linePrefab.GetComponent<SaveButton>());
    }

    private void DoUpdate()
    {
        FileInfo[] files = Save.GetList();
        rect.sizeDelta = lineHeight * Mathf.Max(lines.Count, files.Length) * Vector2.up;

        int i = 0;
        // Edite les boutons déjà existants
        for (; i < Mathf.Min(lines.Count, files.Length); i++)
        {
            lines[i].gameObject.SetActive(true);
            string name = Regex.Split(files[i].Name, " - ")[0];
            lines[i].SetData(name, files[i].Name, files[i].CreationTime.ToString());
        }
        // désactive les boutons en trop
        for (; i < lines.Count; i++)
        {
            lines[i].gameObject.SetActive(false);
        }
        // ajoute les boutons manquant
        for (; i < files.Length; i++)
        {
            GameObject line = Instantiate(linePrefab, transform);
            line.GetComponent<RectTransform>().localPosition = lineHeight * i * Vector3.down;

            SaveButton button = line.GetComponent<SaveButton>();
            string name = Regex.Split(files[i].Name, " - ")[0];
            button.SetData(name, files[i].Name, files[i].CreationTime.ToString());
            lines.Add(button);
        }
    }
}
