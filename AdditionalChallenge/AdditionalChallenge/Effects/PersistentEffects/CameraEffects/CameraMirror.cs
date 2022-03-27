namespace AdditionalChallenge.Effects.PersistentEffects;

public class CameraMirror: BaseCameraEffects
{
    public override string ToggleName { get; protected set; } = "Camera Mirror";
    public override string ToggleDesc { get; protected set; } = "Mirrors the camera";
    
    protected override CameraEffects thisCameraEffect { get; set; } = CameraEffects.Mirror;
    
    protected override void EnableEffect()
    {
    }

    protected override void RemoveEffect()
    {
    }
}