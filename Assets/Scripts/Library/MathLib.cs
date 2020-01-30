using System;
using System.Collections.Generic;
using UnityEngine;

public static class MathLib
{
    /// <summary>
    /// 求直线与平面的交点
    /// </summary>
    /// <param name="planeNormal">平面的法线向量</param>
    /// <param name="planePoint"></param>
    /// <param name="lineVector"></param>
    /// <param name="linePoint"></param>
    /// <returns></returns>
    public static Vector3 CalPlaneLineIntersectPoint(Vector3 planeNormal, Vector3 planePoint, Vector3 lineVector, Vector3 linePoint)
    {
        Vector3 returnResult = Vector3.zero;

        var vpt = lineVector.x * planeNormal.x + lineVector.y * planeNormal.y + lineVector.z * planeNormal.z;
        //首先判断直线是否与平面平行
        if (vpt != 0)
        {
            var t = ((planePoint.x - linePoint.x) * planeNormal.x + (planePoint.y - linePoint.y) * planeNormal.y + (planePoint.z - linePoint.z) * planeNormal.z) / vpt;
            returnResult.x = linePoint.x + lineVector.x * t;
            returnResult.y = linePoint.y + lineVector.y * t;
            returnResult.z = linePoint.z + lineVector.z * t;
        }
        return returnResult;
    }

    /// <summary>
    /// 判断两条线段是否相交
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public static bool IsSegmentCross(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        Vector3 ac = a - c;
        Vector3 dc = d - c;
        Vector3 bc = b - c;
        Vector3 da = d - a;
        Vector3 ba = b - a;
        Vector3 ca = c - a;
        return (SimpleCross(ac, dc) * SimpleCross(bc, dc) <= 0) && (SimpleCross(da, ba) * SimpleCross(ca, ba) <= 0);
    }
    static float SimpleCross(Vector3 a, Vector3 b)
    {
        return a.x * b.z - a.z * b.x;
    }

    /// <summary>
    /// 功能：判断点是否在多边形内 
    /// 方法：求解通过该点的水平线与多边形各边的交点 
    /// 结论：单边交点为奇数，成立!
    /// </summary>
    /// <param name="p">指定的某个点</param>
    /// <param name="ptPolygon">多边形的各个顶点坐标（首末点可以不一致）</param>
    /// <returns>点是否在多边形内</returns>

    public static bool PtInPolygon(float p_x, float p_z, List<Vector3> ptPolygon)
    {
        if (ptPolygon == null)
        {
            Debug.LogError("error [PtInPolygon] ptPolygon == null");
            return false;
        }
        int nCross = 0;

        for (int i = 0; i < ptPolygon.Count; i++)
        {
            Vector3 p1 = ptPolygon[i];
            Vector3 p2 = ptPolygon[(i + 1) % ptPolygon.Count];

            if (p1.z == p2.z)
                continue;

            if (p_z < Mathf.Min(p1.z, p2.z))
                continue;
            if (p_z >= Mathf.Max(p1.z, p2.z))
                continue;

            double x = (double)(p_z - p1.z) * (double)(p2.x - p1.x) / (double)(p2.z - p1.z) + p1.x;

            if (x > p_x)
                nCross++;
        }

        // 单边交点为偶数，点在多边形之外
        return (nCross % 2 == 1);
    }

    /// <summary>
    /// 求点到线段的最短距离
    /// 向量法，计算量少
    /// </summary>
    /// <param name="point">点</param>
    /// <param name="a">端点A</param>
    /// <param name="b">端点B</param>
    /// <returns></returns>
    public static float DistanceOfPointAndSegment(Vector2 point, Vector2 a, Vector2 b)
    {
        float ap_x = point.x - a.x;
        float ap_y = point.y - a.y;
        float ab_x = b.x - a.x;
        float ab_y = b.y - a.y;
        float r = (ap_x * ab_x + ap_y * ab_y) / (ab_x * ab_x + ab_y * ab_y);

        if (r <= 0)
        {
            return Mathf.Sqrt(ap_x * ap_x + ap_y * ap_y);
        }
        else if (r >= 1)
        {
            float bp_x = point.x - b.x;
            float bp_y = point.y - b.y;
            return Mathf.Sqrt(bp_x * bp_x + bp_y * bp_y);
        }
        else
        {
            float cp_x = ap_x - r * ab_x;
            float cp_y = ap_y - r * ab_y;
            return Mathf.Sqrt(cp_x * cp_x + cp_y * cp_y);
        }

    }

