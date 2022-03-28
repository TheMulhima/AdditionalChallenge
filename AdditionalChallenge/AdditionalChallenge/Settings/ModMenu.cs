using AdditionalChallenge.Effects;

namespace AdditionalChallenge;

public static class ModMenu
{
    private static Menu MenuRef;
    private static Menu PersistentEffectsMenu;
    private static Menu CoolDownEffectsMenu;

    public static MenuScreen CreateMenuScreen(MenuScreen modListMenu)
    {
        CreateCoolDownEffectMenu();
        CreatePersistentEffectMenu();
        MenuRef = new Menu("Mod Menu", new Element[]
        {
            Blueprints.NavigateToMenu("Persistent Effect Toggles",
                "", () => PersistentEffectsMenu.GetMenuScreen(MenuRef.menuScreen)),
            Blueprints.NavigateToMenu("CoolDown Effect Toggles",
                "", () => CoolDownEffectsMenu.GetMenuScreen(MenuRef.menuScreen))
        });

        return MenuRef.GetMenuScreen(modListMenu);
    }

    private static void CreateCoolDownEffectMenu()
    {
        CoolDownEffectsMenu ??= new Menu("CoolDown Effects", Array.Empty<Element>());
        foreach (var effect in AdditionalChallenge.AllEffects.Where(effect => effect is AbstractCoolDownEffect))
        {
            var coolDownEffect = effect as AbstractCoolDownEffect;
            CoolDownEffectsMenu.AddElement(new HorizontalOption(coolDownEffect!.ToggleName,
                coolDownEffect.ToggleDesc,
                new string[] { "Enabled", "Disabled" },
                (i) =>
                {
                    AdditionalChallenge.settings.Booleans[MiscExtensions.GetKey(coolDownEffect, nameof(coolDownEffect.IsEnabled))] =
                        i == 0;
                    AdditionalChallenge.Instance.MatchSettings();

                    CoolDownEffectsMenu.Find($"{coolDownEffect.ToggleName} cool down").isVisible = i == 0;
                    CoolDownEffectsMenu.Update();
                },
                () => AdditionalChallenge.settings.Booleans.ContainsKey(MiscExtensions.GetKey(coolDownEffect, nameof(coolDownEffect.IsEnabled))) ?
            (AdditionalChallenge.settings.Booleans[MiscExtensions.GetKey(coolDownEffect, nameof(coolDownEffect.IsEnabled))] ? 0 : 1) 
            : 1));

            //satchel custom slider currently broken
            CoolDownEffectsMenu.AddElement(new CustomSlider($"Cool Down",
                (s) =>
                {
                    AdditionalChallenge.settings.Floats[
                        MiscExtensions.GetKey(coolDownEffect, nameof(coolDownEffect.coolDown))] = s;
                    AdditionalChallenge.Instance.MatchSettings();
                },
                () =>
                {
                    if (AdditionalChallenge.settings.Floats.ContainsKey(
                            MiscExtensions.GetKey(coolDownEffect, nameof(coolDownEffect.coolDown))))
                    {
                        return AdditionalChallenge.settings.Floats[
                            MiscExtensions.GetKey(coolDownEffect, nameof(coolDownEffect.coolDown))];
                    }
                    else
                    {
                        return 0;
                    }
                },
                Id: $"{coolDownEffect.ToggleName} cool down")
                {
                    isVisible =
                        AdditionalChallenge.settings.Booleans.ContainsKey(MiscExtensions.GetKey(coolDownEffect, nameof(coolDownEffect.IsEnabled))) ?
                            AdditionalChallenge.settings.Booleans[MiscExtensions.GetKey(coolDownEffect, nameof(coolDownEffect.IsEnabled))] :
                            false,
                    minValue = 0,
                    maxValue = 150,
                    wholeNumbers = false,
                }
                );
            
            /*float start = 0;
            float step = 0.5f;
            float end = 120f;
            CoolDownEffectsMenu.AddElement(new HorizontalOption($"{coolDownEffect.ToggleName} cool down",
                coolDownEffect.ToggleDesc,
                Enumerable.Range((int)(start / step), (int)(Math.Abs(end - start) / step) + 1)
                    .Select(x => (x * step).ToString()).ToArray(),
                i =>
                {
                    AdditionalChallenge.settings.Floats[
                        MiscExtensions.GetKey(coolDownEffect, nameof(coolDownEffect.coolDown))] =
                    ((i + (start / step)) * step);
                    
                    AdditionalChallenge.Instance.MatchSettings();
                },
                () =>
                {
                    if (AdditionalChallenge.settings.Floats.ContainsKey(
                            MiscExtensions.GetKey(coolDownEffect, nameof(coolDownEffect.coolDown))))
                    {
                        return (int)((AdditionalChallenge.settings.Floats[
                                      MiscExtensions.GetKey(coolDownEffect, nameof(coolDownEffect.coolDown))] / step) -
                                  (start / step));
                    }
                    else
                    {
                        return (int) start;
                    }
                },
                Id: $"{coolDownEffect.ToggleName} cool down")
                {
                isVisible =
                    AdditionalChallenge.settings.Booleans.ContainsKey(MiscExtensions.GetKey(coolDownEffect, nameof(coolDownEffect.IsEnabled))) ?
                     AdditionalChallenge.settings.Booleans[MiscExtensions.GetKey(coolDownEffect, nameof(coolDownEffect.IsEnabled))] :
                     false
                });*/
        }
    }
    
    private static void CreatePersistentEffectMenu()
    {
        PersistentEffectsMenu ??= new Menu("Persistent Effects", Array.Empty<Element>());
        foreach (var effect in AdditionalChallenge.AllEffects.Where(effect => effect is AbstractPersistentEffect))
        {
            var persistentEffect = effect as AbstractPersistentEffect;
            PersistentEffectsMenu.AddElement(new HorizontalOption(persistentEffect!.ToggleName,
                persistentEffect.ToggleDesc,
                new string[] { "Enabled", "Disabled" },
                (i) =>
                {
                    AdditionalChallenge.settings.Booleans[
                            MiscExtensions.GetKey(persistentEffect, nameof(persistentEffect.IsEnabled))] =
                        i == 0;
                    AdditionalChallenge.Instance.MatchSettings();
                },
                () => AdditionalChallenge.settings.Booleans.ContainsKey(MiscExtensions.GetKey(persistentEffect,
                    nameof(persistentEffect.IsEnabled)))
                    ? AdditionalChallenge.settings.Booleans[
                        MiscExtensions.GetKey(persistentEffect, nameof(persistentEffect.IsEnabled))] ? 0 : 1
                    : 1));
        }
    }
}