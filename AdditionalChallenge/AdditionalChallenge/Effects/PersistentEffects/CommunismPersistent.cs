namespace AdditionalChallenge.Effects.PersistentEffects;

public class CommunismPersistent: AbstractPersistentEffect
{
    public override string ToggleName { get; protected set; } = "Communism";
    public override string ToggleDesc { get; protected set; } = "Makes all enemies hp to the average HP in the scene";

    internal override void RepeatedDoEffect()
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

    internal override void StartEffect()
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
}