using UnityEngine;

public class SubCube: MonoBehaviour {
  public int I { get; set; }
  public int J { get; set; }
  public int K { get; set; }

  [SerializeField] private MeshRenderer[] _mrs;
  [SerializeField] private Texture2D[] _textures; // TODO: maybe use world space UI

  private readonly Square[] _squares = new Square[6];
  private readonly SpecialSquare[] _specialSquares = new SpecialSquare[6];

  public void SetSquare(Side cubeSide, Square square) {
    Side subCubeSide = CubeSideToSubCubeSide(cubeSide);

    MaterialPropertyBlock matPropBlock = new();
    Color color = Utils.GetColor(square);

    _mrs[(int) subCubeSide].GetPropertyBlock(matPropBlock);
    matPropBlock.SetColor(Utils.BaseColorShaderPropID, color);
    _mrs[(int) subCubeSide].SetPropertyBlock(matPropBlock);

    _squares[(int) subCubeSide] = square;
  }

  public void SetSpecialSquare(Side cubeSide, SpecialSquare specialSquare) {
    Side subCubeSide = CubeSideToSubCubeSide(cubeSide);

    MaterialPropertyBlock matPropBlock = new();
    Texture2D texture = _textures[(int) specialSquare];

    _mrs[(int) subCubeSide].GetPropertyBlock(matPropBlock);
    matPropBlock.SetTexture(Utils.BaseMapShaderPropID, texture);
    _mrs[(int) subCubeSide].SetPropertyBlock(matPropBlock);

    _specialSquares[(int) subCubeSide] = specialSquare;
  }

  public Square GetSquare(Side cubeSide) {
    Side subCubeSide = CubeSideToSubCubeSide(cubeSide);

    return _squares[(int) subCubeSide];
  }

  public SpecialSquare GetSpecialSquare(Side cubeSide) {
    Side subCubeSide = CubeSideToSubCubeSide(cubeSide);

    return _specialSquares[(int) subCubeSide];
  }

  public Side SubCubeSideToCubeSide(Side subCubeSide) {
    Matrix4x4 cubeToSubCubeInvT = new();

    Utils.InverseTranspose3DAffine(transform.worldToLocalMatrix * Cube.Instance.transform.localToWorldMatrix, ref cubeToSubCubeInvT);

    Vector3 cubeSubCubeXAxis = cubeToSubCubeInvT.GetColumn(0);
    Vector3 cubeSubCubeYAxis = cubeToSubCubeInvT.GetColumn(1);
    Vector3 cubeSubCubeZAxis = cubeToSubCubeInvT.GetColumn(2);

    Vector3 subCubeSideNormal = Utils.GetLocalNormal(subCubeSide);

    float xDot = Mathf.Abs(Vector3.Dot(cubeSubCubeXAxis, subCubeSideNormal));
    float yDot = Mathf.Abs(Vector3.Dot(cubeSubCubeYAxis, subCubeSideNormal));
    float zDot = Mathf.Abs(Vector3.Dot(cubeSubCubeZAxis, subCubeSideNormal));

    Side cubeSide;

    if (xDot > yDot && xDot > zDot) {
      cubeSide = Vector3.Dot(cubeSubCubeXAxis, subCubeSideNormal) > 0 ? Side.Right : Side.Left;
    } else if (yDot > zDot) {
      cubeSide = Vector3.Dot(cubeSubCubeYAxis, subCubeSideNormal) > 0 ? Side.Top : Side.Bottom;
    } else {
      cubeSide = Vector3.Dot(cubeSubCubeZAxis, subCubeSideNormal) > 0 ? Side.Far : Side.Near;
    }

    return cubeSide;
  }

  private Side CubeSideToSubCubeSide(Side cubeSide) {
    Matrix4x4 subCubeToCubeInvT = new();

    Utils.InverseTranspose3DAffine(Cube.Instance.transform.worldToLocalMatrix * transform.localToWorldMatrix, ref subCubeToCubeInvT);

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
