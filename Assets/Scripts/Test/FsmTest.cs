#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;

public class FsmTest : MonoBehaviour,IFsmAnyState<FsmTest>
{
    IFsm<FsmTest> mFsm;
    public string state = "";
    [Button(Name ="Work")]
    void Work()
    {
        Framework.Debug.Log("Start Work");
        mFsm.FireEvent(this, Event.work);
    }
    [Button(Name = "Rest")]
    void Rest()
    {
        Framework.Debug.Log("Start Rest");
        mFsm.FireEvent(this, Event.rest);
    }
    [Button(Name = "Die")]
    void Die()
    {
        Framework.Debug.Log("Start Die");
        mFsm.FireEvent(this, Event.die);
    }

    private void Awake()
    {
        Framework.Debug.SetLogger(new Logger());
        FsmManager.Initialize();
        mFsm = FsmManager.Instance.CreateFsm<FsmTest>("FsmTest", this, this,new NormalState(),new TiredState(),new DeadState());
        mFsm.Start<NormalState>();
    }

    void Update()
    {
        GameFramework.Update(Time.deltaTime, Time.unscaledDeltaTime);
    }

    public void OnEvent(IFsm<FsmTest> fsm, object sender, int eventId, object userData)
    {
        if (eventId == Event.die)
        {
            fsm.ChangeState<DeadState>();
        }
        else
        {
            Framework.Debug.Log("No Response!");
        }
    }
    

    class Event
    {
        public const int work = 1;
        public const int rest = 2;
        public const int die = 3;
    }
    class NormalState:FsmState<FsmTest>
    {
        protected internal override void OnInit(IFsm<FsmTest> fsm)
        {
            base.OnInit(fsm);
            SubscribeEvent(Event.work, OnWork);
        }
        protected internal override void OnEnter(IFsm<FsmTest> fsm)
        {
            Framework.Debug.Log("NormalState Enter");
            fsm.Owner.state = "现在状态正常";
        }

        protected internal override void OnLeave(IFsm<FsmTest> fsm, bool isShutdown)
        {
            Framework.Debug.Log("NormalState Leave");
        }
        

        bool OnWork(IFsm<FsmTest> fsm, object sender, object userData)
        {
            fsm.ChangeState<TiredState>();
            return true;
        }
    }

    class TiredState : FsmState<FsmTest>
    {
        protected internal override void OnInit(IFsm<FsmTest> fsm)
        {
            base.OnInit(fsm);
            SubscribeEvent(Event.rest, OnRest);
        }
        protected internal override void OnEnter(IFsm<FsmTest> fsm)
        {
            Framework.Debug.Log("TiredState Enter");
            fsm.Owner.state = "现在状态疲惫";
        }

        protected internal override void OnLeave(IFsm<FsmTest> fsm, bool isShutdown)
        {
            Framework.Debug.Log("TiredState Leave");
        }

        bool OnRest(IFsm<FsmTest> fsm, object sender, object userData)
        {
            fsm.ChangeState<NormalState>();
            return true;
        }
    }
    class DeadState : FsmState<FsmTest>
    {
        protected internal override void OnInit(IFsm<FsmTest> fsm)
        {
            base.OnInit(fsm);
            SubscribeEvent(Event.die, OnDead);
        }
        protected internal override void OnEnter(IFsm<FsmTest> fsm)
        {
            Framework.Debug.Log("DeadState Enter");
            fsm.Owner.state = "现在状态死亡";
        }

        protected internal override void OnLeave(IFsm<FsmTest> fsm, bool isShutdown)
        {
            Framework.Debug.Log("DeadState Leave");
        }


        bool OnDead(IFsm<FsmTest> fsm, object sender, object userData)
        {
            Framework.Debug.Log("in DeadState!!!!!");
            return true;
        }

    }

}
#endif
