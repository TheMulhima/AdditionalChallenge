namespace ACHKMP;

public class SendEffectCommand:Hkmp.Api.Command.Client.IClientCommand
{
    public string Trigger { get; } = "AC";

    public string[] Aliases { get; } = new string[]
    {
        "AC", @"\AC", "/AC",
        "ac", @"\ac", "/ac",
        "effect", @"\effect", "/effect",
        "additionalChallenge", @"\additionalChallenge", "/additionalChallenge",
        "AdditionalChallenge", @"\AdditionalChallenge", "/AdditionalChallenge",
        "additionalchallenge", @"\additionalchallenge", "/additionalchallenge",
    };
    public void Execute(string[] arguments)
    {
        /*  there are 4 posibilities:
         *  -/AC {playerName} {effectID}
         *  -/AC {playerName} {[EffectName]}
         *  -/AC {effectID}
         *  -/AC {[effectName]}
         * */
        
        //TODO: Build Command
        //TODO: Create delay 2x times the effect duration
    }
}