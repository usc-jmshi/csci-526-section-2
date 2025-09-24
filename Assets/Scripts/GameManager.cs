using System.IO;
using System.Linq;
using UnityEngine;

public class GameManager: MonoBehaviour {
  public static GameManager Instance { get; private set; }

  public bool IsLevelEditor => _isLevelEditor;
  public string LevelFileName => _levelFileName;

  [SerializeField]
  private bool _isLevelEditor;
  [SerializeField]
  private string _levelFileName;
  [SerializeField]
  private TextAsset[] _builtInLevelFiles;

  private string[] _customLevelNames;

  public void SetLevelFileName(string levelFileName) {
    _levelFileName = levelFileName;
  }

  public string[] GetBuiltInLevelNames() {
    return _builtInLevelFiles.Select(file => file.name).ToArray();
  }

  public string[] GetCustomLevelNames() {
    return _customLevelNames;
  }

  public Level GetBuiltInLevel(int index) {
    string levelJSON = _builtInLevelFiles[index].text;

    return JsonUtility.FromJson<Level>(levelJSON);
  }

  public Level GetCustomLevel(int index) {
    string levelJSON = File.ReadAllText($"{Application.persistentDataPath}/Levels/{_customLevelNames[index]}.json");

    return JsonUtility.FromJson<Level>(levelJSON);
  }

  private void Awake() {
    Instance = this;

    Directory.CreateDirectory($"{Application.persistentDataPath}/Levels");

    _customLevelNames = Directory.GetFiles($"{Application.persistentDataPath}/Levels", "*.json")
      .Select(fileName => Path.GetFileNameWithoutExtension(fileName))
      .ToArray();
  }
}
