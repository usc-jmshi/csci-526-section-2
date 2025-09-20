using UnityEngine;

public abstract class Level: MonoBehaviour {
  public struct StartSubCube {
    public SubCube SubCube;
    public Side SubCubeSide;
  }

  public abstract int Size { get; }

  public abstract StartSubCube InitializeSubCubes(SubCube[,,] subCubes);
}
