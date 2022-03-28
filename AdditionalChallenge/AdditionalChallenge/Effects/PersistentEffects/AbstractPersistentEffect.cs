namespace AdditionalChallenge.Effects.PersistentEffects;

/// <summary>
/// Base class for all persistent effects
/// </summary>
public abstract class AbstractPersistentEffect : AbstractEffects
{
    /// <summary>
    /// the condition for whether or not the DoEffect or UndoEffect is called
    /// </summary>
    protected virtual Func<bool> WhenUnDoEffectBeCalled { get; set; } = () => false;

    /// <summary>
    /// Function to be called to start an effect
    /// </summary>
    internal virtual void StartEffect() {}

    /// <summary>
    /// function to be called to repeatedly set/unset something to make effect work
    /// </summary>
    internal virtual void RepeatedDoEffect() {}

    /// <summary>
    /// Function to undo the effect
    /// </summary>
    internal virtual void UnDoEffect() {}

    private bool isEffectRunning = false;

    /// <summary>
    /// Unity update function. We need to do it here because of the WhenUnDoEffectBeCalled option we have
    /// </summary>
    public void Update()
    {
        if (!WhenUnDoEffectBeCalled() && IsEnabled)
        {
            if (!isEffectRunning)
            {
                StartEffect();
                isEffectRunning = true;
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

    private string Key => MiscExtensions.GetKey(this, nameof(IsEnabled));
}