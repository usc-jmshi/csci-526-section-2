using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class LevelSelectUI: MonoBehaviour {
  private struct LevelButtonData {
    public bool Custom;
    public int Index;
  }

  public static LevelSelectUI Instance { get; private set; }

  private ScrollView _scrollView;

  public void Refresh() {
    _scrollView.Clear();

    string[] builtInLevelNames = GameManager.Instance.GetBuiltInLevelNames();

    for (int i = 0; i < builtInLevelNames.Length; i++) {
      Button levelBtn = new() {
        text = builtInLevelNames[i],
        userData = new LevelButtonData {
          Custom = false,
          Index = i
        }
      };

      levelBtn.RegisterCallback<ClickEvent>(OnClickEvent);
      levelBtn.AddToClassList("built-in-level-button");

      _scrollView.Add(levelBtn);
    }

    for (int i = 0; i < GameManager.Instance.CustomLevelNames.Length; i++) {
      VisualElement btnContainer = new();

      btnContainer.AddToClassList("button-container");

      Button levelBtn = new() {
        text = GameManager.Instance.CustomLevelNames[i],
        userData = new LevelButtonData {
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

      _scrollView.Add(btnContainer);
    }
  }

  private void OnEnable() {
    UIDocument uiDoc = GetComponent<UIDocument>();

    _scrollView = uiDoc.rootVisualElement.Q<ScrollView>("scroll-view");
  }

  private void OnDisable() {
    foreach (VisualElement child in _scrollView.Children()) {
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

    if (btn.userData is LevelButtonData levelBtnData) {
      Level level = levelBtnData.Custom ? GameManager.Instance.GetCustomLevel(levelBtnData.Index) : GameManager.Instance.GetBuiltInLevel(levelBtnData.Index);

      Cube.Instance.Load(level);
    } else {
      int index = (int) btn.userData;

      GameManager.Instance.DeleteCustomLevel(index);
    }
  }
}
