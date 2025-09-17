using UnityEngine;
using UnityEngine.InputSystem;

public class Cube: MonoBehaviour {
  public static Cube Instance { get; private set; }

  private const float SubCubeSize = 1f;
  private const float SubCubeGap = 0.1f;

  [Range(2, 10)]
  [SerializeField]
  private int _size = 3;
  [SerializeField]
  private SubCube _subCubePrefab;

  private SubCube[][][] _subCubes;
  private SubCube _selectedSubCube;
  private bool _rotatingCube;

  public void SelectSubCube() {
    if (_rotatingCube) {
      return;
    }

    Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
    RaycastHit hitInfo;

    if (Physics.Raycast(ray, out hitInfo)) {
      _selectedSubCube = hitInfo.collider.GetComponent<SubCube>();
    }
  }

  public void UnselectSubCube() {
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
    if (_rotatingCube) {
      transform.Rotate(delta.y, -delta.x, 0, Space.World);
    }
  }

  private void Initialize() {
    _subCubes = new SubCube[_size][][];
    float bound = (_size - 1) / 2f;

    for (int i = 0; i < _size; i++) {
      _subCubes[i] = new SubCube[_size][];

      for (int j = 0; j < _size; j++) {
        bool border = i == 0 || i == _size - 1 || j == 0 || j == _size - 1;
        _subCubes[i][j] = new SubCube[border ? _size : 2];

        for (int k = 0; k < _subCubes[i][j].Length; k++) {
          SubCube subCube = Instantiate(_subCubePrefab);
          subCube.transform.parent = transform;
          subCube.transform.localScale = new Vector3(SubCubeSize, SubCubeSize, SubCubeSize);
          subCube.transform.localPosition = new Vector3(
            (-bound + j) * (SubCubeSize + SubCubeGap),
            (bound - i) * (SubCubeSize + SubCubeGap),
            (-bound + k * (border ? 1 : _size - 1)) * (SubCubeSize + SubCubeGap)
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
