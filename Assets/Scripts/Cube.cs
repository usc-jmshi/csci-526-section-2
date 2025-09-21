using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cube: MonoBehaviour {
  private struct SubCubeSelection {
    public SubCube SubCube;
    public Side CubeSide;
  }

  private struct LayerSelection {
    public Transform Transform;
    public Axis CubeRotationAxis;
  }

  private struct StartNode {
    public SubCube SubCube;
    public Side SubCubeSide;
  }

  public static Cube Instance { get; private set; }

  public bool Initialized => _subCubes != null;

  private const float SubCubeSize = 1f;
  private const float SubCubeGap = 0.1f;
  private const float RoundLayerDuration = 0.1f;

  [SerializeField]
  private SubCube _subCubePrefab;

  private SubCube[,,] _subCubes;
  private SubCubeSelection? _selectedSubCube;
  private bool _isRotatingCube;
  private LayerSelection? _selectedLayer;
  private float _totalLayerRotation;
  private Coroutine _unselectCoroutine;
  private StartNode _start;
  private int _size;

  public void SelectSubCube() {
    if (_isRotatingCube || _selectedSubCube != null || _selectedLayer != null || _unselectCoroutine != null) {
      return;
    }

    if (GetHoveredSubCube(out SubCube subCube, out Side cubeSide)) {
      _selectedSubCube = new() {
        SubCube = subCube,
        CubeSide = cubeSide
      };
    }
  }

  public void Unselect() {
    if (_unselectCoroutine != null) {
      return;
    }

    _unselectCoroutine = StartCoroutine(UnselectCoroutine());
  }

  public void StartCubeRotation() {
    if (_selectedSubCube != null || _selectedLayer != null) {
      return;
    }

    _isRotatingCube = true;
  }

  public void EndCubeRotation() {
    _isRotatingCube = false;
  }

  public void Drag(Vector2 delta) {
    if (_selectedSubCube != null && _unselectCoroutine == null) {
      if (_selectedLayer == null) {
        SelectLayer(delta);
      } else {
        RotateLayer(delta);
      }
    }

    if (_isRotatingCube) {
      transform.Rotate(delta.y, -delta.x, 0, Space.World);
    }
  }

  public void Submit() {
    if (_selectedLayer != null || _unselectCoroutine != null) {
      return;
    }

    Side startCubeSide = _start.SubCube.SubCubeSideToCubeSide(_start.SubCubeSide);
    Square square = _start.SubCube.GetSquare(startCubeSide);
    bool[,,,] seen = new bool[_size, _size, _size, Enum.GetValues(typeof(Side)).Length];
    bool pass = false;

    Queue<SubCubeSelection> bfsQueue = new();

    bfsQueue.Enqueue(new() {
      SubCube = _start.SubCube,
      CubeSide = startCubeSide
    });

    while (bfsQueue.Count > 0) {
      SubCubeSelection node = bfsQueue.Dequeue();

      seen[node.SubCube.I, node.SubCube.J, node.SubCube.K, (int) node.CubeSide] = true;

      if (node.SubCube.GetSpecialSquare(node.CubeSide) == SpecialSquare.End) {
        pass = true;

        break;
      }

      SubCubeSelection[] neighbors = GetNeighbors(node);

      foreach (SubCubeSelection neighbor in neighbors) {
        if (seen[neighbor.SubCube.I, neighbor.SubCube.J, neighbor.SubCube.K, (int) neighbor.CubeSide]) {
          continue;
        }

        if (neighbor.SubCube.GetSquare(neighbor.CubeSide) != square) {
          continue;
        }

        bfsQueue.Enqueue(neighbor);
      }
    }

    NotificationUI.Instance.Notify(pass ? "Pass" : "Fail", pass ? Color.green : Color.red);
  }

  public void SetHoveredSubCubeSquare(Square square) {
    if (GetHoveredSubCube(out SubCube subCube, out Side cubeSide)) {
      subCube.SetSquare(cubeSide, square);
    }
  }

  public void SetHoveredSubCubeSpecialSquare(SpecialSquare specialSquare) {
    if (GetHoveredSubCube(out SubCube subCube, out Side cubeSide)) {
      subCube.SetSpecialSquare(cubeSide, specialSquare);
    }
  }

  public void WriteLevelToFile() {
    // TODO: non-editor support
    if (!Application.isEditor || string.IsNullOrEmpty(GameManager.Instance.LevelFileName)) {
      return;
    }

    Level level = new(_size);

    for (int i = 0; i < _size; i++) {
      for (int j = 0; j < _size; j++) {
        for (int k = 0; k < _size; k++) {
          if (_subCubes[i, j, k] == null) {
            continue;
          }

          foreach (Side cubeSide in Enum.GetValues(typeof(Side))) {
            Square square = _subCubes[i, j, k].GetSquare(cubeSide);
            SpecialSquare specialSquare = _subCubes[i, j, k].GetSpecialSquare(cubeSide);

            level.SetSquare(i, j, k, cubeSide, square);
            level.SetSpecialSquare(i, j, k, cubeSide, specialSquare);
          }
        }
      }
    }

    string levelJSON = JsonUtility.ToJson(level, true);

    File.WriteAllText($"{Application.dataPath}/Resources/Levels/{GameManager.Instance.LevelFileName}.json", levelJSON);

    NotificationUI.Instance.Notify("Write", Color.green);
  }

  public void ResetEditor(int size) {
    if (_selectedSubCube != null || _selectedLayer != null || _unselectCoroutine != null) {
      return;
    }

    foreach (Transform child in transform) {
      Destroy(child.gameObject);
    }

    Level level = new(size);
    level.SetSpecialSquare(0, 0, 0, Side.Near, SpecialSquare.Start);

    Initialize(level);
  }

  public void LoadLevel() {
    if (_selectedSubCube != null || _selectedLayer != null || _unselectCoroutine != null || string.IsNullOrEmpty(GameManager.Instance.LevelFileName)) {
      return;
    }

    foreach (Transform child in transform) {
      Destroy(child.gameObject);
    }

    TextAsset levelFile = Resources.Load<TextAsset>($"Levels/{GameManager.Instance.LevelFileName}");
    Level level = JsonUtility.FromJson<Level>(levelFile.text);

    Initialize(level);
  }

  private bool GetHoveredSubCube(out SubCube subCube, out Side cubeSide) {
    Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

    if (Physics.Raycast(ray, out RaycastHit hitInfo) && hitInfo.collider.TryGetComponent(out subCube)) {
      Matrix4x4 cubeToWorldInvT = new();

      Utils.InverseTranspose3DAffine(transform.localToWorldMatrix, ref cubeToWorldInvT);

      Vector3 cubeWorldXAxis = cubeToWorldInvT.GetColumn(0);
      Vector3 cubeWorldYAxis = cubeToWorldInvT.GetColumn(1);
      Vector3 cubeWorldZAxis = cubeToWorldInvT.GetColumn(2);

      float xDot = Mathf.Abs(Vector3.Dot(cubeWorldXAxis, hitInfo.normal));
      float yDot = Mathf.Abs(Vector3.Dot(cubeWorldYAxis, hitInfo.normal));
      float zDot = Mathf.Abs(Vector3.Dot(cubeWorldZAxis, hitInfo.normal));

      if (xDot > yDot && xDot > zDot) {
        cubeSide = Vector3.Dot(cubeWorldXAxis, hitInfo.normal) > 0 ? Side.Right : Side.Left;
      } else if (yDot > zDot) {
        cubeSide = Vector3.Dot(cubeWorldYAxis, hitInfo.normal) > 0 ? Side.Top : Side.Bottom;
      } else {
        cubeSide = Vector3.Dot(cubeWorldZAxis, hitInfo.normal) > 0 ? Side.Far : Side.Near;
      }

      return Utils.CheckCubeSideForSubCube(subCube, cubeSide, _size);
    }

    subCube = null;
    cubeSide = Side.Top; // Doesn't matter

    return false;
  }

  private SubCubeSelection[] GetNeighbors(SubCubeSelection node) {
    SubCubeSelection[] neighbors = new SubCubeSelection[4];

    switch (node.CubeSide) {
      case Side.Top:
      case Side.Bottom: {
          neighbors[0] = new() {
            SubCube = node.SubCube.K == _size - 1 ? node.SubCube : _subCubes[node.SubCube.I, node.SubCube.J, node.SubCube.K + 1],
            CubeSide = node.SubCube.K == _size - 1 ? Side.Far : node.CubeSide
          };
          neighbors[1] = new() {
            SubCube = node.SubCube.J == 0 ? node.SubCube : _subCubes[node.SubCube.I, node.SubCube.J - 1, node.SubCube.K],
            CubeSide = node.SubCube.J == 0 ? Side.Left : node.CubeSide
          };
          neighbors[2] = new() {
            SubCube = node.SubCube.J == _size - 1 ? node.SubCube : _subCubes[node.SubCube.I, node.SubCube.J + 1, node.SubCube.K],
            CubeSide = node.SubCube.J == _size - 1 ? Side.Right : node.CubeSide
          };
          neighbors[3] = new() {
            SubCube = node.SubCube.K == 0 ? node.SubCube : _subCubes[node.SubCube.I, node.SubCube.J, node.SubCube.K - 1],
            CubeSide = node.SubCube.K == 0 ? Side.Near : node.CubeSide
          };

          break;
        }

      case Side.Left:
      case Side.Right: {
          neighbors[0] = new() {
            SubCube = node.SubCube.K == _size - 1 ? node.SubCube : _subCubes[node.SubCube.I, node.SubCube.J, node.SubCube.K + 1],
            CubeSide = node.SubCube.K == _size - 1 ? Side.Far : node.CubeSide
          };
          neighbors[1] = new() {
            SubCube = node.SubCube.I == 0 ? node.SubCube : _subCubes[node.SubCube.I - 1, node.SubCube.J, node.SubCube.K],
            CubeSide = node.SubCube.I == 0 ? Side.Top : node.CubeSide
          };
          neighbors[2] = new() {
            SubCube = node.SubCube.I == _size - 1 ? node.SubCube : _subCubes[node.SubCube.I + 1, node.SubCube.J, node.SubCube.K],
            CubeSide = node.SubCube.I == _size - 1 ? Side.Bottom : node.CubeSide
          };
          neighbors[3] = new() {
            SubCube = node.SubCube.K == 0 ? node.SubCube : _subCubes[node.SubCube.I, node.SubCube.J, node.SubCube.K - 1],
            CubeSide = node.SubCube.K == 0 ? Side.Near : node.CubeSide
          };

          break;
        }

      case Side.Near:
      case Side.Far: {
          neighbors[0] = new() {
            SubCube = node.SubCube.I == 0 ? node.SubCube : _subCubes[node.SubCube.I - 1, node.SubCube.J, node.SubCube.K],
            CubeSide = node.SubCube.I == 0 ? Side.Top : node.CubeSide
          };
          neighbors[1] = new() {
            SubCube = node.SubCube.J == 0 ? node.SubCube : _subCubes[node.SubCube.I, node.SubCube.J - 1, node.SubCube.K],
            CubeSide = node.SubCube.J == 0 ? Side.Left : node.CubeSide
          };
          neighbors[2] = new() {
            SubCube = node.SubCube.J == _size - 1 ? node.SubCube : _subCubes[node.SubCube.I, node.SubCube.J + 1, node.SubCube.K],
            CubeSide = node.SubCube.J == _size - 1 ? Side.Right : node.CubeSide
          };
          neighbors[3] = new() {
            SubCube = node.SubCube.I == _size - 1 ? node.SubCube : _subCubes[node.SubCube.I + 1, node.SubCube.J, node.SubCube.K],
            CubeSide = node.SubCube.I == _size - 1 ? Side.Bottom : node.CubeSide
          };

          break;
        }

      default: {
          throw new InvalidOperationException("Invalid side");
        }
    }

    return neighbors;
  }

  private SubCube GetSubCube(int a, int b) {
    switch (_selectedLayer.Value.CubeRotationAxis) {
      case Axis.X: {
          return _subCubes[a, _selectedSubCube.Value.SubCube.J, b];
        }

      case Axis.Y: {
          return _subCubes[_selectedSubCube.Value.SubCube.I, a, b];
        }

      case Axis.Z: {
          return _subCubes[a, b, _selectedSubCube.Value.SubCube.K];
        }

      default: {
          throw new InvalidOperationException("Invalid axis");
        }
    }
  }

  private void SetSubCube(int a, int b, SubCube subCube) {
    switch (_selectedLayer.Value.CubeRotationAxis) {
      case Axis.X: {
          subCube.I = a;
          subCube.K = b;
          _subCubes[a, _selectedSubCube.Value.SubCube.J, b] = subCube;

          break;
        }

      case Axis.Y: {
          subCube.J = a;
          subCube.K = b;
          _subCubes[_selectedSubCube.Value.SubCube.I, a, b] = subCube;

          break;
        }

      case Axis.Z: {
          subCube.I = a;
          subCube.J = b;
          _subCubes[a, b, _selectedSubCube.Value.SubCube.K] = subCube;

          break;
        }

      default: {
          throw new InvalidOperationException("Invalid axis");
        }
    }
  }

  private void UpdateSubCubes(int numTurns, bool flippedRotationAxis) {
    SubCube[] subCubesToUpdate = new SubCube[4];
    bool shouldClockwiseUpdateSubCubes = Utils.GetShouldClockwiseUpdateSubCubes(_selectedLayer.Value.CubeRotationAxis, flippedRotationAxis);

    int layerIndex;

    switch (_selectedLayer.Value.CubeRotationAxis) {
      case Axis.X: {
          layerIndex = _selectedSubCube.Value.SubCube.J;

          break;
        }

      case Axis.Y: {
          layerIndex = _selectedSubCube.Value.SubCube.I;

          break;
        }

      case Axis.Z: {
          layerIndex = _selectedSubCube.Value.SubCube.K;

          break;
        }

      default: {
          throw new InvalidOperationException("Invalid axis");
        }
    }

    bool border = layerIndex == 0 || layerIndex == _size - 1;

    for (int c = 0; c < numTurns; c++) {
      for (int a = 0; a < (border ? _size / 2 : 1); a++) {
        for (int b = a; b < _size - 1 - a; b++) {
          subCubesToUpdate[0] = GetSubCube(a, b);
          subCubesToUpdate[1] = GetSubCube(b, _size - 1 - a);
          subCubesToUpdate[2] = GetSubCube(_size - 1 - a, _size - 1 - b);
          subCubesToUpdate[3] = GetSubCube(_size - 1 - b, a);

          if (shouldClockwiseUpdateSubCubes) {
            SetSubCube(a, b, subCubesToUpdate[3]);
            SetSubCube(b, _size - 1 - a, subCubesToUpdate[0]);
            SetSubCube(_size - 1 - a, _size - 1 - b, subCubesToUpdate[1]);
            SetSubCube(_size - 1 - b, a, subCubesToUpdate[2]);
          } else {
            SetSubCube(a, b, subCubesToUpdate[1]);
            SetSubCube(b, _size - 1 - a, subCubesToUpdate[2]);
            SetSubCube(_size - 1 - a, _size - 1 - b, subCubesToUpdate[3]);
            SetSubCube(_size - 1 - b, a, subCubesToUpdate[0]);
          }
        }
      }
    }
  }

  private IEnumerator RoundLayer() {
    int numTurns = Mathf.RoundToInt(_totalLayerRotation / 90);
    bool shouldFlipRotationAxis = Utils.GetShouldFlipRotationAxis(_selectedSubCube.Value.CubeSide, _selectedLayer.Value.CubeRotationAxis);

    Matrix4x4 cubeToWorldInvT = new();

    Utils.InverseTranspose3DAffine(transform.localToWorldMatrix, ref cubeToWorldInvT);

    Vector3 cubeWorldRotationAxis = cubeToWorldInvT.GetColumn((int) _selectedLayer.Value.CubeRotationAxis) * (shouldFlipRotationAxis ? -1 : 1);

    float timer = 0;
    float remainingRotation = numTurns * 90 - _totalLayerRotation;

    while (timer < RoundLayerDuration) {
      float deltaRotation = (numTurns * 90 - _totalLayerRotation) * Time.deltaTime / RoundLayerDuration;

      _selectedLayer.Value.Transform.Rotate(cubeWorldRotationAxis, deltaRotation, Space.World);

      timer += Time.deltaTime;
      remainingRotation -= deltaRotation;

      yield return null;
    }

    _selectedLayer.Value.Transform.Rotate(cubeWorldRotationAxis, remainingRotation, Space.World);

    UpdateSubCubes(numTurns, shouldFlipRotationAxis);

    _totalLayerRotation = 0;
  }

  private IEnumerator UnselectCoroutine() {
    if (_selectedLayer != null) {
      yield return RoundLayer();

      for (int i = _selectedLayer.Value.Transform.childCount - 1; i >= 0; i--) {
        _selectedLayer.Value.Transform.GetChild(i).parent = transform;
      }

      Destroy(_selectedLayer.Value.Transform.gameObject);

      _selectedLayer = null;
    }

    _selectedSubCube = null;

    _unselectCoroutine = null;
  }

  private void SelectLayer(Vector2 delta) {
    Axis axis1;
    Axis axis2;

    switch (_selectedSubCube.Value.CubeSide) {
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

    Utils.InverseTranspose3DAffine(Camera.main.transform.worldToLocalMatrix * transform.localToWorldMatrix, ref cubeToCameraInvT);

    Vector3 cubeCameraAxis1 = cubeToCameraInvT.GetColumn((int) axis1);
    Vector3 cubeCameraAxis2 = cubeToCameraInvT.GetColumn((int) axis2);

    // TODO: handle projective transformation?
    float dot1 = Mathf.Abs(Vector3.Dot(cubeCameraAxis1, delta));
    float dot2 = Mathf.Abs(Vector3.Dot(cubeCameraAxis2, delta));

    Axis rotationAxis = dot1 > dot2 ? axis2 : axis1;
    GameObject layer = new();

    _selectedLayer = new() {
      Transform = layer.transform,
      CubeRotationAxis = rotationAxis
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
    bool shouldFlipRotationAxis = Utils.GetShouldFlipRotationAxis(_selectedSubCube.Value.CubeSide, _selectedLayer.Value.CubeRotationAxis);
    Axis deltaAxis = Utils.GetDeltaAxis(_selectedSubCube.Value.CubeSide, _selectedLayer.Value.CubeRotationAxis);

    Matrix4x4 cubeToWorldInvT = new();
    Matrix4x4 cubeToCameraInvT = new();

    Utils.InverseTranspose3DAffine(transform.localToWorldMatrix, ref cubeToWorldInvT);
    Utils.InverseTranspose3DAffine(Camera.main.transform.worldToLocalMatrix * transform.localToWorldMatrix, ref cubeToCameraInvT);

    Vector3 cubeWorldRotationAxis = cubeToWorldInvT.GetColumn((int) _selectedLayer.Value.CubeRotationAxis) * (shouldFlipRotationAxis ? -1 : 1);
    Vector3 cubeCameraDeltaAxis = cubeToCameraInvT.GetColumn((int) deltaAxis);

    // TODO: handle projective transformation?
    float rotation = Vector3.Dot(delta, cubeCameraDeltaAxis);
    _totalLayerRotation += rotation;

    if (_totalLayerRotation > 360) {
      _totalLayerRotation -= 360;
    } else if (_totalLayerRotation < 0) {
      _totalLayerRotation += 360;
    }

    _selectedLayer.Value.Transform.Rotate(cubeWorldRotationAxis, rotation, Space.World);
  }

  private void Initialize(Level level) {
    _size = level.Size;
    _subCubes = new SubCube[_size, _size, _size];

    float bound = (_size - 1) / 2f;

    for (int i = 0; i < _size; i++) {
      for (int j = 0; j < _size; j++) {
        bool border = i == 0 || i == _size - 1 || j == 0 || j == _size - 1;

        for (int k = 0; k < _size; k++) {
          if (!border && k != 0 && k != _size - 1) {
            continue;
          }

          SubCube subCube = Instantiate(_subCubePrefab);
          subCube.transform.localScale = new(SubCubeSize, SubCubeSize, SubCubeSize);
          subCube.transform.localPosition = new(
            (-bound + j) * (SubCubeSize + SubCubeGap),
            (bound - i) * (SubCubeSize + SubCubeGap),
            (-bound + k) * (SubCubeSize + SubCubeGap)
          );
          subCube.I = i;
          subCube.J = j;
          subCube.K = k;
          subCube.transform.SetParent(transform, false);

          foreach (Side side in Enum.GetValues(typeof(Side))) {
            Square square = level.GetSquare(i, j, k, side);
            SpecialSquare specialSquare = level.GetSpecialSquare(i, j, k, side);

            subCube.SetSquare(side, square);
            subCube.SetSpecialSquare(side, specialSquare);

            if (specialSquare == SpecialSquare.Start) {
              _start = new() {
                SubCube = subCube,
                SubCubeSide = side
              };
            }
          }

          _subCubes[i, j, k] = subCube;
        }
      }
    }
  }

  private void Awake() {
    Instance = this;
  }

  private void Start() {
    if (GameManager.Instance.IsLevelEditor) {
      ResetEditor(3);
    } else {
      LoadLevel();
    }
  }
}
