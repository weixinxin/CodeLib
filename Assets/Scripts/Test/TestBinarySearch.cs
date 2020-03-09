using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class TestBinarySearch : MonoBehaviour
{
    public List<float> list;
    public float target;

    [Button(Name = "Insert")]
    void Search()
    {
        //MathLib.BinaryInsert(list, target);
    }
}
