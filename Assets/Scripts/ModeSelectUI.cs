using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class ModeSelectUI: MonoBehaviour {
  private Button _modeSelectBtn;

  private void OnEnable() {
    UIDocument uiDoc = GetComponent<UIDocument>();

    _modeSelectBtn = uiDoc.rootVisualElement.Q<Button>("mode-select");

    _modeSelectBtn.RegisterCallback<ClickEvent>(OnClickEvent);
  }

  private void OnDisable() {
    _modeSelectBtn.UnregisterCallback<ClickEvent>(OnClickEvent);
  }

  private void Start() {
    _modeSelectBtn.text = GameManager.Instance.IsLevelEditor ? "Switch to Game" : "Switch to Level Editor";
  }

  private void OnClickEvent(ClickEvent evt) {
    SceneManager.LoadScene(GameManager.Instance.IsLevelEditor ? "Game" : "LevelEditor");
  }
}
