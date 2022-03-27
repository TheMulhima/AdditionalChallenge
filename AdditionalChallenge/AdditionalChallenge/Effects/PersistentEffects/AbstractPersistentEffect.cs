namespace AdditionalChallenge.Effects.PersistentEffects;

/// <summary>
/// Base class for all persistent effects
/// </summary>
public abstract class AbstractPersistentEffect : AbstractEffects
{
    /// <summary>
    /// the condition for whether or not the DoEffect or UndoEffect is called
    /// </summary>
    protected abstract Func<bool> WhenUnDoEffectBeCalled { get; set; }

    protected abstract void StartEffect();

    //this isnt needed as much
    internal override void DoEffect() {}

    private bool isEffectRunning = false;

    public override void Update()
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
                DoEffect();
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

    /// <summary>
    /// Function to undo the effect
    /// </summary>
    internal abstract void UnDoEffect();

}