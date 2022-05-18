using Modding.Menu;
using UnityEngine.EventSystems;
using Logger = Modding.Logger;
using Object = UnityEngine.Object;
using UMenuButton = UnityEngine.UI.MenuButton;
using MenuButton = Satchel.BetterMenus.MenuButton;
using UnityEngine.UI;

namespace ACHKMP;

public static class ModMenu
{
    public static Menu HKMPMenu;

    //TODO: 2 second delay, stop coroutine

    public static string[] effectNames
    {
        get
        {
            return AdditionalChallenge.AdditionalChallenge.AllEffects.Select(effect => effect!.ToggleName).ToArray();
        }
    }

    public static MenuScreen CreateMenuScreen(MenuScreen modListMenu)
    {
        if (HKMPMenu == null)
        {
            HKMPMenu ??= new Menu("AC's HKMP Settings", new Element[]
            {
                new MenuButton("Update Menu", "Click to update the menu", (_) => HKMPMenu.Update())
            });
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
        HKMPMenu.AddElement(new StaticPanel("Target Player", (go) =>
        {
            go.AddTextToStaticPanel();
            go.CreateInputField(effectNumber);
        }));
        HKMPMenu.AddElement(new HorizontalOption("Effect Name", "Choose what effect should happen",
            effectNames,
            (s) => ACHKMP.settings.EffectNames[effectNumber] = effectNames[s],
            () =>
            {
                //effectNames.ToList().ForEach(Modding.Logger.Log);
                int index =  effectNames.ToList().IndexOf(ACHKMP.settings.EffectNames[effectNumber]);
                Modding.Logger.Log(index);
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
            Logger.Log("Returning no connection");
            return AllPlayers;
        }
        else
        {
            Logger.Log("Returning a list");
            var x = AllPlayers.Concat(ACHKMP.Client._clientApi.ClientManager.Players.Select(player => player.Username)).ToArray();
            x.ToList().ForEach(Logger.Log);
            return x;
        }
    }
    
    public static string[] AllPlayers => new string[] { "All" };

    private static void CreateInputField(this GameObject parent, int i)
    {
        UMenuButton backprefab = Object.Instantiate(UIManager.instance.playModeMenuScreen.defaultHighlight
            .FindSelectableOnDown().FindSelectableOnDown().gameObject).GetComponent<UMenuButton>();

        GameObject seedGameObject = backprefab.Clone("Seed", UMenuButton.MenuButtonType.Activate, parent).gameObject;
        Object.DestroyImmediate(seedGameObject.GetComponent<UMenuButton>());
        Object.DestroyImmediate(seedGameObject.GetComponent<EventTrigger>());
        Object.DestroyImmediate(seedGameObject.transform.Find("Text").GetComponent<AutoLocalizeTextUI>());
        Object.DestroyImmediate(seedGameObject.transform.Find("Text").GetComponent<FixVerticalAlign>());
        Object.DestroyImmediate(seedGameObject.transform.Find("Text").GetComponent<ContentSizeFitter>());

        RectTransform seedRect = seedGameObject.transform.Find("Text").GetComponent<RectTransform>();
        seedRect.anchorMin = seedRect.anchorMax = new Vector2(0.5f, 0.5f);
        seedRect.sizeDelta = new Vector2(337, 63.2f);

        InputField customSeedInput = seedGameObject.AddComponent<InputField>();
        customSeedInput.transform.localPosition = new Vector3(400, -50);
        customSeedInput.textComponent = seedGameObject.transform.Find("Text").GetComponent<Text>();

        customSeedInput.text = ACHKMP.settings.TargetPlayers[i];

        customSeedInput.caretColor = Color.white;
        customSeedInput.contentType = InputField.ContentType.Name;
        customSeedInput.onEndEdit.AddListener(call => ParseSeedInput(call, i));
        customSeedInput.navigation = Navigation.defaultNavigation;
        customSeedInput.caretWidth = 8;
        customSeedInput.characterLimit = 20;

        customSeedInput.colors = new ColorBlock
        {
            highlightedColor = Color.yellow,
            pressedColor = Color.red,
            disabledColor = Color.black,
            normalColor = Color.white,
            colorMultiplier = 2f
        };
    }

    private static void ParseSeedInput(string input, int i)
    {
        ACHKMP.settings.TargetPlayers[i] = input;
    }

    public static UMenuButton Clone(this UMenuButton self,
        string name,
        UMenuButton.MenuButtonType type,
        GameObject parent)
    {
        // Set up duplicate of button
        UMenuButton newBtn = Object.Instantiate(self.gameObject).GetComponent<UMenuButton>();
        newBtn.name = name;
        newBtn.buttonType = type;

        Transform transform = newBtn.transform;
        transform.SetParent(parent.transform);
        transform.localScale = self.transform.localScale;

        // Change text on the button
        Transform textTrans = newBtn.transform.Find("Text");
        Object.Destroy(textTrans.GetComponent<AutoLocalizeTextUI>());

        return newBtn;
    }

    private static void AddTextToStaticPanel(this GameObject staticpanel)
    {
        var text = staticpanel.AddComponent<Text>();
        text.text = "Target Player";
        text.fontSize = 35;
        text.font = MenuResources.TrajanBold;
        text.supportRichText = true;
        text.alignment = TextAnchor.MiddleLeft;
    }
}