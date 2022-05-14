namespace AdditionalChallenge.Effects.CoolDownEffects;
public class BeeAttack: AbstractCoolDownEffect
{
    public override string ToggleName { get; protected set; } = "Areal Bee Attack";
    public override string ToggleDesc { get; protected set; } = "Hive Knights bees attack you from above";
    
    public override void DoEffect()
    {
        if (HeroController.instance == null) return;
        Vector3 pos = HeroController.instance.transform.position;
            
        RaycastHit2D floorHit = Physics2D.Raycast(pos, Vector2.down, 500, 1 << 8);

        if (floorHit && floorHit.point.y < pos.y)
        {
            pos = floorHit.point;
        }

        for (int i = 0; i < 7; i++)
        {
            var bee = UObject.Instantiate(Preloads.InstantiableObjects["bee"], Vector3.zero, Quaternion.Euler(0, 0, 180));

            bee.SetActive(true);

            PlayMakerFSM ctrl = bee.LocateMyFSM("Control");

            // Set reset vars so they recycle properly
            ctrl.Fsm.GetFsmFloat("X Left").Value = pos.x - 10;
            ctrl.Fsm.GetFsmFloat("X Right").Value = pos.x + 10;
            ctrl.Fsm.GetFsmFloat("Start Y").Value = pos.y + 17 + URandom.Range(-3f, 3f);

            // Despawn y
            ctrl.GetAction<FloatCompare>("Swarm", 3).float2.Value = pos.y - 5f;

            // Start the swarming
            ctrl.SendEvent("SWARM");
        }
    }
}