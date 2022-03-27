namespace AdditionalChallenge.Effects.PersistentEffects;

public class Slip: AbstractPersistentEffect
{
    public override string ToggleName { get; protected set; } = "Slip";
    public override string ToggleDesc { get; protected set; } = "Makes the floor slipery";

    protected override Func<bool> WhenUnDoEffectBeCalled { get; set; } = () =>
    {
        return false;
    };

    private float last_move_dir = 0;

    protected override void StartEffect()
    {
        On.HeroController.Move += MakeSlip;
    }
    void MakeSlip(On.HeroController.orig_Move orig, HeroController self, float move_direction)
    {
        if (HeroController.instance.transitionState != HeroTransitionState.WAITING_TO_TRANSITION)
        {
            orig(self, move_direction);
                    
            return;
        }

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (move_direction == 0f)
        {
            move_direction = last_move_dir;
        }

        orig(self, move_direction);

        last_move_dir = move_direction;
    }

    internal override void UnDoEffect()
    {

        On.HeroController.Move -= MakeSlip;
    }
}