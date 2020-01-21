using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class TestSmoothCorner : MonoBehaviour
{
    public List<Transform> points;
    public int sampe;
    public float k;
    List<Vector3> vector3s = new List<Vector3>();
    List<Vector3> curve = new List<Vector3>();


    void Update()
    {
        vector3s.Clear();
        foreach (var p in points)
        {
            Vector3 v3 = p.position;
            v3.y = 0;
            vector3s.Add(v3);
            p.position = v3;
        }
        if (points.Count >= 3)
            MathLib.SmoothCorner(vector3s.ToArray(), sampe, k, curve);
    }
    private void OnDrawGizmos()
    {
        if (points != null && vector3s != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < points.Count; i++)
                Gizmos.DrawWireSphere(points[i].position, 0.2f);
            Gizmos.color = Color.blue;
            for (int i = 0; i < vector3s.Count - 1; i++)
            {
                Gizmos.DrawLine(vector3s[i], vector3s[i + 1]);
                Gizmos.DrawWireSphere(vector3s[i], 0.05f);
            }
            Gizmos.color = Color.green;
            for (int i = 0; i < curve.Count - 1; i++)
            {
                Gizmos.DrawLine(curve[i], curve[i + 1]);
                Gizmos.DrawWireSphere(curve[i], 0.05f);
            }
        }
    }
}
