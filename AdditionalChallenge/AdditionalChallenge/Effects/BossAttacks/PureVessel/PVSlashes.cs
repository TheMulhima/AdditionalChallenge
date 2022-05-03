namespace AdditionalChallenge.Effects.CoolDownEffects.BossAttacks;

public class PVSlashes:AbstractPureVessel
{
    public override string ToggleName { get; protected set; } = "PV Slashes";
    public override string ToggleDesc { get; protected set; } = "Let PV come and slash at you";
    
    private string WaitingForSlash = nameof(WaitingForSlash);
    protected override string StartState => "Slash1 Antic";
    private GameObject AudioHolder;
    
    
    protected override void EditFSM()
    {
        ctrl.GetState("Intro 1").ChangeTransition("FINISHED", "Intro Roar End");
        ctrl.GetState("Intro 1").GetAction<Wait>().time.Value = 0;
        var WaitingForSlashState = ctrl.CopyState("Intro Idle", WaitingForSlash);
        WaitingForSlashState.Transitions = Array.Empty<FsmTransition>();
        ctrl.GetState("Intro Idle").ChangeTransition("FINISHED", WaitingForSlash);
        ctrl.GetState("Idle Stance").ChangeTransition("FINISHED", WaitingForSlash);
        WaitingForSlashState.AddMethod(() =>
        {
            PV.GetComponent<MeshRenderer>().enabled = false;
            PV.transform.position = new Vector3(Mathf.Infinity, Mathf.Infinity);
        });
    }
    protected override void SetPos()
    {
        base.SetPos();
        
        if (AudioHolder == null || AudioHolder.GetComponent<AudioSource>() == null)
        {
            AudioHolder = new GameObject("AudioHolder",typeof(AudioSource));
        }
        AudioHolder.GetComponent<AudioSource>().PlayOneShot(AdditionalChallenge.Instance.Clips.FirstOrDefault(x => x.name == "mage_knight_teleport"));
    }

}