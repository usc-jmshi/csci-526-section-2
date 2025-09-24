using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class NewCustomLevelNameUI: MonoBehaviour {
  private TextField _newCustomLevelNameTextField;

  private void OnEnable() {
    UIDocument uiDoc = GetComponent<UIDocument>();

    _newCustomLevelNameTextField = uiDoc.rootVisualElement.Q<TextField>("new-custom-level-name");

    _newCustomLevelNameTextField.RegisterCallback<ChangeEvent<string>>(OnChangeEvent);
  }

  private void OnDisable() {
    _newCustomLevelNameTextField.UnregisterCallback<ChangeEvent<string>>(OnChangeEvent);
  }

  private void OnChangeEvent(ChangeEvent<string> evt) {
    GameManager.Instance.NewCustomLevelName = evt.newValue;
  }
}
