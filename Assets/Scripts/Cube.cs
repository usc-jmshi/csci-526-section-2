using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class Cube: MonoBehaviour {
  private struct SubCubeSelection {
    public SubCube SubCube { get; set; }
    public Side Side { get; set; }
  }

  private struct Layer {
    public Transform Transform { get; set; }
    public Axis RotationAxis { get; set; }
  }

  public static Cube Instance { get; private set; }

  private const float SubCubeSize = 1f;
  private const float SubCubeGap = 0.1f;

  [Range(2, 10)]
  [SerializeField]
  private int _size = 3;
  [SerializeField]
  private SubCube _subCubePrefab;

  private SubCube[][][] _subCubes;
  private SubCubeSelection? _selectedSubCube;
  private bool _rotatingCube;
  private Layer? _selectedLayer;
  private float _totalLayerRotation;

  public void SelectSubCube() {
    if (_rotatingCube) {
      return;
    }

    Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

    if (Physics.Raycast(ray, out RaycastHit hitInfo) && hitInfo.collider.TryGetComponent(out SubCube subCube)) {
      Matrix4x4 cubeToWorldInvT = new();

      Assert.IsTrue(Utils.InverseTranspose3DAffine(transform.localToWorldMatrix, ref cubeToWorldInvT));

      Vector3 cubeWorldXAxis = cubeToWorldInvT.GetColumn(0);
      Vector3 cubeWorldYAxis = cubeToWorldInvT.GetColumn(1);
      Vector3 cubeWorldZAxis = cubeToWorldInvT.GetColumn(2);

      float xDot = Mathf.Abs(Vector3.Dot(cubeWorldXAxis, hitInfo.normal));
      float yDot = Mathf.Abs(Vector3.Dot(cubeWorldYAxis, hitInfo.normal));
      float zDot = Mathf.Abs(Vector3.Dot(cubeWorldZAxis, hitInfo.normal));

      Side side;

      if (xDot > yDot && xDot > zDot) {
        side = Vector3.Dot(cubeWorldXAxis, hitInfo.normal) > 0 ? Side.Right : Side.Left;
      } else if (yDot > zDot) {
        side = Vector3.Dot(cubeWorldYAxis, hitInfo.normal) > 0 ? Side.Top : Side.Bottom;
      } else {
        side = Vector3.Dot(cubeWorldZAxis, hitInfo.normal) > 0 ? Side.Far : Side.Near;
      }

      _selectedSubCube = new() {
        SubCube = subCube,
        Side = side
      };
    }
  }

  public void UnselectSubCube() {
    if (_selectedLayer != null) {
      RoundLayer();

      for (int i = _selectedLayer.Value.Transform.childCount - 1; i >= 0; i--) {
        _selectedLayer.Value.Transform.GetChild(i).parent = transform;
      }

      Destroy(_selectedLayer.Value.Transform.gameObject);

      _selectedLayer = null;
    }

    _selectedSubCube = null;
  }

  public void StartCubeRotation() {
    if (_selectedSubCube != null) {
      return;
    }

    _rotatingCube = true;
  }

  public void EndCubeRotation() {
    _rotatingCube = false;
  }

  public void Drag(Vector2 delta) {
    if (_selectedSubCube != null) {
      if (_selectedLayer == null) {
        SelectLayer(delta);
      } else {
        RotateLayer(delta);
      }
    }

    if (_rotatingCube) {
      transform.Rotate(delta.y, -delta.x, 0, Space.World);
    }
  }

  private SubCube GetSubCube(int a, int b) {
    switch (_selectedLayer.Value.RotationAxis) {
      case Axis.X: {
          return _subCubes[a][_selectedSubCube.Value.SubCube.J][b];
        }

      case Axis.Y: {
          return _subCubes[_selectedSubCube.Value.SubCube.I][a][b];
        }

      case Axis.Z: {
          return _subCubes[a][b][_selectedSubCube.Value.SubCube.K];
        }

      default: {
          throw new InvalidOperationException("Invalid axis");
        }
    }
  }

  private void SetSubCube(int a, int b, SubCube subCube) {
    switch (_selectedLayer.Value.RotationAxis) {
      case Axis.X: {
          subCube.I = a;
          subCube.K = b;
          _subCubes[a][_selectedSubCube.Value.SubCube.J][b] = subCube;

          break;
        }

      case Axis.Y: {
          subCube.J = a;
          subCube.K = b;
          _subCubes[_selectedSubCube.Value.SubCube.I][a][b] = subCube;

          break;
        }

      case Axis.Z: {
          subCube.I = a;
          subCube.J = b;
          _subCubes[a][b][_selectedSubCube.Value.SubCube.K] = subCube;

          break;
        }

      default: {
          throw new InvalidOperationException("Invalid axis");
        }
    }
  }

  private void UpdateSubCubes(int numTurns, bool flippedRotationAxis) {
    SubCube[] subCubesToUpdate = new SubCube[4];
    bool shouldClockwiseUpdateSubCubes = Utils.GetShouldClockwiseUpdateSubCubes(_selectedLayer.Value.RotationAxis, flippedRotationAxis);

    for (int c = 0; c < numTurns; c++) {
      for (int a = 0; a < _size / 2; a++) {
        for (int b = a; b < _size - 1 - a; b++) {
          subCubesToUpdate[0] = GetSubCube(a, b);

          if (subCubesToUpdate[0] == null) {
            return;
          }

          subCubesToUpdate[1] = GetSubCube(b, _size - 1 - a);
          subCubesToUpdate[2] = GetSubCube(_size - 1 - a, _size - 1 - a - b);
          subCubesToUpdate[3] = GetSubCube(_size - 1 - a - b, a);

          if (shouldClockwiseUpdateSubCubes) {
            SetSubCube(a, b, subCubesToUpdate[3]);
            SetSubCube(b, _size - 1 - a, subCubesToUpdate[0]);
            SetSubCube(_size - 1 - a, _size - 1 - a - b, subCubesToUpdate[1]);
            SetSubCube(_size - 1 - a - b, a, subCubesToUpdate[2]);
          } else {
            SetSubCube(a, b, subCubesToUpdate[1]);
            SetSubCube(b, _size - 1 - a, subCubesToUpdate[2]);
            SetSubCube(_size - 1 - a, _size - 1 - a - b, subCubesToUpdate[3]);
            SetSubCube(_size - 1 - a - b, a, subCubesToUpdate[0]);
          }
        }
      }
    }
  }

  private void RoundLayer() {
    int numTurns = Mathf.RoundToInt(_totalLayerRotation / 90);
    bool shouldFlipRotationAxis = Utils.GetShouldFlipRotationAxis(_selectedSubCube.Value.Side, _selectedLayer.Value.RotationAxis);
    Matrix4x4 cubeToWorldInvT = new();

    Assert.IsTrue(Utils.InverseTranspose3DAffine(transform.localToWorldMatrix, ref cubeToWorldInvT));

    Vector3 cubeWorldRotationAxis = cubeToWorldInvT.GetColumn((int) _selectedLayer.Value.RotationAxis) * (shouldFlipRotationAxis ? -1 : 1);

    // TODO: interpolate rotation over some amount of time
    _selectedLayer.Value.Transform.Rotate(cubeWorldRotationAxis, numTurns * 90 - _totalLayerRotation, Space.World);

    UpdateSubCubes(numTurns, shouldFlipRotationAxis);

    _totalLayerRotation = 0;
  }

  private void SelectLayer(Vector2 delta) {
    Axis axis1;
    Axis axis2;

    switch (_selectedSubCube.Value.Side) {
      case Side.Top:
      case Side.Bottom: {
          axis1 = Axis.X;
          axis2 = Axis.Z;

          break;
        }

      case Side.Left:
      case Side.Right: {
          axis1 = Axis.Y;
          axis2 = Axis.Z;

          break;
        }

      case Side.Near:
      case Side.Far: {
          axis1 = Axis.X;
          axis2 = Axis.Y;

          break;
        }

      default: {
          throw new InvalidOperationException("Invalid side");
        }
    }

    Matrix4x4 cubeToCameraInvT = new();

    Assert.IsTrue(Utils.InverseTranspose3DAffine(Camera.main.transform.worldToLocalMatrix * transform.localToWorldMatrix, ref cubeToCameraInvT));

    Vector3 cubeCameraAxis1 = cubeToCameraInvT.GetColumn((int) axis1);
    Vector3 cubeCameraAxis2 = cubeToCameraInvT.GetColumn((int) axis2);

    // TODO: handle projective transformation?
    float dot1 = Mathf.Abs(Vector3.Dot(cubeCameraAxis1, delta));
    float dot2 = Mathf.Abs(Vector3.Dot(cubeCameraAxis2, delta));

    Axis rotationAxis = dot1 > dot2 ? axis2 : axis1;
    GameObject layer = new();

    _selectedLayer = new() {
      Transform = layer.transform,
      RotationAxis = rotationAxis
    };

    layer.transform.parent = transform;

    for (int a = 0; a < _size; a++) {
      for (int b = 0; b < _size; b++) {
        SubCube subCube = GetSubCube(a, b);

        if (subCube == null) {
          continue;
        }

        subCube.transform.parent = layer.transform;
      }
    }
  }

  private void RotateLayer(Vector2 delta) {
    bool shouldFlipRotationAxis = Utils.GetShouldFlipRotationAxis(_selectedSubCube.Value.Side, _selectedLayer.Value.RotationAxis);
    Axis deltaAxis = Utils.GetDeltaAxis(_selectedSubCube.Value.Side, _selectedLayer.Value.RotationAxis);

    Matrix4x4 cubeToWorldInvT = new();
    Matrix4x4 cubeToCameraInvT = new();

    Assert.IsTrue(Utils.InverseTranspose3DAffine(transform.localToWorldMatrix, ref cubeToWorldInvT));
    Assert.IsTrue(Utils.InverseTranspose3DAffine(Camera.main.transform.worldToLocalMatrix * transform.localToWorldMatrix, ref cubeToCameraInvT));

    Vector3 cubeWorldRotationAxis = cubeToWorldInvT.GetColumn((int) _selectedLayer.Value.RotationAxis) * (shouldFlipRotationAxis ? -1 : 1);
    Vector3 cubeCameraDeltaAxis = cubeToCameraInvT.GetColumn((int) deltaAxis);

    // TODO: handle projective transformation?
    float rotation = Vector3.Dot(delta, cubeCameraDeltaAxis);
    _totalLayerRotation = _totalLayerRotation + rotation;

    if (_totalLayerRotation > 360) {
      _totalLayerRotation -= 360;
    } else if (_totalLayerRotation < 0) {
      _totalLayerRotation += 360;
    }

    _selectedLayer.Value.Transform.Rotate(cubeWorldRotationAxis, rotation, Space.World);
  }

  private void Initialize() {
    _subCubes = new SubCube[_size][][];
    float bound = (_size - 1) / 2f;

    for (int i = 0; i < _size; i++) {
      _subCubes[i] = new SubCube[_size][];

      for (int j = 0; j < _size; j++) {
        bool border = i == 0 || i == _size - 1 || j == 0 || j == _size - 1;
        _subCubes[i][j] = new SubCube[_size];

        for (int k = 0; k < _size; k++) {
          if (!border && k != 0 && k != _size - 1) {
            continue;
          }

          SubCube subCube = Instantiate(_subCubePrefab);
          subCube.transform.parent = transform;
          subCube.transform.localScale = new(SubCubeSize, SubCubeSize, SubCubeSize);
          subCube.transform.localPosition = new(
            (-bound + j) * (SubCubeSize + SubCubeGap),
            (bound - i) * (SubCubeSize + SubCubeGap),
            (-bound + k) * (SubCubeSize + SubCubeGap)
          );
          subCube.I = i;
          subCube.J = j;
          subCube.K = k;
          _subCubes[i][j][k] = subCube;
        }
      }
    }
  }

  private void Awake() {
    Initialize();

    Instance = this;
  }
}
