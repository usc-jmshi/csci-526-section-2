using System;
using UnityEngine;

public static class Utils {
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
}
