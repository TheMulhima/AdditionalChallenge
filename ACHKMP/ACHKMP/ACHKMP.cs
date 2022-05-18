namespace ACHKMP;

public class ACHKMP : Mod, IGlobalSettings<GlobalSettings>, ICustomMenuMod
{
    internal static ACHKMP Instance;

    private bool holdInput = false;

    public float EffectUnloadTime = HKMPInfo.DefaultEffectUnloadTime;
    public float KeyPressDownTime = HKMPInfo.DefaultKeyPressDownTime;
    
    public static GlobalSettings settings { get; set; } = new GlobalSettings();
    public void OnLoadGlobal(GlobalSettings s) => settings = s;

        public GlobalSettings OnSaveGlobal() => settings;

    public override string GetVersion() => AssemblyUtils.GetAssemblyVersionHash();
    
    internal static ACClient Client = new ACClient();
    internal static ACServer Server = new ACServer();

    public override void Initialize()
    {
        Instance ??= this;
        
        ClientAddon.RegisterAddon(Client);
        ServerAddon.RegisterAddon(Server);

        ModHooks.HeroUpdateHook += () =>
        {
            if (Client._clientApi.NetClient is { IsConnected: true })
            {
                settings.Keybinds.RunIfKeyPressed((i) =>
                {
                    if (!holdInput)
                    {
                        holdInput = true;
                        AdditionalChallenge.AdditionalChallenge.CoroutineSlave.StartCoroutine(ResetHoldInput());
                    }
                    bool targetEveryone = settings.TargetPlayers[i] == ModMenu.AllPlayers[0];
                    ushort playerId;
                    if (!targetEveryone)
                    {
                        try
                        {
                            playerId =  Client._clientApi.ClientManager.Players
                                .First(player => string.Equals(player.Username, settings.TargetPlayers[i], StringComparison.CurrentCultureIgnoreCase)).Id;
                        }
                        catch (InvalidOperationException e)
                        {
                            playerId = 60001;
                        }
                    }
                    else
                    {
                        playerId = 60000;
                    }
                    if (settings.EffectNames[i] == "")
                    {
                        Log($"No connection {settings.TargetPlayers[i]} or effect name empty {settings.EffectNames[i]}");
                        return;
                    }
                    Client.SendRequest(new ToServerRequestEffect()
                    {
                        effectName = settings.EffectNames[i],
                        targetEveryone = targetEveryone,
                        targetPlayerId = playerId,
                    });
                });
            }
        };
        Dictionary<string, float> x = new Dictionary<string, float>()
        {
            { nameof(EffectUnloadTime), HKMPInfo.DefaultEffectUnloadTime },
            { nameof(KeyPressDownTime), HKMPInfo.DefaultKeyPressDownTime },
        };
        Log(JsonConvert.SerializeObject(x, Formatting.Indented));
    }

    private IEnumerator ResetHoldInput()
    {
        yield return new WaitForSecondsRealtime(KeyPressDownTime);
        holdInput = false;
    }

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? _) =>
        ModMenu.CreateMenuScreen(modListMenu);


    public bool ToggleButtonInsideMenu { get; }
}