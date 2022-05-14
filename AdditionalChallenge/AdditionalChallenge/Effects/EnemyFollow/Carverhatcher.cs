namespace AdditionalChallenge.Effects.EnemyFollow;

public class CarverHatcher:AbstractEnemyFollow
{
    public override string ToggleName { get; protected set; } = "Carver Hatcher";
    public override string ToggleDesc { get; protected set; } = "Let a Carver Hatcher follow you";
    
    protected override void CreateEnemy()
    {
        SetUpEnemy(Preloads.InstantiableObjects["carverHatcher"]);
        Enemy.GetChild("Attack Range").GetComponent<CircleCollider2D>().radius = 20;
        var fsm = Enemy.LocateMyFSM("Centipede Hatcher");
        fsm.GetState("Init").RemoveAction(1);
        fsm.GetState("Init").InsertMethod(1, () =>
        {
            var cage = Instantiate(Preloads.InstantiableObjects["carverHatcherSpawner"]);
            fsm.FsmVariables.FindFsmGameObject("Cage").Value = cage;
        });
        fsm.FsmVariables.FindFsmInt("Hatched Max").Value = Int32.MaxValue;
        fsm.GetState("Exhausted").AddMethod(() =>
        {
            var cage = Instantiate(Preloads.InstantiableObjects["carverHatcherSpawner"]);
            fsm.FsmVariables.FindFsmGameObject("Cage").Value = cage;
        });
        fsm.SetState("Init");
        //SFCore.Utils.FsmUtil.MakeLog(Enemy.LocateMyFSM("Centipede Hatcher"));
    }
}