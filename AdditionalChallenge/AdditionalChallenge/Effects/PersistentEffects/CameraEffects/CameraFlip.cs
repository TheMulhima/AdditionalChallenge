namespace AdditionalChallenge.Effects.PersistentEffects;

public class CameraFlip: BaseCameraEffects
{
    public override string ToggleName { get; protected set; } = "Camera Flip";
    public override string ToggleDesc { get; protected set; } = "Flip the camera";
    
    protected override CameraEffects thisCameraEffect { get; set; } = CameraEffects.Flip;

    protected override void EnableEffect()
    {
    }

    protected override void RemoveEffect()
    {
    }
    
    
}