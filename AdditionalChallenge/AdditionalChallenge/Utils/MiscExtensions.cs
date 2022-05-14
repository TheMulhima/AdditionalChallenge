using System.Text;
using AdditionalChallenge.Effects.EnemyFollow;
using Logger = UnityEngine.Logger;

namespace AdditionalChallenge.Extensions;

public static class MiscExtensions
{
    public static string GetKey(AbstractEffects instance, PropertyInfo pi)
    {
        return $"{instance.ToggleName}:{pi.Name}";
    } 
    public static string GetKey(AbstractEffects instance, string PropertyName)
    {
        return $"{instance.ToggleName}:{PropertyName}";
    }
    public static bool HasValue(this CameraEffects @enum, CameraEffects toCheck)
        => (@enum & toCheck) != 0;
    
    public static GameObject GetChild(this GameObject go, string child)
    {
        return go.transform.Find(child).gameObject;
    }
    
    public static void GenerateReadmeFeature()
    {
        StringBuilder readme = new StringBuilder();
        List<AbstractEffects> AllEffects = AdditionalChallenge.ComponentHolder.GetComponents(typeof(AbstractEffects))
            .Select(effect => effect as AbstractEffects).ToList();

        readme.AppendLine("## How it works");
        readme.AppendLine($"There are {AllEffects.Count} [available effects](#available-effects) that can each be enabled separately from the modmenu. Also, cooldown effects and boss attacks have an additional setting called \"coolDown\" that dictates after how much time the effect will run again.");
        readme.AppendLine("There is also a [chaos mode](#chaos-mode) that randomly selects from enabled list and does the effects\n");
        readme.AppendLine("## Available Effects");
        readme.AppendLine("There are 4 types of effects available, persistent, cool down, boss attacks and enemy follow");
        readme.AppendLine("#### **Persistent Effects**");
        readme.AppendLine("These are effects that are completely enabled or completely disabled.");
        foreach (var effect in AllEffects.Where(effect => effect is AbstractPersistentEffect))
        {
            readme.AppendLine($"{effect!.ToggleName}: {effect.ToggleDesc}");
        }
        readme.AppendLine("#### **CoolDown Effects**");
        readme.AppendLine("These are effects that happen periodically based on the cooldown time set.");
        foreach (var effect in AllEffects.Where(effect => effect is AbstractCoolDownEffect))
        {
            readme.AppendLine($"{effect!.ToggleName}: {effect.ToggleDesc}");
        }
        readme.AppendLine("#### **Boss Attacks**");
        readme.AppendLine("These are boss attacks that will periodically based on the cooldown time set.");
        foreach (var effect in AllEffects.Where(effect => effect is AbstractBossAttack))
        {
            readme.AppendLine($"{effect!.ToggleName}: {effect.ToggleDesc}");
        }
        readme.AppendLine("#### **Enemy Follow**");
        readme.AppendLine("These are enemies that will follow you if enabled.");
        foreach (var effect in AllEffects.Where(effect => effect is AbstractEnemyFollow))
        {
            readme.AppendLine($"- {effect!.ToggleName}: {effect.ToggleDesc}");
        }
        
        Modding.Logger.Log(readme);
    }
}