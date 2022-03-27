namespace AdditionalChallenge.Effects.PersistentEffects;

public class CameraInvert: BaseCameraEffects
{
    public override string ToggleName { get; protected set; } = "Camera Invert";
    public override string ToggleDesc { get; protected set; } = "Inverts the colors of the camera";

    protected override CameraEffects thisCameraEffect { get; set; } = CameraEffects.Invert;

    private ApplyShader ivc;
    
    protected override void EnableEffect()
    {
        ivc = cam.gameObject.GetAddComponent<ApplyShader>();
         invertMat ??= new(Preloads.Shaders["Custom/InvertColor"]);;
         ivc.CurrentMaterial = invertMat;
        ivc.enabled = true;
    }

    protected override void RemoveEffect()
    {
        ivc = cam.gameObject.GetAddComponent<ApplyShader>();
        invertMat ??= new(Preloads.Shaders["Custom/InvertColor"]);;
        ivc.CurrentMaterial = invertMat;
        ivc.enabled = false;
    }
}