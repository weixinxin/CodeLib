using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.TreeViewExamples;

namespace AssetBundleBrowser
{
    public class AssetBundleConfigTab
    {
        [SerializeField]
        TreeViewState m_TreeViewState;
        [SerializeField] MultiColumnHeaderState m_MultiColumnHeaderState;

        DocumentTree m_TreeView;
        
        EditorWindow m_Parent = null;
        Rect m_Position;
        Rect m_HorizontalSplitterRect;
        bool m_ResizingHorizontalSplitter = false;

        [SerializeField]
        float m_HorizontalSplitterPercent;

        const float k_SplitterWidth = 3f;
        const float k_BorderWidth = 3f;
        const float k_ToolsHeight = 50f;
        const float k_SearchBarHeight = 20f;
        
        private static float s_UpdateDelay = 0f;

        SearchField m_searchField;
        string searchString = string.Empty;
        
        public AssetBundleConfigTab()
        {
            m_HorizontalSplitterPercent = 0.4f;
        }

        Rect multiColumnTreeViewRect
        {
            get { return new Rect(20, 30, m_Position.width - 40, m_Position.height - 60); }
        }
        internal void ForceReloadData()
        {
            m_TreeView.SetSelection(new int[0]);
            m_TreeView.ReloadData();
            m_TreeView.ExpandExported();
            m_Parent.Repaint();
        }
        public void OnEnable(Rect pos, EditorWindow parent)
        {
            m_Parent = parent;
            m_Position = pos;

            m_HorizontalSplitterRect = new Rect(
                (int)(m_Position.x + m_Position.width * m_HorizontalSplitterPercent),
                m_Position.y,
                k_SplitterWidth,
                m_Position.height);

            m_searchField = new SearchField();
        }
        
        public void OnGUI(Rect pos)
        {
            m_Position = pos;
            var docTreeRect = new Rect(
                m_Position.x + k_BorderWidth,
                m_Position.y + k_SearchBarHeight,
                m_Position.width - 2 * k_BorderWidth,//m_HorizontalSplitterRect.x - k_BorderWidth,
                m_Position.height - k_BorderWidth - k_ToolsHeight - k_SearchBarHeight);

            var searchBarRect = new Rect(
                docTreeRect.x,
                m_Position.y,
                docTreeRect.width,
                k_SearchBarHeight);
            if (m_TreeView == null)
            {
                bool firstInit = m_MultiColumnHeaderState == null;
                var headerState = DocumentTree.CreateDefaultMultiColumnHeaderState(docTreeRect.width);
                if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
                    MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
                m_MultiColumnHeaderState = headerState;

                if (m_TreeViewState == null)
                    m_TreeViewState = new TreeViewState();

                m_TreeView = new DocumentTree(m_TreeViewState, new MultiColumnHeader(m_MultiColumnHeaderState));
            }
            OnGUISearchBar(searchBarRect);

            HandleHorizontalResize();

            m_TreeView.OnGUI(docTreeRect);

            var bottomRect = new Rect(
                docTreeRect.x,
                docTreeRect.y + docTreeRect.height + k_BorderWidth,
                docTreeRect.width,
                k_ToolsHeight - k_BorderWidth);
            BottomToolBar(bottomRect);
            if (m_ResizingHorizontalSplitter)
                m_Parent.Repaint();
        }

        void OnGUISearchBar(Rect rect)
        {
            EditorGUI.BeginChangeCheck();
            searchString = m_searchField.OnGUI(rect, searchString);
            if(EditorGUI.EndChangeCheck())
            {
                m_TreeView.Search(searchString);
            }
        }

        void BottomToolBar(Rect rect)
        {
            GUILayout.BeginArea(rect);
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    var style = "miniButton";
                    if (GUILayout.Button("Expand Exported", style))
                    {
                        Reset();
                        m_TreeView.ExpandExported();
                    }
                    if (GUILayout.Button("Expand All", style))
                    {
                        Reset();
                        m_TreeView.ExpandAll();
                    }

                    if (GUILayout.Button("Collapse All", style))
                    {
                        Reset();
                        m_TreeView.CollapseAll();
                    }
                }
                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    var style = "miniButton";
                    if (GUILayout.Button("Uncheck All", style))
                    {
                        if (EditorUtility.DisplayDialog("警告", "所有文件夹导出AB包设置都将被清除，确定吗？", "确定", "取消"))
                        {
                            Reset();
                            m_TreeView.UnExportAll();
                        }
                    }
                    if (GUILayout.Button("Reset All", style))
                    {
                        if (EditorUtility.DisplayDialog("警告", "所有形式(自定义、文件夹)导出AB包设置都将被清除，确定吗？", "确定", "取消"))
                        {
                            string[] abNames = AssetDatabase.GetAllAssetBundleNames();
                            for (int i = 0; i < abNames.Length; i++)
                            {
                                AssetDatabase.RemoveAssetBundleName(abNames[i], true);
                            }
                            Reset();
                            m_TreeView.ReloadData();
                            AssetBundleBrowserMain.instance.FreshManageTab();
                        }
                    }

                    if (GUILayout.Button("Clear Custom", style))
                    {
                        if(EditorUtility.DisplayDialog("警告","所有自定义的AB包设置都将被清除，确定吗？","确定","取消"))
                        {
                            Reset();
                            m_TreeView.ClearCustom();
                        }
                    }
                }
            }

            GUILayout.EndArea();
        }


        void Reset()
        {
            searchString = string.Empty;
            m_TreeView.SetSelection(new List<int>());
        }
        private void HandleHorizontalResize()
        {
            m_HorizontalSplitterRect.x = (int)(m_Position.width * m_HorizontalSplitterPercent);
            m_HorizontalSplitterRect.height = m_Position.height;

            EditorGUIUtility.AddCursorRect(m_HorizontalSplitterRect, MouseCursor.ResizeHorizontal);
            if (Event.current.type == EventType.MouseDown && m_HorizontalSplitterRect.Contains(Event.current.mousePosition))
                m_ResizingHorizontalSplitter = true;

            if (m_ResizingHorizontalSplitter)
            {
                m_HorizontalSplitterPercent = Mathf.Clamp(Event.current.mousePosition.x / m_Position.width, 0.1f, 0.9f);
                m_HorizontalSplitterRect.x = (int)(m_Position.width * m_HorizontalSplitterPercent);
            }

            if (Event.current.type == EventType.MouseUp)
                m_ResizingHorizontalSplitter = false;
        }
    }
    
}
