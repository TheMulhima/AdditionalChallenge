using Satchel;
using SFCore.Utils;

namespace AdditionalChallenge.Effects.EnemyFollow;

public class NormalAspid:AbstractEnemyFollow
{
    public override string ToggleName { get; protected set; } = "Aspid Hunter";
    public override string ToggleDesc { get; protected set; } = "Let a aspid hunter follow you";
    
    protected override void CreateEnemy()
    {
        SetUpEnemy(Preloads.InstantiableObjects["aspidHunter"]);
        Enemy.GetChild("Alert Range New").GetComponent<CircleCollider2D>().radius = 20;
        Enemy.GetChild("Unalert Range").GetComponent<CircleCollider2D>().radius = 30;
    }
}