using UnityEngine;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEditorInternal;


[CustomEditor(typeof(DATA_BlackBoard))]
public class DATA_BlackBoardEditor : Editor
{
    #region PROPERTIES
    private static readonly string SaveFolder = "Assets/XX-TOOLS/Data/BlackboardsKeys/";

    private DATA_BlackBoard Target;
    private ReorderableList ExecuteList;
    #endregion

    
    #region UNITY METHODS
    private void OnDisable()
    {

        Target.OnAssetDeletion -= ClearBeforeDeletion;
    }
   
    private void OnEnable()
    {
        if (target == null)
        {
            return;
        }

        Target = (DATA_BlackBoard)target;
        Target.OnAssetDeletion += ClearBeforeDeletion;

        // Step 1: Get properties.
        SerializedProperty BlackboardParameters = serializedObject.FindProperty("BlackboardParameters");


        // Step 1.1: Security check.
        if (BlackboardParameters == null)
        {
            return;
        }

        // Step 2: Setup Reoderable lists.
        ExecuteList = new ReorderableList(serializedObject, BlackboardParameters, true, true, true, true);

        // Step 2.1: CallBacks setup.
        // Draw Header
        ExecuteList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, new GUIContent("Blackboard parameters"));
        };

        // Draw Element
        ExecuteList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocus) =>
        {
            serializedObject.Update();
            BlackboardParameterDrawerUtility.DrawElementCallback(ExecuteList, rect, index);
        };

        // On Add
        ExecuteList.onAddDropdownCallback = (Rect rect, ReorderableList rlist) =>
        {
            GenericMenu dropdownMenu = new GenericMenu();
            string[] assetGUIDS = AssetDatabase.FindAssets("l:BlackboardParameter");

            for (int i = 0; i < assetGUIDS.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetGUIDS[i]);
                path = System.IO.Path.GetFileNameWithoutExtension(path);

                dropdownMenu.AddItem(new GUIContent(path.Replace("InternalBlackboardParameter_", "")), false, AddItem, new AssetInfo<ReorderableList> { AssetPath = path, ComplementaryData = ExecuteList });
            }

            dropdownMenu.ShowAsContext();
        };

        ExecuteList.onRemoveCallback = (ReorderableList rlist) =>
        {
            int i = ExecuteList.index;

            if (i >= Target.BlackboardParameters.Count)
            {
                return;
            }

            RemoveItem(i);
        };

        ExecuteList.elementHeightCallback = (int index) =>
        {
            return BlackboardParameterDrawerUtility.ElementHeightCallback(ExecuteList, index);
        };
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //EditorGUILayout.BeginVertical();//FilledBackgroundStyle.BackgroundStyle((int)EditorGUIUtility.currentViewWidth));
        ExecuteList.DoLayoutList();
        //EditorGUILayout.EndVertical();

        EditorUtility.SetDirty(Target);
    }
    #endregion


    #region PRIVATE METHODS
    private void RemoveItem(int index)
    {       
        InternalBlackboardParameter dataCondition = Target.BlackboardParameters[index];

        Target.BlackboardParameters.RemoveAt(index);

        if (dataCondition)
        {
            //dataCondition.hideFlags = HideFlags.None;
            EditorUtility.SetDirty(dataCondition);
            string pathToScript = AssetDatabase.GetAssetPath(dataCondition);
            AssetDatabase.DeleteAsset(pathToScript);
            DestroyImmediate(dataCondition, true);
        }
    }

    private void AddItem(object obj)
    {
        AssetInfo<ReorderableList> assetInfo = (AssetInfo<ReorderableList>)obj;

        string assetName = (assetInfo.AssetPath);

        System.Type assetType = System.Type.GetType(assetName + ", Assembly-CSharp");

        InternalBlackboardParameter param = ScriptableObject.CreateInstance(assetType) as InternalBlackboardParameter;

        string path = SaveFolder + GUID.Generate() + ".asset";

        {
            // Create the directory if needed.
            (new System.IO.FileInfo(path)).Directory.Create();
        }

        //param.hideFlags = HideFlags.HideInHierarchy;// | HideFlags.NotEditable;
        AssetDatabase.CreateAsset(param, path);
        EditorUtility.SetDirty(param);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (!param)
        {
            Debug.Break();
            Debug.LogError(this + ": ScriptableObject.CreateInstance(assetType) as DATA_BlackboardParameter is null!");
        }


        Target.BlackboardParameters.Add(param);
        int index = assetInfo.ComplementaryData.serializedProperty.arraySize++;
        assetInfo.ComplementaryData.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue = param;

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }

    private void ClearBeforeDeletion()
    {
        for (int i = Target.BlackboardParameters.Count - 1; i >= 0; --i)
        {
            RemoveItem(i);
        }
        Target.BlackboardParameters.Clear();
    }
    #endregion
}

