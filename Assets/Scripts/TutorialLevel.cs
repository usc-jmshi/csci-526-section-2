using System;

public class TutorialLevel: Level {
  public override int Size => 2;

  public override Start InitializeSubCubes(SubCube[,,] subCubes) {
    foreach (Side cubeSide in Enum.GetValues(typeof(Side))) {
      for (int a = 0; a < Size; a++) {
        for (int b = 0; b < Size; b++) {
          switch (cubeSide) {
            case Side.Top: {
                subCubes[0, a, b].SetSquare(cubeSide, Square.White);

                break;
              }

            case Side.Bottom: {
                subCubes[Size - 1, a, b].SetSquare(cubeSide, Square.White);

                break;
              }

            case Side.Left: {
                subCubes[a, 0, b].SetSquare(cubeSide, Square.White);

                break;
              }

            case Side.Right: {
                subCubes[a, Size - 1, b].SetSquare(cubeSide, Square.White);

                break;
              }

            case Side.Near: {
                subCubes[a, b, 0].SetSquare(cubeSide, Square.White);

                break;
              }

            case Side.Far: {
                subCubes[a, b, Size - 1].SetSquare(cubeSide, Square.White);

                break;
              }

            default: {
                throw new InvalidOperationException("Invalid side");
              }
          }
        }
      }
    }

    subCubes[0, 1, 1].SetSquare(Side.Top, Square.Yellow);
    subCubes[1, 0, 0].SetSquare(Side.Near, Square.Yellow);

    subCubes[0, 1, 1].SetSpecialSquare(Side.Top, SpecialSquare.Start);
    subCubes[1, 0, 0].SetSpecialSquare(Side.Near, SpecialSquare.End);

    return new() {
      SubCube = subCubes[0, 1, 1],
      SubCubeSide = Side.Top
    };
  }
}
