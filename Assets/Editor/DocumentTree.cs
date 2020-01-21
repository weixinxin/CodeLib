using AssetBundleBrowser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

public class DocumentTree : TreeView
{
    const float kToggleWidth = 18f;
    // All columns
    enum Columns
    {
        Folder,
        Bundle
    }
    internal DocumentTree(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
    {
        showBorder = true;
        showAlternatingRowBackgrounds = true;
        ReloadData();
        SetExpanded(ExpandedItems);
    }

    public void ReloadData()
    {
        ExpandedItems.Clear();
        Reload();
    }
    List<int> ExpandedItems = new List<int>();

    internal void ExpandExported()
    {
        SetExpanded(ExpandedItems);
    }

    internal void Search(string searchString)
    {
        List<int> res = new List<int>();
        if (searchString == string.Empty)
        {
            SetExpanded(ExpandedItems);
            SetSelection(res);
        }
        else
        {
            SearchRecursive(searchString.ToLower(), this.rootItem, res);
            SetSelection(res);
        }
    }

    protected bool SearchRecursive(string searchString, TreeViewItem parent,List<int> res)
    {
        bool needExpend = false;
        foreach (DirectoryTreeItem item in parent.children)
        {
            if (item.displayName.ToLower().Contains(searchString))
            {
                needExpend = true;
                res.Add(item.id);
            }
            if(SearchRecursive(searchString, item, res))
            {
                SetExpanded(item.id,true);
                needExpend = true;
            }
            else
                SetExpanded(item.id, false);


        }
        return needExpend;
    }

    public void UnExportAll()
    {
        UnExportRecursive(this.rootItem);
        ReloadData();
        AssetBundleBrowserMain.instance.FreshManageTab();
    }


    void UnExportRecursive(TreeViewItem parent)
    {
        foreach (DirectoryTreeItem item in parent.children)
        {
            item.SetExport(false);
            UnExportRecursive(item);
        }
    }
    public void ClearCustom()
    {
        ClearCustomRecursive(this.rootItem);
        ReloadData();
        AssetBundleBrowserMain.instance.FreshManageTab();
    }

    void ClearCustomRecursive(TreeViewItem parent)
    {
        foreach (DirectoryTreeItem item in parent.children)
        {
            item.ClearCustom();
            ClearCustomRecursive(item);
        }
    }

    protected override TreeViewItem BuildRoot()
    {
        var root = new TreeViewItem(-1, -1);
        DirectoryInfo dir_info = new DirectoryInfo(Application.dataPath);
        BuildDirectoryRecursive(dir_info, root);
        return root;
    }

    bool BuildDirectoryRecursive(DirectoryInfo dir_info, TreeViewItem parent)
    {
        bool needExpend = false;
        DirectoryInfo[] dir_arr = dir_info.GetDirectories();

        parent.children = new List<TreeViewItem>(dir_arr.Length);

        for (int i = 0; i < dir_arr.Length; i++)
        {
            DirectoryInfo current_dir = dir_arr[i];
            DirectoryTreeItem item = new DirectoryTreeItem(parent.depth + 1, current_dir);
            parent.children.Add(item);
            if(item.isExport)
                needExpend = true;
            if (BuildDirectoryRecursive(current_dir, item))
            {
                needExpend = true;
                ExpandedItems.Add(item.id);
            }
            item.Check();
        }

        return needExpend;
    }

    protected override void DoubleClickedItem(int id)
    {
        DirectoryTreeItem item =  FindItem(id, rootItem) as DirectoryTreeItem;
        if(item.isExport)
        {
            AssetBundleBrowserMain.instance.ShowAssetBundle(item.assetBundle);
        }
    }
    protected override void RowGUI(RowGUIArgs args)
    {
        var item = args.item;
        extraSpaceBeforeIconAndLabel = 18f;
        for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
        {
            CellGUI(args.GetCellRect(i), item as DirectoryTreeItem, (Columns)args.GetColumn(i), ref args);
        }
    }

    void CellGUI(Rect cellRect, DirectoryTreeItem item, Columns column, ref RowGUIArgs args)
    {
        // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
        CenterRectUsingSingleLineHeight(ref cellRect);
        Event evt = Event.current;
        switch (column)
        {
            case Columns.Folder:

                Rect toggleRect = cellRect;
                cellRect.width -= 4;
                toggleRect.x += GetContentIndent(args.item);
                toggleRect.width = 16f;
                EditorGUI.BeginChangeCheck();
                bool isExport = EditorGUI.Toggle(toggleRect, item.isExport);
                if (EditorGUI.EndChangeCheck())
                {
                    item.SetExport(isExport);
                    ReloadData();
                    AssetBundleBrowserMain.instance.FreshManageTab();
                }
                args.rowRect = cellRect;
                if (evt.type == EventType.MouseDown && cellRect.Contains(evt.mousePosition))
                    SelectionClick(args.item, false);
                base.RowGUI(args);
                break;
            //case Columns.Export:
            //    Rect toggleRect = cellRect;
            //    toggleRect.x += (cellRect.width - kToggleWidth) * 0.5f;
            //    toggleRect.width = kToggleWidth;
            //    bool isExport = EditorGUI.Toggle(toggleRect, item.isExport);
            //    if(isExport != item.isExport)
            //    {

            //    }
                //DrawRect(cellRect.x, cellRect.y,1, cellRect.height,Color.gray);
                //DrawRect(cellRect.x + cellRect.width, cellRect.y , 1, cellRect.height, Color.gray);
                //break;
            case Columns.Bundle:
                Rect labelRect = cellRect;
                Color oldColor = GUI.color;
                bool NameError = item.isExport && item.curAssetBundle != item.assetBundle;
                if(NameError)
                {
                    //命名规范检测
                    labelRect.x += cellRect.height;
                    Rect messageRect = new Rect(cellRect.x, cellRect.y, cellRect.height, cellRect.height);
                    GUI.Label(messageRect, new GUIContent(MessageSystem.GetIcon(MessageType.Error), "Incorrect name detected"));
                }
                else if(item.warnningMsg != string.Empty)
                {
                    //检测是否包含自定义命名的ab包

                    labelRect.x += cellRect.height;
                    Rect messageRect = new Rect(cellRect.x, cellRect.y, cellRect.height, cellRect.height);
                    GUI.Label(messageRect, new GUIContent(MessageSystem.GetIcon(MessageType.Warning), item.warnningMsg));
                }
                GUI.color = NameError ? Color.red : oldColor;
                EditorGUI.LabelField(labelRect, item.curAssetBundle);
                GUI.color = oldColor;
                //DrawRect(cellRect.x - 4, cellRect.y, 1, cellRect.height, Color.gray);
                break;
        }
    }

    void DrawRect(float x, float y, float width,float height,Color color)
    {
        EditorGUI.DrawRect(new Rect(x, y, width, height), color);
    }
    public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
    {
        var columns = new[]
        {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Folder"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = false,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = treeViewWidth * 0.45f,
                    //minWidth = 200,
                    autoResize = true,
                    canSort = false,
                    allowToggleVisibility = false
                },
                //new MultiColumnHeaderState.Column
                //{
                //    headerContent = new GUIContent(EditorGUIUtility.FindTexture("SettingsIcon"), "勾选则打包目录下子文件"),
                //    contextMenuText = "Export",
                //    headerTextAlignment = TextAlignment.Center,
                //    sortedAscending = false,
                //    sortingArrowAlignment = TextAlignment.Right,
                //    width = 30,
                //    minWidth = 30,
                //    maxWidth = 30,
                //    autoResize = true,
                //    canSort = false,
                //    allowToggleVisibility = true
                //},
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("AssetBundle"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = false,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = treeViewWidth * 0.54f,
                    //minWidth = 150,
                    autoResize = true,
                    canSort = false,
                    allowToggleVisibility = false
                },
            };

        Assert.AreEqual(columns.Length, Enum.GetValues(typeof(Columns)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");

        var state = new MultiColumnHeaderState(columns);
        return state;
    }

}

public class DirectoryTreeItem :TreeViewItem
{
    public string path { get; private set;}
    public string assetBundle { get; private set; }
    public bool isExport { get; private set; }
    public string curAssetBundle { get; private set; }

    public string warnningMsg { get; private set; }

    private DirectoryInfo directoryInfo;

    public DirectoryTreeItem(int depth, DirectoryInfo directoryInfo)
    {
        this.directoryInfo = directoryInfo;
        this.depth = depth;
        this.displayName = directoryInfo.Name;
        string dir_path = directoryInfo.FullName;
        dir_path = dir_path.Replace('\\', '/');
        this.path = dir_path.Replace(Application.dataPath, "Assets");
        this.id = this.path.GetHashCode();
        AssetImporter ai = AssetImporter.GetAtPath(this.path);
        string ab_path = dir_path.Replace(Application.dataPath, "").Substring(1).ToLower();

        this.assetBundle = Regex.Replace(ab_path.Replace('/', '-'), @"\s", "");
        this.isExport = ai.assetBundleName != String.Empty;
        curAssetBundle = ai.assetBundleName;


    }

    public void Check()
    {
        List<string> abs = new List<string>();
        CheckRecursive(abs, this);
        if(abs.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("custom asset bundle detected");
            for (int i =0;i<abs.Count;++i)
            {
                sb.AppendLine(abs[i]);
            }
            warnningMsg = sb.ToString();
        }
        else
        {
            warnningMsg = string.Empty;
        }
    }

    void CheckRecursive(List<string> abs, DirectoryTreeItem parent)
    {
        var files = parent.directoryInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);
        foreach(var file in files)
        {
            string path = file.FullName.Replace('\\', '/').Replace(Application.dataPath, "Assets");
            AssetImporter ai = AssetImporter.GetAtPath(path);
            if(ai!= null && ai.assetBundleName != String.Empty)
            {
                abs.Add(string.Format("[{0}-{1}]", file.Name, ai.assetBundleName));
            }
        }
        foreach (DirectoryTreeItem child in parent.children)
        {
            if(!child.isExport)
            {
                CheckRecursive(abs, child);
            }
        }
    }
    public void SetExport(bool isExport)
    {
        AssetImporter ai = AssetImporter.GetAtPath(this.path);
        ai.assetBundleName = isExport ? this.assetBundle : String.Empty;
        curAssetBundle = ai.assetBundleName;
        this.isExport = isExport;
    }

    public void ClearCustom()
    {
        var files = directoryInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            string path = file.FullName.Replace('\\', '/').Replace(Application.dataPath, "Assets");
            AssetImporter ai = AssetImporter.GetAtPath(path);
            if (ai != null && ai.assetBundleName != String.Empty)
            {
                ai.assetBundleName = string.Empty;
            }
        }
    }
}