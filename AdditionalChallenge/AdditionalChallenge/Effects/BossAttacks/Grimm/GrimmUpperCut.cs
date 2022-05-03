namespace AdditionalChallenge.Effects.CoolDownEffects.BossAttacks;
public class GrimmUpperCut:AbstractNKG
{
    public override string ToggleName { get; protected set; } = "Grimm Uppercut";
    public override string ToggleDesc { get; protected set; } = "Summon and NKG to do an uppercut and leave";
    private string WaitingForUppercut = nameof(WaitingForUppercut);
    protected override string StartState => "Slash Tele In";
    
    protected override void EditFSM()
    {
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
    }

    protected override void SetVars(Vector3 pos)
    {
        float dir = HeroController.instance.transform.localScale.x;
        float num = 7f;
        pos += new Vector3(dir * num, 2f, 0f);
        ctrl.FsmVariables.FindFsmFloat("Tele X").Value = pos.x;
        ctrl.FsmVariables.FindFsmFloat("Ground Y").Value = pos.y;
        ctrl.GetState("UP Explode").GetAction<SetPosition>().y.Value = pos.y + 7f;
        ctrl.GetAction<FloatCompare>("Uppercut Up", 7).float2.Value = pos.y + 7f;
        ctrl.SetState(StartState);
    }
}