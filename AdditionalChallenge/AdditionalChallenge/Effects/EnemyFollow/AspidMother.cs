namespace AdditionalChallenge.Effects.EnemyFollow;

public class AspidMother:AbstractEnemyFollow
{
    public override string ToggleName { get; protected set; } = "Aspid Mother";
    public override string ToggleDesc { get; protected set; } = "Let a aspid mother follow you";
    
    protected override void CreateEnemy()
    {
        SetUpEnemy(Preloads.InstantiableObjects["aspidMother"]);
        Enemy.GetChild("Alert Range New").GetComponent<CircleCollider2D>().radius = 20;
        var fsm = Enemy.LocateMyFSM("Initiate");
        fsm.GetState("Initiate").RemoveAction(2);
        fsm.GetState("Initiate").InsertMethod(2, () =>
        {
            var cage = Instantiate(Preloads.InstantiableObjects["aspidMotherSpawner"]);
            fsm.FsmVariables.FindFsmGameObject("Cage").Value = cage;
        });
        fsm.FsmVariables.FindFsmInt("Hatched Max").Value = Int32.MaxValue;
        Enemy.LocateMyFSM("Hatcher").SetState("Initiate");
    }
}