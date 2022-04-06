/*
namespace AdditionalChallenge.Effects.EnemyFollow;

public class SoulTwister:AbstractEnemyFollow
{
    public override string ToggleName { get; protected set; } = "Soul Twister";
    public override string ToggleDesc { get; protected set; } = "Let a soul twister follow you";
    
    protected override void CreateEnemy()
    {
        SetUpEnemy(Preloads.InstantiableObjects["soulTwister"]);
        Enemy.LocateMyFSM("Mage").SetState("Init");
    }
}*/