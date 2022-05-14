namespace AdditionalChallenge;

public static class ModMenu
{
    private static Menu MenuRef;
    private static Menu EffectsMenu, PersistentEffectsMenu, CoolDownEffectsMenu, BossAttacksMenu, EnemyFollowMenu;

    public static MenuScreen CreateMenuScreen(MenuScreen modListMenu)
    {
        float start = 0, step = 0.5f, end = 120f;
        CreateEffectsMenu();
        MenuRef ??= new Menu("Additional Challenge", new Element[]
        {
            Blueprints.NavigateToMenu("Toggle Effects", 
                "Click here to toggle all available effects", 
                () => EffectsMenu.GetMenuScreen(MenuRef!.menuScreen)),
            
            new HorizontalOption("Chaos Mode", "A mode that randomly chooses effects for you",
                new []{"Enabled", "Disabled"},
                (s) =>
                {
                    AdditionalChallenge.settings.ChaosModeEnabled = s == 0;
                    
                    ChaosMode.ApplySetting();
                    
                    MenuRef.UpdateVisibility(AdditionalChallenge.settings.ChaosModeEnabled, new []{ "Effects coolDown option", "Effects number option", "Effects delay option" });
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
        
        MenuRef.GetMenuScreen(modListMenu);
        PersistentEffectsMenu.GetMenuScreen(EffectsMenu.GetMenuScreen(MenuRef.menuScreen));
        CoolDownEffectsMenu.GetMenuScreen(EffectsMenu.GetMenuScreen(MenuRef.menuScreen));
        BossAttacksMenu.GetMenuScreen(EffectsMenu.GetMenuScreen(MenuRef.menuScreen));
        EnemyFollowMenu.GetMenuScreen(EffectsMenu.GetMenuScreen(MenuRef.menuScreen));
        return MenuRef.menuScreen;
    }
    private static void CreateEffectsMenu()
    {
        PersistentEffectsMenu ??= CreateAnEffectMenu<AbstractPersistentEffect>("Persistent Effects");
        CoolDownEffectsMenu ??= CreateAnEffectMenu<AbstractCoolDownEffect>("CoolDown Effects");
        BossAttacksMenu ??= CreateAnEffectMenu<AbstractBossAttack>("Boss Attack Effects");
        EnemyFollowMenu ??= CreateAnEffectMenu<AbstractEnemyFollow>("Enemy Follow Effects");

        EffectsMenu ??= new Menu("Effects", new Element[]
        {
            Blueprints.NavigateToMenu("Persistent Effect Toggles",
                "", () => PersistentEffectsMenu.menuScreen),
            Blueprints.NavigateToMenu("CoolDown Effect Toggles",
                "", () => CoolDownEffectsMenu.menuScreen),
            Blueprints.NavigateToMenu("Boss Attack Toggles",
                "", () => BossAttacksMenu.menuScreen),
            Blueprints.NavigateToMenu("Enemy Follow Toggles",
                "", () => EnemyFollowMenu.menuScreen),
            new TextPanel(""),
            new MenuButton("Enable All Effects",
                "Use this to enable all effects.",
                (_) =>
                {
                    foreach (var effectname in AdditionalChallenge.settings.EffectIsEnabledDictionary.Keys.ToList())
                    {
                        AdditionalChallenge.settings.EffectIsEnabledDictionary[effectname] = true;
                    }

                    AdditionalChallenge.Instance.MatchSettings();
                    PersistentEffectsMenu.Update(); //TODO:Fix NRE here
                    CoolDownEffectsMenu.Update();
                    BossAttacksMenu.Update();
                    EnemyFollowMenu.Update();
                }),
            new MenuButton("Disable All Effects",
                "Use this to disable all effects.",
                _ =>
                {
                    foreach (var effectname in AdditionalChallenge.settings.EffectIsEnabledDictionary.Keys.ToList())
                    {
                        AdditionalChallenge.settings.EffectIsEnabledDictionary[effectname] = false;
                    }

                    AdditionalChallenge.Instance.MatchSettings();
                    PersistentEffectsMenu.Update();
                    CoolDownEffectsMenu.Update();
                    BossAttacksMenu.Update();
                    EnemyFollowMenu.Update();
                })
        });
    }

    private static Menu CreateAnEffectMenu<AbstractEffectsClass>(string name)
    {
        var menu = new Menu(name, Array.Empty<Element>());
        foreach (var effect in AdditionalChallenge.AllEffects.Where(effect => effect is AbstractEffectsClass))
        {
            effect.AddElementsToModMenu(menu);
        }
        return menu;
    }
    //TODO: New button that takes to HKMP screen
    //TODO: main menu screens  for keybind one
    /*Text Heading with ID
     * Desc
     * KeyBInd
     *  HO for effect name
     *  HO for player
     * x5
     * */
     
     //TODO: Toggle for effect length (for persistent and enemy follow) (5-60)
}