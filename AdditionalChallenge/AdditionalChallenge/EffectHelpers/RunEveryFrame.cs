namespace AdditionalChallenge.EffectHelpers;

public class RunEveryFrame:FsmStateAction
{
    public Action MethodToRun;

    public override void OnUpdate()
    {
        MethodToRun();
    }
}