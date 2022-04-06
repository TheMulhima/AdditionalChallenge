using JetBrains.Annotations;
using MonoMod.RuntimeDetour;

namespace AdditionalChallenge.Effects.PersistentEffects;

public class Bindings: AbstractPersistentEffect
{
    public override string ToggleName { get; protected set; } = "Bindings";
    public override string ToggleDesc { get; protected set; } = "Enables all pantheon bindings";

    internal override bool StartEffect()
    {
        BindingsHelper.AddDetours();

        On.BossSceneController.RestoreBindings += BindingsHelper.NoOp;
        On.GGCheckBoundSoul.OnEnter += BindingsHelper.CheckBoundSoulEnter;

        BindingsHelper.ShowIcons();

        return true;
    }

    internal override void UnDoEffect()
    {
        BindingsHelper.Unload();
    }
}