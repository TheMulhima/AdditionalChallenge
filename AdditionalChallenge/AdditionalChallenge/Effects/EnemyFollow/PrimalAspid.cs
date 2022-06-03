using Satchel;

namespace AdditionalChallenge.Effects.EnemyFollow;

public class PrimalAspid:AbstractEnemyFollow
{
    public override string ToggleName { get; protected set; } = "Primal Aspid";
    public override string ToggleDesc { get; protected set; } = "Let a primal aspic follow you";
    
    protected override void CreateEnemy()
    {
        SetUpEnemy(Preloads.InstantiableObjects["aspid"]);
        Enemy.GetChild("Alert Range New").GetComponent<CircleCollider2D>().radius = 20;
        Enemy.GetChild("Unalert Range").GetComponent<CircleCollider2D>().radius = 30;
    }
}