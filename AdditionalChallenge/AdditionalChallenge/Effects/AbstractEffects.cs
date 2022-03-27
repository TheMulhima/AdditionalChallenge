namespace AdditionalChallenge.Effects;

/// <summary>
/// Base Base class for all effects
/// </summary>
public abstract class AbstractEffects: MonoBehaviour
{

    /// <summary>
    /// Function to be called to load a effect
    /// </summary>
    public void Load()
    {
        //everything happens in update so we only need to mess with enabled
        IsEnabled = true;
    }
    
    /// <summary>
    /// Function to be called to unload an effect
    /// </summary>
    public void Unload()
    {
        IsEnabled = false;
    }
    /// <summary>
    /// the function to run on to actually do the effect needs to be defined
    /// </summary>
    /// <returns></returns>
    internal abstract void DoEffect();
    
    /// <summary>
    /// Function to be overrided in the 2nd level of aabstract classes. Runs every frame
    /// </summary>
    public abstract void Update();
    
    /// <summary>
    /// the name of the horizontal option in modmenu for this effect 
    /// </summary>
    public abstract string ToggleName { get; protected set; }
    
    /// <summary>
    /// The description of the horizontal option in modmenu for this effect
    /// </summary>
    public abstract string ToggleDesc { get; protected set; }

    /// <summary>
    /// Function to be overrided in the 2nd level of aabstract classes. Gives the information required to change varibales from settings
    /// </summary>
    /// <returns></returns>
    public abstract List<(PropertyInfo,AbstractEffects)> GetPropertiesToAdd();

    /// <summary>
    /// Gets whether or not its enabled
    /// </summary>
    public bool IsEnabled { get; protected set; } = false;

    /// <summary>
    /// A function to set whether its enabled or not
    /// </summary>
    /// <param name="newval"></param>
    public void SetEnabled(bool newval)
    {
        if (IsEnabled == newval) return;
        if (newval) Load();
        else Unload();
    }

    protected void Log(string message) => AdditionalChallenge.Instance.Log(message);
    protected void Log(object message) => AdditionalChallenge.Instance.Log(message);
    protected void LogError(string message) => AdditionalChallenge.Instance.LogError(message);
    protected void LogDebug(string message) => AdditionalChallenge.Instance.LogDebug(message);

}