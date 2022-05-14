namespace AdditionalChallenge.Effects.PersistentEffects;
public class Wind: AbstractPersistentEffect
{
    public override string ToggleName { get; protected set; } = "Wind";
    public override string ToggleDesc { get; protected set; } = "Makes it so you always have to face wind";

    protected void Start()
    {
        IL.HeroController.FixedUpdate += DontStopCdash;
        ModHooks.BeforePlayerDeadHook += BeforePlayerDead;
        ModHooks.SetPlayerBoolHook += NoWindOnBench;
        ModHooks.BeforeSceneLoadHook += NoWindInTransitions;
        On.HeroController.FinishedEnteringScene += (orig, self, marker, bob) =>
        {
            orig(self, marker, bob);
            if (IsEnabled)
            {
                HeroController.instance.cState.inConveyorZone = true;
            }
        };
    }

    private string NoWindInTransitions(string arg)
    {
        HeroController.instance.cState.inConveyorZone = false;
        return arg;
    }

    private bool NoWindOnBench(string s, bool orig)
    {
        if (s == nameof(PlayerData.atBench))
        {
            if (orig)
            {
                HeroController.instance.cState.inConveyorZone = false;
            }
            else
            {
                if (IsEnabled)
                {
                    HeroController.instance.cState.inConveyorZone = true;
                }
            }
        }

        return orig;
    }

    internal override bool StartEffect()
    {
        if (HeroController.instance == null)
        {
            return false;
        }
        HeroController.instance.cState.inConveyorZone = true;

        SetWind();
        return true;
    }

    private void SetWind()
    {
        float speed;
        if (AdditionalChallenge.settings.windSpeed == Mathf.Infinity)
        {
            speed =  URandom.Range(-6f, 6f);
        }
        else
        {
            speed = AdditionalChallenge.settings.windSpeed;
        }
        Log($"Speed is {speed}");

        HeroController.instance.conveyorSpeed = speed;
    }
    
    private void BeforePlayerDead()
    {
        HeroController.instance.cState.inConveyorZone = false;
    }

    private void DontStopCdash(ILContext il)
    {
        ILCursor cursor = new ILCursor(il).Goto(0);
        
        if(cursor.TryGotoNext(MoveType.After, i => i.MatchLdstr("SLOPE CANCEL")))
        {
            cursor.EmitDelegate<Func<string, string>>((s) =>
            {
                if (IsEnabled && HeroController.instance.cState.inConveyorZone)
                {
                    return "";
                }
                else
                {
                    return s;
                }
            });
        }
    }

    internal override void UnDoEffect()
    {
        HeroController.instance.cState.inConveyorZone = false;
    }
    
    public override void AddElementsToModMenu(Menu MenuRef)
    {
        MenuRef.AddElement(new HorizontalOption(ToggleName, ToggleDesc,
            new [] { "Enabled", "Disabled" },
            (i) =>
            {
                AdditionalChallenge.settings.EffectIsEnabledDictionary[Key] = i == 0;
                AdditionalChallenge.Instance.MatchSettings();
                
                MenuRef.Find($"WindSpeedOption").isVisible = i == 0;
                MenuRef.Update();
            },
            () => AdditionalChallenge.settings.EffectIsEnabledDictionary.ContainsKey(Key)
                ? AdditionalChallenge.settings.EffectIsEnabledDictionary[Key] ? 0 : 1 
                : 1));
        
        float start = -10f, stop = 10f, step = 0.5f;
        string[] options = new[] { "Random" };
        options = options.Concat(Enumerable.Range((int)(start / step), (int)(Math.Abs(stop - start) / step) + 1)
            .Select(x => (x * step).ToString()).ToArray()).ToArray();
        MenuRef.AddElement(new HorizontalOption("Wind Speed", "Choose Wind speed",
            options,
            i =>
            {
                if (i == 0)
                {
                    AdditionalChallenge.settings.windSpeed = Mathf.Infinity;
                }
                else
                {
                    i--;
                    AdditionalChallenge.settings.windSpeed = ((i + (start / step)) * step);
                }

                if (IsEnabled && HeroController.instance.cState.inConveyorZone)
                {
                    SetWind();
                }
            },
            () =>
            {
                if (AdditionalChallenge.settings.windSpeed == Mathf.Infinity)
                {
                    return 0;
                }
                else
                {
                    return (int)((AdditionalChallenge.settings.windSpeed / step) - (start / step)) + 1;
                }
            },
            Id: $"WindSpeedOption")
        {
            isVisible =
                AdditionalChallenge.settings.EffectIsEnabledDictionary.ContainsKey(isEnabledKey) ?
                    AdditionalChallenge.settings.EffectIsEnabledDictionary[isEnabledKey] :
                    false
        });
    }
    
    private string isEnabledKey => MiscExtensions.GetKey(this, nameof(IsEnabled));
}