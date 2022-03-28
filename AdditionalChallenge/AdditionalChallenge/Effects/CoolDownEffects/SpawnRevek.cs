namespace AdditionalChallenge.Effects.CoolDownEffects;

public class SpawnRevek:AbstractCoolDownEffect
{
    public override string ToggleName { get; protected set; } = "Spawn a revek";
    public override string ToggleDesc { get; protected set; } = "Spawns a revek that goes away after 30 seconds or 1 parry";

    internal override void DoEffect()
    {
        AdditionalChallenge.CoroutineSlave.StartCoroutine(DoSpawnRevek());
    }

    private IEnumerator DoSpawnRevek()
    {
        GameObject revek = UObject.Instantiate
        (
            Preloads.InstantiableObjects["Revek"],
            HeroController.instance.gameObject.transform.position,
            Quaternion.identity
        );

        yield return new WaitForSecondsRealtime(1);

        UObject.DontDestroyOnLoad(revek);

        revek.SetActive(true);

        PlayMakerFSM ctrl = revek.LocateMyFSM("Control");

        // Make sure init gets to run.
        yield return null;

        // Actually spawn.
        ctrl.SetState("Appear Pause");

        // ReSharper disable once ImplicitlyCapturedClosure (ctrl)
        ctrl.GetState("Hit").AddMethod(() => UObject.Destroy(revek));

        // ReSharper disable once ImplicitlyCapturedClosure (ctrl)
        void OnUnload()
        {
            if (revek == null)
                return;

            revek.SetActive(false);
        }

        void OnLoad(Scene a, Scene b)
        {
            try
            {
                if (revek == null)
                    return;

                revek.SetActive(true);

                ctrl.SetState("Appear Pause");
            }
            catch
            {
                UObject.Destroy(revek);
            }
        }

        GameManager.instance.UnloadingLevel += OnUnload;
        USceneManager.activeSceneChanged += OnLoad;

        yield return new WaitForSecondsRealtime(30);

        UObject.Destroy(revek);

        GameManager.instance.UnloadingLevel -= OnUnload;
        USceneManager.activeSceneChanged -= OnLoad;
    }
}