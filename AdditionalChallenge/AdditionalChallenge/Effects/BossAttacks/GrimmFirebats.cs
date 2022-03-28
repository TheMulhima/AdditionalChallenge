﻿namespace AdditionalChallenge.Effects.CoolDownEffects.BossAttacks;
public class GrimmFireBats:AbstractCoolDownEffect
{
    public override string ToggleName { get; protected set; } = "Grimm Firebats";
    public override string ToggleDesc { get; protected set; } = "Summon and NKG to throw firebats and leave";

    private GameObject Grimm;
    private PlayMakerFSM ctrl;
    private string WaitingForFirebats = nameof(WaitingForFirebats);
    
    public void Awake()
    {
        Preloads.OnPreloadsFinish += InstantiateGrimm;
    }

    void InstantiateGrimm()
    {
        DestroyImmediate(Grimm);
        Grimm = Instantiate(Preloads.InstantiableObjects["NKG"]);
        DontDestroyOnLoad(Grimm);
        Grimm.SetActive(true);
        ctrl = Grimm.LocateMyFSM("Control");
        
        //we will use this as a "idle" state
        FsmState WaitingForUppercutState = ctrl.CopyState("Dormant", WaitingForFirebats);
        WaitingForUppercutState.ChangeTransition("WAKE", "FB Hero Pos");
        
        ctrl.GetState("Tele Out").ChangeTransition("FINISHED", WaitingForFirebats);
        ctrl.Fsm.SaveActions();
        
        DestroyImmediate(Grimm.LocateMyFSM("constrain_x"));
        DestroyImmediate(Grimm.LocateMyFSM("Constrain Y"));
        DestroyImmediate(Grimm.LocateMyFSM("Stun"));
        
        Grimm.SetActive(true);
        ctrl.SetState("Init");
        Grimm.GetComponent<HealthManager>().hp = Int32.MaxValue;
    }

    internal override void DoEffect()
    {
        if (Grimm == null || ctrl == null)
        {
            InstantiateGrimm();
        }

        ctrl.FsmVariables.FindFsmGameObject("Hero Obj").Value = HeroController.instance.gameObject;
        ctrl.FsmVariables.FindFsmGameObject("Self").Value = Grimm;
        Vector3 pos = HeroController.instance.transform.position;
        ctrl.GetState("FB Hero Pos").GetAction<FloatCompare>().float2.Value =
            HeroController.instance.transform.position.x + (URandom.value <= 0.5 ? 10f : -10f);
        ctrl.FsmVariables.FindFsmFloat("Ground Y").Value = pos.y + 2;
        var teleL = ctrl.GetState("FB Tele L");
        teleL.GetAction<RandomFloat>().min = pos.x - 7f;
        teleL.GetAction<RandomFloat>().max = pos.x - 7f;
        var teleR = ctrl.GetState("FB Tele R");
        teleR.GetAction<RandomFloat>().min = pos.x + 7f;
        teleR.GetAction<RandomFloat>().max = pos.x + 7f;
        ctrl.SetState("FB Hero Pos");
    }
}