﻿namespace AdditionalChallenge.Effects.BossAttacks;

public abstract class AbstractBossAttack : AbstractEffects
{
    public virtual void Awake()
    {
        On.HeroController.Start += (orig, self) =>
        {
            orig(self);
            CreateBoss();
        };
    }
    
    public abstract void Attack();
    protected abstract void CreateBoss();

    public float timeBetweenAttacks { get; set; }

    protected float timer;

    protected bool HandleTimer()
    {
        if (timeBetweenAttacks == 0f)
        {
            return false;
        }

        if (HeroController.instance == null)
        {
            return false;
        }

        timer += Time.deltaTime;
        if (timer > timeBetweenAttacks)
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
                Attack();
            }
        }
        else
        {
            timer = 0;
        }
    }

    public override List<(PropertyInfo, AbstractEffects)> GetPropertiesToAdd()
    {
        return new List<(PropertyInfo, AbstractEffects)>()
        {
            (ReflectionHelper.GetPropertyInfo(typeof(AbstractEffects), nameof(IsEnabled)), this),
            (ReflectionHelper.GetPropertyInfo(typeof(AbstractBossAttack), nameof(timeBetweenAttacks)), this),
        };
    }

    public override void AddElementsToModMenu(Menu MenuRef)
    {
        MenuRef.AddElement(new HorizontalOption(ToggleName, ToggleDesc,
            new[] { "Enabled", "Disabled" },
            (i) =>
            {
                AdditionalChallenge.settings.EffectIsEnabledDictionary[isEnabledKey] = i == 0;
                AdditionalChallenge.Instance.MatchSettings();

                MenuRef.Find($"{ToggleName} cool down").isVisible = i == 0;
                MenuRef.Update();
            },
            () => AdditionalChallenge.settings.EffectIsEnabledDictionary.ContainsKey(isEnabledKey)
                ? (AdditionalChallenge.settings.EffectIsEnabledDictionary[isEnabledKey] ? 0 : 1)
                : 1));

        float start = 0;
        float step = 0.5f;
        float end = 120f;
        MenuRef.AddElement(new HorizontalOption($"{ToggleName} cool down", ToggleDesc,
            Enumerable.Range((int)(start / step), (int)(Math.Abs(end - start) / step) + 1)
                .Select(x => (x * step).ToString()).ToArray(),
            i =>
            {
                AdditionalChallenge.settings.EffectCoolDownDictionary[TimeBetweenAttacksKey] = ((i + (start / step)) * step);
                AdditionalChallenge.Instance.MatchSettings();
            },
            () =>
            {
                if (AdditionalChallenge.settings.EffectCoolDownDictionary.ContainsKey(TimeBetweenAttacksKey))
                {
                    return (int)((AdditionalChallenge.settings.EffectCoolDownDictionary[TimeBetweenAttacksKey] / step) - (start / step));
                }

                return (int)start;
            },
            Id: $"{ToggleName} cool down")
        {
            isVisible =
                AdditionalChallenge.settings.EffectIsEnabledDictionary.ContainsKey(isEnabledKey)
                    ? AdditionalChallenge.settings.EffectIsEnabledDictionary[isEnabledKey]
                    : false
        });
    }

    private string isEnabledKey => MiscExtensions.GetKey(this, nameof(IsEnabled));
    private string TimeBetweenAttacksKey => MiscExtensions.GetKey(this, nameof(timeBetweenAttacks));

}