using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Texture), true)]
public class TexturePreviewDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label);
    }

}