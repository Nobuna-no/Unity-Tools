using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

public class ProceduralDatabaseEditor : EditorWindow
{
    /** VARIABLES
     */
    private SO_ProceduralRowList m_Database;
    private SO_DynamicRow m_RowStructure;

    private Rect m_LeftPanel;
    private Rect m_RightPanel;
    private Rect m_ResizerPanel;
    private float m_SizeRatio = 0.3f;
    private float m_ResizerSize = 5f;
    private float m_ResizerTreshold = 0.3f;
    private GUIStyle m_ResizerStyle;

    private int m_ViewIndex = 0;
    private bool m_bInvalidRowStructure;
    private bool m_bGenerateDataObject = true;
    private bool m_bIsResizing = false;


    /** EDITOR METHODS
     */
    [MenuItem("Window/Procedural Database Editor %#2")]
    private static void Init()
    {
        ProceduralDatabaseEditor window = GetWindow<ProceduralDatabaseEditor>();
        window.titleContent = new GUIContent("Database Editor");
    }

    private void OnEnable() 
    {
        //if (EditorPrefs.HasKey("ObjectPath"))
        //{
        //    string ObjectPath = EditorPrefs.GetString("ObjectPath");
        //    m_Database = AssetDatabase.LoadAssetAtPath(ObjectPath, typeof(SO_ProceduralRowList)) as SO_ProceduralRowList;
        //}

        m_ResizerStyle = new GUIStyle();
        m_ResizerStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;
    }

    private void OnDisable()
    {
        CloseDatabase();
    }
  
    private void OnGUI()
    {
        Draw_LeftPanel();

        m_RightPanel = new Rect(m_SizeRatio * position.width - m_ResizerSize, 0, (1 - m_SizeRatio) * position.width, position.height);
        GUILayout.BeginArea(m_RightPanel);

        GUILayout.BeginHorizontal();
        /* ============================ */
        GUILayout.Label("Procedural Database Editor", EditorStyles.boldLabel);
        /* ============================ */
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        /* ============================ */
        DrawMainMenu_HeaderButton();
        /* ============================ */

        DrawMainMenu_DatabaseSettings();


        if (m_Database)
        {

            if (m_Database.bGenerateDataObject)
            {
                /* ============================ */
                GUILayout.BeginHorizontal();

                m_Database.DataSavePath = EditorGUILayout.TextField("Save Row Path", m_Database.DataSavePath);
                if (GUILayout.Button("Browse...", GUILayout.ExpandWidth(false)))
                {
                    m_Database.DataSavePath = BrowseRowSavePath();
                }
                /* ============================ */
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }

            /* ============================ */
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Previous", GUILayout.ExpandWidth(false)))
            {
                if (m_ViewIndex > 1)
                {
                    m_ViewIndex--;
                }
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
            {
                if (m_ViewIndex < m_Database.Rows.Count)
                {
                    m_ViewIndex++;
                }
            }

            GUILayout.Space(50);

            if (GUILayout.Button("Add Item", GUILayout.ExpandWidth(false)))
            {
                AddRow();
            }
            if (GUILayout.Button("Delete Item", GUI_ColorButtonStyle(Color.red), GUILayout.ExpandWidth(false)))
            {
                DeleteRow(m_ViewIndex - 1);
            }
            /* ============================ */
            GUILayout.EndHorizontal();


            if (m_Database.Rows.Count > 0)
            {
                /* ============================ */
                GUILayout.BeginHorizontal();
                GUIStyle style = new GUIStyle();
                style.alignment = TextAnchor.MiddleRight;
                m_Database.Rows[m_ViewIndex - 1].name = EditorGUILayout.TextField(new GUIContent("Identifier", "The key use to get the value from the database at runtime."), m_Database.Rows[m_ViewIndex - 1].name, GUILayout.ExpandWidth(true));
                
                m_ViewIndex = Mathf.Clamp(EditorGUILayout.IntField("Current Data", m_ViewIndex, GUILayout.ExpandWidth(false)), 1, m_Database.Rows.Count);
                EditorGUILayout.LabelField("of   " + m_Database.Rows.Count.ToString() + "  items", "", style, GUILayout.ExpandWidth(false));
                /* ============================ */
                GUILayout.EndHorizontal();


                foreach (ProceduralData Data in m_Database.Rows[m_ViewIndex - 1].Data)
                {
                    GUILayout.Space(5);
                    GenerateField(m_Database.Rows[m_ViewIndex - 1].GetDynamicData(Data));
                }

                GUILayout.Space(10);
            }           

            if (GUI.changed)
            {
                EditorUtility.SetDirty(m_Database);
            }
        }
        else
        {
            GUILayout.Space(10);
            GUILayout.Label("Create a new database with a defined row structure or open an existing one.");

            if(m_bInvalidRowStructure)
            {
                GUILayout.Label("Please assign one row structure to create a database!", EditorStyles.boldLabel);
            }
        }
        GUILayout.EndArea();

        DrawResizer();

        ProcessEvents(Event.current);
        if (GUI.changed)
        {
            Repaint();
        }
    }


    /** GUI METHODS
     */
    GUIStyle GUI_ColorButtonStyle(Color _Color)
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.normal.textColor = _Color;        
        return style;
    }

