namespace AdditionalChallenge.Effects.PersistentEffects;

public class Darkness: AbstractPersistentEffect
{
    public override string ToggleName { get; protected set; } = "Darkness";
    public override string ToggleDesc { get; protected set; } = "Makes everything dark";

    protected override Func<bool> WhenUnDoEffectBeCalled { get; set; } = () =>
    {
        if (HeroController.instance == null)
        {
            return true;
        }
        return false;
    };

    internal override void StartEffect()
    {
        DarknessHelper.Darken();
        On.GameManager.EnterHero += OnSceneLoad;
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