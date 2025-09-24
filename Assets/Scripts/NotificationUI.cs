using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class NotificationUI: MonoBehaviour {
  public static NotificationUI Instance { get; private set; }

  private const float NotifyDuration = 0.5f;

  private Label _label;
  private Coroutine _notifyCoroutine;

  public void Notify(string message, Color color) {
    if (_notifyCoroutine != null) {
      StopCoroutine(_notifyCoroutine);
    }

    _notifyCoroutine = StartCoroutine(NotifyCoroutine(message, color));
  }

  private IEnumerator NotifyCoroutine(string message, Color color) {
    _label.style.opacity = 1;
    _label.style.color = color;
    _label.text = message;

    float timer = 0;

    do {
      yield return null;

      timer += Time.deltaTime;
      _label.style.opacity = Mathf.Clamp(_label.style.opacity.value - Time.deltaTime / NotifyDuration, 0, 1);
    } while (timer < NotifyDuration);

    _notifyCoroutine = null;
  }

  private void OnEnable() {
    UIDocument uiDoc = GetComponent<UIDocument>();

    _label = uiDoc.rootVisualElement.Q<Label>("label");
  }

  private void Awake() {
    Instance = this;
  }
}
