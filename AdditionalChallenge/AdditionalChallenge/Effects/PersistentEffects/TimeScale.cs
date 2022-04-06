using Satchel;
namespace AdditionalChallenge.Effects.PersistentEffects;
public class TimeScale:AbstractPersistentEffect
{
    private FieldInfo DebugModCurrentTimeScale;
    private bool isSet = false;
    private float? previousVal = null;
    
    public override string ToggleName { get; protected set; } = "Increase TimeScale";
    public override string ToggleDesc { get; protected set; } = "Make TimeScale to a random value between 0.5 and 3 ";

    //TODO: Add menu override for set scale
    public void Start()
    {
        this.DebugModCurrentTimeScale = typeof(DebugMod.DebugMod).GetField("CurrentTimeScale", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
    }

    internal override void RepeatedDoEffect()
    {
        if (HeroController.instance == null
            || HeroController.instance.cState.transitioning
            || GameManager.instance == null
            || GameManager.instance.IsNonGameplayScene()
            || GameManager.instance.IsGamePaused())
        {
            return;
        }

        GameManager.instance.gameObject.GetAddComponent<DebugMod.MonoBehaviours.TimeScale>();
        float num = UnityEngine.Random.Range(0.5f, 3f);
        //float num = 2;
        float newTimescaleValue = Mathf.Round(num * 10) / 10f;
        DebugModCurrentTimeScale.SetValue(null, newTimescaleValue);
        if (Time.timeScale != newTimescaleValue)
        {
            Time.timeScale = newTimescaleValue;
        }

        isSet = true;
        previousVal = newTimescaleValue;

    }

    internal override void UnDoEffect()
    {
        if (!GameManager.instance.IsGamePaused())
        {
            if (isSet && previousVal != null)
            {
                if (previousVal == (float)DebugModCurrentTimeScale.GetValue(null)
                    && Time.timeScale == previousVal)
                {
                    DebugModCurrentTimeScale.SetValue(null, 1);
                    Time.timeScale = 1;
                    isSet = false;
                }
            }
        }
    }
}