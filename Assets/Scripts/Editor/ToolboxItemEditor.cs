using SnakeUtilities;
using UnityEditor;
using UnityEngine;


//Editor window for the toolbox items in the level editor
//Only shows a sprite field and an enum drop down
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
