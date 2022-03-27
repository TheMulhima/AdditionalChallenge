namespace AdditionalChallenge.Effects.PersistentEffects;

public class Darkness: AbstractPersistentEffect
{
    public override string ToggleName { get; protected set; } = "Darkness";
    public override string ToggleDesc { get; protected set; } = "Makes everything dark";

    protected override Func<bool> WhenUnDoEffectBeCalled { get; set; } = () =>
    {
        if (HeroController.instance == null)
        {
            return true;
        }
        return false;
    };

    protected override void StartEffect()
    {
        DarknessHelper.Darken();
        On.GameManager.EnterHero += OnSceneLoad;
    }

    void OnSceneLoad(On.GameManager.orig_EnterHero orig, GameManager self, bool additiveGateSearch)
    {
        orig(self, additiveGateSearch);
                
        DarknessHelper.Darken();
    }

    internal override void UnDoEffect()
    {
        On.GameManager.EnterHero -= OnSceneLoad;
        DarknessHelper.Lighten();
    }
}

// Stolen from Darkness mod by Katie and 56 again https://github.com/fifty-six/HollowKnight.Darkenss
    public static class DarknessHelper
    {
        private static readonly Dictionary<(string Name, string EventName), string> _originalTransitions = new();

        public static void Lighten()
        {
            if (HeroController.instance == null) return;

            foreach (FsmState state in HeroController.instance.vignetteFSM.FsmStates)
            {
                foreach (FsmTransition trans in state.Transitions)
                {
                    if (!_originalTransitions.TryGetValue((state.Name, trans.EventName), out string orig)) continue;

                    trans.ToState = orig;
                }
            }

            HeroController.instance.vignetteFSM.SetState("Normal");

            HeroController.instance.vignette.enabled = false;
        }

        public static void Darken()
        {
            if (HeroController.instance == null) return;

            foreach (FsmState state in HeroController.instance.vignetteFSM.FsmStates)
            {
                foreach (FsmTransition trans in state.Transitions)
                {
                    switch (trans.ToState)
                    {
                        case "Dark -1":
                        case "Normal":
                        case "Dark 1":
                        case "Lantern":

                            _originalTransitions[(state.Name, trans.EventName)] = trans.ToState;

                            trans.ToState = "Dark 2";
                            break;
                        case "Dark -1 2":
                        case "Normal 2":
                        case "Dark 1 2":
                        case "Lantern 2":

                            _originalTransitions[(state.Name, trans.EventName)] = trans.ToState;

                            trans.ToState = "Dark 2 2";
                            break;
                    }
                }
            }

            HeroController.instance.vignetteFSM.SetState("Dark 2");
            HeroController.instance.vignette.enabled = true;
        }
    }