namespace AdditionalChallenge.Effects.CoolDownEffects;

public class AbsRadOrbs:AbstractCoolDownEffect
{
    public override string ToggleName { get; protected set; } = "Abs Rad Orbs";
    public override string ToggleDesc { get; protected set; } = "Spawns Abs Rad orbs on you";

    public override void DoEffect()
    {
        AdditionalChallenge.CoroutineSlave.StartCoroutine(ReleaseAbsRadOrbs());
    }

    private IEnumerator ReleaseAbsRadOrbs()
    {
        if (HeroController.instance == null)
            yield break;

        GameObject orbgroup = Preloads.InstantiableObjects["AbsOrb"]; // get an go contains orb and it's effect

        GameObject orbPre = orbgroup.transform.Find("Radiant Orb").gameObject;

        GameObject ShotCharge_Pre = orbgroup.transform.Find("Shot Charge").gameObject; //get charge effect
        GameObject ShotCharge2_Pre = orbgroup.transform.Find("Shot Charge 2").gameObject;

        GameObject ShotCharge = UObject.Instantiate(ShotCharge_Pre);
        GameObject ShotCharge2 = UObject.Instantiate(ShotCharge2_Pre);

        float x = HeroController.instance.transform.position.x + URandom.Range(-7f, 8f);
        float y = HeroController.instance.transform.position.y + URandom.Range(4f, 8f);
        var spawnPoint = new Vector3(x, y);

        ShotCharge.transform.position = spawnPoint;
        ShotCharge2.transform.position = spawnPoint;

        ShotCharge.SetActive(true);
        ShotCharge2.SetActive(true);

        ParticleSystem.EmissionModule em = ShotCharge.GetComponent<ParticleSystem>().emission;
        ParticleSystem.EmissionModule em2 = ShotCharge2.GetComponent<ParticleSystem>().emission;

        em.enabled = true; // emit some effect 
        em2.enabled = true;

        yield return new WaitForSeconds(1);

        GameObject orb = orbPre.Spawn(spawnPoint); // Spawn Orb

        orb.GetComponent<Rigidbody2D>().isKinematic = false;
        orb.LocateMyFSM("Orb Control").SetState("Chase Hero");

        em.enabled = false;
        em2.enabled = false;


        UObject.Destroy(ShotCharge);
        UObject.Destroy(ShotCharge2);
    }
}
