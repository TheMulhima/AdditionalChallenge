namespace AdditionalChallenge.Effects.PersistentEffects;

//TODO: Fix doesnt work. only see black screen
/*
public class CameraBackwards : BaseCameraEffects
{
    public override string ToggleName { get; protected set; } = "Camera Backwards";
    public override string ToggleDesc { get; protected set; } = "Flip the camera around the y axis";

    protected override CameraEffects thisCameraEffect { get; set; } = CameraEffects.Backwards;

    
 // When you get hit, spell control tries to reset the camera.
 // This camera reset moves the camera super far back in z
 // and as a result you get an unusable black screen.
 // This prevents that.
 
    void PreventCameraReset(On.HutongGames.PlayMaker.Actions.SetPosition.orig_DoSetPosition orig,
        HutongGames.PlayMaker.Actions.SetPosition self)
    {
        if (self.Fsm.Name == "Spell Control" && self.Fsm.ActiveState.Name == "Reset Cam Zoom")
            return;

        orig(self);
    }

    protected override void EnableEffect()
    {
        On.HutongGames.PlayMaker.Actions.SetPosition.DoSetPosition += PreventCameraReset;
        float new_z = cam.transform.position.z + 80;
        cam.transform.SetPositionZ(new_z);
        // Rotate around the y-axis to flip the vector.
        cam.transform.Rotate(Vector3.up, 180);
    }

    protected override void RemoveEffect()
    {
        On.HutongGames.PlayMaker.Actions.SetPosition.DoSetPosition -= PreventCameraReset;
        
        float new_z = cam.transform.position.z - 80;
        cam.transform.SetPositionZ(new_z);
        
        cam.transform.Rotate(Vector3.down, 180);
    }
}*/