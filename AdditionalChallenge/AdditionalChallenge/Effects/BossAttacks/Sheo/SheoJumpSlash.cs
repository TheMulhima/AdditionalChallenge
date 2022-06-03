namespace AdditionalChallenge.Effects.BossAttacks;
public class SheoJumpSlash:AbstractSheoAttack
{
    public override string ToggleName { get; protected set; } = "Sheo Jump Slash";

    public override string ToggleDesc { get; protected set; } = "Let Sheo come and jump slash at you (red)";

    protected override string StartState { get; } = "JumpSlash1";

    protected override void AdditionalFSMChangesInit()
    {
        nailmaster_sheo.GetState("Dstab").AddAction(new RunEveryFrame()
        {
            MethodToRun = () =>
            {
                if (Sheo.transform.position.y < HeroController.instance.transform.position.y + 2f)
                {
                    nailmaster_sheo.SetState("Dstab Land");
                    nailmaster_sheo.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                }
            }
        });
        
        
        nailmaster_sheo.GetState("Dstab").GetAction<SetVelocity2d>().y = -60f;
        
    }

    protected override void AdditionalFSMChangesAttack()
    {
        nailmaster_sheo.GetState("JumpSlash1").GetAction<SetPosition>().y = HeroController.instance.transform.position.y + 10;
        nailmaster_sheo.FsmVariables.FindFsmFloat("Topslash Y").Value =
            HeroController.instance.transform.position.y + 10;
        
        
    }

    protected override void SetPos()
    {
        Log("Setting pos");
        var pos = HeroController.instance.transform.position;
        Sheo.transform.position = new Vector3(pos.x, pos.y + 10, Sheo.transform.position.z);
        Sheo.GetComponent<Rigidbody2D>().gravityScale = 0;
    }
}