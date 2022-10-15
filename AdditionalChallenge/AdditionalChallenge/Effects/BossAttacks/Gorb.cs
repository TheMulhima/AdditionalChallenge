namespace AdditionalChallenge.Effects.BossAttacks;

public class GorbAttack:AbstractBossAttack
{
    public override string ToggleName { get; protected set; } = "Gorb Attack";

    public override string ToggleDesc { get; protected set; } = "Let Gorb come and throw a circle of spikes 3 times at you";

    private GameObject Gorb;
    private PlayMakerFSM Attacking;
    private PlayMakerFSM Movement;
    private string StartState = "Antic";
    protected override void CreateBoss()
    {
        DestroyImmediate(Gorb);
        Gorb = Instantiate(Preloads.InstantiableObjects["gorb"]);
        DontDestroyOnLoad(Gorb);
        Gorb.gameObject.layer = 31;
        Gorb.SetActive(true);
        
        //short circut FSMS
        Gorb.LocateMyFSM("Distance Attack").GetState("Init").RemoveTransition(FSMExtensions.FINISHED);
        Gorb.LocateMyFSM("Distance Attack").GetState("Attack").ClearState();
        Gorb.LocateMyFSM("Warp messenger").GetState("Send").RemoveAction(0);
        
        Attacking = Gorb.LocateMyFSM(nameof(Attacking));
        Movement = Gorb.LocateMyFSM(nameof(Movement));
        
        Movement.GetState("Choose Target").RemoveAction(0);
        Movement.GetState("Hover").ClearState();
        Movement.GetState("Init").RemoveTransition(FSMExtensions.FINISHED);
        Movement.GetState("Warp In").RemoveTransition(FSMExtensions.FINISHED);
        Movement.GetState("Warp Out").RemoveTransition(FSMExtensions.FINISHED);
        
        Attacking.GetState("Init 2").RemoveTransition(FSMExtensions.FINISHED);
        Attacking.GetState("Wait").ClearState();
        Attacking.GetState("Damaged").ClearState();
        Attacking.GetState("Is Hero Away?").ClearState();
        
        Attacking.GetState("Attack").AddMethod(() => Log("ATTACCKKKKK"));
        Attacking.GetState("Attack").ChangeTransition(FSMExtensions.FINISHED, "Double Pause");
        Attacking.GetState("Double").ChangeTransition(FSMExtensions.FINISHED, "Anim");
        Attacking.GetState("End").ClearTransitions();
        Attacking.GetState("End").AddMethod(() =>
        {
            Gorb.GetComponent<MeshRenderer>().enabled = false;
            Gorb.GetComponent<BoxCollider2D>().enabled = false;
            Gorb.transform.position = new Vector2(Mathf.Infinity, Mathf.Infinity);
        });
        
        Attacking.GetState("Antic").InsertMethod(0, () =>
        {
            Gorb.SetActive(true);
            Gorb.GetComponent<MeshRenderer>().enabled = true;
            Gorb.GetComponent<BoxCollider2D>().enabled = true;
            var pos = HeroController.instance.transform.position;

            float posadderx = URandom.Range(-7f, 7f);
            float posaddery = URandom.Range(4, 10);

            Gorb.transform.position = pos + new Vector3(posadderx, posaddery, Gorb.transform.position.z);
        });


        Gorb.GetComponent<MeshRenderer>().enabled = false;
        Gorb.GetComponent<BoxCollider2D>().enabled = false;
        var HM = Gorb.GetComponent<HealthManager>().Reflect();
        HM.hp = Int32.MaxValue;
        HM.IsInvincible = true;
        HM.ignoreAcid = true;
        Attacking.SetState("Init");
        Movement.SetState("Init");
        
        
    }
    public override void Attack()
    {
        if (Gorb == null || Attacking == null)
        {
            CreateBoss();
        }
        
        Attacking.SetState(StartState);
    }
}