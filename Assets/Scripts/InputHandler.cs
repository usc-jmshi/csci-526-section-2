using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler: MonoBehaviour {
  private void Update() {
    if (!Cube.Instance.Initialized) {
      return;
    }

    if (Mouse.current.leftButton.wasPressedThisFrame) {
      Cube.Instance.SelectSubCube();
    } else if (Mouse.current.leftButton.wasReleasedThisFrame) {
      Cube.Instance.Unselect();
    }

    if (Mouse.current.rightButton.wasPressedThisFrame) {
      Cube.Instance.StartCubeRotation();
    } else if (Mouse.current.rightButton.wasReleasedThisFrame) {
      Cube.Instance.EndCubeRotation();
    }

    // TODO: minimum threshold?
    Vector2 mouseDelta = Mouse.current.delta.ReadValue();

    if (mouseDelta.sqrMagnitude > 0) {
      Cube.Instance.Drag(mouseDelta);
    }

    float scrollDelta = Mouse.current.scroll.y.ReadValue();

    if (Mathf.Abs(scrollDelta) > 0) {
      CameraManager.Instance.Zoom(Mouse.current.scroll.y.ReadValue());
    }

    if (GameManager.Instance.IsLevelEditor) {
      if (Keyboard.current.rKey.wasPressedThisFrame) {
        Cube.Instance.SetHoveredSubCubeSquare(Square.Red);
      } else if (Keyboard.current.gKey.wasPressedThisFrame) {
        Cube.Instance.SetHoveredSubCubeSquare(Square.Green);
      } else if (Keyboard.current.bKey.wasPressedThisFrame) {
        Cube.Instance.SetHoveredSubCubeSquare(Square.Blue);
      } else if (Keyboard.current.oKey.wasPressedThisFrame) {
        Cube.Instance.SetHoveredSubCubeSquare(Square.Orange);
      } else if (Keyboard.current.yKey.wasPressedThisFrame) {
        Cube.Instance.SetHoveredSubCubeSquare(Square.Yellow);
      } else if (Keyboard.current.wKey.wasPressedThisFrame) {
        Cube.Instance.SetHoveredSubCubeSquare(Square.White);
      } else if (Keyboard.current.sKey.wasPressedThisFrame) {
        Cube.Instance.SetHoveredSubCubeSpecialSquare(SpecialSquare.Start);
      } else if (Keyboard.current.eKey.wasPressedThisFrame) {
        Cube.Instance.SetHoveredSubCubeSpecialSquare(SpecialSquare.End);
      } else if (Keyboard.current.dKey.wasPressedThisFrame) {
        Cube.Instance.SetHoveredSubCubeSquare(Square.None);
        Cube.Instance.SetHoveredSubCubeSpecialSquare(SpecialSquare.None);
      } else if (Keyboard.current.spaceKey.wasPressedThisFrame) {
        Cube.Instance.WriteLevelToFile();
      } else if (Keyboard.current.digit2Key.wasPressedThisFrame) {
        Cube.Instance.ResetEditor(2);
      } else if (Keyboard.current.digit3Key.wasPressedThisFrame) {
        Cube.Instance.ResetEditor(3);
      } else if (Keyboard.current.digit4Key.wasPressedThisFrame) {
        Cube.Instance.ResetEditor(4);
      } else if (Keyboard.current.digit5Key.wasPressedThisFrame) {
        Cube.Instance.ResetEditor(5);
      } else if (Keyboard.current.digit6Key.wasPressedThisFrame) {
        Cube.Instance.ResetEditor(6);
      } else if (Keyboard.current.digit7Key.wasPressedThisFrame) {
        Cube.Instance.ResetEditor(7);
      } else if (Keyboard.current.digit8Key.wasPressedThisFrame) {
        Cube.Instance.ResetEditor(8);
      } else if (Keyboard.current.digit9Key.wasPressedThisFrame) {
        Cube.Instance.ResetEditor(9);
      }
    } else {
      if (Keyboard.current.spaceKey.wasPressedThisFrame) {
        Cube.Instance.Submit();
      }
    }
  }
}
