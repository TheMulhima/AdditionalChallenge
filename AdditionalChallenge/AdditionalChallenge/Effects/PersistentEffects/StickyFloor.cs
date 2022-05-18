namespace AdditionalChallenge.Effects.PersistentEffects;

public class StickyFloor:AbstractPersistentEffect
{
    public override string ToggleName { get; protected set; } = "Sticky Floor";
    public override string ToggleDesc { get; protected set; } = "You cannot move (run/dash) while grounded";
    
    internal override bool StartEffect()
    {
        On.HeroController.Move += StickOnFloor;
        ModHooks.DashPressedHook += DontAllowFloorDash;
        return true;
    }

    private bool DontAllowFloorDash()
    {
        if (HeroController.instance.cState.onGround)
        {
            return true;//override normal dash
        }

        return false; //dont override
    }

    void StickOnFloor(On.HeroController.orig_Move orig, HeroController self, float move_direction)
    {
        if (HeroController.instance.transitionState != HeroTransitionState.WAITING_TO_TRANSITION)
        {
            orig(self, move_direction);
                    
            return;
        }

        if (HeroController.instance.cState.onGround)
        {
            move_direction = 0f;
        }

        orig(self, move_direction);
    }

    internal override void UnDoEffect()
    {
        On.HeroController.Move -= StickOnFloor;
        ModHooks.DashPressedHook -= DontAllowFloorDash;
    }
}