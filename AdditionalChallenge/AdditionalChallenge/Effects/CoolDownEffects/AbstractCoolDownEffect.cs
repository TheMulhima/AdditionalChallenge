namespace AdditionalChallenge.Effects.CoolDownEffects;
/// <summary>
/// A Base class for all effects that have cooldowns
/// </summary>
public abstract class AbstractCoolDownEffect : AbstractEffects
{
    public abstract void DoEffect();

    //cooldown for non persistent effects
    public float coolDown { get; set; }
    
    protected float timer;

    protected bool HandleTimer()
    {
        if (coolDown == 0f)
        {
            return false;
        }

        if (HeroController.instance == null)
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

    public void Update()
    {
        if (IsEnabled && !AdditionalChallenge.settings.ChaosModeEnabled)
        {
            if (HandleTimer())
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

    public override void AddElementsToModMenu(Menu MenuRef)
    {
            MenuRef.AddElement(new HorizontalOption(ToggleName, ToggleDesc,
                new [] { "Enabled", "Disabled" },
                (i) =>
                {
                    AdditionalChallenge.settings.EffectIsEnabledDictionary[isEnabledKey] = i == 0;
                    AdditionalChallenge.Instance.MatchSettings();

                    MenuRef.Find($"{ToggleName} cool down").isVisible = i == 0;
                    MenuRef.Update();
                },
                () => AdditionalChallenge.settings.EffectIsEnabledDictionary.ContainsKey(isEnabledKey) ? 
                    (AdditionalChallenge.settings.EffectIsEnabledDictionary[isEnabledKey] ? 0 : 1) 
                    : 1));

            float start = 0;
            float step = 0.5f;
            float end = 120f;
            MenuRef.AddElement(new HorizontalOption($"{ToggleName} cool down", ToggleDesc,
                Enumerable.Range((int)(start / step), (int)(Math.Abs(end - start) / step) + 1)
                    .Select(x => (x * step).ToString()).ToArray(),
                i =>
                {
                    AdditionalChallenge.settings.EffectCoolDownDictionary[CoolDownKey] = ((i + (start / step)) * step);
                    AdditionalChallenge.Instance.MatchSettings();
                },
                () =>
                {
                    if (AdditionalChallenge.settings.EffectCoolDownDictionary.ContainsKey(CoolDownKey))
                    {
                        return (int)((AdditionalChallenge.settings.EffectCoolDownDictionary[CoolDownKey] / step) - (start / step));
                    }
                    return (int) start;
                },
                Id: $"{ToggleName} cool down")
                {
                isVisible =
                    AdditionalChallenge.settings.EffectIsEnabledDictionary.ContainsKey(isEnabledKey) ?
                     AdditionalChallenge.settings.EffectIsEnabledDictionary[isEnabledKey] :
                     false
                });
    }
    
    private string isEnabledKey => MiscExtensions.GetKey(this, nameof(IsEnabled));
    private string CoolDownKey => MiscExtensions.GetKey(this, nameof(coolDown));
    
}