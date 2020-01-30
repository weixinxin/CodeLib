using System;
using UnityEngine;

public static class CameraUtility
{

    /// <summary>
    /// 屏幕坐标换算成地面坐标
    /// </summary>
    /// <param name="camera">相机</param>
    /// <param name="sx">屏幕坐标x</param>
    /// <param name="sy">屏幕坐标y</param>
    /// <param name="height">地面高度</param>
    /// <returns></returns>
    public static Vector3 ScreenToGround(Camera camera, float sx, float sy, float height)
    {
        sy = sy / Screen.height;
        sx = sx / Screen.width;
        float halfHeight = camera.orthographic ? camera.orthographicSize : Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad * 0.5f);
        float halfWidth = halfHeight * Screen.width / Screen.height;
        float x = halfWidth * (2 * sx - 1);
        float y = halfHeight * (2 * sy - 1);
        Vector3 p12 = camera.orthographic ? camera.transform.forward : camera.transform.rotation * new Vector3(x, y, 1);
        if (p12.y != 0)
        {
            Vector3 p1 = camera.orthographic ? camera.transform.position + camera.transform.rotation * new Vector3(x, y, 0) : camera.transform.position;
            return p1 + p12 * ((height - p1.y) / p12.y);
        }
        return Vector3.zero;

    }
}

public class CameraDragHelper
{
    Vector3 temp_pos;
    Vector3 temp_offset;
    Vector3 forward;
    Quaternion rotation;
    Vector3 position;
    Camera camera;
    Vector3 ScreenToGround(Vector2 sp)
    {
        float sy = sp.y / Screen.height;
        float sx = sp.x / Screen.width;
        float halfHeight = camera.orthographic ? camera.orthographicSize : Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad * 0.5f);
        float halfWidth = halfHeight * Screen.width / Screen.height;
        float x = halfWidth * (2 * sx - 1);
        float y = halfHeight * (2 * sy - 1);
        Vector3 p12 = camera.orthographic ? forward : rotation * new Vector3(x, y, 1);
        if (p12.y != 0)
        {
            Vector3 p1 = camera.orthographic ? position + rotation * new Vector3(x, y, 0) : position;
            return p1 - p12 * (p1.y / p12.y);
        }
        return Vector3.zero;

    }

    /// <summary>
    /// 拖拽开始时调用
    /// </summary>
    /// <param name="camera">主相机</param>
    /// <param name="sp">屏幕坐标</param>
    /// <param name="pos">虚拟相机坐标</param>
    public void OnDragBegin(Camera camera, Vector2 sp,Vector3 pos)
    {
        this.camera = camera;
        var transform = camera.transform;
        forward = transform.forward;
        rotation = transform.rotation;
        position = transform.position;
        temp_offset = ScreenToGround(sp);
        temp_pos = pos;
    }

    /// <summary>
    /// 拖动处理函数
    /// </summary>
    /// <param name="sp">屏幕坐标</param>
    /// <returns>虚拟相机坐标</returns>
    public Vector3 OnDraging(Vector2 sp)
    {
        var n_offset = ScreenToGround(sp);
        return temp_pos - (n_offset - temp_offset);
    }
}


