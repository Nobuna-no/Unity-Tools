using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

public class ResizablePanels : EditorWindow
{
    /** VARIABLES
     */
    private Rect m_UpperPanel;
    private Rect m_LowerPanel;
    private Rect m_Resizer;

    private float m_SizeRatio = 0.5f;
    private bool m_bIsResizing;
    private float m_ResizerHeight = 3f;

    private GUIStyle m_ResizerStyle;

    /** UNITY EDITOR METHODS
     */
    [MenuItem("Window/Custom Console %#0")]
    private static void Init()
    {
        ResizablePanels window = GetWindow<ResizablePanels>();
        window.titleContent = new GUIContent("Console");
    }

    private void OnEnable()
    {
        m_ResizerStyle = new GUIStyle();
        m_ResizerStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;
    }

    private void OnGUI()
    {
        DrawUpperPanel();
        DrawLowerPanel();
        DrawResizer();

        ProcessEvents(Event.current);

        if (GUI.changed)
        {
            Repaint();
        }
    }

    
    /** CUSTOM METHODS
     */
    private void DrawUpperPanel()
    {
        m_UpperPanel = new Rect(0, 0, position.width, position.height * m_SizeRatio);

        GUILayout.BeginArea(m_UpperPanel);
        GUILayout.Label("Upper Panel");
        GUILayout.EndArea();
    }

    private void DrawResizer()
    {
        m_Resizer = new Rect(0, (position.height * m_SizeRatio) - m_ResizerHeight, position.width, m_ResizerHeight * 2f);

        GUILayout.BeginArea(new Rect(m_Resizer.position + Vector2.up * m_ResizerHeight, new Vector2(position.width, 2)), m_ResizerStyle);
        GUILayout.EndArea();

        EditorGUIUtility.AddCursorRect(m_Resizer, MouseCursor.ResizeVertical);
    }

    private void DrawLowerPanel()
    {
        m_LowerPanel = new Rect(0, position.height * m_SizeRatio, position.width, position.height * (1 - m_SizeRatio) - m_ResizerHeight);

        GUILayout.BeginArea(m_LowerPanel);
        GUILayout.Label("Lower Panel");
        GUILayout.EndArea();
    }

    private void ProcessEvents(Event _event)
    {
        switch (_event.type)
        {
            case EventType.MouseDown:
                if (_event.button == 0 && m_Resizer.Contains(_event.mousePosition))
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
            m_SizeRatio = Mathf.Clamp(_event.mousePosition.y / position.height, 0.1f, 0.9f);
            Repaint();
        }
    }


}
