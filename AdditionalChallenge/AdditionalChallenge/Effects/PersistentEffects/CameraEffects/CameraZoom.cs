namespace AdditionalChallenge.Effects.PersistentEffects;

//TODO: Zoom too high
public class CameraZoom: BaseCameraEffects
{
    public override string ToggleName { get; protected set; } = "Camera Zoom";
    public override string ToggleDesc { get; protected set; } = "Increase camera zoom by 5x";
    
    protected override CameraEffects thisCameraEffect { get; set; } = CameraEffects.Zoom;
    
    protected override void EnableEffect()
    {
        GameCameras.instance.tk2dCam .ZoomFactor = 5f;
    }

    protected override void RemoveEffect()
    {
        GameCameras.instance.tk2dCam .ZoomFactor = 1f;
    }
}