using System;

[Serializable]
public class Level {
  [Serializable]
  public class Array<T> {
    public int Size => Elements.Length;

    public T[] Elements;

    public Array(int size) {
      Elements = new T[size];
    }

    public void SetElement(int index, T value) {
      Elements[index] = value;
    }

    public T GetElement(int index) {
      return Elements[index];
    }
  }

  public int Size => Squares.Size;

  public Array<Array<Array<Array<Square>>>> Squares;
  public Array<Array<Array<Array<SpecialSquare>>>> SpecialSquares;

  public Level(int size) {
    Squares = new(size);
    SpecialSquares = new(size);

    for (int i = 0; i < size; i++) {
      Squares.SetElement(i, new(size));
      SpecialSquares.SetElement(i, new(size));

      for (int j = 0; j < size; j++) {
        Squares.GetElement(i).SetElement(j, new(size));
        SpecialSquares.GetElement(i).SetElement(j, new(size));

        for (int k = 0; k < size; k++) {
          Squares.GetElement(i).GetElement(j).SetElement(k, new(Enum.GetValues(typeof(Side)).Length));
          SpecialSquares.GetElement(i).GetElement(j).SetElement(k, new(Enum.GetValues(typeof(Side)).Length));
        }
      }
    }
  }

  public void SetSquare(int i, int j, int k, Side side, Square square) {
    Squares.GetElement(i).GetElement(j).GetElement(k).SetElement((int) side, square);
  }

  public void SetSpecialSquare(int i, int j, int k, Side side, SpecialSquare specialSquare) {
    SpecialSquares.GetElement(i).GetElement(j).GetElement(k).SetElement((int) side, specialSquare);
  }

  public Square GetSquare(int i, int j, int k, Side side) {
    return Squares.GetElement(i).GetElement(j).GetElement(k).GetElement((int) side);
  }

  public SpecialSquare GetSpecialSquare(int i, int j, int k, Side side) {
    return SpecialSquares.GetElement(i).GetElement(j).GetElement(k).GetElement((int) side);
  }
}
