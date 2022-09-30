using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;

[CustomEditor(typeof(BetterButton))]
[CanEditMultipleObjects]
public class BetterButtonEditor : UnityEditor.UI.ButtonEditor
{
    SerializedProperty m_OnRightClickProperty;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_OnRightClickProperty = serializedObject.FindProperty("m_OnRightClick");
    }

    public override void OnInspectorGUI()
    {

        BetterButton component = (BetterButton)target;

        base.OnInspectorGUI();

        serializedObject.Update();
        EditorGUILayout.PropertyField(m_OnRightClickProperty);
        serializedObject.ApplyModifiedProperties();
    }
}