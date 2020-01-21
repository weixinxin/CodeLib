using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetLabelsTools
{
    [MenuItem("Assets/Generate Asset Bundle Name")]
    static public void CopyPath()
    {
        if (Selection.activeObject != null)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            AssetImporter ai = AssetImporter.GetAtPath(path);
            if(ai != null)
            {
                ai.assetBundleName = UnityUtility.ConvertAssetPathToBundleName(path.Replace("Assets/",""));
            }
        }

    }
    
}
