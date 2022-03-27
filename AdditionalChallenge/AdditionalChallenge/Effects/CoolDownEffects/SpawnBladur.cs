namespace AdditionalChallenge.Effects.CoolDownEffects;

public class SpawnBaldur:AbstractCoolDownEffect
{
    public override string ToggleName { get; protected set; } = "Spawns an baldur";
    public override string ToggleDesc { get; protected set; } = "Spawns an baldur on you";

    internal override void DoEffect()
    {
        Vector2 dirH = (URandom.value < 0.5 ? Vector2.left : Vector2.right) * 5f;
        GameObject enemy = UObject.Instantiate(
            Preloads.InstantiableObjects["roller"], 
            HeroController.instance.gameObject.transform.position + (Vector3) dirH, 
            Quaternion.identity);
        enemy.SetActive(true);
    }
}