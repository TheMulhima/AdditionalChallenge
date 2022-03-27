namespace AdditionalChallenge.Effects.PersistentEffects;

public class NailScale: AbstractPersistentEffect
{
    public override string ToggleName { get; protected set; } = "Nail Scale";
    public override string ToggleDesc { get; protected set; } = "Change your nail length";

    protected override Func<bool> WhenUnDoEffectBeCalled { get; set; } = () =>
    {
        return false;
    };

    private float nailScale;

    protected override void StartEffect()
    {
        nailScale = URandom.Range(0.3f, 5f);
        On.NailSlash.StartSlash += ChangeNailScale;
    }
    
    void ChangeNailScale(On.NailSlash.orig_StartSlash orig, NailSlash self)
    {
        orig(self);
                
        self.transform.localScale *= nailScale;
    }

    internal override void UnDoEffect()
    {
        On.NailSlash.StartSlash -= ChangeNailScale;
    }
}