    /// <summary>
    /// 求点到直线的最短距离
    /// </summary>
    /// <param name="point">点</param>
    /// <param name="a">直线上A点</param>
    /// <param name="b">直线上B点</param>
    /// <returns></returns>
    public static float DistanceOfPointAndline(Vector2 point, Vector2 a, Vector2 b)
    {
        float ap_x = point.x - a.x;
        float ap_y = point.y - a.y;
        float ab_x = b.x - a.x;
        float ab_y = b.y - a.y;
        float r = (ap_x * ab_x + ap_y * ab_y) / (ab_x * ab_x + ab_y * ab_y);

        float cp_x = ap_x - r * ab_x;
        float cp_y = ap_y - r * ab_y;
        return Mathf.Sqrt(cp_x * cp_x + cp_y * cp_y);
    }

    /// <summary>
    /// 比较两个浮点型是否相等
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool Approximately(float a, float b)
    {
        return a >= b - float.Epsilon && a <= b + float.Epsilon;
    }

    /// <summary>
    /// 线段是否与圆相交
    /// </summary>
    /// <param name="point"></param>
    /// <param name="radius"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool IsSegmentThroughCircle(Vector2 point, float radius, Vector2 a, Vector2 b)
    {
        float ap_x = point.x - a.x;
        float ap_y = point.y - a.y;
        float ab_x = b.x - a.x;
        float ab_y = b.y - a.y;
        float r = (ap_x * ab_x + ap_y * ab_y) / (ab_x * ab_x + ab_y * ab_y);

        if (r <= 0)
        {
            return ap_x * ap_x + ap_y * ap_y <= radius * radius;
        }
        else if (r >= 1)
        {
            float bp_x = point.x - b.x;
            float bp_y = point.y - b.y;
            return bp_x * bp_x + bp_y * bp_y <= radius * radius;
        }
        else
        {
            float cp_x = ap_x - r * ab_x;
            float cp_y = ap_y - r * ab_y;
            return cp_x * cp_x + cp_y * cp_y <= radius * radius;
        }
    }

    public static Vector3 Bezier2(float t, Vector3 P0, Vector3 P1, Vector3 P2)
    {
        float u = 1 - t;
        float uu = u * u;
        float tt = t * t;

        return uu * P0 + 2 * u * t * P1 + tt * P2;
    }

    public static Vector3 Bezier3(float t, Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3)
    {
        float u = 1 - t;
        float uu = u * u;
        float tt = t * t;
        float ttt = tt * t;
        float uuu = uu * u;
        return uuu * P0 + 3 * uu * t * P1 + 3 * u * tt * P2 + ttt * P3;
    }

    static List<Vector3> curvePoint = new List<Vector3>();
    static List<float> segmentLength = new List<float>();

    /// <summary>
    /// 生成平滑曲线
    /// </summary>
    /// <param name="ls">曲线经过点</param>
    /// <param name="smoothSample">过渡插值数</param>
    /// <param name="k">曲线弯曲程度</param>
    /// <returns></returns>
    public static void SmoothCurve(Vector3[] ls, int smoothSample, float k, List<Vector3> result)
    {
        int capacity = ls.Length + smoothSample * (ls.Length - 1);

        if (result == null)
        {
            return;
        }
        result.Clear();
        if (capacity > result.Capacity)
            result.Capacity = capacity;

        if (ls.Length < 3)
        {
            result.AddRange(ls);
            return;
        }
        curvePoint.Clear();
        segmentLength.Clear();
        if (segmentLength.Capacity < ls.Length - 1)
            segmentLength.Capacity = ls.Length - 1;
        curvePoint.Add(ls[0]);
        for (int i = 0; i < ls.Length - 1; i++)
            segmentLength.Add((ls[i + 1] - ls[i]).magnitude);
        for (int i = 1; i < ls.Length - 1; i++)
        {
            var offset = (ls[i - 1] - ls[i + 1]) * k * 0.5f;
            var ab = 1.0f / (segmentLength[i] + segmentLength[i - 1]);
            curvePoint.Add(ls[i] + offset * segmentLength[i - 1] * ab);
            curvePoint.Add(ls[i]);
            curvePoint.Add(ls[i] - offset * segmentLength[i] * ab);
        }
        curvePoint.Add(ls[ls.Length - 1]);

        float unit = 1.0f / (smoothSample + 1);
        result.Add(ls[0]);
        for (int i = 1; i <= smoothSample; ++i)
        {
            var p = Bezier2(i * unit, curvePoint[0], curvePoint[1], curvePoint[2]);
            result.Add(p);
        }
        for (int i = 1; i < ls.Length - 2; i++)
        {
            result.Add(ls[i]);
            int idx = (i - 1) * 3 + 2;
            for (int n = 1; n <= smoothSample; ++n)
            {
                var p = Bezier3(n * unit, curvePoint[idx], curvePoint[idx + 1], curvePoint[idx + 2], curvePoint[idx + 3]);
                result.Add(p);
            }
        }
        result.Add(ls[ls.Length - 2]);
        int _index = curvePoint.Count - 3;
        for (int i = 1; i <= smoothSample; ++i)
        {
            var p = Bezier2(i * unit, curvePoint[_index], curvePoint[_index + 1], curvePoint[_index + 2]);
            result.Add(p);
        }
        result.Add(ls[ls.Length - 1]);
    }


