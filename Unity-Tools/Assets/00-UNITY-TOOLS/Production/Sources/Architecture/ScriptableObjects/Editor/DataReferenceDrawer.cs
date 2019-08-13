using UnityEngine;
using UnityEditor;

/*public class DataReferenceDrawer : PropertyDrawer
{
    private readonly string[] popupOptions =
            { "Use Constant", "Use Reference" };

    private GUIStyle popupStyle;


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (popupStyle == null)
        {
            popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
            popupStyle.imagePosition = ImagePosition.ImageOnly;
        }

        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);

        // Step 1: Get properties
        SerializedProperty useConstant = property.FindPropertyRelative("UseConstant");
        SerializedProperty constantValue = property.FindPropertyRelative("ConstantValue");
        SerializedProperty variable = property.FindPropertyRelative("Variable");


        // Step 2: Calculate rect for configuration button
        Rect buttonRect = new Rect(position);
        buttonRect.yMin += popupStyle.margin.top;
        buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
        position.xMin = buttonRect.xMax;

        // Step 3: Store old indent level and set it to 0, the prefix Lavel takes care of it
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        int result = EditorGUI.Popup(buttonRect, useConstant.boolValue ? 0 : 1, popupOptions, popupStyle);
        useConstant.boolValue = result == 0;

        EditorGUI.PropertyField(position, useConstant.boolValue? constantValue : variable, GUIContent.none);

        if(property.serializedObject.hasModifiedProperties)
        {
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}*/

public class DataReferenceDrawer : PropertyDrawer
{
    private readonly string[] popupOptions =
            { "Use Constant", "Use Reference" };

    private GUIStyle popupStyle;


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (popupStyle == null)
        {
            popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
            popupStyle.imagePosition = ImagePosition.ImageOnly;
        }

        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);

        EditorGUI.BeginChangeCheck();

        // Step 1: Get properties
        SerializedProperty useConstant = property.FindPropertyRelative("UseConstant");
        SerializedProperty constantValue = property.FindPropertyRelative("ConstantValue");
        SerializedProperty variable = property.FindPropertyRelative("Variable");


        // Step 2: Calculate rect for configuration button
        Rect buttonRect = new Rect(position);
        buttonRect.yMin += popupStyle.margin.top;
        buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
        position.xMin = buttonRect.xMax;

        // Step 3: Store old indent level and set it to 0, the prefix Lavel takes care of it
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        int result = EditorGUI.Popup(buttonRect, useConstant.boolValue ? 0 : 1, popupOptions, popupStyle);
        useConstant.boolValue = result == 0;

        EditorGUI.PropertyField(position, useConstant.boolValue ? constantValue : variable, GUIContent.none);

        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(FloatReference))]
public class FloatReferenceDrawer : DataReferenceDrawer
{ }

[CustomPropertyDrawer(typeof(CurveReference))]
public class CurveReferenceDrawer : DataReferenceDrawer
{ }

[CustomPropertyDrawer(typeof(BoolReference))]
public class BoolReferenceDrawer : DataReferenceDrawer
{ }

[CustomPropertyDrawer(typeof(IntReference))]
public class IntReferenceDrawer : DataReferenceDrawer
{ }

[CustomPropertyDrawer(typeof(StringReference))]
public class StringReferenceDrawer : DataReferenceDrawer
{ }

[CustomPropertyDrawer(typeof(Vector2Reference))]
public class Vector2ReferenceDrawer : DataReferenceDrawer
{ }

[CustomPropertyDrawer(typeof(Vector3Reference))]
public class Vector3ReferenceDrawer : DataReferenceDrawer
{ }
