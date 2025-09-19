using UnityEngine;
using UnityEngine.Assertions;

public class SubCube: MonoBehaviour {
  public int I { get; set; }
  public int J { get; set; }
  public int K { get; set; }

  [SerializeField] private MeshRenderer _topMR;
  [SerializeField] private MeshRenderer _bottomMR;
  [SerializeField] private MeshRenderer _leftMR;
  [SerializeField] private MeshRenderer _rightMR;
  [SerializeField] private MeshRenderer _nearMR;
  [SerializeField] private MeshRenderer _farMR;

  // Side is in Cube space
  public void SetSquare(Side side, Square square) {
    Matrix4x4 subCubeToCubeInvT = new();

    Assert.IsTrue(Utils.InverseTranspose3DAffine(Cube.Instance.transform.worldToLocalMatrix * transform.localToWorldMatrix, ref subCubeToCubeInvT));

    Vector3 subCubeCubeXAxis = subCubeToCubeInvT.GetColumn(0);
    Vector3 subCubeCubeYAxis = subCubeToCubeInvT.GetColumn(1);
    Vector3 subCubeCubeZAxis = subCubeToCubeInvT.GetColumn(2);

    Vector3 cubeSideNormal = Utils.GetLocalNormal(side);

    float xDot = Mathf.Abs(Vector3.Dot(subCubeCubeXAxis, cubeSideNormal));
    float yDot = Mathf.Abs(Vector3.Dot(subCubeCubeYAxis, cubeSideNormal));
    float zDot = Mathf.Abs(Vector3.Dot(subCubeCubeZAxis, cubeSideNormal));

    MeshRenderer mr;

    if (xDot > yDot && xDot > zDot) {
      mr = Vector3.Dot(subCubeCubeXAxis, cubeSideNormal) > 0 ? _rightMR : _leftMR;
    } else if (yDot > zDot) {
      mr = Vector3.Dot(subCubeCubeYAxis, cubeSideNormal) > 0 ? _topMR : _bottomMR;
    } else {
      mr = Vector3.Dot(subCubeCubeZAxis, cubeSideNormal) > 0 ? _farMR : _nearMR;
    }

    MaterialPropertyBlock matPropBlock = new();
    Color color = Utils.GetColor(square);

    mr.GetPropertyBlock(matPropBlock);
    matPropBlock.SetColor(Utils.BaseColorShaderPropID, color);
    mr.SetPropertyBlock(matPropBlock);
  }
}
