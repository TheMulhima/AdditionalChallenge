namespace AdditionalChallenge.Extensions;

public static class FSMExtensions
{
    public static void ClearState(this FsmState state)
    {
        state.Actions = Array.Empty<FsmStateAction>();
        state.Transitions = Array.Empty<FsmTransition>();
    }
    public static void ClearTransitions(this FsmState state)
    {
        state.Transitions = Array.Empty<FsmTransition>();
    }
    public static void ClearActions(this FsmState state)
    {
        state.Actions = Array.Empty<FsmStateAction>();
    }

    public const string FINISHED = nameof(FINISHED);
}