using UnityEngine;

public class CameraManager: MonoBehaviour {
  public static CameraManager Instance { get; private set; }

  private const float ZoomSpeed = 5f;
  private const float MinFOV = 50f;
  private const float MaxFOV = 100f;

  public void Zoom(float delta) {
    float currFOV = Camera.VerticalToHorizontalFieldOfView(Camera.main.fieldOfView, Camera.main.aspect);
    float newFOV = Mathf.Clamp(currFOV - ZoomSpeed * delta, MinFOV, MaxFOV);

    if (!Mathf.Approximately(currFOV, newFOV)) {
      Camera.main.fieldOfView = Camera.HorizontalToVerticalFieldOfView(newFOV, Camera.main.aspect);
    }
  }

  private void Awake() {
    Instance = this;
  }
}
