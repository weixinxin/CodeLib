using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class TestSmoothCurve : MonoBehaviour
{
    public List<Transform> points;
    public float sampe;
    public float k;
    public int sampe2;

    public bool show;
    List<Vector3> vector3s = new List<Vector3>();
    List<Vector3> vector3s1 = new List<Vector3>();
    List<Vector3> vector3s2 = new List<Vector3>();



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
        MathLib.SmoothCurve(vector3s.ToArray(), sampe, k, vector3s1);
        MathLib.SmoothCurve(vector3s.ToArray(), sampe2, k, vector3s2);
    }

    private void OnDrawGizmos()
    {
        if (points != null && vector3s != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < points.Count; i++)
                Gizmos.DrawWireSphere(points[i].position, 0.2f);
            Gizmos.color = Color.green;
            for (int i = 0; i < vector3s2.Count - 1; i++)
            {
                Gizmos.DrawLine(vector3s2[i] - Vector3.up, vector3s2[i + 1] - Vector3.up);
                Gizmos.DrawWireSphere(vector3s2[i] - Vector3.up, 0.05f);
            }

            Gizmos.color = Color.white;
            for (int i = 0; i < vector3s1.Count - 1; i++)
            {
                Gizmos.DrawLine(vector3s1[i], vector3s1[i + 1]);
                Gizmos.DrawWireSphere(vector3s1[i], 0.05f);
            }
            Gizmos.color = Color.blue;
            for (int i = 0; i < vector3s.Count - 1; i++)
            {
                Gizmos.DrawLine(vector3s[i], vector3s[i + 1]);
                Gizmos.DrawWireSphere(vector3s[i], 0.05f);
            }

        }
    }

}
