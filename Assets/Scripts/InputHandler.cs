using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler: MonoBehaviour {

  private void Update() {
    if (Mouse.current.leftButton.wasPressedThisFrame) {
      Cube.Instance.SelectSubCube();
    } else if (Mouse.current.leftButton.wasReleasedThisFrame) {
      Cube.Instance.UnselectSubCube();
    }

    CameraManager.Instance.Zoom(Mouse.current.scroll.y.ReadValue());
  }
}
