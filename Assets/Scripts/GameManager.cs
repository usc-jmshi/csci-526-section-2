﻿using UnityEngine;

public class GameManager: MonoBehaviour {
  public static GameManager Instance { get; private set; }

  public bool IsLevelEditor => _isLevelEditor;
  public string LevelFileName => _levelFileName;

  [SerializeField]
  private bool _isLevelEditor;
  [SerializeField]
  private string _levelFileName;

  private void Awake() {
    Instance = this;
  }
}
