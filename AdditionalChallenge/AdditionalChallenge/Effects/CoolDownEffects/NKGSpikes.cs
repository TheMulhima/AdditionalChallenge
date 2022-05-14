namespace AdditionalChallenge.Effects.CoolDownEffects;
public class NKGSpikes:AbstractCoolDownEffect
{
    public override string ToggleName { get; protected set; } = "NKG spikes";
    public override string ToggleDesc { get; protected set; } = "Spawns NKG spikes where you stand";

    public override void DoEffect()
    {
        AdditionalChallenge.CoroutineSlave.StartCoroutine(ReleaseNKGSpikes());
    }

    private IEnumerator ReleaseNKGSpikes()
    {
        Vector3 hero_pos = HeroController.instance.transform.position;

            var audio_player = new GameObject().AddComponent<AudioSource>();

            audio_player.volume = GameManager.instance.GetImplicitCinematicVolume();

            var spike_fsms = new List<PlayMakerFSM>();

            const float SPACING = 2.5f;
 
            for (int i = -8; i <= 8; i++)
            {
                GameObject spike = UObject.Instantiate(Preloads.InstantiableObjects["nkgspike"]);

                spike.SetActive(true);

                Vector3 pos = hero_pos + new Vector3(i * SPACING, 0);
                
                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, 500, 1 << 8);
                
                pos.y -= hit ? hit.distance : 0;
                
                spike.transform.position = pos;

                PlayMakerFSM ctrl = spike.LocateMyFSM("Control");
                
                spike_fsms.Add(ctrl);
                
                ctrl.SendEvent("SPIKES READY");
            }
            
            audio_player.PlayOneShot(AdditionalChallenge.Instance.Clips.FirstOrDefault(x => x.name == "grimm_spikes_pt_1_grounded"));
            
            yield return new WaitForSeconds(0.55f);
            
            foreach (PlayMakerFSM spike in spike_fsms)
            {
                spike.SendEvent("SPIKES UP");
            }
            
            yield return new WaitForSeconds(0.15f);
            
            GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
            
            audio_player.PlayOneShot(AdditionalChallenge.Instance.Clips.FirstOrDefault(x => x.name == "grimm_spikes_pt_2_shoot_up"));
            
            yield return new WaitForSeconds(0.45f);
            
            foreach (PlayMakerFSM spike in spike_fsms)
            {
                spike.SendEvent("SPIKES DOWN");
            }
            
            audio_player.PlayOneShot(AdditionalChallenge.Instance.Clips.FirstOrDefault(x => x.name == "grimm_spikes_pt_3_shrivel_back"));
            
            yield return new WaitForSeconds(0.5f);

            foreach (GameObject go in spike_fsms.Select(x => x.gameObject))
            {
                UObject.Destroy(go);
            }
    }
}