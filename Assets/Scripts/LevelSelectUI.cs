using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class LevelSelectUI: MonoBehaviour {
  private struct ButtonData {
    public bool Custom;
    public int Index;
  }

  private ScrollView _levelSelectScrollView;

  private void OnEnable() {
    UIDocument uiDoc = GetComponent<UIDocument>();

    _levelSelectScrollView = uiDoc.rootVisualElement.Q<ScrollView>("level-select");
  }

  private void OnDisable() {
    foreach (VisualElement child in _levelSelectScrollView.Children()) {
      child.UnregisterCallback<ClickEvent>(OnClickEvent);
    }
  }

  private void Start() {
    string[] builtInLevelNames = GameManager.Instance.GetBuiltInLevelNames();

    for (int i = 0; i < builtInLevelNames.Length; i++) {
      Button btn = new() {
        text = builtInLevelNames[i],
        userData = new ButtonData {
          Custom = false,
          Index = i
        }
      };

      btn.RegisterCallback<ClickEvent>(OnClickEvent);

      _levelSelectScrollView.Add(btn);
    }

    string[] customLevelNames = GameManager.Instance.GetCustomLevelNames();

    for (int i = 0; i < customLevelNames.Length; i++) {
      Button btn = new() {
        text = customLevelNames[i],
        userData = new ButtonData {
          Custom = true,
          Index = i
        }
      };

      btn.RegisterCallback<ClickEvent>(OnClickEvent);

      _levelSelectScrollView.Add(btn);
    }
  }

  private void OnClickEvent(ClickEvent evt) {
    Button btn = (Button) evt.target;
    ButtonData data = (ButtonData) btn.userData;

    Level level = data.Custom ? GameManager.Instance.GetCustomLevel(data.Index) : GameManager.Instance.GetBuiltInLevel(data.Index);

    Cube.Instance.Load(level);
  }
}
