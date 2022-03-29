namespace AdditionalChallenge;

//this was taken from https://github.com/Sid-003/HKTwitch/blob/master/HollowTwitch/ObjectLoader.cs which is allowed by the GNU General Public License v3.0
public static class Preloads
{
    public static readonly Dictionary<(string, Func<GameObject, GameObject>), (string, string)> ObjectList = new()
    {
        {
            ("aspid", obj =>
            {
                obj.LocateMyFSM("spitter").SetState("Init");

                UObject.Destroy(obj.GetComponent<PersistentBoolItem>());

                return obj;
            }),
            ("Deepnest_East_11", "Super Spitter")
        },
        {
            ("Revek", null),
            ("RestingGrounds_08", "Ghost Battle Revek")
        },
        {
            ("pv", null),
            ("GG_Hollow_Knight", "Battle Scene/HK Prime")
        },
        {
            ("spike", obj =>
            {
                obj.AddComponent<DamageHero>().damageDealt = 1;

                return obj;
            }),
            ("Room_Colosseum_Bronze", "Colosseum Manager/Ground Spikes/Colosseum Spike")
        },
        {
            ("cave_spikes", null),
            ("Tutorial_01", "_Props/Cave Spikes")
        },
        {
            ("jar", null), ("GG_Collector", "Spawn Jar")
        },
        {
            ("roller", null), ("Crossroads_ShamanTemple", "_Enemies/Roller")
        },
        {
            ("buzzer", null), ("Crossroads_ShamanTemple", "_Enemies/Buzzer")
        },
        {
            ("prefab_jar", null), ("Ruins2_11", "Break Jar (6)")
        },
        {
            ("zap",
                go => go.LocateMyFSM("Mega Jellyfish").GetAction<SpawnObjectFromGlobalPool>("Gen", 2).gameObject.Value),
            ("GG_Uumuu", "Mega Jellyfish GG")
        },
        {
            ("Laser Turret", null), ("Mines_31", "Laser Turret")
        },
        {
            ("bee", null), ("GG_Hive_Knight", "Battle Scene/Droppers/Bee Dropper")
        },
        {
            ("nkgspike", (go =>
            {
                go.SetActive(true);

                GameObject spike = UObject.Instantiate(
                    go.GetComponentsInChildren<Transform>(true)
                        .First(x => x.name.Contains("Nightmare Spike"))
                        .gameObject
                );

                UObject.DontDestroyOnLoad(spike);

                spike.LocateMyFSM("Control").ChangeTransition("Dormant", "SPIKES READY", "Ready");

                go.SetActive(false);

                return spike;
            })),
            ("GG_Grimm_Nightmare", "Grimm Spike Holder")
        },
        {
            ("AbsOrb", abs =>
            {
                PlayMakerFSM fsm = abs.LocateMyFSM("Attack Commands");

                var spawn = fsm.GetAction<SpawnObjectFromGlobalPool>("Spawn Fireball", 1);

                GameObject orbPre = spawn.gameObject.Value;

                GameObject ShotCharge = abs.transform.Find("Shot Charge").gameObject;
                GameObject ShotCharge2 = abs.transform.Find("Shot Charge 2").gameObject;

                var orb = new GameObject("AbsOrb");
                orbPre.transform.SetParent(orb.transform);
                ShotCharge.transform.SetParent(orb.transform);
                ShotCharge2.transform.SetParent(orb.transform);

                UObject.DontDestroyOnLoad(orb);
                UObject.Destroy(abs);

                return orb;
            }),
            ("GG_Radiance", "Boss Control/Absolute Radiance")
        },
        {
            ("NKG", null), ("GG_Grimm_Nightmare", "Grimm Control/Nightmare Grimm Boss")
        },
        {
            ("markoth", null), ("GG_Ghost_Markoth_V", "Warrior/Ghost Warrior Markoth")
        },
    };
    public static Dictionary<string, GameObject> InstantiableObjects { get; } = new();
    public static Dictionary<string, Shader> Shaders { get; } = new();

    public static Sprite hwurmpU;

    public static void Load(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        static GameObject Spawnable(GameObject obj, Func<GameObject, GameObject> modify)
        {
            GameObject go = UObject.Instantiate(obj);
            go = modify?.Invoke(go) ?? go;
            UObject.DontDestroyOnLoad(go);
            go.SetActive(false);
            return go;
        }

        // ReSharper disable once SuggestVarOrType_DeconstructionDeclarations
        foreach (var ((name, modify), (room, go_name)) in ObjectList)
        {
            if (!preloadedObjects[room].TryGetValue(go_name, out GameObject go))
            {
                AdditionalChallenge.Instance.LogError($"Unable to load GameObject {go_name}");

                continue;
            }

            InstantiableObjects.Add(name, Spawnable(go, modify));
        }

        Satchel.AssemblyUtils.GetAssetBundleFromResources("Assets.shaders").LoadAllAssets<Shader>().ToList().ForEach(shader =>
        {
            Shaders.Add(shader.name, shader);
            AdditionalChallenge.Instance.Log(shader.name);
        });

        hwurmpU = Satchel.AssemblyUtils.GetSpriteFromResources("Assets.hwurmpU.png");

        OnPreloadsFinish?.Invoke();
    }

    internal static event Action OnPreloadsFinish;

}