    private void DrawMainMenu_HeaderButton()
    {
        GUILayout.BeginHorizontal();
        /* ============================ */
        if (m_Database)
        {
            if (GUILayout.Button("Close Database"))
            {
                CloseDatabase();
            }
        }
        else
        {
            if (GUILayout.Button("New Database"))
            {
                m_bInvalidRowStructure = !CreateDatabase();
            }
        }
        if (GUILayout.Button("Open Database"))
        {
            OpenDatabase();
        }
        if (m_Database)
        {
            if (GUILayout.Button("Browse Databases"))
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = m_Database;
                return;
            }

            Draw_SaveDeleteDatabaseButton();
        }
        /* ============================ */
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    private void DrawMainMenu_DatabaseSettings()
    {
        if (!m_Database)
        {
            m_RowStructure = EditorGUILayout.ObjectField("Row structure", m_RowStructure, typeof(SO_DynamicRow), false) as SO_DynamicRow;
            if (m_RowStructure)
            {
                m_bInvalidRowStructure = false;
            }

            m_bGenerateDataObject = EditorGUILayout.Toggle(new GUIContent("Generate data instance", "Must create an editable instance when a data is added?"), m_bGenerateDataObject, GUILayout.ExpandWidth(false));
        }
    }

    private void Draw_SaveDeleteDatabaseButton()
    {
        if(GUILayout.Button("Save"))
        {
            AssetDatabase.SaveAssets();
        }
        if (GUILayout.Button("Delete", GUI_ColorButtonStyle(Color.red)))
        {
            DeleteDatabase();
        }
    }

    private void Draw_LeftPanel()
    {
        if(m_Database)
        {
            m_LeftPanel = new Rect(0, 0, m_SizeRatio * position.width, position.height);

            GUILayout.BeginArea(m_LeftPanel);
            GUILayout.Label("Data List");

            /**/GUILayout.BeginVertical();/**/
            int count = m_Database.Rows.Count;
            for (int i = 0; i < count; ++i)
            {
                SO_DynamicRow Data = m_Database.Rows[i];
                if (Data && GUILayout.Button(Data.name, GUILayout.MaxWidth(m_SizeRatio * position.width - 2f * m_ResizerSize)))
                {
                    m_ViewIndex = i;
                }
            }
            /**/GUILayout.EndVertical();/**/

            GUILayout.EndArea();            
        }
    }

    private void DrawResizer()
    {
         m_ResizerPanel = new Rect(position.width * m_SizeRatio - m_ResizerSize, 0, m_ResizerSize * 2f, position.height);

         GUILayout.BeginArea(new Rect(m_ResizerPanel.position + Vector2.left * m_ResizerSize, new Vector2(2, position.height)), m_ResizerStyle);
         GUILayout.EndArea();

         EditorGUIUtility.AddCursorRect(m_ResizerPanel, MouseCursor.ResizeHorizontal);
    }
    
    private void ProcessEvents(Event _event)
    {
        switch (_event.type)
        {
            case EventType.MouseDown:
                if (_event.button == 0 && m_ResizerPanel.Contains(_event.mousePosition))
                {
                    m_bIsResizing = true;
                }
                break;
            case EventType.mouseUp:
                m_bIsResizing = false;
                break;
        }
        Resize(_event);
    }

    private void Resize(Event _event)
    {
        if (m_bIsResizing)
        {
            m_SizeRatio = Mathf.Clamp(_event.mousePosition.x / position.width, m_ResizerTreshold * 0.5f, 1 - m_ResizerTreshold);
            Repaint();
        }
    }

    /** CUSTOM METHODS
     */
    bool CreateDatabase()
    {
        if(!m_RowStructure)
        {
            return false;
        }

        m_ViewIndex = 1;

        //SecurePath(ref SaveDatabasePath);
        //string FinalPath = SaveDatabasePath + DatabaseName + ".asset";

        //string extension = m_bGenerateDataObject ? "asset" : "db";
        string absPath = EditorUtility.SaveFilePanel("Save database at...", "Asset", "[ProceduralDatabase]", "asset");
        string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);

