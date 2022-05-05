namespace AdditionalChallenge.Effects.CoolDownEffects.BossAttacks;

public class PVSmallShot:AbstractPureVessel
{
    public override string ToggleName { get; protected set; } = "PV Soul Daggers";
    public override string ToggleDesc { get; protected set; } = "Let PV come and shoot small shots at you";
    
    private string WaitingForSlash = nameof(WaitingForSlash);
    protected override string StartState => "SmallShot Antic";
    
    
    protected override void EditFSM()
    {
        ctrl.GetState("Intro 1").ChangeTransition("FINISHED", "Intro Roar End");
        ctrl.GetState("Intro 1").GetAction<Wait>().time.Value = 0;
        var WaitingForSlashState = ctrl.CopyState("Intro Idle", WaitingForSlash);
        WaitingForSlashState.ClearTransitions();
        ctrl.GetState("Intro Idle").ChangeTransition("FINISHED", WaitingForSlash);
        ctrl.GetState("Idle Stance").ChangeTransition("FINISHED", WaitingForSlash);
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
        wait.ChangeTransition("FINISHED", "SmallShot Start");
        ctrl.GetState("SmallShot Antic").ChangeTransition("FINISHED", "WaitPls");
    }
}