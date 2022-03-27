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

    public static void InsertCustomAction(this FsmState state, int index, Action method)
    {
        state.InsertCustomAction(state.Name, method, index);
    }
    public static void AddCustomAction(this FsmState state, Action method)
    {
        state.AddCustomAction(state.Name, method);
    }
}