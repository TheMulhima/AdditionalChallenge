namespace AdditionalChallenge.Effects.CoolDownEffects.BossAttacks;

public abstract class AbstractNKG:AbstractBossAttack
{
    protected GameObject Grimm;
    protected PlayMakerFSM ctrl;
    private string WaitingForDiveDash = nameof(WaitingForDiveDash);
    protected abstract string StartState { get; }

    protected abstract void EditFSM();
    protected abstract void SetVars(Vector3 pos);
    
    
    protected override void CreateBoss()
    {
        DestroyImmediate(Grimm);
        Grimm = Instantiate(Preloads.InstantiableObjects["NKG"]);
        DontDestroyOnLoad(Grimm);
        Grimm.SetActive(true);
        ctrl = Grimm.LocateMyFSM("Control");
        Grimm.gameObject.layer = 31; //set to a layer that isnt in GlobalEnums.PhysLayers cuz i wanna avoid collision

        EditFSM();
        
        DestroyImmediate(Grimm.LocateMyFSM("constrain_x"));
        DestroyImmediate(Grimm.LocateMyFSM("Constrain Y"));
        DestroyImmediate(Grimm.LocateMyFSM("Stun"));
        
        Grimm.SetActive(true);
        ctrl.SetState("Init");
        Grimm.GetComponent<HealthManager>().hp = Int32.MaxValue;
    }

    internal override void Attack()
    {
        if (Grimm == null || ctrl == null)
        {
            CreateBoss();
        }
        Grimm.GetComponent<HealthManager>().hp = Int32.MaxValue;
        
        var pos = HeroController.instance.transform.position;
        
        ctrl.FsmVariables.FindFsmGameObject("Hero Obj").Value = HeroController.instance.gameObject;
        ctrl.FsmVariables.FindFsmGameObject("Self").Value = Grimm;
        
        SetVars(pos);
        
        ctrl.SetState(StartState);
    }
}