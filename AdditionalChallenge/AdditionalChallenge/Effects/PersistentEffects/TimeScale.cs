using Satchel;
namespace AdditionalChallenge.Effects.PersistentEffects;
public class TimeScale:AbstractPersistentEffect
{
    private FieldInfo DebugModCurrentTimeScale;
    private bool isSet = false;
    private float? previousVal = null;
    
    public override string ToggleName { get; protected set; } = "Increase TimeScale";
    public override string ToggleDesc { get; protected set; } = "Make TimeScale to a random value between 0.5 and 3 ";

    public void Start()
    {
        this.DebugModCurrentTimeScale = typeof(DebugMod.DebugMod).GetField("CurrentTimeScale", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
    }

    internal override void RepeatedDoEffect()
    {
        if (HeroController.instance == null
            || HeroController.instance.cState.transitioning
            || GameManager.instance == null
            || GameManager.instance.IsNonGameplayScene()
            || GameManager.instance.IsGamePaused())
        {
            return;
        }

        GameManager.instance.gameObject.GetAddComponent<DebugMod.MonoBehaviours.TimeScale>();
        float num = GetScale();
        float newTimescaleValue = Mathf.Round(num * 10) / 10f;
        DebugModCurrentTimeScale.SetValue(null, newTimescaleValue);
        if (Time.timeScale != newTimescaleValue)
        {
            Time.timeScale = newTimescaleValue;
        }

        isSet = true;
        previousVal = newTimescaleValue;
    }

    internal override void UnDoEffect()
    {
        if (!GameManager.instance.IsGamePaused())
        {
            if (isSet && previousVal != null)
            {
                if (previousVal == (float)DebugModCurrentTimeScale.GetValue(null)
                    && Time.timeScale == previousVal)
                {
                    DebugModCurrentTimeScale.SetValue(null, 1);
                    Time.timeScale = 1;
                    isSet = false;
                }
            }
        }
    }

    private float GetScale()
    {
        if (AdditionalChallenge.settings.timeScale == Mathf.Infinity)
        {
            return UnityEngine.Random.Range(0.5f, 3f);
        }
        else
        {
            return AdditionalChallenge.settings.timeScale;
        }
    }
    
    public override void AddElementsToModMenu(Menu MenuRef)
    {
        MenuRef.AddElement(new HorizontalOption(ToggleName, ToggleDesc,
            new [] { "Enabled", "Disabled" },
            (i) =>
            {
                AdditionalChallenge.settings.EffectIsEnabledDictionary[Key] = i == 0;
                AdditionalChallenge.Instance.MatchSettings();
                
                MenuRef.Find($"TimeScaleOption").isVisible = i == 0;
                MenuRef.Update();
            },
            () => AdditionalChallenge.settings.EffectIsEnabledDictionary.ContainsKey(Key)
                ? AdditionalChallenge.settings.EffectIsEnabledDictionary[Key] ? 0 : 1 
                : 1));
        
        float start = 0.5f, stop = 3f, step = 0.05f;
        string[] options = new[] { "Random" };
        options = options.Concat(Enumerable.Range((int)(start / step), (int)(Math.Abs(stop - start) / step) + 1)
            .Select(x => (x * step).ToString()).ToArray()).ToArray();
        MenuRef.AddElement(new HorizontalOption("Time Scale", "Choose Time Scale",
            options,
            i =>
            {
                if (i == 0)
                {
                    AdditionalChallenge.settings.timeScale = Mathf.Infinity;
                }
                else
                {
                    i--;
                    AdditionalChallenge.settings.timeScale = ((i + (start / step)) * step);
                }

                //we dont need to set it as this one uses RepeatedDo method so when it can itll auto set it
                
            },
            () =>
            {
                if (AdditionalChallenge.settings.timeScale == Mathf.Infinity)
                {
                    return 0;
                }
                else
                {
                    return (int)((AdditionalChallenge.settings.timeScale / step) - (start / step)) + 1;
                }
            },
            Id: $"TimeScaleOption")
        {
            isVisible =
                AdditionalChallenge.settings.EffectIsEnabledDictionary.ContainsKey(isEnabledKey) ?
                    AdditionalChallenge.settings.EffectIsEnabledDictionary[isEnabledKey] :
                    false
        });
    }
    
    private string isEnabledKey => MiscExtensions.GetKey(this, nameof(IsEnabled));
}