using UnityEngine;

public class GameManager: MonoBehaviour {
  public static GameManager Instance { get; private set; }

  public bool LevelEditor => _levelEditor;

  [SerializeField]
  private bool _levelEditor;

  private void Awake() {
    Instance = this;
  }
}
