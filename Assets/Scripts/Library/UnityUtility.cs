using System;
using UnityEngine;

public static class UnityUtility
{
    public static bool IsSceneInBuildSetting(string sceneName)
    {
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; ++i)
        {
            string path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            if (path.Contains(sceneName))
            {
                return true;
            }
        }
        return false;
    }

    public static string TrimAll(this string str)
    {
        return System.Text.RegularExpressions.Regex.Replace(str, @"\s", "");
    }


    public static string ConvertAssetPathToBundleName(string path)
    {
        string assetBundleName = path.Replace('/', '-');
        assetBundleName = assetBundleName.Replace('\\', '-').ToLower().TrimAll();
        return assetBundleName;
    }
}
