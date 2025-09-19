using UnityEngine;

public abstract class Level: MonoBehaviour {
  public abstract int Size { get; }

  public abstract void InitializeSubCubes(SubCube[,,] subCubes);
}
