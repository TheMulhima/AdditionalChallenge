using Satchel;
namespace AdditionalChallenge.Effects.PersistentEffects;

public class CameraPixelate: BaseCameraEffects
{
    public override string ToggleName { get; protected set; } = "Camera Pixelate";
    public override string ToggleDesc { get; protected set; } = "Pixelates the screen";
    
    protected override CameraEffects thisCameraEffect { get; set; } = CameraEffects.Pixelate;

    private Pixelate pix;

    protected override void EnableEffect()
    {
        pix = cam.gameObject.GetAddComponent<Pixelate>();
        pix.mainCamera ??= cam;
        pix.enabled = true;
    }

    protected override void RemoveEffect()
    {
        pix = cam.gameObject.GetAddComponent<Pixelate>();
        pix.mainCamera ??= cam;
        pix.enabled = false;
    }
}