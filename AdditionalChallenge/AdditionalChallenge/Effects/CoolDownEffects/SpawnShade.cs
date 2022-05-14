namespace AdditionalChallenge.Effects.CoolDownEffects;

public class SpawnShade:AbstractCoolDownEffect
{
    public override string ToggleName { get; protected set; } = "Spawn Shade";
    public override string ToggleDesc { get; protected set; } = "Spawns a shade";

    public override void DoEffect()
    {
        UObject.Instantiate(GameManager.instance.sm.hollowShadeObject, HeroController.instance.transform.position, Quaternion.identity);
    }
}