using UnityEngine;

public abstract class Level: MonoBehaviour {
  public struct Start {
    public SubCube SubCube;
    public Side SubCubeSide;
  }

  public abstract int Size { get; }

  public abstract Start InitializeSubCubes(SubCube[,,] subCubes);
}
