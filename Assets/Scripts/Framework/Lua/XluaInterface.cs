using System;
using System.Collections.Generic;
using System.IO;
using Framework;
using XLua;

public class XluaInterface : ILuaInterface
{
    readonly LuaEnv m_LuaEnv;
    public object LuaEnv => m_LuaEnv;
    const string PATH = "Assets/LuaScripts/";

    public XluaInterface()
    {
        m_LuaEnv = new LuaEnv();
        m_LuaEnv.AddLoader(LuaLoadCallback);
    }

    private byte[] LuaLoadCallback(ref string outFileName)
    {
#if UNITY_EDITOR
        return GetEditorLuaFile(outFileName);
#else
        return GetEditorLuaFile(outFileName);
#endif
    }


    private byte[] GetEditorLuaFile(string fileName)
    {
        fileName = fileName.Replace('.', '/') + ".lua";
        string path = PATH + fileName;

        using (Stream input = new FileStream(path, FileMode.Open))
        {
            if (input != null)
            {
                int count = (int)input.Length;
                byte[] data = new byte[count];
                if (input.Read(data, 0, count) != count)
                {
                    Debug.LogError("读取lua文件出现错误！");
                }
                input.Close();

                return data;
            }
            else
            {
                Debug.LogError("读取lua文件出现错误！");
                return null;
            }
        }
    }

    public object[] DoString(string chunk, string chunkName)
    {
        return m_LuaEnv.DoString(chunk, chunkName);
    }

    public void Update(float deltaTime, float unscaledDeltaTime)
    {
        m_LuaEnv.Tick();
    }

    public void FixedUpdate(float deltaTime, float unscaledDeltaTime)
    {

    }

    public void LateUpdate(float deltaTime, float unscaledDeltaTime)
    {

    }

    public void GC()
    {
        m_LuaEnv.GC();
    }

    public T Get<T>(string path)
    {
        return m_LuaEnv.Global.GetInPath<T>(path);
    }

    public void Set<T>(string path, T val)
    {
        m_LuaEnv.Global.SetInPath(path, val);
    }

    public void OnDestroy()
    {
        m_LuaEnv.Dispose();
    }

}