namespace AdditionalChallenge.Effects.CoolDownEffects;
public class LaserTurret:AbstractCoolDownEffect
{
    public override string ToggleName { get; protected set; } = "Summon Laser Turret";
    public override string ToggleDesc { get; protected set; } = "Summons crystal peak lasers";

    public override void DoEffect()
    {
        Vector3 pos = HeroController.instance.transform.position;

        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, 500, 1 << 8);

        // Take the minimum so that we go from the floor
        if (hit && hit.point.y < pos.y)
        {
            pos = hit.point;
        }

        const float MAX_ADD = 10;

        for (int i = -2; i <= 2; i++)
        {
            Vector3 turret_pos = pos + new Vector3(i * 5, MAX_ADD, 0);

            RaycastHit2D up = Physics2D.Raycast(pos, (turret_pos - pos).normalized, 500, 1 << 8);

            // If the ceiling is above where we're going to spawn, put it right beneath the ceiling.
            if (up.point.y > pos.y + 10)
            {
                turret_pos = up.point + new Vector2(0, -0.5f);
            }

            var turret = UObject.Instantiate
            (
                Preloads.InstantiableObjects["Laser Turret"],
                turret_pos,
                Quaternion.Euler(0, 0, 180 + URandom.Range(-30f, 30f))
            );

            turret.LocateMyFSM("Laser Bug").GetState("Init").AddAction
            (
                new WaitRandom
                {
                    timeMax = .75f,
                    timeMin = 0
                }
            );

            turret.SetActive(true);
        }
    }
}