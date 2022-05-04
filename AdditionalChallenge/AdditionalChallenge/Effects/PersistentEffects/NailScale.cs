namespace AdditionalChallenge.Effects.PersistentEffects;

public class NailScale: AbstractPersistentEffect
{
    public override string ToggleName { get; protected set; } = "Nail Scale";
    public override string ToggleDesc { get; protected set; } = "Change your nail length";

    private float nailScale;

    internal override bool StartEffect()
    {
        nailScale = GetScale();
        On.NailSlash.StartSlash += ChangeNailScale;
        return true;
    }

    private float GetScale()
    {
        if (AdditionalChallenge.settings.nailScale == Mathf.Infinity)
        {
            return URandom.Range(0.3f, 5f);
        }
        else
        {
            return AdditionalChallenge.settings.nailScale;
        }
    }
    
    void ChangeNailScale(On.NailSlash.orig_StartSlash orig, NailSlash self)
    {
        orig(self);
                
        self.transform.localScale *= nailScale;
    }

    internal override void UnDoEffect()
    {
        On.NailSlash.StartSlash -= ChangeNailScale;
    }
    
    public override void AddElementsToModMenu(Menu MenuRef)
    {
        MenuRef.AddElement(new HorizontalOption(ToggleName, ToggleDesc,
            new [] { "Enabled", "Disabled" },
            (i) =>
            {
                AdditionalChallenge.settings.Booleans[Key] = i == 0;
                AdditionalChallenge.Instance.MatchSettings();
                
                MenuRef.Find($"NailScaleOption").isVisible = i == 0;
                MenuRef.Update();
            },
            () => AdditionalChallenge.settings.Booleans.ContainsKey(Key)
                ? AdditionalChallenge.settings.Booleans[Key] ? 0 : 1 
                : 1));
        
        float start = 0.3f, stop = 5f, step = 0.05f;
        string[] options = new[] { "Random" };
        options = options.Concat(Enumerable.Range((int)(start / step), (int)(Math.Abs(stop - start) / step) + 1)
            .Select(x => (x * step).ToString()).ToArray()).ToArray();
        MenuRef.AddElement(new HorizontalOption("Nail Scale", "Choose Nail Scale",
            options,
            i =>
            {
                if (i == 0)
                {
                    AdditionalChallenge.settings.nailScale = Mathf.Infinity;
                }
                else
                {
                    i--;
                    AdditionalChallenge.settings.nailScale = ((i + (start / step)) * step);
                }

                nailScale = GetScale();
                
            },
            () =>
            {
                if (AdditionalChallenge.settings.nailScale == Mathf.Infinity)
                {
                    return 0;
                }
                else
                {
                    return (int)((AdditionalChallenge.settings.nailScale / step) - (start / step)) + 1;
                }
            },
            Id: $"NailScaleOption")
        {
            isVisible =
                AdditionalChallenge.settings.Booleans.ContainsKey(isEnabledKey) ?
                    AdditionalChallenge.settings.Booleans[isEnabledKey] :
                    false
        });
    }
    
    private string isEnabledKey => MiscExtensions.GetKey(this, nameof(IsEnabled));
}