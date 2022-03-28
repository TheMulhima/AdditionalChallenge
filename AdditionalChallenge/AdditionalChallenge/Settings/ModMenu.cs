using AdditionalChallenge.Effects;

namespace AdditionalChallenge;

public static class ModMenu
{
    private static Menu MenuRef;
    private static Menu PersistentEffectsMenu;
    private static Menu CoolDownEffectsMenu;
    private static Menu BossAttacksMenu;

    public static MenuScreen CreateMenuScreen(MenuScreen modListMenu)
    {
        CreateCoolDownEffectMenu();
        CreatePersistentEffectMenu();
        CreateBossAttacksMenu();
        MenuRef = new Menu("Mod Menu", new Element[]
        {
            Blueprints.NavigateToMenu("Persistent Effect Toggles",
                "", () => PersistentEffectsMenu.GetMenuScreen(MenuRef.menuScreen)),
            Blueprints.NavigateToMenu("CoolDown Effect Toggles",
                "", () => CoolDownEffectsMenu.GetMenuScreen(MenuRef.menuScreen)),
            Blueprints.NavigateToMenu("Boss Attack Toggles",
                "", () => BossAttacksMenu.GetMenuScreen(MenuRef.menuScreen))
        });

        return MenuRef.GetMenuScreen(modListMenu);
    }

    private static void CreateBossAttacksMenu()
    {
        BossAttacksMenu ??= new Menu("Boss Attacks", Array.Empty<Element>());
        foreach (var effect in AdditionalChallenge.AllEffects.Where(effect => effect is AbstractBossAttack))
        {
            effect.AddElementsToModMenu(BossAttacksMenu);
        }
    }
    
    private static void CreateCoolDownEffectMenu()
    {
        CoolDownEffectsMenu ??= new Menu("CoolDown Effects", Array.Empty<Element>());
        foreach (var effect in AdditionalChallenge.AllEffects.Where(effect => effect is AbstractCoolDownEffect))
        {
            effect.AddElementsToModMenu(CoolDownEffectsMenu);
        }
    }
    
    private static void CreatePersistentEffectMenu()
    {
        PersistentEffectsMenu ??= new Menu("Persistent Effects", Array.Empty<Element>());
        foreach (var effect in AdditionalChallenge.AllEffects.Where(effect => effect is AbstractPersistentEffect))
        {
            effect.AddElementsToModMenu(PersistentEffectsMenu);
        }
    }
}