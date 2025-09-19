using UnityEngine;
using UnityEngine.Assertions;

public class SubCube: MonoBehaviour {
  public int I { get; set; }
  public int J { get; set; }
  public int K { get; set; }

  [SerializeField] private MeshRenderer[] _mrs;

  private readonly Square[] _squares = new Square[6];

  // Input is in Cube space, need to transform to local SubCube space
  public void SetSquare(Side cubeSide, Square square) {
    Side subCubeSide = CubeSideToSubCubeSide(cubeSide);

    MaterialPropertyBlock matPropBlock = new();
    Color color = Utils.GetColor(square);

    _mrs[(int) subCubeSide].GetPropertyBlock(matPropBlock);
    matPropBlock.SetColor(Utils.BaseColorShaderPropID, color);
    _mrs[(int) subCubeSide].SetPropertyBlock(matPropBlock);

    _squares[(int) subCubeSide] = square;
  }

  // Input is in Cube space, need to transform to local SubCube space
  public Square GetSquare(Side cubeSide) {
    Side subCubeSide = CubeSideToSubCubeSide(cubeSide);

    return _squares[(int) subCubeSide];
  }

  private Side CubeSideToSubCubeSide(Side cubeSide) {
    Matrix4x4 subCubeToCubeInvT = new();

    Assert.IsTrue(Utils.InverseTranspose3DAffine(Cube.Instance.transform.worldToLocalMatrix * transform.localToWorldMatrix, ref subCubeToCubeInvT));

    Vector3 subCubeCubeXAxis = subCubeToCubeInvT.GetColumn(0);
    Vector3 subCubeCubeYAxis = subCubeToCubeInvT.GetColumn(1);
    Vector3 subCubeCubeZAxis = subCubeToCubeInvT.GetColumn(2);

    Vector3 cubeSideNormal = Utils.GetLocalNormal(cubeSide);

    float xDot = Mathf.Abs(Vector3.Dot(subCubeCubeXAxis, cubeSideNormal));
    float yDot = Mathf.Abs(Vector3.Dot(subCubeCubeYAxis, cubeSideNormal));
    float zDot = Mathf.Abs(Vector3.Dot(subCubeCubeZAxis, cubeSideNormal));

    Side subCubeSide;

    if (xDot > yDot && xDot > zDot) {
      subCubeSide = Vector3.Dot(subCubeCubeXAxis, cubeSideNormal) > 0 ? Side.Right : Side.Left;
    } else if (yDot > zDot) {
      subCubeSide = Vector3.Dot(subCubeCubeYAxis, cubeSideNormal) > 0 ? Side.Top : Side.Bottom;
    } else {
      subCubeSide = Vector3.Dot(subCubeCubeZAxis, cubeSideNormal) > 0 ? Side.Far : Side.Near;
    }

    return subCubeSide;
  }
}
