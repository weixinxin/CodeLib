/*
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
public class MenuItemTools
{

    //static string root = string.Empty;
    //[MenuItem(("GameObject/CopyPath"), false, -1)]
    //static public void CopyPath()
    //{
    //    if (Selection.activeGameObject != null)
    //    {
    //        string path = GetUrl(Selection.activeGameObject);
    //        if(root != null && root != string.Empty)
    //            path = path.Replace(root, "");
    //        GUIUtility.systemCopyBuffer = path;
    //        Debug.Log(path);
    //    }

    //}
    //[MenuItem(("GameObject/SetRoot(CopyPath)"), false, -1)]
    //static public void SetRoot()
    //{
    //    if (Selection.activeGameObject != null)
    //    {
    //        root = GetUrl(Selection.activeGameObject) + "/";
    //        Debug.Log(root);
    //    }

    //}

    //static string GetUrl(GameObject obj)
    //{
    //    Transform node = obj.transform;
    //    string path = node.name;
    //    while (node.parent != null)
    //    {
    //        node = node.parent;
    //        path = node.name + "/" + path;
    //    }
    //    return path;
    //}

    //[MenuItem("Assets/Build AssetBundle MyScene")]
    //static void MyBuild()
    //{
    //    // 需要打包的场景名字
    //    string[] path = { "Assets/Scenes/SwitchSceneA.unity" };
    //    BuildPipeline.BuildPlayer(path, Application.dataPath.Replace("Assets", "AseetBundles") + "/Assets-Scenes-SwitchSceneA.unity.unity3d", BuildTarget.StandaloneWindows64, BuildOptions.BuildAdditionalStreamedScenes);

    //    string[]  path1 = { "Assets/Scenes/SwitchSceneB.unity" };
    //    BuildPipeline.BuildPlayer(path1, Application.dataPath.Replace("Assets", "AseetBundles") + "/Assets-Scenes-SwitchSceneB.unity.unity3d", BuildTarget.StandaloneWindows64, BuildOptions.BuildAdditionalStreamedScenes);
    //    // 刷新，可以直接在Unity工程中看见打包后的文件
    //    AssetDatabase.Refresh();

    //}
}
*/