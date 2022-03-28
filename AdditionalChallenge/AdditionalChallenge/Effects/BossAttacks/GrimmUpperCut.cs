namespace AdditionalChallenge.Effects.CoolDownEffects.BossAttacks;
public class GrimmUpperCut:AbstractBossAttack
{
    public override string ToggleName { get; protected set; } = "Grimm Uppercut";
    public override string ToggleDesc { get; protected set; } = "Summon and NKG to do an uppercut and leave";

    private GameObject Grimm;
    private PlayMakerFSM ctrl;
    private string WaitingForUppercut = nameof(WaitingForUppercut);
    private string StartState = "Slash Tele In";
    
    protected override void CreateBoss()
    {
        DestroyImmediate(Grimm);
        Grimm = Instantiate(Preloads.InstantiableObjects["NKG"]);
        DontDestroyOnLoad(Grimm);
        Grimm.SetActive(true);
        ctrl = Grimm.LocateMyFSM("Control");
        Grimm.gameObject.layer = 31;
        
        //we will use this as a "idle" state
        FsmState WaitingForUppercutState = ctrl.CopyState("Dormant", WaitingForUppercut);
        WaitingForUppercutState.ChangeTransition("WAKE", StartState);
        
        ctrl.GetState("Tele Out").ChangeTransition("FINISHED", WaitingForUppercut);
        ctrl.GetState("Explode Pause").ChangeTransition("FINISHED", WaitingForUppercut);
        ctrl.GetState("After Evade").ChangeTransition("FIREBATS", "Slash Antic");
        ctrl.GetState("Auto Evade?").ChangeTransition("EVADE", "Slash Antic");
        ctrl.GetState("Dormant").AddTransition("FINISHED", "Tele Out");
        ctrl.GetAction<FloatCompare>("After Evade").greaterThan = ctrl.FsmEvents.First(trans => trans.Name == "SLASH");
        ctrl.Fsm.SaveActions();
        
        DestroyImmediate(Grimm.LocateMyFSM("constrain_x"));
        DestroyImmediate(Grimm.LocateMyFSM("Constrain Y"));
        DestroyImmediate(Grimm.LocateMyFSM("Stun"));
        
        Grimm.SetActive(true);
        ctrl.SetState("Init");
        Grimm.GetComponent<HealthManager>().hp = Int32.MaxValue;
    }

    internal override void Attack()
    {
        if (Grimm == null || ctrl == null)
        {
            CreateBoss();
        }

        ctrl.FsmVariables.FindFsmGameObject("Hero Obj").Value = HeroController.instance.gameObject;
        ctrl.FsmVariables.FindFsmGameObject("Self").Value = Grimm;
        Vector3 vector = HeroController.instance.transform.position;
        float dir = HeroController.instance.transform.localScale.x;
        float num = 7f;
        vector += new Vector3(dir * num, 2f, 0f);
        ctrl.FsmVariables.FindFsmFloat("Tele X").Value = vector.x;
        ctrl.FsmVariables.FindFsmFloat("Ground Y").Value = vector.y;
        ctrl.GetState("UP Explode").GetAction<SetPosition>().y.Value = vector.y + 7f;
        ctrl.GetAction<FloatCompare>("Uppercut Up", 7).float2.Value = vector.y + 7f;
        ctrl.SetState(StartState);
    }
}