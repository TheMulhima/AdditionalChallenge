namespace AdditionalChallenge.Effects.CoolDownEffects;

public class LightningAttack:AbstractCoolDownEffect
{
    public override string ToggleName { get; protected set; } = "LightningAttack";
    public override string ToggleDesc { get; protected set; } = "Get followed by Uumuu's zap attack";

    public override void DoEffect()
    {
        AdditionalChallenge.CoroutineSlave.StartCoroutine(DoZapAttack());
    }

    private IEnumerator DoZapAttack()
    {
        GameObject prefab = Preloads.InstantiableObjects["zap"];
            
        for (int i = 0; i < 12; i++)
        {
            GameObject zap = UObject.Instantiate(prefab, HeroController.instance.transform.position, Quaternion.identity);
                
            zap.SetActive(true);
                
            yield return  new WaitForSeconds(0.5f);
        }
    }
}