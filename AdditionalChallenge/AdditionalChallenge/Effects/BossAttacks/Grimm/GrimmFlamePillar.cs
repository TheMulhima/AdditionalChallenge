namespace AdditionalChallenge.Effects.CoolDownEffects.BossAttacks;
public class GrimmFlamePillar:AbstractNKG
{
    public override string ToggleName { get; protected set; } = "Grimm FlamePillar";
    public override string ToggleDesc { get; protected set; } = "Summon and NKG to do flame pillars and leave";

    private string WaitingForDiveDash = nameof(WaitingForDiveDash);
    protected override string StartState => "Pillar Pos";
    
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
        ctrl.FsmVariables.FindFsmFloat("Min X").Value = pos.x - 100f;
        ctrl.FsmVariables.FindFsmFloat("Max X").Value = pos.x + 100f;
        ctrl.GetState("Pillar Tele In").GetAction<SetPosition>().y = pos.y + 7f;
        ctrl.GetState("Pillar").GetAction<SetPosition>().y = pos.y - 1;
    }
}