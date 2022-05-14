namespace AdditionalChallenge.Effects.BossAttacks;
public class GrimmDiveDash:AbstractNKG
{
    public override string ToggleName { get; protected set; } = "Grimm DiveDash";
    public override string ToggleDesc { get; protected set; } = "Summon and NKG to do an dive dash and leave";
    
    private string WaitingForDiveDash = nameof(WaitingForDiveDash);
    protected override string StartState => "AD Pos";
    
    protected override void EditFSM()
    {
        //we will use this as a "idle" state
        FsmState WaitingForUppercutState = ctrl.CopyState("Dormant", WaitingForDiveDash);
        WaitingForUppercutState.ChangeTransition("WAKE", StartState);
        ctrl.GetState("Tele Out").ChangeTransition("FINISHED", WaitingForDiveDash);
        ctrl.Fsm.SaveActions();
    }

    protected override void SetVars(Vector3 pos)
    {
        ctrl.FsmVariables.FindFsmFloat("Ground Y").Value = pos.y + 2f;
        ctrl.FsmVariables.FindFsmFloat("AD Min X").Value = pos.x - 100f;
        ctrl.FsmVariables.FindFsmFloat("AD Max X").Value = pos.x + 100f;
        ctrl.GetState("AD Tele In").GetAction<SetPosition>().y = pos.y + 10f;
    }
}