    /// <summary>
    /// 固定间隔距离采样进行平滑曲线
    /// </summary>
    /// <param name="ls">曲线经过点</param>
    /// <param name="sampleDis">采样间隔距离</param>
    /// <param name="k">曲线弯曲程度</param>
    /// <returns></returns>
    public static void SmoothCurve(Vector3[] ls, float sampleDis, float k, List<Vector3> result)
    {
        if (result == null)
        {
            return;
        }
        result.Clear();

        if (ls.Length < 3)
        {
            result.AddRange(ls);
            return;
        }
        curvePoint.Clear();
        segmentLength.Clear();
        if (segmentLength.Capacity < ls.Length - 1)
            segmentLength.Capacity = ls.Length - 1;
        curvePoint.Add(ls[0]);
        for (int i = 0; i < ls.Length - 1; i++)
            segmentLength.Add((ls[i + 1] - ls[i]).magnitude);
        for (int i = 1; i < ls.Length - 1; i++)
        {
            var offset = (ls[i - 1] - ls[i + 1]) * k * 0.5f;
            var ab = 1.0f / (segmentLength[i] + segmentLength[i - 1]);
            curvePoint.Add(ls[i] + offset * segmentLength[i - 1] * ab);
            curvePoint.Add(ls[i]);
            curvePoint.Add(ls[i] - offset * segmentLength[i] * ab);
        }
        curvePoint.Add(ls[ls.Length - 1]);

        int smoothSample = Mathf.RoundToInt(segmentLength[0] / sampleDis);
        float unit = 1.0f / (smoothSample + 1);
        result.Add(ls[0]);
        for (int i = 1; i <= smoothSample; ++i)
        {
            var p = Bezier2(i * unit, curvePoint[0], curvePoint[1], curvePoint[2]);
            result.Add(p);
        }
        for (int i = 1; i < ls.Length - 2; i++)
        {
            result.Add(ls[i]);
            smoothSample = Mathf.RoundToInt(segmentLength[i] / sampleDis);
            unit = 1.0f / (smoothSample + 1);
            int idx = (i - 1) * 3 + 2;
            for (int n = 1; n <= smoothSample; ++n)
            {
                var p = Bezier3(n * unit, curvePoint[idx], curvePoint[idx + 1], curvePoint[idx + 2], curvePoint[idx + 3]);
                result.Add(p);
            }
        }
        result.Add(ls[ls.Length - 2]);
        smoothSample = Mathf.RoundToInt(segmentLength[ls.Length - 2] / sampleDis);
        unit = 1.0f / (smoothSample + 1);
        int _index = curvePoint.Count - 3;
        for (int i = 1; i <= smoothSample; ++i)
        {
            var p = Bezier2(i * unit, curvePoint[_index], curvePoint[_index + 1], curvePoint[_index + 2]);
            result.Add(p);
        }
        result.Add(ls[ls.Length - 1]);
    }

