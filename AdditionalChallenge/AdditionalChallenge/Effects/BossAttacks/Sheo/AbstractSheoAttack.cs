namespace AdditionalChallenge.Effects.BossAttacks;

public abstract class AbstractSheoAttack:AbstractBossAttack
{
    protected GameObject Sheo;
    protected PlayMakerFSM nailmaster_sheo;
    protected abstract string StartState { get; }

    protected virtual void AdditionalFSMChangesInit() {}
    protected virtual void AdditionalFSMChangesAttack() {}

    protected virtual void SetPos()
    {
        var pos = HeroController.instance.transform.position;
        float posadder = 0;
        if (HeroController.instance.move_input == 0)
        {
            posadder += URandom.value < 0.5 ? -moveAmount : moveAmount;
        }
        else if (Math.Abs(HeroController.instance.move_input - 1) < Mathf.Epsilon)
        {
            posadder += moveAmount;
        }
        else if (Math.Abs(HeroController.instance.move_input + 1) < Mathf.Epsilon)
        {
            posadder += -moveAmount;
        }
        Sheo.transform.position = pos + new Vector3(posadder, 1, 0);
    }
    
    protected float moveAmount = 12;

    protected override void CreateBoss()
    {
        DestroyImmediate(Sheo);
        Sheo = Instantiate(Preloads.InstantiableObjects["sheo"]);
        DontDestroyOnLoad(Sheo);
        Sheo.gameObject.layer = 31;
        Sheo.SetActive(true);

        nailmaster_sheo = Sheo.LocateMyFSM(nameof(nailmaster_sheo));
        
        nailmaster_sheo.GetState("Set Paint HP").ClearTransitions();

        var Idle = nailmaster_sheo.GetState("Idle");
        Idle.Actions = new FsmStateAction[]
        {
            Idle.GetAction<Tk2dPlayAnimation>()
        };
        Idle.ClearTransitions();
        
        Idle.AddMethod(() =>
        {
            Sheo.GetComponent<MeshRenderer>().enabled = false;
            Sheo.GetComponent<BoxCollider2D>().enabled = false;
            Sheo.transform.position = new Vector2(Mathf.Infinity, Mathf.Infinity);
        });
        
        
        nailmaster_sheo.GetState(StartState).InsertMethod(0,() =>
        {
            Sheo.SetActive(true);
            Sheo.GetComponent<MeshRenderer>().enabled = true;
            Sheo.GetComponent<BoxCollider2D>().enabled = true;
            SetPos();
        });
        
        AdditionalFSMChangesInit();
        
        Sheo.GetComponent<MeshRenderer>().enabled = false;
        Sheo.GetComponent<BoxCollider2D>().enabled = false;
        Sheo.GetComponent<HealthManager>().hp = Int32.MaxValue;
        nailmaster_sheo.SetState("Init");
    }
    public override void Attack()
    {
        if (Sheo == null || nailmaster_sheo == null)
        {
            CreateBoss();
        }
        AdditionalFSMChangesAttack();
        nailmaster_sheo.SetState(StartState);
    }
}