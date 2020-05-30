using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


[CustomEditor(typeof(FSMStateTransition))]
public class FSMStateTransitionCustomEditor : Editor
{
    FSMStateTransition Target;

    private ReorderableList ConditionArray;
    private CyclicText RegisteredBanner = new CyclicText(new string[]{"Registered", "Registered.", "Registered..", "Registered..."});
    private float lineHeight;
    private float lineHeightSpace;

    private void OnEnable()
    {
        if (target == null)
        {
            return;
        }
        
        // Step 0: Setup.
        lineHeight = EditorGUIUtility.singleLineHeight;
        lineHeightSpace = lineHeight + 10;

        Target = (FSMStateTransition)target;

        // Step 1: Get properties.
        SerializedProperty ExConditions = serializedObject.FindProperty("Conditions");

        // Step 1.1: Security check.
        if (ExConditions == null)
        {
            return;
        }

        // Step 2: Setup Reoderable lists.
        ConditionArray = new ReorderableList(serializedObject, ExConditions, true, true, true, true);

        // Step 2.1: CallBacks setup.
        // Draw Header
        ConditionArray.drawHeaderCallback = (Rect rect) =>
        {
            Rect r1 = new Rect(rect.x, rect.y, rect.width * 0.75f, rect.height);
            Rect r2 = new Rect(rect.x + r1.width, rect.y, rect.width * 0.25f, rect.height);
            EditorGUI.LabelField(r1, "Transition conditions", EditorStyles.boldLabel);

            GUIStyle customHelpBoxStyle = new GUIStyle(EditorStyles.helpBox);
            customHelpBoxStyle.alignment = TextAnchor.MiddleCenter;
            SerializedProperty subProp = serializedObject.FindProperty("_CurrentlySubscribed");
            if(subProp.boolValue)
            {
                customHelpBoxStyle.fontStyle = FontStyle.Bold;
                customHelpBoxStyle.normal.textColor = new Color(0.95f, 0.55f, 0.05f);
                EditorGUI.LabelField(r2, "Registered", customHelpBoxStyle);
            }
            else
            {
                customHelpBoxStyle.normal.textColor = Color.grey;
                EditorGUI.LabelField(r2, "Pending", customHelpBoxStyle);
            }
        };

        // Draw Element
        ConditionArray.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocus) =>
        {
            TransitionConditionDrawerUtility.DrawPropertyAsReorderableList(ConditionArray, rect, index);
        };

        // On Add
        SetAddDropdownCallback(ConditionArray);

