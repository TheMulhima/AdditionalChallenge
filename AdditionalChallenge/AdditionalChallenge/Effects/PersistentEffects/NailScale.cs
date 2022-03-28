namespace AdditionalChallenge.Effects.PersistentEffects;

public class NailScale: AbstractPersistentEffect
{
    public override string ToggleName { get; protected set; } = "Nail Scale";
    public override string ToggleDesc { get; protected set; } = "Change your nail length";

    private float nailScale;

    internal override void StartEffect()
    {
        nailScale = URandom.Range(0.3f, 5f);
        On.NailSlash.StartSlash += ChangeNailScale;
    }
    
    //TODO: Add Option for nail scale
    
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