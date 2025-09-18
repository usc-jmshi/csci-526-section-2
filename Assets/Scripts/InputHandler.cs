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

    Vector2 mouseDelta = Mouse.current.delta.ReadValue();

    if (mouseDelta.sqrMagnitude > 0) {
      Cube.Instance.Drag(mouseDelta);
    }

    float scrollDelta = Mouse.current.scroll.y.ReadValue();

    if (scrollDelta > 0) {
      CameraManager.Instance.Zoom(Mouse.current.scroll.y.ReadValue());
    }
  }
}
