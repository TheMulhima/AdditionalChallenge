namespace AdditionalChallenge.Effects.CoolDownEffects.BossAttacks;

public class PVSlashes:AbstractBossAttack
{
    public override string ToggleName { get; protected set; } = "PV Slashes";
    public override string ToggleDesc { get; protected set; } = "Let PV come and slash at you";
    
    private GameObject PV;
    private PlayMakerFSM ctrl;
    private string WaitingForSlash = nameof(WaitingForSlash);
    private string StartState = "Slash1 Antic";
    private GameObject AudioHolder;
    
    
    protected override void CreateBoss()
    {
        DestroyImmediate(PV);
        PV = Instantiate(Preloads.InstantiableObjects["pv"]);
        DontDestroyOnLoad(PV);
        PV.SetActive(true);
        ctrl = PV.LocateMyFSM("Control");
        DestroyImmediate(PV.GetComponent<ConstrainPosition>());
        PV.gameObject.layer = 31;
        //SFCore.Utils.FsmUtil.MakeLog(ctrl);
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
        ctrl.FsmVariables.FindFsmFloat("Plume Y").Value = y - 3.2f;
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
        if (AudioHolder == null || AudioHolder.GetComponent<AudioSource>() == null)
        {
            AudioHolder = new GameObject("AudioHolder",typeof(AudioSource));
        }
        AudioHolder.GetComponent<AudioSource>().PlayOneShot(AdditionalChallenge.Instance.Clips.FirstOrDefault(x => x.name == "mage_knight_teleport"));

        ctrl.SetState(StartState);
    }

}