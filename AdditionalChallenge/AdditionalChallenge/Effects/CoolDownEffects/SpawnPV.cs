﻿namespace AdditionalChallenge.Effects.CoolDownEffects;

public class SpawnPV:AbstractCoolDownEffect
{
    public override string ToggleName { get; protected set; } = "Spawn Pure Vessel";
    public override string ToggleDesc { get; protected set; } = "Spawns the pure vessel near you";

    public override void DoEffect()
    {
        // stolen from https://github.com/SalehAce1/PathOfPureVessel
        var pos = HeroController.instance.gameObject.transform.position;
        float x = pos.x;
        float y = pos.y;

        GameObject pv = Instantiate
        (
            Preloads.InstantiableObjects["pv"],
            pos + new Vector3(0, 2.6f),
            Quaternion.identity
        );

        pv.GetComponent<HealthManager>().hp /= 4;

        pv.SetActive(true);

        RaycastHit2D castLeft = Physics2D.Raycast(new Vector2(x, y), Vector2.left, 1000, 1 << 8);
        RaycastHit2D castRight = Physics2D.Raycast(new Vector2(x, y), Vector2.right, 1000, 1 << 8);

        if (!castLeft)
            castLeft.distance = 30f;
        if (!castRight)
            castRight.distance = 30f;


        PlayMakerFSM control = pv.LocateMyFSM("Control");
        control.FsmVariables.FindFsmFloat("Left X").Value = x - castLeft.distance;
        control.FsmVariables.FindFsmFloat("Right X").Value = x + castRight.distance;
        control.FsmVariables.FindFsmFloat("TeleRange Max").Value = x - castLeft.distance;
        control.FsmVariables.FindFsmFloat("TeleRange Min").Value = x + castRight.distance;
        control.FsmVariables.FindFsmFloat("Plume Y").Value = y - 3.2f;
        control.FsmVariables.FindFsmFloat("Stun Land Y").Value = y + 3f;

        var plume_gen = control.GetState("Plume Gen");

        plume_gen.InsertMethod(3, () =>
        {
            GameObject go = control.GetAction<SpawnObjectFromGlobalPool>("Plume Gen", 0).storeObject.Value;
            PlayMakerFSM fsm = go.LocateMyFSM("FSM");
            fsm.GetAction<FloatCompare>("Outside Arena?", 2).float2.Value = Mathf.Infinity;
            fsm.GetAction<FloatCompare>("Outside Arena?", 3).float2.Value = -Mathf.Infinity;
        });
        plume_gen.InsertMethod(5, () =>
            {
                GameObject go = control.GetAction<SpawnObjectFromGlobalPool>("Plume Gen", 4).storeObject.Value;
                PlayMakerFSM fsm = go.LocateMyFSM("FSM");
                fsm.GetAction<FloatCompare>("Outside Arena?", 2).float2.Value = Mathf.Infinity;
                fsm.GetAction<FloatCompare>("Outside Arena?", 3).float2.Value = -Mathf.Infinity;
            }
        );
        
        
        control.GetState("HUD Out").RemoveAction(0);
        var introRoar = control.GetState("Intro Roar");
        introRoar.RemoveAction(5);
        introRoar.RemoveAction(4);
        control.GetState("Intro Roar End").RemoveAction(3);

        var cp = pv.GetComponent<ConstrainPosition>();
        cp.xMax = x + castRight.distance;
        cp.xMin = x - castLeft.distance;
    }

}