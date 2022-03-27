namespace AdditionalChallenge.Effects.PersistentEffects;

public class CameraNausea: BaseCameraEffects
{
    public override string ToggleName { get; protected set; } = "Camera Nausea";
    public override string ToggleDesc { get; protected set; } = "Creases a nausea effect with the camera";
    
    protected override CameraEffects thisCameraEffect { get; set; } = CameraEffects.Nausea;
    
    protected override void EnableEffect()
    {
    }

    protected override void RemoveEffect()
    {
    }
}