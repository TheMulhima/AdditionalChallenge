namespace ACHKMP;

public static class ModMenu
{
    public static Menu HKMPMenu;
    
    private static string[] _effectNames = null;

    public static string[] effectNames
    {
        get
        {
            if (_effectNames != null) return _effectNames;

            return AdditionalChallenge.AdditionalChallenge.AllEffects.Select(effect => effect!.ToggleName).ToArray();
        }
    }

    public static MenuScreen CreateMenuScreen(MenuScreen modListMenu)
    {
        if (HKMPMenu == null)
        {
            HKMPMenu ??= new Menu("AC's HKMP Settings", Array.Empty<Element>());
            for (int i = 0; i < AdditionalChallenge.AdditionalChallenge.StoreableEffects; i++)
            {
                CreateHKMPMenuElems(i);
            }
        }

        return HKMPMenu.GetMenuScreen(modListMenu);
        
    }

    private static void CreateHKMPMenuElems(int effectNumber)
    {
        HKMPMenu.AddElement(new TextPanel($"Effect ID {effectNumber}", fontSize: 55));
        HKMPMenu.AddElement(new HorizontalOption("Target Player", 
            "Choose which player to target",
            GetHKMPPlayers(),
            (s) =>
            {
                ACHKMP.settings.TargetPlayers[effectNumber] = GetHKMPPlayers()[s];
            },
            () =>
            {
                if (ACHKMP.Client._clientApi == null ||
                    !ACHKMP.Client._clientApi.NetClient.IsConnected)
                {
                    return 0;
                }

                int index = GetHKMPPlayers().ToList().IndexOf(ACHKMP.settings.TargetPlayers[effectNumber]);
                if (index == -1)
                {
                    ACHKMP.settings.TargetPlayers[effectNumber] = AllPlayers[effectNumber];
                    return 0; //0th element is all players
                }
                return index;
            }));
        HKMPMenu.AddElement(new HorizontalOption("Effect Name", "Choose what effect should happen",
            effectNames,
            (s) => ACHKMP.settings.EffectNames[effectNumber] = effectNames[s],
            () =>
            {
                int index =  effectNames.ToList().IndexOf(ACHKMP.settings.EffectNames[effectNumber]);
                if (index == -1)
                {
                    ACHKMP.settings.EffectNames[effectNumber] = effectNames[0]; //easier to just set it
                    return 0;
                }

                return index;

            }));
        HKMPMenu.AddElement(new KeyBind($"Keybind effect {effectNumber}", ACHKMP.settings.Keybinds.GetKey(effectNumber)));
        HKMPMenu.AddElement(new StaticPanel("Empty Space", _ => { }));
    }

    

    public static string[] GetHKMPPlayers()
    {
        if (ACHKMP.Client._clientApi == null ||
            !ACHKMP.Client._clientApi.NetClient.IsConnected)
        {
            return NoConnection;
        }
        else
        {
            return AllPlayers.Concat(ACHKMP.Client._clientApi.ClientManager.Players.Select(player => player.Username)).ToArray();
        }
    }
    
    
    private static string[] NoConnection => new string[] { "No Connection" };
    private static string[] AllPlayers => new string[] { "All Players" };
}