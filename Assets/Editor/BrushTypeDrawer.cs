using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomPropertyDrawer(typeof(BrushTypeAttribute))]
public class BrushTypeDrawer : PropertyDrawer 
{
    public override void OnGUI( Rect position,
                                SerializedProperty prop,
                                GUIContent label )
	{
		WorldGrid grid = prop.serializedObject.targetObject as WorldGrid;
		GUIContent[] values = System.Array.ConvertAll(grid.cellTypes, t => new GUIContent(t.name));
		grid.brushType = EditorGUI.Popup(position, label, grid.brushType, values);
		//EditorGUI.EnumPopup(position, label, grid.brushType);
    }

}