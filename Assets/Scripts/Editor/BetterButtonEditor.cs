using UnityEditor;

[CustomEditor(typeof(BetterButton))]
[CanEditMultipleObjects]
public class BetterButtonEditor : UnityEditor.UI.ButtonEditor
{
    SerializedProperty m_onRightClickProperty;
    SerializedProperty pressActionProperty;
    SerializedProperty alternativePressActionProperty;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_onRightClickProperty = serializedObject.FindProperty("m_onRightClick");
        pressActionProperty = serializedObject.FindProperty("pressAction");
        alternativePressActionProperty = serializedObject.FindProperty("alternativePressAction");
    }

    public override void OnInspectorGUI()
    {

        BetterButton component = (BetterButton)target;

        base.OnInspectorGUI();

        serializedObject.Update();
        EditorGUILayout.PropertyField(m_onRightClickProperty);
        EditorGUILayout.PropertyField(pressActionProperty);
        EditorGUILayout.PropertyField(alternativePressActionProperty);
        serializedObject.ApplyModifiedProperties();
    }
}