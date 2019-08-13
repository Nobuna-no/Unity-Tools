using UnityEngine;


[CreateAssetMenu(menuName = "Framework Data/Variable/Boolean", order = 1)]
public class BoolVariable : DataVariable<bool>
{
    public virtual bool IsTrue()
    {
        return Value;
    }
}


/*
public class DataConditionComparerDrawer : PropertyDrawer
{
    private GUIStyle popupStyle;

    private readonly string[] popupOptions =
            { "==", "!=", ">", ">=", "<", "<=" };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (popupStyle == null)
        {
            //popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
            popupStyle = GUI.skin.textArea;// new GUIStyle(GUI.skin.GetStyle("PaneOptions"));

            //popupStyle.imagePosition = ImagePosition.ImageOnly;
        }

        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);
        
        EditorGUI.BeginChangeCheck();

        // Step 1: Get properties
        SerializedProperty Comp = property.FindPropertyRelative("CompareAsEqual");
        SerializedProperty Alpha = property.FindPropertyRelative("Alpha");
        SerializedProperty Beta = property.FindPropertyRelative("Beta");

        // Step 2: Store old indent level and set it to 0, the prefix Lavel takes care of it
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        // Step 2: Calculate rect for configuration button
        Rect buttonRect1 = new Rect(position.x, position.y, position.width * .4f, position.height);
        Rect buttonRect2 = new Rect(position.x + position.width * .42f, position.y, position.width * .16f, position.height);
        Rect buttonRect3 = new Rect(position.x + position.width * .6f, position.y, position.width * .4f, position.height);

        EditorGUI.PropertyField(buttonRect1, Alpha, GUIContent.none);

        Comp.intValue = EditorGUI.Popup(buttonRect2, Comp.enumValueIndex, popupOptions);

        EditorGUI.PropertyField(buttonRect3, Beta, GUIContent.none);

        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
*/

//public class Test
//{
//    // Variable qui stocke les references de tous les objets qui le reference et qui remplace leurs reference par hascode
//}