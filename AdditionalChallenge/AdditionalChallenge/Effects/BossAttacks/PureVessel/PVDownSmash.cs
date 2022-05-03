namespace AdditionalChallenge.Effects.CoolDownEffects.BossAttacks;

public class PVDownSmash:AbstractPureVessel
{
    public override string ToggleName { get; protected set; } = "PV Down Stab";
    public override string ToggleDesc { get; protected set; } = "Let PV come and down stab at you";
    
    private string WaitingForSlash = nameof(WaitingForSlash);
    protected override string StartState => "Dstab Antic";


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
        ctrl.GetState("Stomp Down").AddCoroutine(CheckForGround);

        IEnumerator CheckForGround()
        {
            while (PV.transform.position.y < ctrl.FsmVariables.FindFsmFloat("Stun Land Y").Value)
            {
                if (ctrl.ActiveStateName != "Stomp Down") yield break;
                Log($"Not less than yet {PV.transform.position.y} {ctrl.FsmVariables.FindFsmFloat("Stun Land Y").Value} ");
                yield return null;
            }

            PV.transform.position = new Vector3(PV.transform.position.x,ctrl.FsmVariables.FindFsmFloat("Stun Land Y").Value);
            ctrl.SetState("Stomp Land");
        }
        var stabjump = ctrl.GetState("Dstab Jump");
        stabjump.RemoveAction(7);
        stabjump.RemoveAction(1);
        var stabair = ctrl.GetState("Dstab Air");
        stabair.RemoveAction(4);
        stabair.RemoveAction(1);
        stabair.RemoveAction(0);
        ctrl.Fsm.SaveActions();
    }
}