        // On Remove
        SetRemoveCallback(ConditionArray, Target.Conditions);
    }

    private void SetAddDropdownCallback(ReorderableList list)
    {
        list.onAddDropdownCallback = (Rect rect, ReorderableList rlist) =>
        {
            GenericMenu dropDownMenu = new GenericMenu();
            string[] GUIDs = AssetDatabase.FindAssets("l:FSMTransitionCondition");

            for (int j = 0; j < GUIDs.Length; ++j)
            {
                string path = AssetDatabase.GUIDToAssetPath(GUIDs[j]);
                path = System.IO.Path.GetFileNameWithoutExtension(path);
                dropDownMenu.AddItem(new GUIContent(path), false, AddItem, new AssetInfo<ReorderableList> { AssetPath = path,  ComplementaryData = list });
            }

            dropDownMenu.ShowAsContext();
        };
    }

    private void SetRemoveCallback(ReorderableList list, List<FSMTransitionCondition> targetList)
    {
        list.onRemoveCallback = (ReorderableList rlist) =>
        {
            int j = rlist.index;

            FSMTransitionCondition dataCondition = targetList[j];

            targetList.RemoveAt(j);

            if (dataCondition)
            {
                dataCondition.hideFlags = HideFlags.None;
                DestroyImmediate(dataCondition);
                EditorGUIUtility.ExitGUI();
            }
        };
    }

    private void AddItem(object obj)
    {
        AssetInfo<ReorderableList> assetInfo = obj as AssetInfo<ReorderableList>;

        if(assetInfo == null)
        {
            return;
        }

        //string assetName = System.IO.Path.GetFileNameWithoutExtension(assetInfo.AssetPath);

        System.Type AssetType = System.Type.GetType(assetInfo.AssetPath + ", Assembly-CSharp");
        FSMTransitionCondition newCondition = (FSMTransitionCondition)Target.gameObject.AddComponent(AssetType);

        int index = assetInfo.ComplementaryData.serializedProperty.arraySize++;
        assetInfo.ComplementaryData.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue = newCondition;
        newCondition.hideFlags = HideFlags.HideInInspector;

        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(".FSM STATE TRANSITION/Settings", EditorStyles.boldLabel);

        {
            SerializedProperty currStateProp = serializedObject.FindProperty("_CurrentState");

            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Current state:", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(currStateProp, GUIContent.none);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
        }


        //SerializedProperty subProp = serializedObject.FindProperty("_CurrentlySubscribed");
        //if (subProp != null && subProp.boolValue)
        //{
        //    GUIStyle sty = new GUIStyle(EditorStyles.miniButton);
        //    sty.fontStyle = FontStyle.BoldAndItalic;
        //    sty.normal.textColor = Color.grey;
        //    EditorGUILayout.LabelField(new GUIContent("Registered..."), sty);
        //}

        ConditionArray.DoLayoutList();

        {
            SerializedProperty nextStateProp = serializedObject.FindProperty("_NextState");

            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Next State:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(nextStateProp, GUIContent.none);
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }
    }
}

public class CyclicText
{
    public List<string> RotationStrings = new List<string>();
    private int index = 0;


    public CyclicText(string[] strs)
    {
        AddStrings(strs);
    }

    public void Clear()
    {
        RotationStrings.Clear();
        index = 0;
    }

    public void AddString(string str)
    {
        RotationStrings.Add(str);
    }

    public void AddStrings(string[] strs)
    {
        for(int i = 0, c = strs.Length; i < c; ++i)
        {
            RotationStrings.Add(strs[i]);
        }
    }

    public string Update()
    {
        if(RotationStrings.Count == 0)
        {
            return "";
        }

        string str = RotationStrings[index++];

        if (index >= RotationStrings.Count)
        {
            index = 0;
        }

        return str;
    }
}

public static class TransitionConditionDrawerUtility
{
    private static readonly string[] ConditionPopupOptions =
           { "==", "!=", ">", ">=", "<", "<=" };

    public static void DrawPropertyAsReorderableList(ReorderableList rList, Rect rect, int index)
    {
        SerializedProperty element = rList.serializedProperty.GetArrayElementAtIndex(index);
        if (element.objectReferenceValue == null)
            return;

        SerializedObject elementObject = new SerializedObject(element.objectReferenceValue);

        SerializedProperty Comp = elementObject.FindProperty("CompareAsEqual");
        SerializedProperty Alpha = elementObject.FindProperty("Alpha");
        SerializedProperty Beta = elementObject.FindProperty("Beta");

        float Yup = rect.y + rect.height * 0.1f;
        float height = rect.height * 0.8f;
        Rect buttonRect1 = new Rect(rect.x, Yup, rect.width * .4f, height);
        Rect buttonRect2 = new Rect(rect.x + rect.width * .42f, Yup, rect.width * .16f, rect.height);
        Rect buttonRect3 = new Rect(rect.x + rect.width * .6f, Yup, rect.width * .4f, height);


        EditorGUI.PropertyField(buttonRect1, Alpha, GUIContent.none);

        Comp.enumValueIndex = EditorGUI.Popup(buttonRect2, Comp.enumValueIndex, ConditionPopupOptions);
        // Force apply cause don't work alone?
        Comp.serializedObject.ApplyModifiedProperties();

        EditorGUI.PropertyField(buttonRect3, Beta, GUIContent.none);

        if (element.serializedObject.hasModifiedProperties)
        {
            element.serializedObject.ApplyModifiedProperties();
        }
    }
}

