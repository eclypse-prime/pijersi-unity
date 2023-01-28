using System.Collections.Generic;
using System.IO;
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
        lines = new List<SaveButton>();
        lineHeight = linePrefab.GetComponent<RectTransform>().rect.height;
        lines.Add(linePrefab.GetComponent<SaveButton>());
    }

    private void DoUpdate()
    {
        FileInfo[] files = Save.GetList();
        rect.sizeDelta = Vector2.up * lineHeight * Mathf.Max(lines.Count, files.Length);

        int i = 0;
        // Edite les boutons déjà existants
        for (; i < Mathf.Min(lines.Count, files.Length); i++)
        {
            lines[i].gameObject.SetActive(true);
            lines[i].SetData(files[i].Name.Substring(0, files[i].Name.Length - 4), files[i].CreationTime.ToString());
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
            line.GetComponent<RectTransform>().localPosition = Vector3.down * lineHeight * i;

            SaveButton button = line.GetComponent<SaveButton>();
            button.SetData(files[i].Name.Substring(0, files[i].Name.Length - 4), files[i].CreationTime.ToString());
            lines.Add(button);
        }
    }
}
