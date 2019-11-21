using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SegmentPointDistanceTest : MonoBehaviour
{
    public Transform point;
    public Transform a;
    public Transform b;

    public float radius;
    

    private void OnDrawGizmos()
    {
        var va = a.position;
        va.z = 0;

        var vb = b.position;
        vb.z = 0;

        Gizmos.DrawLine(va,vb);

        var vp = point.position;
        vp.z = 0;
        float radius = MathLib.DistanceOfPointAndSegment(vp, va, vb);
        Gizmos.DrawWireSphere(vp, radius);

        radius = MathLib.DistanceOfPointAndline(vp, va, vb);
        Gizmos.DrawWireSphere(vp, radius);


        Stopwatch sw = new Stopwatch();
        bool b0 = false;
        sw.Start();
        for (int i = 0; i < 100000; ++i)
        {
            b0 = MathLib.isSegmentThroughCircle(vp, this.radius, va, vb);
        }
        sw.Stop();
        UnityEngine.Debug.Log("method =" + sw.ElapsedMilliseconds.ToString());
        Gizmos.color = b0 ? Color.red : Color.green;
        Gizmos.DrawWireSphere(vp, this.radius);
    }

}
