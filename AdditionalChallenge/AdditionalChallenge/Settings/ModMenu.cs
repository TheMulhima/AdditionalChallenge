using AdditionalChallenge.Effects.EnemyFollow;

namespace AdditionalChallenge;

public static class ModMenu
{
    private static Menu MenuRef;
    private static Menu PersistentEffectsMenu, CoolDownEffectsMenu, BossAttacksMenu, EnemyFollowMenu;

    public static MenuScreen CreateMenuScreen(MenuScreen modListMenu)
    {
        float start = 0;
        float step = 0.5f;
        float end = 120f;
        CreateCoolDownEffectMenu();
        CreatePersistentEffectMenu();
        CreateBossAttacksMenu();
        CreateEnemyFollowMenu();
        MenuRef = new Menu("Mod Menu", new Element[]
        {
            Blueprints.NavigateToMenu("Persistent Effect Toggles",
                "", () => PersistentEffectsMenu.GetMenuScreen(MenuRef.menuScreen)),
            Blueprints.NavigateToMenu("CoolDown Effect Toggles",
                "", () => CoolDownEffectsMenu.GetMenuScreen(MenuRef.menuScreen)),
            Blueprints.NavigateToMenu("Boss Attack Toggles",
                "", () => BossAttacksMenu.GetMenuScreen(MenuRef.menuScreen)),
            Blueprints.NavigateToMenu("Enemy Follow Toggles",
                "", () => EnemyFollowMenu.GetMenuScreen(MenuRef.menuScreen)),
            
            new HorizontalOption("Chaos Mode", "A mode that randomly chooses effects for you",
                new []{"Enabled", "Disabled"},
                (s) =>
                {
                    AdditionalChallenge.settings.ChaosModeEnabled = s == 0;
                    
                    ChaosMode.ApplySetting();
                    
                    var cooldownoption = MenuRef.Find($"Effects coolDown option");
                    cooldownoption.isVisible = AdditionalChallenge.settings.ChaosModeEnabled;
                    var numEffectsdownoption = MenuRef.Find($"Effects number option");
                    numEffectsdownoption.isVisible = AdditionalChallenge.settings.ChaosModeEnabled;
                    var effectsDelaydownoption = MenuRef.Find($"Effects delay option");
                    effectsDelaydownoption.isVisible = AdditionalChallenge.settings.ChaosModeEnabled;
                    MenuRef.Update();
                },
                () => AdditionalChallenge.settings.ChaosModeEnabled ? 0: 1),
            
            new HorizontalOption("Effects coolDown", "Cool down between choosing effects",
                Enumerable.Range((int)(start / step), (int)(Math.Abs(end - start) / step) + 1)
                    .Select(x => (x * step).ToString()).ToArray(),
                i =>
                {
                    AdditionalChallenge.settings.chaosCoolDown = ((i + (start / step)) * step);
                },
                () => (int)((AdditionalChallenge.settings.chaosCoolDown / step) - (start / step)),
                Id: $"Effects coolDown option")
            {
                isVisible = AdditionalChallenge.settings.ChaosModeEnabled
            },
            new HorizontalOption("Number of effects", "The number of effects to be simutaneously enabled",
                Enumerable.Range(0, 20 + 1)
                    .Select(x => x.ToString()).ToArray(),
                i =>
                {
                    AdditionalChallenge.settings.numEffects = i;
                },
                () => AdditionalChallenge.settings.numEffects,
                Id: $"Effects number option")
            {
                isVisible = AdditionalChallenge.settings.ChaosModeEnabled
            },
            new HorizontalOption("Effects trigger delay", "Delay between triggering effects. Allows you to make it so effects are happening all the time",
                Enumerable.Range((int)(start / step), (int)(Math.Abs(end - start) / step) + 1)
                    .Select(x => (x * step).ToString()).ToArray(),
                i =>
                {
                    AdditionalChallenge.settings.delayBetweenTriggeringEffects = ((i + (start / step)) * step);
                },
                () => (int)((AdditionalChallenge.settings.delayBetweenTriggeringEffects / step) - (start / step)),
                Id: $"Effects delay option")
            {
                isVisible = AdditionalChallenge.settings.ChaosModeEnabled
            },
        });

        return MenuRef.GetMenuScreen(modListMenu);
    }

    private static void CreateEnemyFollowMenu()
    {
        if (EnemyFollowMenu != null) return;
        EnemyFollowMenu = new Menu("Enemy Follow", Array.Empty<Element>());
        foreach (var effect in AdditionalChallenge.AllEffects.Where(effect => effect is AbstractEnemyFollow))
        {
            effect.AddElementsToModMenu(EnemyFollowMenu);
        }
    }
    private static void CreateBossAttacksMenu()
    {
        if (BossAttacksMenu != null) return;
        BossAttacksMenu = new Menu("Boss Attacks", Array.Empty<Element>());
        foreach (var effect in AdditionalChallenge.AllEffects.Where(effect => effect is AbstractBossAttack))
        {
            effect.AddElementsToModMenu(BossAttacksMenu);
        }
    }
    
    private static void CreateCoolDownEffectMenu()
    {
        if (CoolDownEffectsMenu != null) return;
        CoolDownEffectsMenu = new Menu("CoolDown Effects", Array.Empty<Element>());
        foreach (var effect in AdditionalChallenge.AllEffects.Where(effect => effect is AbstractCoolDownEffect))
        {
            effect.AddElementsToModMenu(CoolDownEffectsMenu);
        }
    }
    
    private static void CreatePersistentEffectMenu()
    {
        if (PersistentEffectsMenu != null) return;
        PersistentEffectsMenu = new Menu("Persistent Effects", Array.Empty<Element>());
        foreach (var effect in AdditionalChallenge.AllEffects.Where(effect => effect is AbstractPersistentEffect))
        {
            effect.AddElementsToModMenu(PersistentEffectsMenu);
        }
    }
}