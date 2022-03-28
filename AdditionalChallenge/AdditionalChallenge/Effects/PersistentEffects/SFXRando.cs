using MonoMod.RuntimeDetour;

namespace AdditionalChallenge.Effects.PersistentEffects;

public class SFXRando: AbstractPersistentEffect
{
    public override string ToggleName { get; protected set; } = "SFX Rando";
    public override string ToggleDesc { get; protected set; } = "randomizes SFX";

    private Hook PlayOneShotHook; 
    private Hook PlayHook;
    
    internal override void StartEffect()
    {
        if (PlayHook == null)
        {
            PlayHook = new Hook
            (
                typeof(AudioSource).GetMethod("Play", Type.EmptyTypes),
                new Action<Action<AudioSource>, AudioSource>(Play)
            );
        }
        if (PlayOneShotHook == null)
        {
            PlayOneShotHook = new Hook
            (
                typeof(AudioSource).GetMethod("PlayOneShot", new[] {typeof(AudioClip), typeof(float)}),
                new Action<Action<AudioSource, AudioClip, float>, AudioSource, AudioClip, float>(PlayOneShot)
            );
        }
    }
    
    private static void PlayOneShot(Action<AudioSource, AudioClip, float> orig, AudioSource self, AudioClip clip, float volumeScale)
    {
        orig(self, AdditionalChallenge.Instance.Clips[URandom.Range(0, AdditionalChallenge.Instance.Clips.Count - 1)], volumeScale);
    }

    private static void Play(Action<AudioSource> orig, AudioSource self)
    {
        AudioClip orig_clip = self.clip;

        self.clip = AdditionalChallenge.Instance.Clips[URandom.Range(0, AdditionalChallenge.Instance.Clips.Count - 1)];

        orig(self);

        self.clip = orig_clip;
    }

    internal override void UnDoEffect()
    {
        PlayHook?.Dispose();
        PlayOneShotHook?.Dispose();
        PlayHook = null;
        PlayOneShotHook = null;
    }
}