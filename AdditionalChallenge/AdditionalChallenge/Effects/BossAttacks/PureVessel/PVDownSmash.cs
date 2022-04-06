namespace AdditionalChallenge.Effects.CoolDownEffects.BossAttacks;

public class PVDownSmash:AbstractBossAttack
{
    public override string ToggleName { get; protected set; } = "PV Down Stab";
    public override string ToggleDesc { get; protected set; } = "Let PV come and down stab at you";
    
    private GameObject PV;
    private PlayMakerFSM ctrl;
    private string WaitingForSlash = nameof(WaitingForSlash);
    private string StartState = "Dstab Antic";
    
    
    protected override void CreateBoss()
    {
        DestroyImmediate(PV);
        PV = Instantiate(Preloads.InstantiableObjects["pv"]);
        DontDestroyOnLoad(PV);
        PV.SetActive(true);
        ctrl = PV.LocateMyFSM("Control");
        DestroyImmediate(PV.GetComponent<ConstrainPosition>());
        PV.gameObject.layer = 31;
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
        PV.GetComponent<MeshRenderer>().enabled = false;
        PV.GetComponent<HealthManager>().hp = Int32.MaxValue;
        ctrl.SetState("Init");
    }
    internal override void Attack()
    {
        if (PV == null || ctrl == null)
        {
            CreateBoss();
        }
        PV.SetActive(true);
        PV.GetComponent<MeshRenderer>().enabled = true;
        var pos = HeroController.instance.transform.position;
        float x = pos.x;
        float y = pos.y;
        ctrl.FsmVariables.FindFsmFloat("Left X").Value = x - 30;
        ctrl.FsmVariables.FindFsmFloat("Right X").Value = x + 30;
        ctrl.FsmVariables.FindFsmFloat("TeleRange Max").Value = x - 30;
        ctrl.FsmVariables.FindFsmFloat("TeleRange Min").Value = x + 30;
        ctrl.FsmVariables.FindFsmFloat("Plume Y").Value = y - 2.5f;
        ctrl.FsmVariables.FindFsmFloat("Stun Land Y").Value = y + 3f;
        ctrl.FsmVariables.FindFsmGameObject("Hero").Value = HeroController.instance.gameObject;
        ctrl.FsmVariables.FindFsmGameObject("Self").Value = PV;

        float posadder = 0;
        if (HeroController.instance.move_input == 0)
        {
            posadder += URandom.value < 0.5 ? -7 : 7;
        }
        else if (Math.Abs(HeroController.instance.move_input - 1) < Mathf.Epsilon)
        {
            posadder += 7;
        }
        else if (Math.Abs(HeroController.instance.move_input + 1) < Mathf.Epsilon)
        {
            posadder += -7;
        }
        
        PV.transform.position = pos + new Vector3(posadder, 3, 0);

        ctrl.SetState(StartState);
    }

}