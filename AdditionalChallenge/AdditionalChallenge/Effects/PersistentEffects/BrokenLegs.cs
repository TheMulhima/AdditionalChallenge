using MonoMod.RuntimeDetour;

namespace AdditionalChallenge.Effects.PersistentEffects;

public class BrokenLegs:AbstractPersistentEffect
{
    public override string ToggleName { get; protected set; } = "Broken Legs";
    public override string ToggleDesc { get; protected set; } = "All falls are hard falls";

    internal override bool StartEffect()
    {
        //IL.HeroController.FallCheck -= ForceHardLand;
        IL.HeroController.FallCheck += ForceHardLand;
        return true;
    }

    internal override void UnDoEffect()
    {
        IL.HeroController.FallCheck -= ForceHardLand;
    }

    private void ForceHardLand(ILContext il)
    {
        var cursor = new ILCursor(il);

        cursor.GotoNext(MoveType.After,i => i.MatchLdstr("HeroCtrl-LeftGround"));
        cursor.GotoNext();
        cursor.EmitDelegate<Action>(() => HeroController.instance.cState.willHardLand = true);
    }
}