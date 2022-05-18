using System.Runtime.Serialization;

namespace AdditionalChallenge;

public class GlobalSettings
{
    //to store in game and onquit will be transfered to Booleans and Floats
    [NonSerialized]
    internal readonly List<(PropertyInfo, AbstractEffects)> Properties = new();
    
    //to store enabled 
    public Dictionary<string, bool> EffectIsEnabledDictionary { get; set; } = new();
    
    //to store cooldowns
    public Dictionary<string, float> EffectCoolDownDictionary { get; set; } = new();

    public bool ChaosModeEnabled = false;
    public int numEffects = 1;
    public float chaosCoolDown = 0f;
    public float delayBetweenTriggeringEffects = 0f;
    
    //Infinity means random;
    public float windSpeed = Mathf.Infinity;
    public float nailScale = Mathf.Infinity;
    public float timeScale = Mathf.Infinity;

    public GlobalSettings()
    {
        GameManager.instance.StartCoroutine(PopulateDicts());

        IEnumerator PopulateDicts()
        {
            AdditionalChallenge.ComponentHolder = new GameObject();
            UObject.DontDestroyOnLoad(AdditionalChallenge.ComponentHolder);
            foreach (Type effect in typeof(AdditionalChallenge).Assembly.GetTypes()
                         .Where(type => type.IsSubclassOf(typeof(AbstractEffects)) && !type.IsAbstract))
            {
                AdditionalChallenge.ComponentHolder.AddComponent(effect);
            }
            
            //we need to wait for the components to actually be gettable
            yield return null;

            foreach (var effect in AdditionalChallenge.ComponentHolder.GetComponents(typeof(AbstractEffects)))
            {
                var component  = effect as AbstractEffects;
                if (!AdditionalChallenge.AllEffects.Contains(component))
                {
                    AdditionalChallenge.AllEffects.Add(component);

                    var list = component.GetPropertiesToAdd();
                    for (int i = 0; i < list.Count; i++)
                    {
                        var propertyInfo = list[i].Item1;
                        var instance = list[i].Item2;
                        Properties.Add((propertyInfo, instance));
                        SetValuesInDicts(propertyInfo, instance);
                    }
                }
            }
            MiscExtensions.GenerateReadmeFeature();
        }
    }

    private void SetValuesInDicts(PropertyInfo propertyInfo, AbstractEffects instance)
    {
        if (propertyInfo.PropertyType == typeof(bool))
        {
            EffectIsEnabledDictionary[MiscExtensions.GetKey(instance, propertyInfo)] =
                ReflectionHelper.GetProperty<AbstractEffects, bool>(instance, propertyInfo.Name);
        }
        else if (propertyInfo.PropertyType == typeof(float))
        {
            if (instance is AbstractCoolDownEffect coolDownEffect)
            {
                EffectCoolDownDictionary[MiscExtensions.GetKey(instance, propertyInfo)] =
                    ReflectionHelper.GetProperty<AbstractCoolDownEffect, float>(coolDownEffect,propertyInfo.Name);
            }
            else if (instance is AbstractBossAttack bossAttack)
            {
                EffectCoolDownDictionary[MiscExtensions.GetKey(instance, propertyInfo)] =
                    ReflectionHelper.GetProperty<AbstractBossAttack, float>(bossAttack,propertyInfo.Name);
            }
        }
    }

    [OnSerializing]
    public void OnBeforeSerialize(StreamingContext _)
    {
        foreach (var (pi, instance) in Properties)
        {
            SetValuesInDicts(pi, instance);
        }
    }

    [OnDeserialized]
    public void OnAfterDeserialize(StreamingContext _)
    {
        foreach (var (pi, instance) in Properties)
        {
            if (pi.PropertyType == typeof(bool))
            {
                if (EffectIsEnabledDictionary.TryGetValue(MiscExtensions.GetKey(instance, pi), out var val))
                    ReflectionHelper.SetProperty(instance, pi.Name, val);
            }
            else if (pi.PropertyType == typeof(float))
            {
                if (instance is AbstractCoolDownEffect coolDownEffect)
                {
                    if (EffectCoolDownDictionary.TryGetValue(MiscExtensions.GetKey(instance, pi), out float val))
                    {
                        ReflectionHelper.SetProperty(coolDownEffect, pi.Name, val);
                    }
                }
                else if (instance is AbstractBossAttack bossAttack)
                {
                    if (EffectCoolDownDictionary.TryGetValue(MiscExtensions.GetKey(instance, pi), out float val))
                    {
                        ReflectionHelper.SetProperty(bossAttack, pi.Name, val);
                    }
                }
            }
            AdditionalChallenge.Instance.MatchSettings();
        }
    }
}
