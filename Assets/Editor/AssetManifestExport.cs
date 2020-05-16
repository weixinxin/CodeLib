using System.Collections.Generic;
using System.IO;
using System.Text;

public static class AssetManifestExport
{
    public static byte[] Export()
    {
        byte[] result;
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryWriter bw = new BinaryWriter(ms);
            string[] boundleNames = UnityEditor.AssetDatabase.GetAllAssetBundleNames();
            bw.Write(boundleNames.Length);
            foreach (string boundleName in boundleNames)
            {
                WriteString(bw, boundleName);
                string[] paths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(boundleName);
                bw.Write(paths.Length);
                foreach (var path in paths)
                {
                    WriteString(bw, path);
                }
            }
            result = ms.ToArray();
        }
        return result;
    }
    static void WriteString(BinaryWriter bw, string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        bw.Write(bytes.Length);
        bw.Write(bytes);
    }

}