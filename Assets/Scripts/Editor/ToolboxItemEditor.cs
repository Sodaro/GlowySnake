using UnityEngine;
using UnityEditor;
using SnakeUtilities;
using UnityEngine.UI;
using System.Collections;

[CustomEditor(typeof(ToolboxItem))]
public class ToolBoxItemEditor : UnityEditor.UI.ButtonEditor
{
    public override void OnInspectorGUI()
    {
        ToolboxItem component = (ToolboxItem)target;

        component.sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", component.sprite, typeof(Sprite), true);
        component.contentType = (ContentType)EditorGUILayout.EnumPopup(component.contentType);

        serializedObject.FindProperty("sprite").objectReferenceValue = component.sprite;
        serializedObject.FindProperty("contentType").enumValueIndex = (int)component.contentType;
        serializedObject.ApplyModifiedProperties();
    }
}
