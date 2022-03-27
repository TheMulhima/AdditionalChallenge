namespace AdditionalChallenge.Effects.CoolDownEffects;
/// <summary>
/// A Base class for all effects that have cooldowns
/// </summary>
public abstract class AbstractCoolDownEffect : AbstractEffects
{
    protected float timer;

    protected bool HandleTimer()
    {
        if (coolDown == 0f)
        {
            return false;
        }
        timer += Time.deltaTime;
        if (timer > coolDown)
        {
            timer = 0;
            return true;
        }

        return false;
    }

    public override void Update()
    {
        if (IsEnabled)
        {
            bool shouldEffectBeDone = HandleTimer();


            if (shouldEffectBeDone)
            {
                DoEffect();
            }
        }
        else
        {
            timer = 0;
        }
    }

    public override List<(PropertyInfo,AbstractEffects)> GetPropertiesToAdd()
    {
        return new List<(PropertyInfo, AbstractEffects)>()
        {
            (ReflectionHelper.GetPropertyInfo(typeof(AbstractEffects), nameof(IsEnabled)), this),
            (ReflectionHelper.GetPropertyInfo(typeof(AbstractCoolDownEffect), nameof(coolDown)), this),
        };
    }

    //cooldown for non persistent effects
    public float coolDown { get; set; }
}