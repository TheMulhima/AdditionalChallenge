namespace AdditionalChallenge.Effects.PersistentEffects;
public class Wind: AbstractPersistentEffect
{
    public override string ToggleName { get; protected set; } = "Wind";
    public override string ToggleDesc { get; protected set; } = "Makes it so you always have to face wind";

    protected void Start()
    {
        IL.HeroController.FixedUpdate += DontStopCdash;
        ModHooks.BeforePlayerDeadHook += BeforePlayerDead;
        ModHooks.SetPlayerBoolHook += NoWindOnBench;
        ModHooks.BeforeSceneLoadHook += NoWindInTransitions;
        On.HeroController.FinishedEnteringScene += (orig, self, marker, bob) =>
        {
            orig(self, marker, bob);
            if (IsEnabled)
            {
                HeroController.instance.cState.inConveyorZone = true;
            }
        };
    }

    private string NoWindInTransitions(string arg)
    {
        HeroController.instance.cState.inConveyorZone = false;
        return arg;
    }

    private bool NoWindOnBench(string s, bool orig)
    {
        if (s == nameof(PlayerData.atBench))
        {
            if (orig)
            {
                HeroController.instance.cState.inConveyorZone = false;
            }
            else
            {
                if (IsEnabled)
                {
                    HeroController.instance.cState.inConveyorZone = true;
                }
            }
        }

        return orig;
    }

    internal override bool StartEffect()
    {
        if (HeroController.instance == null)
        {
            return false;
        }
        HeroController.instance.cState.inConveyorZone = true;
        HeroController.instance.conveyorSpeed = URandom.Range(-6f, 6f);
        return true;
    }
    
    private void BeforePlayerDead()
    {
        HeroController.instance.cState.inConveyorZone = false;
    }

    private void DontStopCdash(ILContext il)
    {
        ILCursor cursor = new ILCursor(il).Goto(0);
        
        if(cursor.TryGotoNext(MoveType.After, i => i.MatchLdstr("SLOPE CANCEL")))
        {
            cursor.EmitDelegate<Func<string, string>>((s) =>
            {
                if (IsEnabled && HeroController.instance.cState.inConveyorZone)
                {
                    return "";
                }
                else
                {
                    return s;
                }
            });
        }
    }

    internal override void UnDoEffect()
    {
        HeroController.instance.cState.inConveyorZone = false;
    }
}

//TODO: add menu to change wind direction