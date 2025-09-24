using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class LevelSelectUI: MonoBehaviour {
  public static LevelSelectUI Instance { get; private set; }

  private struct ButtonData {
    public bool Custom;
    public int Index;
  }

  private ScrollView _levelSelectScrollView;

  public void Refresh() {
    _levelSelectScrollView.Clear();

    string[] builtInLevelNames = GameManager.Instance.GetBuiltInLevelNames();

    for (int i = 0; i < builtInLevelNames.Length; i++) {
      Button levelBtn = new() {
        text = builtInLevelNames[i],
        userData = new ButtonData {
          Custom = false,
          Index = i
        }
      };

      levelBtn.RegisterCallback<ClickEvent>(OnClickEvent);
      levelBtn.AddToClassList("built-in-level-button");

      _levelSelectScrollView.Add(levelBtn);
    }

    for (int i = 0; i < GameManager.Instance.CustomLevelNames.Length; i++) {
      VisualElement btnContainer = new();

      btnContainer.AddToClassList("button-container");

      Button levelBtn = new() {
        text = GameManager.Instance.CustomLevelNames[i],
        userData = new ButtonData {
          Custom = true,
          Index = i
        }
      };

      levelBtn.RegisterCallback<ClickEvent>(OnClickEvent);
      levelBtn.AddToClassList("custom-level-button");

      Button deleteBtn = new() {
        text = "X",
        userData = i
      };

      deleteBtn.RegisterCallback<ClickEvent>(OnClickEvent);
      deleteBtn.AddToClassList("delete-button");

      btnContainer.Add(levelBtn);
      btnContainer.Add(deleteBtn);

      _levelSelectScrollView.Add(btnContainer);
    }
  }

  private void OnEnable() {
    UIDocument uiDoc = GetComponent<UIDocument>();

    _levelSelectScrollView = uiDoc.rootVisualElement.Q<ScrollView>("level-select");
  }

  private void OnDisable() {
    foreach (VisualElement child in _levelSelectScrollView.Children()) {
      if (child is Button button) {
        button.UnregisterCallback<ClickEvent>(OnClickEvent);
      } else {
        foreach (VisualElement grandchild in child.Children()) {
          grandchild.UnregisterCallback<ClickEvent>(OnClickEvent);
        }
      }
    }
  }

  private void Start() {
    Refresh();
  }

  private void Awake() {
    Instance = this;
  }

  private void OnClickEvent(ClickEvent evt) {
    Button btn = (Button) evt.target;

    if (btn.userData is ButtonData data) {
      Level level = data.Custom ? GameManager.Instance.GetCustomLevel(data.Index) : GameManager.Instance.GetBuiltInLevel(data.Index);

      Cube.Instance.Load(level);
    } else {
      int index = (int) btn.userData;

      GameManager.Instance.DeleteCustomLevel(index);
    }
  }
}
