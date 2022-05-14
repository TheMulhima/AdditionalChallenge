namespace ACHKMP;

public class ACHKMP : Mod, IGlobalSettings<GlobalSettings>, ICustomMenuMod
{
    internal static ACHKMP Instance;

    public static GlobalSettings settings { get; set; } = new GlobalSettings();
    public void OnLoadGlobal(GlobalSettings s)
    {
        settings = s;
        if (settings.EffectNames == null || settings.TargetPlayers == null || settings.EffectNames.Count == 0 || settings.TargetPlayers.Count == 0)
        {
            //TODO: MAke it use values from list and not make it 0,1,2,3,4
            settings.EffectNames = new List<string>(AdditionalChallenge.AdditionalChallenge.StoreableEffects);
            settings.TargetPlayers = new List<string>(AdditionalChallenge.AdditionalChallenge.StoreableEffects);

            settings.EffectNames.AddRange(Enumerable.Range(0, AdditionalChallenge.AdditionalChallenge.StoreableEffects).Select(x => x.ToString()));
            settings.TargetPlayers.AddRange(Enumerable.Range(0, AdditionalChallenge.AdditionalChallenge.StoreableEffects).Select(x => x.ToString()));
        }
    }

    public GlobalSettings OnSaveGlobal() => settings;

    public override string GetVersion() => AssemblyUtils.GetAssemblyVersionHash();
    
    internal static ACClient Client = new ACClient();
    internal static ACServer Server = new ACServer();

    public override void Initialize()
    {
        Instance ??= this;
        
        ClientAddon.RegisterAddon(Client);
        ServerAddon.RegisterAddon(Server);
    }

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? _) =>
        ModMenu.CreateMenuScreen(modListMenu);


    public bool ToggleButtonInsideMenu { get; }
}