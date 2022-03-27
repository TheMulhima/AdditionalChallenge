namespace AdditionalChallenge.Effects.PersistentEffects;

public class InvertedControls: AbstractPersistentEffect
{
    public override string ToggleName { get; protected set; } = "InvertedControls";
    public override string ToggleDesc { get; protected set; } = "Invert your controls";

    protected override Func<bool> WhenUnDoEffectBeCalled { get; set; } = () =>
    {
        return false;
    };

    protected override void StartEffect()
    {
        
        On.HeroController.Move += Invert;
        ModHooks.DashVectorHook += InvertDash;
        
    }
    
    void Invert(On.HeroController.orig_Move orig, HeroController self, float dir)
    {
        if (HeroController.instance.transitionState != HeroTransitionState.WAITING_TO_TRANSITION)
        {
            orig(self, dir);
                    
            return;
        }

        orig(self, -dir);
    }
            
    Vector2 InvertDash(Vector2 change)
    {
        return -change;
    }

    internal override void UnDoEffect()
    {

        On.HeroController.Move -= Invert;
        ModHooks.DashVectorHook -= InvertDash;
    }
}