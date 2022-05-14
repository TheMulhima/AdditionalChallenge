namespace ACHKMP;

public class GlobalSettings
{
    [JsonConverter(typeof(PlayerActionSetConverter))]
    public KeyBinds Keybinds = new KeyBinds();
    
    public List<string> EffectNames = new List<string>();
    public List<string> TargetPlayers = new List<string>();
}


public class KeyBinds : PlayerActionSet
{
    public List<PlayerAction> EffectsKeys;
    public KeyBinds()
    {
        EffectsKeys = new List<PlayerAction>(AdditionalChallenge.AdditionalChallenge.StoreableEffects);
        for (int i = 0; i < AdditionalChallenge.AdditionalChallenge.StoreableEffects; i++)
        {
            EffectsKeys.Add(CreatePlayerAction($"Effect {i}"));
        }
    }

    public PlayerAction GetKey(int effectId)
    {
        return EffectsKeys[effectId];
    }
    public bool IsKeyPressed(int effectId)
    {
        return EffectsKeys[effectId].IsPressed;
    }

    public void RunIfKeyPressed(Action<int> callback)
    {
        for (int i = 0; i < EffectsKeys.Count; i++)
        {
            if (EffectsKeys[i].IsPressed)
            {
                callback(i);
            }
        }
    }
}