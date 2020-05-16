using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework;
public class DemoSceneB : BaseScene
{
    protected override string url => "Assets/Scenes/SwitchSceneB.unity";

    protected override IEnumerator OnEnter(params object[] args)
    {
        UIManager.Instance.OpenUI("TestPanelBf");
        yield return null;
    }

    protected override IEnumerator OnExit()
    {
        yield return null;
    }
}
