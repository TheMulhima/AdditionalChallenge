namespace AdditionalChallenge.Effects.CoolDownEffects;

public class SpawnVengefly:AbstractCoolDownEffect
{
    public override string ToggleName { get; protected set; } = "Spawns an vengefly";
    public override string ToggleDesc { get; protected set; } = "Spawns an vengefly on you";

    internal override void DoEffect()
    {
        Vector2 dirH = (URandom.value < 0.5 ? Vector2.left : Vector2.right) * 5f;
        GameObject enemy = UObject.Instantiate(
            Preloads.InstantiableObjects["buzzer"], 
            HeroController.instance.gameObject.transform.position + (Vector3) dirH, 
            Quaternion.identity);
        enemy.SetActive(true);
    }
}