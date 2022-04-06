using System.Runtime.Serialization;
using System.Text;
using AdditionalChallenge.Effects.EnemyFollow;
using Logger = InControl.Logger;

namespace AdditionalChallenge;

public class GlobalSettings
{
    [NonSerialized]
    internal readonly List<(PropertyInfo, AbstractEffects)> Properties = new();
    public Dictionary<string, bool> Booleans { get; set; } = new();
    public Dictionary<string, float> Floats { get; set; } = new();

    public bool ChaosModeEnabled = false;
    public int numEffects = 1;
    public float chaosCoolDown = 0f;
    public float delayBetweenTriggeringEffects = 0f;
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
            
            //we need to wait atleast one frame for the components to actually be gettable
            yield return null;
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
            GenerateReadmeFeature();
        }
    }

    private void SetValuesInDicts(PropertyInfo propertyInfo, AbstractEffects instance)
    {
        if (propertyInfo.PropertyType == typeof(bool))
        {
            Booleans[MiscExtensions.GetKey(instance, propertyInfo)] =
                ReflectionHelper.GetProperty<AbstractEffects, bool>(instance, propertyInfo.Name);
        }
        else if (propertyInfo.PropertyType == typeof(float))
        {
            if (instance is AbstractCoolDownEffect coolDownEffect)
            {
                Floats[MiscExtensions.GetKey(instance, propertyInfo)] =
                    ReflectionHelper.GetProperty<AbstractCoolDownEffect, float>(coolDownEffect,propertyInfo.Name);
            }
            else if (instance is AbstractBossAttack bossAttack)
            {
                Floats[MiscExtensions.GetKey(instance, propertyInfo)] =
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
                if (Booleans.TryGetValue(MiscExtensions.GetKey(instance, pi), out var val))
                    ReflectionHelper.SetProperty(instance, pi.Name, val);
            }
            else if (pi.PropertyType == typeof(float))
            {
                if (instance is AbstractCoolDownEffect coolDownEffect)
                {
                    if (Floats.TryGetValue(MiscExtensions.GetKey(instance, pi), out float val))
                    {
                        ReflectionHelper.SetProperty(coolDownEffect, pi.Name, val);
                    }
                }
                else if (instance is AbstractBossAttack bossAttack)
                {
                    if (Floats.TryGetValue(MiscExtensions.GetKey(instance, pi), out float val))
                    {
                        ReflectionHelper.SetProperty(bossAttack, pi.Name, val);
                    }
                }
            }
            AdditionalChallenge.Instance.MatchSettings();
        }
    }

    private void GenerateReadmeFeature()
    {
        StringBuilder readme = new StringBuilder();
        List<AbstractEffects> AllEffects = AdditionalChallenge.ComponentHolder.GetComponents(typeof(AbstractEffects))
            .Select(effect => effect as AbstractEffects).ToList();

        readme.AppendLine("There are 2 types of effects available, persistent effects or cool down effects");
        readme.AppendLine("**Persistent Effects**");
        readme.AppendLine("These are effects that are completely enabled or completely disabled.");
        foreach (var effect in AllEffects.Where(effect => effect is AbstractPersistentEffect))
        {
            readme.AppendLine($"{effect!.ToggleName}: {effect.ToggleDesc}");
        }
        readme.AppendLine("**CoolDown Effects**");
        readme.AppendLine("These are effects that happen periodically based on the cooldown time set.");
        foreach (var effect in AllEffects.Where(effect => effect is AbstractCoolDownEffect))
        {
            readme.AppendLine($"{effect!.ToggleName}: {effect.ToggleDesc}");
        }
        readme.AppendLine("**Boss Attacks**");
        readme.AppendLine("These are boss attacks that will periodically based on the cooldown time set.");
        foreach (var effect in AllEffects.Where(effect => effect is AbstractBossAttack))
        {
            readme.AppendLine($"{effect!.ToggleName}: {effect.ToggleDesc}");
        }
        readme.AppendLine("**Enemy Follow**");
        readme.AppendLine("These are enemies that will follow you if enabled.");
        foreach (var effect in AllEffects.Where(effect => effect is AbstractEnemyFollow))
        {
            readme.AppendLine($"{effect!.ToggleName}: {effect.ToggleDesc}");
        }
        
        Modding.Logger.Log(readme);
    }
}
