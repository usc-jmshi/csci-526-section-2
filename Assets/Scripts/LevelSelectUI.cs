using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class LevelSelectUI: MonoBehaviour {
  private Button _tutorialBtn;
  private Button _demoBtn;

  private void OnEnable() {
    UIDocument uiDoc = GetComponent<UIDocument>();

    _tutorialBtn = uiDoc.rootVisualElement.Q<Button>("tutorial");
    _demoBtn = uiDoc.rootVisualElement.Q<Button>("demo");

    _tutorialBtn.RegisterCallback<ClickEvent>(LoadTutorial);
    _demoBtn.RegisterCallback<ClickEvent>(LoadDemo);
  }

  private void OnDisable() {
    _tutorialBtn.UnregisterCallback<ClickEvent>(LoadTutorial);
    _demoBtn.UnregisterCallback<ClickEvent>(LoadDemo);
  }

  private void LoadTutorial(ClickEvent evt) {
    GameManager.Instance.SetLevelFileName("Tutorial");
    Cube.Instance.LoadLevel();
  }

  private void LoadDemo(ClickEvent evt) {
    GameManager.Instance.SetLevelFileName("Demo");
    Cube.Instance.LoadLevel();
  }
}
