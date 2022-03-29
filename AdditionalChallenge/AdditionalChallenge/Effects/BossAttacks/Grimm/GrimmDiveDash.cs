namespace AdditionalChallenge.Effects.CoolDownEffects.BossAttacks;
public class GrimmDiveDash:AbstractBossAttack
{
    public override string ToggleName { get; protected set; } = "Grimm DiveDash";
    public override string ToggleDesc { get; protected set; } = "Summon and NKG to do an dive dash and leave";

    private GameObject Grimm;
    private PlayMakerFSM ctrl;
    private string WaitingForDiveDash = nameof(WaitingForDiveDash);
    private string StartState = "AD Pos";
    
    protected override void CreateBoss()
    {
        DestroyImmediate(Grimm);
        Grimm = Instantiate(Preloads.InstantiableObjects["NKG"]);
        DontDestroyOnLoad(Grimm);
        Grimm.SetActive(true);
        ctrl = Grimm.LocateMyFSM("Control");
        //SFCore.Utils.FsmUtil.Log(ctrl);
        Grimm.gameObject.layer = 31; //set to a layer that isnt in GlobalEnums.PhysLayers cuz i wanna avoid collision
        
        
        //we will use this as a "idle" state
        FsmState WaitingForUppercutState = ctrl.CopyState("Dormant", WaitingForDiveDash);
        WaitingForUppercutState.ChangeTransition("WAKE", StartState);
        ctrl.GetState("Tele Out").ChangeTransition("FINISHED", WaitingForDiveDash);
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
        ctrl.FsmVariables.FindFsmFloat("Ground Y").Value = HeroController.instance.transform.position.y + 2f;
        ctrl.FsmVariables.FindFsmFloat("AD Min X").Value = HeroController.instance.transform.position.x - 100f;
        ctrl.FsmVariables.FindFsmFloat("AD Max X").Value = HeroController.instance.transform.position.x + 100f;
        ctrl.GetState("AD Tele In").GetAction<SetPosition>().y = HeroController.instance.transform.position.y + 10f;
        ctrl.SetState(StartState);
    }
}