    public static void SmoothCorner(Vector3 a, Vector3 b, Vector3 c, int smoothSample, float k, List<Vector3> result)
    {
        if (result == null)
        {
            return;
        }
        result.Clear();

        Vector3 b_1 = Vector3.Lerp(a, b, k);
        Vector3 b_2 = Vector3.Lerp(c, b, k);
        float unit = 1.0f / (smoothSample + 1);
        for (int n = 1; n <= smoothSample; ++n)
        {
            var p = Bezier3(n * unit, a, b_1, b_2, c);
            result.Add(p);
        }
    }

    /// <summary>
    /// 平滑折线转角
    /// </summary>
    /// <param name="ls">折线点集</param>
    /// <param name="smoothSample">平滑插值数</param>
    /// <param name="k">平滑距离</param>
    /// <param name="result">存储列表(为空则用共享列表)</param>
    /// <returns>平滑后的点集</returns>
    public static void SmoothCorner(Vector3[] ls, int smoothSample, float k, List<Vector3> result)
    {
        if (result == null)
        {
            return;
        }
        result.Clear();
        if (k <= 0)
        {
            result.AddRange(ls);
            return;
        }
        result.Add(ls[0]);
        curvePoint.Clear();
        if (curvePoint.Capacity < ls.Length - 1)
            curvePoint.Capacity = ls.Length - 1;
        segmentLength.Clear();
        if (segmentLength.Capacity < ls.Length - 1)
            segmentLength.Capacity = ls.Length - 1;
        for (int i = 1; i < ls.Length; ++i)
        {
            Vector3 segment = ls[i] - ls[i - 1];
            curvePoint.Add(segment.normalized);
            segmentLength.Add(segment.magnitude * 0.5f);
        }

        for (int i = 1; i < ls.Length - 1; ++i)
        {
            Vector3 b = ls[i];
            Vector3 b_1 = b - curvePoint[i - 1] * Mathf.Min(k, segmentLength[i - 1]);
            Vector3 b_2 = b + curvePoint[i] * Mathf.Min(k, segmentLength[i]);

            float unit = 1.0f / (smoothSample + 1);
            for (int n = 0; n <= smoothSample; ++n)
            {
                var p = Bezier3(n * unit, b_1, b, b, b_2);
                result.Add(p);
            }
        }
        result.Add(ls[ls.Length - 1]);
    }

    /// <summary>
    /// 二分查找将数据插入到有序列表中
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="item"></param>
    public static void BinaryInsert<T>(List<T> list,T item)where T : IComparable
    {
        int left = 0;
        int right = list.Count - 1;
        while(left <= right)
        {
            int mid = (left + right) / 2;
            int res = list[mid].CompareTo(item);
            if(res == 0)
            {
                list.Insert(mid, item);
                return;
            }
            else if(res > 0)
            {
                right = mid - 1;
            }
            else
            {
                left = mid + 1;
            }
        }
        list.Insert(left, item);
    }

    /// <summary>
    /// 判断向量方向是否为顺时针方向
    /// 180度时候返回true
    /// </summary>
    /// <param name="from">起点向量</param>
    /// <param name="to">终点向量</param>
    /// <returns></returns>
    public static bool IsClockwiseOnPlane(Vector3 from, Vector3 to)
    {
        float cross = from.x * to.z - to.x * from.z;
        return cross <= 0;
    }

    /// <summary>
    /// 在平面上顺时针旋转向量
    /// </summary>
    /// <param name="vector">向量</param>
    /// <param name="angle">旋转角度，正数为顺时针角度，负数为逆时针角度</param>
    /// <returns></returns>
    public static Vector3 RotateOnPlane(Vector3 vector, float angle)
    {
        float radian = Mathf.Deg2Rad * angle;
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);
        float x = vector.x;
        vector.x = vector.z * sin + x * cos;
        vector.z = vector.z * cos - x * sin;
        return vector;
    }

    /// <summary>
    /// 返回两向量的水平面上的旋转角度
    /// </summary>
    /// <param name="from">起点向量</param>
    /// <param name="to">终点向量</param>
    /// <returns>正数为顺时针角度，负数为逆时针角度</returns>
    public static float SignedAngleOnPlane(Vector3 from, Vector3 to)
    {
        from.y = 0;
        to.y = 0;
        return Vector3.SignedAngle(from, to, Vector3.up);
    }
}
