using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class LevelSaveUI: MonoBehaviour {
  public static LevelSaveUI Instance { get; private set; }

  public bool IsFocused => _levelNameTextField.focusController.focusedElement == _levelNameTextField;

  private TextField _levelNameTextField;
  private Button _saveLevelButton;

  private void OnEnable() {
    UIDocument uiDoc = GetComponent<UIDocument>();

    _levelNameTextField = uiDoc.rootVisualElement.Q<TextField>("level-name-text-field");
    _saveLevelButton = uiDoc.rootVisualElement.Q<Button>("save-level-button");

    _levelNameTextField.RegisterCallback<ChangeEvent<string>>(OnChangeEvent);
    _saveLevelButton.RegisterCallback<ClickEvent>(OnClickEvent);
  }

  private void OnDisable() {
    _levelNameTextField.UnregisterCallback<ChangeEvent<string>>(OnChangeEvent);
    _saveLevelButton.UnregisterCallback<ClickEvent>(OnClickEvent);
  }

  private void Awake() {
    Instance = this;
  }

  private void OnDestroy() {
    Instance = null;
  }

  private void OnChangeEvent(ChangeEvent<string> evt) {
    GameManager.Instance.NewCustomLevelName = evt.newValue;
  }

  private void OnClickEvent(ClickEvent evt) {
    GameManager.Instance.SaveCustomLevel();
  }
}
