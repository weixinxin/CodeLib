using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

class AssetMainifestBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        byte[] bytes = AssetManifest.Export();
        File.WriteAllBytes($"{report.summary.outputPath}/AssetManifest", bytes);
    }
}