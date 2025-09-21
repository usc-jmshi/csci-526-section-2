using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class NotificationUI: MonoBehaviour {
  public static NotificationUI Instance { get; private set; }

  private const float NotifyDuration = 0.5f;

  private Label _notifLabel;
  private Coroutine _notifyCoroutine;

  public void Notify(string message, Color color) {
    if (_notifyCoroutine != null) {
      StopCoroutine(_notifyCoroutine);
    }

    _notifyCoroutine = StartCoroutine(NotifyCoroutine(message, color));
  }

  private IEnumerator NotifyCoroutine(string message, Color color) {
    _notifLabel.style.opacity = 1;
    _notifLabel.style.color = color;
    _notifLabel.text = message;

    float timer = 0;

    do {
      yield return null;

      timer += Time.deltaTime;
      _notifLabel.style.opacity = Mathf.Clamp(_notifLabel.style.opacity.value - Time.deltaTime / NotifyDuration, 0, 1);
    } while (timer < NotifyDuration);

    _notifyCoroutine = null;
  }

  private void OnEnable() {
    UIDocument uiDoc = GetComponent<UIDocument>();

    _notifLabel = uiDoc.rootVisualElement.Q<Label>("notification");
  }

  private void Awake() {
    Instance = this;
  }
}
