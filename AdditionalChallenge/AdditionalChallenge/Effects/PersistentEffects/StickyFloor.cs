namespace AdditionalChallenge.Effects.PersistentEffects;

public class StickyFloor:AbstractPersistentEffect
{
    public override string ToggleName { get; protected set; } = "Sticky Floor";
    public override string ToggleDesc { get; protected set; } = "You cannot move (run/dash) while grounded";
    
    //TODO: implement
}