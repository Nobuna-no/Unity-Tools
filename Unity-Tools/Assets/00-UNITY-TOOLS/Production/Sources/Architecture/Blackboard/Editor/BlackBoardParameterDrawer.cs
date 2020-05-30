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

public class BlackboardParameterDrawer : PropertyDrawer
{
    private readonly GUIContent[] popupOptions =
            { new GUIContent("Use EntryName", "Get the variable from the entity blackboard. Useful when use of duplicated value."), new GUIContent("Force Variable", "Use directly this variable, regardless to the blackboard values. Useful for none-required duplicated value.")};

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
        SerializedProperty useEntryName = property.FindPropertyRelative("UseEntryName");
        SerializedProperty entryName = property.FindPropertyRelative("EntryName");
        SerializedProperty variable = property.FindPropertyRelative("_Variable");


        // Step 2: Calculate rect for configuration button
        Rect buttonRect = new Rect(position);
        buttonRect.yMin += popupStyle.margin.top;
        buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
        position.xMin = buttonRect.xMax;

        // Step 3: Store old indent level and set it to 0, the prefix Lavel takes care of it
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        GUIContent t = new GUIContent("", "Blackboard parameter");
        int result = EditorGUI.Popup(buttonRect, t, useEntryName.boolValue ? 0 : 1, popupOptions, popupStyle);
        useEntryName.boolValue = result == 0;

        SerializedProperty prop = useEntryName.boolValue ? entryName : variable;
        EditorGUI.PropertyField(position, prop, GUIContent.none);

        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(BBP_Bool))]
public class BBP_BoolDrawer : BlackboardParameterDrawer
{ }

[CustomPropertyDrawer(typeof(BBP_Int))]
public class BBP_IntDrawer : BlackboardParameterDrawer
{ }

[CustomPropertyDrawer(typeof(BBP_Float))]
public class BBP_FloatDrawer : BlackboardParameterDrawer
{ }

[CustomPropertyDrawer(typeof(BBP_Vector2))]
public class BBP_Vector2Drawer : BlackboardParameterDrawer
{ }

[CustomPropertyDrawer(typeof(BBP_Vector3))]
public class BBP_Vector3Drawer : BlackboardParameterDrawer
{ }

[CustomPropertyDrawer(typeof(BBP_GameObject))]
public class BBP_GameObjectDrawer : BlackboardParameterDrawer
{ }

[CustomPropertyDrawer(typeof(BBP_String))]
public class BBP_StringDrawer : BlackboardParameterDrawer
{ }
