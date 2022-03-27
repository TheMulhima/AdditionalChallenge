namespace AdditionalChallenge.Effects.PersistentEffects;

public class CommunismPersistent: AbstractPersistentEffect
{
    public override string ToggleName { get; protected set; } = "Communism";
    public override string ToggleDesc { get; protected set; } = "Makes all enemies hp to the average HP in the scene";

    protected override Func<bool> WhenUnDoEffectBeCalled { get; set; } = () =>
    {
        return false;
    };

    internal override void DoEffect()
    {
        HealthManager[] hms = UObject.FindObjectsOfType<HealthManager>();
        
        if (hms.Length == 0) return;
        
        float totalHealth = 0;
        hms.ToList().ForEach(hm => totalHealth += hm.hp);
        int average = (int) totalHealth / hms.Length;
        
        foreach (HealthManager hm in hms)
        {
            hm.hp = average;
        }
    }

    protected override void StartEffect()
    {
        HealthManager[] hms = UObject.FindObjectsOfType<HealthManager>();
        
        if (hms.Length == 0) return;
        
        float totalHealth = 0;
        hms.ToList().ForEach(hm => totalHealth += hm.hp);
        int average = (int) totalHealth / hms.Length;
        
        foreach (HealthManager hm in hms)
        {
            hm.hp = average;
        }
    }

    internal override void UnDoEffect()
    {
        //you dont undo it you kinda just live with it
    }
}