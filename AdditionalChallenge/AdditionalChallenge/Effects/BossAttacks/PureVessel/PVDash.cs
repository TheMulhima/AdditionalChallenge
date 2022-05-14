namespace AdditionalChallenge.Effects.BossAttacks;

public class PVDash:AbstractPureVessel
{
    public override string ToggleName { get; protected set; } = "PV Lunge";
    public override string ToggleDesc { get; protected set; } = "Let PV come and dash at you";
    
    private string WaitingForDash = nameof(WaitingForDash);
    protected override string StartState => "Dash Antic";

    protected override void EditFSM()
    {
        ctrl.GetState("Intro 1").ChangeTransition("FINISHED", "Intro Roar End");
        ctrl.GetState("Intro 1").GetAction<Wait>().time.Value = 0;
        var WaitingForSlashState = ctrl.CopyState("Intro Idle", WaitingForDash);
        WaitingForSlashState.ClearTransitions();
        ctrl.GetState("Intro Idle").ChangeTransition("FINISHED", WaitingForDash);
        ctrl.GetState("Idle Stance").ChangeTransition("FINISHED", WaitingForDash);
        WaitingForSlashState.AddMethod(() =>
        {
            PV.GetComponent<MeshRenderer>().enabled = false;
            PV.GetComponent<BoxCollider2D>().enabled = false;
            PV.transform.position = new Vector3(Mathf.Infinity, Mathf.Infinity);
        });
        
        var wait = ctrl.CopyState("Dash Recover", "WaitPls");
        wait.Actions = new FsmStateAction[]
        {
            wait.Actions[6]
        };
        wait.GetAction<Wait>().time.Value = 0.25f;
        wait.ChangeTransition("FINISHED", "Dash");
        ctrl.GetState("Dash Antic").ChangeTransition("FINISHED", "WaitPls");
    }
}