        if (m_Database = ProceduralDatabaseUtility.Create(relPath))
        {
            m_Database.bGenerateDataObject = m_bGenerateDataObject;
            m_Database.RowStructure = m_RowStructure;
            m_Database.Rows = new List<SO_DynamicRow>();
            string Path = AssetDatabase.GetAssetPath(m_Database);
            //EditorPrefs.SetString("ObjectPath", Path);
        }

        return true;
    }

    void OpenDatabase()
    {
        m_ViewIndex = 1;
        string absPath = EditorUtility.OpenFilePanel("Open database...", "", "");  
        if (absPath.StartsWith(Application.dataPath))
        {
            string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            m_Database = AssetDatabase.LoadMainAssetAtPath(relPath) as SO_ProceduralRowList;
            //ProceduralDatabase = AssetDatabase.LoadAssetAtPath(relPath, typeof(SO_ProceduralRowList)) as SO_ProceduralRowList;
            if(!m_Database)
            {
                Debug.LogWarning("Failed to load database at location: \"" + relPath + "\"!");
                return;
            }
            if (m_Database.Rows == null)
            {
                m_Database.Rows = new List<SO_DynamicRow>();
            }
            if (m_Database)
            {
                //EditorPrefs.SetString("ObjectPath", relPath);
            }
        }
    }

    private void CloseDatabase()
    {
        AssetDatabase.SaveAssets();
        m_Database = null;
    }

    private void DeleteDatabase()
    {
        if(ProceduralDatabaseUtility.Delete(m_Database))
        {
            m_Database = null;
        }
    }

    string BrowseRowSavePath()
    {
        string absPath = EditorUtility.OpenFolderPanel("Save row structure at...", m_Database.DataSavePath, "asset");
        return absPath.Substring(Application.dataPath.Length - "Assets".Length);
    }

    void AddRow()
    {
        SecurePath(ref m_Database.DataSavePath);
        string RowName = m_Database.name + "_item_" +  m_Database.Rows.Count.ToString();
        SO_DynamicRow NewRow = ProceduralDatabaseUtility.CreateRow(m_Database.DataSavePath + RowName, m_Database.RowStructure, m_Database.bGenerateDataObject);
        NewRow.name = RowName;
        m_Database.Rows.Add(NewRow);
        m_ViewIndex = m_Database.Rows.Count;
    }

    void DeleteRow(int _Index)
    {
        if (!ProceduralDatabaseUtility.Delete(m_Database.Rows[_Index]))
        {
            Debug.LogError("Fail to delete row.");
        }
        m_Database.Rows.RemoveAt(_Index);
    }

    void GenerateField(ProceduralData _Data)
    {
        if(_Data == null)
        {
            Debug.LogWarning("Procedural data is null!");
            return;
        }
        switch(_Data.Type)
        {
            case EDataType.Boolean:
                GenerateField_Boolean(_Data as DynamicData_Boolean);
                break;
            case EDataType.Integer:
                GenerateField_Integer(_Data as DynamicData_Integer);
                break;
            case EDataType.Floating:
                GenerateField_Float(_Data as DynamicData_Float);
                break;
            case EDataType.String:
                GenerateField_String(_Data as DynamicData_String);
                break;
            case EDataType.Gameobject:
                GenerateField_GameObject(_Data as DynamicData_GameObject);
                break;
            case EDataType.Texture2D:
                GenerateField_Texture2D(_Data as DynamicData_Texture2D);
                break;
            default:
                break;
        }

    }

    void GenerateField_Boolean(DynamicData_Boolean _Data)
    {
        _Data.Value = EditorGUILayout.Toggle(_Data.Name, _Data.Value, GUILayout.ExpandWidth(false));
    }
    void GenerateField_Integer(DynamicData_Integer _Data)
    {
        _Data.Value = EditorGUILayout.IntField(_Data.Name, _Data.Value, GUILayout.ExpandWidth(false));
    }
    void GenerateField_Float(DynamicData_Float _Data)
    {
        _Data.Value = EditorGUILayout.FloatField(_Data.Name, _Data.Value, GUILayout.ExpandWidth(false));
    }
    void GenerateField_String(DynamicData_String _Data)
    {
        _Data.Value = EditorGUILayout.TextField(_Data.Name, _Data.Value);
    }
    void GenerateField_GameObject(DynamicData_GameObject _Data)
    {
        _Data.Value = EditorGUILayout.ObjectField(_Data.Name, _Data.Value, typeof(GameObject), false) as GameObject;
    }
    void GenerateField_Texture2D(DynamicData_Texture2D _Data)
    {
        _Data.Value = EditorGUILayout.ObjectField(_Data.Name, _Data.Value, typeof(Texture2D), false) as Texture2D;
    }

    void SecurePath(ref string _Path)
    {
        if (!_Path.EndsWith("/"))
        {
            _Path += "/";
        }
    }
}
