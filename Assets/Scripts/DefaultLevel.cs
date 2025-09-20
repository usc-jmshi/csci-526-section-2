using System;

public class DefaultLevel: Level {
  public override int Size => 3;

  public override StartSubCube InitializeSubCubes(SubCube[,,] subCubes) {
    foreach (Side side in Enum.GetValues(typeof(Side))) {
      for (int a = 0; a < Size; a++) {
        for (int b = 0; b < Size; b++) {
          switch (side) {
            case Side.Top: {
                subCubes[0, a, b].SetSquare(side, Square.Red);

                break;
              }

            case Side.Bottom: {
                subCubes[Size - 1, a, b].SetSquare(side, Square.Blue);

                break;
              }

            case Side.Left: {
                subCubes[a, 0, b].SetSquare(side, Square.Green);

                break;
              }

            case Side.Right: {
                subCubes[a, Size - 1, b].SetSquare(side, Square.Yellow);

                break;
              }

            case Side.Near: {
                subCubes[a, b, 0].SetSquare(side, Square.Orange);

                break;
              }

            case Side.Far: {
                subCubes[a, b, Size - 1].SetSquare(side, Square.White);

                break;
              }

            default: {
                throw new InvalidOperationException("Invalid side");
              }
          }
        }
      }
    }

    subCubes[0, 0, 0].SetSpecialSquare(Side.Near, SpecialSquare.Start);
    subCubes[Size - 1, Size - 1, 0].SetSpecialSquare(Side.Near, SpecialSquare.End);

    return new() {
      SubCube = subCubes[0, 0, 0],
      SubCubeSide = Side.Near
    };
  }
}
