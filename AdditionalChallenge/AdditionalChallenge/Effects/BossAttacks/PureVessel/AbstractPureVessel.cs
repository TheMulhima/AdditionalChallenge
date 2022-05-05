namespace AdditionalChallenge.Effects.CoolDownEffects.BossAttacks;

public abstract class AbstractPureVessel:AbstractBossAttack
{
    protected GameObject PV;
    protected PlayMakerFSM ctrl;
    protected abstract string StartState { get; }

    /// <summary>
    /// Run code to edit fsms to isolate the attack so it only does that attack when set state and after attack goes to an idle state
    /// </summary>
    protected abstract void EditFSM();
    
    protected override void CreateBoss()
    {
        DestroyImmediate(PV);
        PV = Instantiate(Preloads.InstantiableObjects["pv"]);
        DontDestroyOnLoad(PV);
        PV.SetActive(true);
        ctrl = PV.LocateMyFSM("Control");
        DestroyImmediate(PV.GetComponent<ConstrainPosition>());
        PV.gameObject.layer = 31;
        PV.GetChild("Colliders").GetComponentsInChildren<BoxCollider2D>().ToList()
            .ForEach(collider => collider.isTrigger = true);
        
        EditFSM();

        PV.GetComponent<MeshRenderer>().enabled = false;
        PV.GetComponent<BoxCollider2D>().enabled = false;
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
        PV.GetComponent<BoxCollider2D>().enabled = true;
        
        SetVars();

        SetPos();
        
        ctrl.SetState(StartState);
    }


    protected virtual void SetVars()
    {
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
    }

    protected virtual void SetPos()
    {
        var pos = HeroController.instance.transform.position;
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
    }
}