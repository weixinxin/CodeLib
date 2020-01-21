using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework;
public class DemoSceneB : BaseScene
{
    protected override string url => "Scenes/SwitchSceneB.unity";

    public override IEnumerator OnEnter(params object[] args)
    {
        yield return null;
    }

    public override IEnumerator OnExit()
    {
        yield return null;
    }
}
