using AdditionalChallenge.Effects.EnemyFollow;

namespace AdditionalChallenge;

public static class ChaosMode
{
    static ChaosMode()
    {
        controllerGO = new GameObject("Choas Controller");
        controller = controllerGO.AddComponent<ChaosModeController>();
        UObject.DontDestroyOnLoad(controllerGO);
    }
    public static void ApplySetting()
    {
        if (AdditionalChallenge.settings.ChaosModeEnabled)
        {
            ReleaseTheChaos();
        }
        else
        {
            ContainTheChaos();
        }
    }

    internal static HashSet<AbstractEffects> RNGList = new HashSet<AbstractEffects>();
    private static ChaosModeController controller;
    private static GameObject controllerGO;

    public static void ReleaseTheChaos()
    {
        //call unload on all effects
        foreach (var effect in AdditionalChallenge.AllEffects)
        {
            effect!.Unload();
        }
        
        ChooseList();


        //if is persistent, call load
        //if its coolDown based, call load but somehow fix handle timer
    }

    public static void ChooseList()
    {
        //choose numEffects number of effects
        AdditionalChallenge.Instance.Log("Gettting enabled list");
        List<AbstractEffects> enabledList = AdditionalChallenge.AllEffects.Where(effect => 
            AdditionalChallenge.settings.EffectIsEnabledDictionary.ContainsKey(MiscExtensions.GetKey(effect, nameof(effect.IsEnabled))) 
            && AdditionalChallenge.settings.EffectIsEnabledDictionary[MiscExtensions.GetKey(effect, nameof(effect.IsEnabled))]).ToList();

        int numEffects = AdditionalChallenge.settings.numEffects;
        if (enabledList.Count == 0) return;
        if (AdditionalChallenge.settings.numEffects > enabledList.Count)
        {
            AdditionalChallenge.Instance.LogWarn("The number of effects exceeds the number of enabled effects. You need to enable more effects for this option to work as intended");
            numEffects = enabledList.Count;
        }
        
        RNGList = new HashSet<AbstractEffects>();
        while (RNGList.Count < numEffects)
        {
            RNGList.Add(enabledList[URandom.Range(0,enabledList.Count)]);
        }
        AdditionalChallenge.Instance.Log("Logging effects chosen");
        RNGList.ToList().ForEach(effect => AdditionalChallenge.Instance.Log($"Chosen {effect.ToggleName}"));
        
    }

    public static void ContainTheChaos()
    {
        controller.Stop();
        foreach (var effect in AdditionalChallenge.AllEffects.
                     Where(effect => AdditionalChallenge.settings.EffectIsEnabledDictionary.ContainsKey(MiscExtensions.GetKey(effect, nameof(effect.IsEnabled)))).
                     Where(effect => AdditionalChallenge.settings.EffectIsEnabledDictionary[MiscExtensions.GetKey(effect, nameof(effect.IsEnabled))]))
        {
            effect!.Load();
        }
    }
}

public class ChaosModeController : MonoBehaviour
{
    private float timer;
    
    private bool HandleTimer()
    {
        if (AdditionalChallenge.settings.chaosCoolDown == 0f)
        {
            return false;
        }

        if (HeroController.instance == null)
        {
            return false;
        }
        timer += Time.deltaTime;
        if (timer > AdditionalChallenge.settings.chaosCoolDown)
        {
            timer = 0;
            return true;
        }

        return false;
    }

    private int index = 0;

    public List<AbstractEffects> ChosenList;
    public void Update()
    {
        if (AdditionalChallenge.settings.ChaosModeEnabled)
        {
            if (HandleTimer())
            {
                AdditionalChallenge.Instance.Log("Time to release chaos");
                if (ChosenList != null)
                {
                    foreach (var effect in ChosenList)
                    {
                        effect.Unload();
                    }
                }
                ChosenList = ChaosMode.RNGList.ToArray().ToList();
                ChaosMode.ChooseList(); //choose new effects
                index = 0;
                AdditionalChallenge.CoroutineSlave.StartCoroutine(HandleDelay());
            }
        }
    }

    public void Stop()
    {
        GameManager.instance.StopCoroutine(HandleDelay());
        if (ChosenList != null)
        {
            foreach (var effect in ChosenList)
            {
                effect.Unload();
            }
        }
    }

    private IEnumerator HandleDelay()
    {
        while (index < ChosenList.Count)
        {
            if (AdditionalChallenge.settings.delayBetweenTriggeringEffects > 0)
            {
                yield return new WaitForSeconds(AdditionalChallenge.settings.delayBetweenTriggeringEffects);
            }
            var effect = ChosenList[index];
            AdditionalChallenge.Instance.Log($"Loading {effect.ToggleName} from chaos mode");

            effect.Load();
            switch (effect)
            {
                case AbstractCoolDownEffect coolDownEffect:
                    coolDownEffect.DoEffect();
                    break;
                case AbstractBossAttack bossAttack:
                    bossAttack.Attack();
                    break;
            }
            index++;
        }
    }
}