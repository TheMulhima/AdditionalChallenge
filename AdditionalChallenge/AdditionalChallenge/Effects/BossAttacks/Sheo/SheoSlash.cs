namespace AdditionalChallenge.Effects.CoolDownEffects.BossAttacks.Sheo;
public class SheoSlash:AbstractBossAttack
{
    public override string ToggleName { get; protected set; } = "Sheo Slash";

    public override string ToggleDesc { get; protected set; } = "Let Sheo come and slash at you (blue)";

    private GameObject Sheo;
    private PlayMakerFSM nailmaster_sheo;
    private string StartState = "Slash Antic";
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
            var pos = HeroController.instance.transform.position;
            float posadder = 0;
            float amount = 12;
            if (HeroController.instance.move_input == 0)
            {
                posadder += URandom.value < 0.5 ? -amount : amount;
            }
            else if (Math.Abs(HeroController.instance.move_input - 1) < Mathf.Epsilon)
            {
                posadder += amount;
            }
            else if (Math.Abs(HeroController.instance.move_input + 1) < Mathf.Epsilon)
            {
                posadder += -amount;
            }
            Sheo.transform.position = pos + new Vector3(posadder, 1, 0);
        });
        
        Sheo.GetComponent<MeshRenderer>().enabled = false;
        Sheo.GetComponent<BoxCollider2D>().enabled = false;
        Sheo.GetComponent<HealthManager>().hp = Int32.MaxValue;
        nailmaster_sheo.SetState("Init");
    }
    internal override void Attack()
    {
        if (Sheo == null || nailmaster_sheo == null)
        {
            CreateBoss();
        }
        
        nailmaster_sheo.SetState(StartState);
    }
}