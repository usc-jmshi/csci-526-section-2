using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler: MonoBehaviour {

  private void Update() {
    if (Mouse.current.leftButton.wasPressedThisFrame) {
      Cube.Instance.SelectSubCube();
    } else if (Mouse.current.leftButton.wasReleasedThisFrame) {
      Cube.Instance.UnselectSubCube();
    }

    if (Mouse.current.rightButton.wasPressedThisFrame) {
      Cube.Instance.StartCubeRotation();
    } else if (Mouse.current.rightButton.wasReleasedThisFrame) {
      Cube.Instance.EndCubeRotation();
    }

    Cube.Instance.Drag(Mouse.current.delta.ReadValue());

    CameraManager.Instance.Zoom(Mouse.current.scroll.y.ReadValue());
  }
}
