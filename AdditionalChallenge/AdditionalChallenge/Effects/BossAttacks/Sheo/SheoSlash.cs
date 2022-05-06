namespace AdditionalChallenge.Effects.CoolDownEffects.BossAttacks.Sheo;
public class SheoSlash:AbstractSheoAttack
{
    public override string ToggleName { get; protected set; } = "Sheo Slash";

    public override string ToggleDesc { get; protected set; } = "Let Sheo come and slash at you (blue)";

    protected override string StartState { get; } = "Slash Antic";
}