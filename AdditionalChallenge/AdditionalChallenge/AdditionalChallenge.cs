using AdditionalChallenge.Effects;
using JetBrains.Annotations;
using Logger = On.InControl.Logger;

namespace AdditionalChallenge;

public class AdditionalChallenge : Mod, IGlobalSettings<GlobalSettings>, ICustomMenuMod
{
    internal static AdditionalChallenge Instance;
    internal static GameObject ComponentHolder;
    internal static NonBouncer CoroutineSlave;
    public static GlobalSettings settings { get; set; } = new GlobalSettings();
    public void OnLoadGlobal(GlobalSettings s) => settings = s;
    public GlobalSettings OnSaveGlobal() => settings;

    public override string GetVersion() => AssemblyUtils.GetAssemblyVersionHash();

    [ItemCanBeNull] internal static List<AbstractEffects> AllEffects = new List<AbstractEffects>();

    internal List<AudioClip> Clips = new List<AudioClip>();

    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        Instance ??= this;
        Preloads.Load(preloadedObjects);

        On.HeroController.Start += (orig, self) =>
        {
            orig(self);
            MatchSettings();
        };
        
        GetAllAudioClips();
        HooksForCameraEffects();
        AddToDebugBinds();

        CoroutineSlave = ComponentHolder.GetAddComponent<NonBouncer>();
    }

    private void HooksForCameraEffects()
    {
        On.tk2dCamera.UpdateCameraMatrix += BaseCameraEffects.OnUpdateCameraMatrix;

        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += BaseCameraEffects.DisableInMainMenu;
    }
    
    

    private void GetAllAudioClips()
    {
        //Get audioclips
        Resources.LoadAll("");
        Clips = Resources.FindObjectsOfTypeAll<AudioClip>().ToList();
    }

    private void AddToDebugBinds()
    {
        //add to debug binds
        foreach (var effect in ComponentHolder.GetComponents(typeof(AbstractEffects)).Select(effect => effect as AbstractEffects))
        {
            if (effect is AbstractCoolDownEffect)
            {
                DebugMod.DebugMod.AddActionToKeyBindList(() => effect!.DoEffect(), effect!.ToggleName, "AC CoolDown");
            }
            else if (effect is AbstractPersistentEffect persistentEffect)
            {
                DebugMod.DebugMod.AddActionToKeyBindList(() => persistentEffect!.DoEffect(), $"Do {persistentEffect!.ToggleName}", "AC Persistent");
                DebugMod.DebugMod.AddActionToKeyBindList(() => persistentEffect!.UnDoEffect(), $"UnDo {persistentEffect!.ToggleName}", "AC Persistent");
            }
        }
    }
    public void MatchSettings()
    {
        foreach (var effect in AllEffects)
        {
            if (!settings.Booleans.ContainsKey(MiscExtensions.GetKey(effect, nameof(effect.IsEnabled))))
            {
                settings.Booleans[MiscExtensions.GetKey(effect, nameof(effect.IsEnabled))] = false;
            }
            effect.SetEnabled(settings.Booleans[MiscExtensions.GetKey(effect, nameof(effect.IsEnabled))]);
            if (effect is AbstractCoolDownEffect cooldownEffect)
            {
                if (!settings.Floats.ContainsKey(MiscExtensions.GetKey(cooldownEffect, nameof(cooldownEffect.coolDown))))
                {
                    settings.Floats[MiscExtensions.GetKey(cooldownEffect, nameof(cooldownEffect.coolDown))] = 0;
                }
                cooldownEffect.coolDown =
                    settings.Floats[MiscExtensions.GetKey(cooldownEffect, nameof(cooldownEffect.coolDown))];
            }
        }
    }

    public override List<(string, string)> GetPreloadNames() => Preloads.ObjectList.Values.ToList();

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? _) =>
        ModMenu.CreateMenuScreen(modListMenu);


    public bool ToggleButtonInsideMenu { get; }
}