namespace AdditionalChallenge.Effects.CoolDownEffects.BossAttacks;
public class GrimmFireBats:AbstractNKG
{
    public override string ToggleName { get; protected set; } = "Grimm Firebats";
    public override string ToggleDesc { get; protected set; } = "Summon and NKG to throw firebats and leave";

    private string WaitingForFirebats = nameof(WaitingForFirebats);
    protected override string StartState => "FB Hero Pos";
    

    protected override void EditFSM()
    {
        //we will use this as a "idle" state
        FsmState WaitingForUppercutState = ctrl.CopyState("Dormant", WaitingForFirebats);
        WaitingForUppercutState.ChangeTransition("WAKE", StartState);
        
        ctrl.GetState("Tele Out").ChangeTransition("FINISHED", WaitingForFirebats);
        ctrl.Fsm.SaveActions();
    }

    protected override void SetVars(Vector3 pos)
    {
        ctrl.GetState("FB Hero Pos").GetAction<FloatCompare>().float2.Value =
            HeroController.instance.transform.position.x + (URandom.value <= 0.5 ? 10f : -10f);
        ctrl.FsmVariables.FindFsmFloat("Ground Y").Value = pos.y + 2;
        var teleL = ctrl.GetState("FB Tele L");
        teleL.GetAction<RandomFloat>().min = pos.x - 7f;
        teleL.GetAction<RandomFloat>().max = pos.x - 7f;
        var teleR = ctrl.GetState("FB Tele R");
        teleR.GetAction<RandomFloat>().min = pos.x + 7f;
        teleR.GetAction<RandomFloat>().max = pos.x + 7f;
    }
}