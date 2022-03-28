namespace AdditionalChallenge.Effects.PersistentEffects;

[Flags]
public enum CameraEffects
{
    Flip = 1,
    Nausea = 1 << 1,
    Mirror = 1 << 2,
    Zoom = 1 << 3,
    Invert = 1 << 4,
    Pixelate = 1 << 5,
    Backwards = 1 << 6
}

/// <summary>
/// class to hold enums and effects for the 7 camera effects
/// </summary>
public abstract class BaseCameraEffects: AbstractPersistentEffect
{

    protected static CameraEffects activeEffects;
    protected static Matrix4x4 reflectMatrix = Matrix4x4.identity;
    public static Material invertMat;
    protected static UCamera cam => ReflectionHelper.GetField<tk2dCamera, UCamera>(GameCameras.instance.tk2dCam, "_unityCamera");
    protected abstract CameraEffects thisCameraEffect { get; set; }
    protected abstract void RemoveEffect();
    protected abstract void EnableEffect();
    
    internal override void StartEffect()
    {
        activeEffects |= thisCameraEffect;
        EnableEffect();
    }

    internal override void UnDoEffect()
    {
        if (activeEffects.HasValue(thisCameraEffect))
        {
            RemoveEffect();
            activeEffects &= ~thisCameraEffect;
        }
    }

    internal static void OnUpdateCameraMatrix(On.tk2dCamera.orig_UpdateCameraMatrix orig, tk2dCamera self)
    {
        orig(self);

        // Can't use ?. on a Unity type because they override == to null.
        if (GameCameras.instance == null || GameCameras.instance.tk2dCam  == null)
            return;
        
        if (cam == null)
            return;

        Matrix4x4 projectionMatrix = cam.projectionMatrix;

        if (activeEffects.HasValue(CameraEffects.Nausea))
        {
            projectionMatrix.m01 += Mathf.Sin(Time.time * 1.2f) * 1f;
            projectionMatrix.m10 += Mathf.Sin(Time.time * 1.5f) * 1f;
        }

        if (activeEffects.HasValue(CameraEffects.Flip))
        {
            reflectMatrix[1, 1] = -1;
            projectionMatrix *= reflectMatrix;
        }

        if (activeEffects.HasValue(CameraEffects.Mirror))
        {
            reflectMatrix[0, 0] = -1;
            projectionMatrix *= reflectMatrix;
        }

        if (activeEffects.HasValue(CameraEffects.Zoom))
        {
            // ReSharper disable once SuggestVarOrType_DeconstructionDeclarations
            if (HeroController.instance != null)
            {
                var heroPos = HeroController.instance.gameObject.transform.position;
                if (GameManager.instance.cameraCtrl == null)
                {
                    Modding.Logger.Log("Camera ctrl is null");
                }
                else
                {
                    GameManager.instance.cameraCtrl.SnapTo(heroPos.x, heroPos.y);
                }
            }
        }

        cam.projectionMatrix = projectionMatrix;
    }
    internal static void DisableInMainMenu(Scene From, Scene To)
    {
        if (To.name == "Main_Menu")
        {
            activeEffects = default;
        }
    }
}