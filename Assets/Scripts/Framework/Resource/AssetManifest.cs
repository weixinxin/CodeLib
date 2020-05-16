using System.Collections.Generic;
using System.IO;
using System.Text;

public static class AssetManifest
{
#if UNITY_EDITOR
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
#endif
    static void WriteString(BinaryWriter bw, string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        bw.Write(bytes.Length);
        bw.Write(bytes);
    }

    static string ReadString(BinaryReader br)
    {
        int length = br.ReadInt32();
        byte[] bytes = br.ReadBytes(length);
        return Encoding.UTF8.GetString(bytes);
    }

    public static Dictionary<string, string> Import(byte[] bytes)
    {
        Dictionary<string, string> manifest;
        using (MemoryStream ms = new MemoryStream(bytes))
        {
            BinaryReader br = new BinaryReader(ms);
            int boundleCount = br.ReadInt32();
            manifest = new Dictionary<string, string>(boundleCount);
            for (int i = 0; i < boundleCount; ++i)
            {
                string boundleName = ReadString(br);
                int assetCount = br.ReadInt32();
                for (int n = 0; n < assetCount; ++n)
                {
                    string assetPath = ReadString(br);
                    manifest.Add(assetPath, boundleName);
                }
            }
        }
        return manifest;
    }
}