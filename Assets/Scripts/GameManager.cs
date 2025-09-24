using System.IO;
using System.Linq;
using UnityEngine;

public class GameManager: MonoBehaviour {
  public static GameManager Instance { get; private set; }

  public bool IsLevelEditor => _isLevelEditor;
  public string[] CustomLevelNames { get; private set; }
  public string NewCustomLevelName { private get; set; }

  [SerializeField]
  private bool _isLevelEditor;
  [SerializeField]
  private TextAsset[] _builtInLevelFiles;

  public string[] GetBuiltInLevelNames() {
    return _builtInLevelFiles.Select(file => file.name).ToArray();
  }

  public Level GetBuiltInLevel(int index) {
    string levelJSON = _builtInLevelFiles[index].text;

    return JsonUtility.FromJson<Level>(levelJSON);
  }

  public Level GetCustomLevel(int index) {
    string levelJSON = File.ReadAllText($"{Application.persistentDataPath}/Levels/{CustomLevelNames[index]}.json");

    return JsonUtility.FromJson<Level>(levelJSON);
  }

  public void SaveCustomLevel() {
    if (string.IsNullOrWhiteSpace(NewCustomLevelName)) {
      return;
    }

    Level level = Cube.Instance.GetLevel();

    string levelJSON = JsonUtility.ToJson(level, true);

    File.WriteAllText($"{Application.persistentDataPath}/Levels/{NewCustomLevelName}.json", levelJSON);

    RefreshCustomLevelNames();

    NotificationUI.Instance.Notify("Save", Color.green);
  }

  public void DeleteCustomLevel(int index) {
    File.Delete($"{Application.persistentDataPath}/Levels/{CustomLevelNames[index]}.json");

    RefreshCustomLevelNames();

    LevelSelectUI.Instance.Refresh();

    NotificationUI.Instance.Notify("Delete", Color.red);
  }

  private void RefreshCustomLevelNames() {
    CustomLevelNames = Directory.GetFiles($"{Application.persistentDataPath}/Levels", "*.json")
      .Select(fileName => Path.GetFileNameWithoutExtension(fileName))
      .ToArray();
  }

  private void Awake() {
    Instance = this;

    Directory.CreateDirectory($"{Application.persistentDataPath}/Levels");

    RefreshCustomLevelNames();
  }
}
