using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class ModeSelectUI: MonoBehaviour {
  private Button _switchModeBtn;

  private void OnEnable() {
    UIDocument uiDoc = GetComponent<UIDocument>();

    _switchModeBtn = uiDoc.rootVisualElement.Q<Button>("switch-mode-button");

    _switchModeBtn.RegisterCallback<ClickEvent>(OnClickEvent);
  }

  private void OnDisable() {
    _switchModeBtn.UnregisterCallback<ClickEvent>(OnClickEvent);
  }

  private void Start() {
    _switchModeBtn.text = GameManager.Instance.IsLevelEditor ? "Switch to Game" : "Switch to Level Editor";
  }

  private void OnClickEvent(ClickEvent evt) {
    SceneManager.LoadScene(GameManager.Instance.IsLevelEditor ? "Game" : "LevelEditor");
  }
}
