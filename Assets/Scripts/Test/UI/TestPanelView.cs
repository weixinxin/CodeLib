using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestPanelView : MonoBehaviour
{
    public Button BackButton;
    public Button FullButton;
    public Button PartialButton;
    public Image Background;

    private void Awake()
    {
        Background.color = Random.ColorHSV(0, 1, 0, 1, 0, 1, 1, 1);
    }
}
