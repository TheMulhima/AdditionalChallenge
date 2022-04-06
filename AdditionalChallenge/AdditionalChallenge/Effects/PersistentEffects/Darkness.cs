namespace AdditionalChallenge.Effects.PersistentEffects;

public class Darkness: AbstractPersistentEffect
{
    public override string ToggleName { get; protected set; } = "Darkness";
    public override string ToggleDesc { get; protected set; } = "Makes everything dark";

    internal override bool StartEffect()
    {
        if (HeroController.instance == null) return false;
        
        DarknessHelper.Darken();
        On.GameManager.EnterHero += OnSceneLoad;
        return true;
    }

    void OnSceneLoad(On.GameManager.orig_EnterHero orig, GameManager self, bool additiveGateSearch)
    {
        orig(self, additiveGateSearch);
                
        DarknessHelper.Darken();
    }

    internal override void UnDoEffect()
    {
        On.GameManager.EnterHero -= OnSceneLoad;
        DarknessHelper.Lighten();
    }
}