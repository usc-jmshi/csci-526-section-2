public class TutorialLevel: Level {
  public override int Size => 3;

  // TODO: update
  public override StartSubCube InitializeSubCubes(SubCube[,,] subCubes) {
    subCubes[0, 0, 0].SetSquare(Side.Near, Square.Red);

    subCubes[0, 0, Size - 1].SetSquare(Side.Top, Square.Blue);

    subCubes[0, Size - 1, 0].SetSquare(Side.Right, Square.Green);

    subCubes[0, 0, Size - 1].SetSquare(Side.Left, Square.Orange);

    subCubes[0, Size - 1, Size - 1].SetSquare(Side.Far, Square.White);

    subCubes[Size - 1, 0, 0].SetSquare(Side.Bottom, Square.Yellow);

    // TODO: update
    return new();
  }
}
