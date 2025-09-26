using System;
using UnityEngine;

public static class Utils {
  public static readonly int BaseColorShaderPropID = Shader.PropertyToID("_BaseColor");
  public static readonly int BaseMapShaderPropID = Shader.PropertyToID("_BaseMap");
  public static readonly int EmissionColorShaderPropID = Shader.PropertyToID("_EmissionColor");
  public static readonly int EmissionMapShaderPropID = Shader.PropertyToID("_EmissionMap");

  public static bool InverseTranspose3DAffine(Matrix4x4 input, ref Matrix4x4 result) {
    Matrix4x4 inverse = new();

    if (!Matrix4x4.Inverse3DAffine(input, ref inverse)) {
      return false;
    }

    result = inverse.transpose;

    return true;
  }

  public static Axis GetDeltaAxis(Side side, Axis rotationAxis) {
    switch (side) {
      case Side.Top:
      case Side.Bottom: {
          return rotationAxis == Axis.X ? Axis.Z : Axis.X;
        }

      case Side.Left:
      case Side.Right: {
          return rotationAxis == Axis.Y ? Axis.Z : Axis.Y;
        }

      case Side.Near:
      case Side.Far: {
          return rotationAxis == Axis.X ? Axis.Y : Axis.X;
        }

      default: {
          throw new InvalidOperationException("Invalid side");
        }
    }
  }

  public static bool GetShouldFlipRotationAxis(Side side, Axis rotationAxis) {
    switch (side) {
      case Side.Top: {
          return rotationAxis == Axis.Z;
        }

      case Side.Bottom: {
          return rotationAxis == Axis.X;
        }

      case Side.Left: {
          return rotationAxis == Axis.Z;
        }

      case Side.Right: {
          return rotationAxis == Axis.Y;
        }

      case Side.Near: {
          return rotationAxis == Axis.Y;
        }

      case Side.Far: {
          return rotationAxis == Axis.X;
        }

      default: {
          throw new InvalidOperationException("Invalid side");
        }
    }
  }

  public static bool GetShouldClockwiseUpdateSubCubes(Axis rotationAxis, bool flippedRotationAxis) {
    switch (rotationAxis) {
      case Axis.X: {
          return !flippedRotationAxis;
        }

      case Axis.Y: {
          return !flippedRotationAxis;
        }

      case Axis.Z: {
          return flippedRotationAxis;
        }

      default: {
          throw new InvalidOperationException("Invalid axis");
        }
    }
  }

  public static Color GetColor(Square square) {
    switch (square) {
      case Square.None: {
          return Color.gray;
        }

      case Square.White: {
          return Color.white;
        }

      case Square.Red: {
          return Color.red;
        }

      case Square.Blue: {
          return Color.blue;
        }

      case Square.Orange: {
          return new(1, 0.39f, 0, 1);
        }

      case Square.Green: {
          return Color.green;
        }

      case Square.Yellow: {
          return Color.yellow;
        }

      default: {
          throw new InvalidOperationException("Invalid square");
        }
    }
  }

  public static Vector3 GetLocalNormal(Side side) {
    switch (side) {
      case Side.Top: {
          return Vector3.up;
        }

      case Side.Bottom: {
          return Vector3.down;
        }

      case Side.Left: {
          return Vector3.left;
        }

      case Side.Right: {
          return Vector3.right;
        }

      case Side.Near: {
          return Vector3.back;
        }

      case Side.Far: {
          return Vector3.forward;
        }

      default: {
          throw new InvalidOperationException("Invalid side");
        }
    }
  }

  public static bool CheckCubeSideForSubCube(SubCube subCube, Side cubeSide, int cubeSize) {
    switch (cubeSide) {
      case Side.Top: {
          return subCube.I == 0;
        }

      case Side.Bottom: {
          return subCube.I == cubeSize - 1;
        }

      case Side.Left: {
          return subCube.J == 0;
        }

      case Side.Right: {
          return subCube.J == cubeSize - 1;
        }

      case Side.Near: {
          return subCube.K == 0;
        }

      case Side.Far: {
          return subCube.K == cubeSize - 1;
        }

      default: {
          throw new InvalidOperationException("Invalid side");
        }
    }
  }
}
