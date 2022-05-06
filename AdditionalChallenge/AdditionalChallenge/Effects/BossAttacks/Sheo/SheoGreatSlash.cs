namespace AdditionalChallenge.Effects.CoolDownEffects.BossAttacks.Sheo;

public class SheoGreatSlash:AbstractSheoAttack
{
    public override string ToggleName { get; protected set; } = "Sheo Great Slash";
    public override string ToggleDesc { get; protected set; } = "Let Sheo come and great slash at you (purple)";
    protected override string StartState { get; } = "GSlash Charge";
    
    // min 32 max 60 g 7
}