using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class NewCustomLevelNameUI: MonoBehaviour {
  public static NewCustomLevelNameUI Instance { get; private set; }

  public bool IsFocused => _newCustomLevelNameTextField.panel.focusController.focusedElement == _newCustomLevelNameTextField;

  private TextField _newCustomLevelNameTextField;
  private Button _saveButton;

  private void OnEnable() {
    UIDocument uiDoc = GetComponent<UIDocument>();

    _newCustomLevelNameTextField = uiDoc.rootVisualElement.Q<TextField>("new-custom-level-name");
    _saveButton = uiDoc.rootVisualElement.Q<Button>("save");

    _newCustomLevelNameTextField.RegisterCallback<ChangeEvent<string>>(OnChangeEvent);
    _saveButton.RegisterCallback<ClickEvent>(OnClickEvent);
  }

  private void OnDisable() {
    _newCustomLevelNameTextField.UnregisterCallback<ChangeEvent<string>>(OnChangeEvent);
    _saveButton.UnregisterCallback<ClickEvent>(OnClickEvent);
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
