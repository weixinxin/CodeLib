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
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public IEnumerator Falls()
    {
        gameObject.SetActive(true);
        float scale = 0;
        Color color = curtain.color;
        while (scale < 1)
        {
            scale += Time.deltaTime * 2;
            color.a = scale;
            curtain.color = color;
            yield return null;
        }
    }

    public IEnumerator Raise()
    {
        float scale = 1;
        Color color = curtain.color;
        while (scale > 0)
        {
            scale -= Time.deltaTime * 2;
            color.a = scale;
            curtain.color = color;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}