using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ReadOnlyAttribute att = (ReadOnlyAttribute)attribute;

        if(att.CanBeEditedInGame)GUI.enabled = Application.isPlaying;
        else GUI.enabled = false;
        
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true;
    }
}
