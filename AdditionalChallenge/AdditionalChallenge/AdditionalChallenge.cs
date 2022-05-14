using Satchel;
using JetBrains.Annotations;

namespace AdditionalChallenge;

public class AdditionalChallenge : Mod, IGlobalSettings<GlobalSettings>, ICustomMenuMod
{
    internal static AdditionalChallenge Instance;
    internal static GameObject ComponentHolder;
    public static NonBouncer CoroutineSlave;
    public static GlobalSettings settings { get; set; } = new GlobalSettings();
    public void OnLoadGlobal(GlobalSettings s) => settings = s;
    public GlobalSettings OnSaveGlobal() => settings;
    
    public override string GetVersion() => Satchel.AssemblyUtils.GetAssemblyVersionHash();
    
    [ItemCanBeNull] public static List<AbstractEffects> AllEffects = new List<AbstractEffects>();
    internal List<AudioClip> Clips = new List<AudioClip>();

    public static int StoreableEffects = 5;

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
        /*
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
        }*/
    }
    public void MatchSettings()
    {
        if (settings.ChaosModeEnabled)
        {
            //ChaosMode.ChooseList();
            return;
        }
        foreach (var effect in AllEffects)
        {
            //if key is missing, add entry in dict and set default to false
            if (!settings.EffectIsEnabledDictionary.ContainsKey(MiscExtensions.GetKey(effect, nameof(effect.IsEnabled))))
            {
                settings.EffectIsEnabledDictionary[MiscExtensions.GetKey(effect, nameof(effect.IsEnabled))] = false;
            }
            
            //match the isEnabled of effect with the GS Dict setting
            effect!.SetEnabled(settings.EffectIsEnabledDictionary[MiscExtensions.GetKey(effect, nameof(effect.IsEnabled))]);
            
            //Do same for cooldowns
            if (effect is AbstractCoolDownEffect cooldownEffect)
            {
                if (!settings.EffectCoolDownDictionary.ContainsKey(MiscExtensions.GetKey(cooldownEffect, nameof(cooldownEffect.coolDown))))
                {
                    settings.EffectCoolDownDictionary[MiscExtensions.GetKey(cooldownEffect, nameof(cooldownEffect.coolDown))] = 0;
                }
                cooldownEffect.coolDown =
                    settings.EffectCoolDownDictionary[MiscExtensions.GetKey(cooldownEffect, nameof(cooldownEffect.coolDown))];
            }
            else if (effect is AbstractBossAttack bossAttack)
            {
                if (!settings.EffectCoolDownDictionary.ContainsKey(MiscExtensions.GetKey(bossAttack, nameof(bossAttack.timeBetweenAttacks))))
                {
                    settings.EffectCoolDownDictionary[MiscExtensions.GetKey(bossAttack, nameof(bossAttack.timeBetweenAttacks))] = 0;
                }
                bossAttack.timeBetweenAttacks =
                    settings.EffectCoolDownDictionary[MiscExtensions.GetKey(bossAttack, nameof(bossAttack.timeBetweenAttacks))];
            }
        }
    }

    public override List<(string, string)> GetPreloadNames() => Preloads.ObjectList.Values.ToList();

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? dels) =>
        ModMenu.CreateMenuScreen(modListMenu);


    public bool ToggleButtonInsideMenu { get; }
}