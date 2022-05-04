namespace AdditionalChallenge.Effects.PersistentEffects;

/// <summary>
/// Base class for all persistent effects
/// </summary>
public abstract class AbstractPersistentEffect : AbstractEffects
{
    /// <summary>
    /// Function to start the effect
    /// </summary>
    /// <returns>Whether or not it actually was started</returns>
    internal virtual bool StartEffect() => false;

    /// <summary>
    /// function to be called to repeatedly set/unset something to make effect work
    /// </summary>
    internal virtual void RepeatedDoEffect() {}

    /// <summary>
    /// Function to undo the effect
    /// </summary>
    /// <returns>Whether or not it actually was undone</returns>
    internal virtual void UnDoEffect() {}


    protected bool isEffectRunning = false;

    /// <summary>
    /// Unity update function. We need to do it here because of the WhenUnDoEffectBeCalled option we have
    /// </summary>
    public void Update()
    {
        if (IsEnabled)
        {
            if (!isEffectRunning)
            {
                isEffectRunning = StartEffect();
            }
            else
            {
                RepeatedDoEffect();
            }
        }
        else
        {
            if (isEffectRunning)
            {
                UnDoEffect();
                isEffectRunning = false;
            }
        }
    }
    
    public override List<(PropertyInfo,AbstractEffects)> GetPropertiesToAdd()
    {
        return new List<(PropertyInfo, AbstractEffects)>()
        {
            (ReflectionHelper.GetPropertyInfo(typeof(AbstractEffects), nameof(IsEnabled)), this),
        };
    }

    public override void AddElementsToModMenu(Menu MenuRef)
    {
        MenuRef.AddElement(new HorizontalOption(ToggleName, ToggleDesc,
            new [] { "Enabled", "Disabled" },
            (i) =>
            {
                AdditionalChallenge.settings.Booleans[Key] = i == 0;
                AdditionalChallenge.Instance.MatchSettings();
            },
            () => AdditionalChallenge.settings.Booleans.ContainsKey(Key)
                ? AdditionalChallenge.settings.Booleans[Key] ? 0 : 1 
                : 1));
    }

    protected string Key => MiscExtensions.GetKey(this, nameof(IsEnabled));
}