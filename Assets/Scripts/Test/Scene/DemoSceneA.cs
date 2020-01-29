using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework;
public class DemoSceneA : BaseScene
{
    protected override string url => "Scenes/SwitchSceneA.unity";

    protected override IEnumerator OnEnter(params object[] args)
    {
        UIManager.Instance.OpenUI("TestPanelAf");
        yield return null;
    }

    protected override IEnumerator OnExit()
    {
        yield return null;
    }
}
