namespace AdditionalChallenge.Effects.CoolDownEffects;
public class SpawnJars:AbstractCoolDownEffect
{
    public override string ToggleName { get; protected set; } = "Spawn Jars";
    public override string ToggleDesc { get; protected set; } = "Spawns 5 collector jars from the ceiling";

    public override void DoEffect()
    {
        AdditionalChallenge.CoroutineSlave.StartCoroutine(DoSpawnJars());
    }

    private IEnumerator DoSpawnJars()
    {
        const string path = "_GameCameras/CameraParent/tk2dCamera/SceneParticlesController/town_particle_set/Particle System";

        string[] enemies = {"roller", "aspid", "buzzer"};

        AudioClip shatter_clip = AdditionalChallenge.Instance.Clips.First(x => x.name == "globe_break_larger");

        Vector3 pos = HeroController.instance.transform.position;

        GameObject break_jar = Preloads.InstantiableObjects["prefab_jar"];

        for (int i = -2; i <= 2; i++)
        {
            // Spawn the jar
            GameObject go = UObject.Instantiate
            (
                Preloads.InstantiableObjects["jar"],
                pos + new Vector3(i * 7, 10, 0),
                Quaternion.identity
            );

            go.AddComponent<CircleCollider2D>().radius = .3f;
            go.AddComponent<NonThunker>();
            go.AddComponent<Rigidbody2D>();
            go.AddComponent<DamageHero>().damageDealt = 1;
            go.AddComponent<AudioSource>();

            var ctrl = go.AddComponent<BetterSpawnJarControl>();

            var ps = GameObject.Find(path).GetComponent<ParticleSystem>();

            ctrl.Clip = shatter_clip;

            ctrl.ParticleBreak = break_jar.GetChild("Pt Glass L").GetComponent<ParticleSystem>();
            ctrl.ParticleBreakSouth = break_jar.GetChild("Pt Glass S").GetComponent<ParticleSystem>();

            ctrl.ReadyDust = ctrl.Trail = ps;
            ctrl.StrikeNailReaction = new GameObject();

            ctrl.EnemyPrefab = Preloads.InstantiableObjects[enemies[URandom.Range(0, enemies.Length)]];
            ctrl.EnemyHP = 10;

            yield return new WaitForSeconds(0.1f);

            go.SetActive(true);
        }
    }
}