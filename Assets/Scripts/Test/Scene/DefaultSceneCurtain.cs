using System;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using System.Collections;
using UnityEngine.UI;

public class DefaultSceneCurtain : MonoBehaviour, ISceneCurtain
{
    [SerializeField]
    Image curtain;
    public IEnumerator Falls()
    {
        curtain.CrossFadeAlpha(1, 1, true);
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator Raise()
    {
        curtain.CrossFadeAlpha(0, 1, true);
        yield return new WaitForSeconds(1f);
    }
}