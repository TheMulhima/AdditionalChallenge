namespace AdditionalChallenge.Effects.CoolDownEffects.BossAttacks;

public class Markoth:AbstractBossAttack
{
    public override string ToggleName { get; protected set; } = "Markoth Shield Attack";
    public override string ToggleDesc { get; protected set; } = "Let Markoth come and do a shield attack";
    
    private GameObject MarkothObj;
    private PlayMakerFSM attacking, movement, sheildAttack;
    private string doNothing = nameof(doNothing);
    private string NewWarpIn = nameof(NewWarpIn);
    
    public override void Awake()
    {
        //temp fix cuz causing problems
    }
    
    protected override void CreateBoss()
    {
        DestroyImmediate(MarkothObj);
        MarkothObj = Instantiate(Preloads.InstantiableObjects["markoth"]);
        DontDestroyOnLoad(MarkothObj);
        MarkothObj.SetActive(true);
        MarkothObj.gameObject.layer = 31;

        DestroyImmediate(MarkothObj.LocateMyFSM("Rage Check"));
        attacking = MarkothObj.LocateMyFSM("Attacking");
        movement = MarkothObj.LocateMyFSM("Movement");
        sheildAttack = MarkothObj.LocateMyFSM("Shield Attack");

        var doNothingState_attacking = attacking.CopyState("Init", doNothing);
        doNothingState_attacking.Actions = Array.Empty<FsmStateAction>();
        doNothingState_attacking.Transitions = Array.Empty<FsmTransition>();
        
        //basically let the fsm rot in hell
        attacking.GetState("Wait").ChangeTransition("FINISHED", doNothing);
        attacking.GetState("Wait").ChangeTransition("TOOK DAMAGE", doNothing);
        attacking.GetState("Wait").ChangeTransition("ATTACK", doNothing);
        attacking.GetState("Wait").ChangeTransition("RAGE", doNothing);
        
        var doNothingState_movement = movement.CopyState("Init", doNothing);
        doNothingState_movement.Actions = Array.Empty<FsmStateAction>();
        doNothingState_movement.Transitions = Array.Empty<FsmTransition>();
        
        movement.GetState("Warp In").ChangeTransition("FINISHED", doNothing);

        var warpInState = movement.CopyState("Warp In", NewWarpIn);
        warpInState.ChangeTransition("FINISHED", doNothing);
        warpInState.AddMethod(() =>
          {
              MarkothObj.SetActive(true);
              MarkothObj.GetComponent<MeshRenderer>().enabled = true;
              var pos = HeroController.instance.transform.position;

              float posadderx = URandom.Range(-7f, 7f);
              float posaddery = URandom.Range(4, 10);
        
              MarkothObj.transform.position = pos + new Vector3(posadderx, posaddery, 0);
          });
        
        var doNothingState_shield= sheildAttack.CopyState("Init", doNothing);
        doNothingState_shield.Actions = Array.Empty<FsmStateAction>();
        doNothingState_shield.Transitions = Array.Empty<FsmTransition>();
        
        sheildAttack.GetState("Init").ChangeTransition("FINISHED", doNothing);
        sheildAttack.GetState("Reset").ChangeTransition("FINISHED", doNothing);

        sheildAttack.GetState("Reset").Actions = new FsmStateAction[]
        {
            sheildAttack.GetState("Reset").Actions[2]
        };
        sheildAttack.GetState("Reset").AddMethod(() =>
        {
            MarkothObj.GetComponent<MeshRenderer>().enabled = false;
            MarkothObj.transform.position = new Vector3(Mathf.Infinity, Mathf.Infinity);
        });
        
        MarkothObj.GetComponent<MeshRenderer>().enabled = false;
        MarkothObj.GetComponent<HealthManager>().hp = Int32.MaxValue;
        //MarkothObj.transform.position = new Vector3(Mathf.Infinity, Mathf.Infinity);
        attacking.SetState("Init");
        movement.SetState("Init");
        sheildAttack.SetState("Init");
    }
    internal override void Attack()
    {
        if (MarkothObj == null)
        {
            CreateBoss();
        }
        movement.SetState(NewWarpIn);
        sheildAttack.SetState("Ready");
    }

}