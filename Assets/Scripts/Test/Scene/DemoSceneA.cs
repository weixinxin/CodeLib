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

    public override IEnumerator OnEnter(params object[] args)
    {
        yield return null;
    }

    public override IEnumerator OnExit()
    {
        yield return null;
    }
}
