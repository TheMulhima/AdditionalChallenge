namespace AdditionalChallenge.Effects.BossAttacks;
public class SheoStab:AbstractSheoAttack
{
    public override string ToggleName { get; protected set; } = "Sheo Stab";

    public override string ToggleDesc { get; protected set; } = "Let Sheo come and Stab at you (yellow)";

    protected override string StartState { get; } = "Stab Antic";
}