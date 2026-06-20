using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FortuneWheel
{
    [DisallowMultipleComponent]
    public class FailWindow : MonoBehaviour
    {
        [SerializeField, HideInInspector] private Button _reviveButton;
        [SerializeField, HideInInspector] private Button _leaveButton;
        [SerializeField, HideInInspector] private TMP_Text _costText;

        public event Action Revive;
        public event Action Leave;

        private void Awake() => gameObject.SetActive(false);

        private void OnEnable()
        {
            if (_reviveButton != null) _reviveButton.onClick.AddListener(RaiseRevive);
            if (_leaveButton != null) _leaveButton.onClick.AddListener(RaiseLeave);
        }

        private void OnDisable()
        {
            if (_reviveButton != null) _reviveButton.onClick.RemoveListener(RaiseRevive);
            if (_leaveButton != null) _leaveButton.onClick.RemoveListener(RaiseLeave);
        }

        public void Show(int cost, bool canAfford)
        {
            if (_costText != null) _costText.text = cost.ToString("N0");
            if (_reviveButton != null) _reviveButton.interactable = canAfford;
            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);

        private void RaiseRevive() => Revive?.Invoke();
        private void RaiseLeave() => Leave?.Invoke();

#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (Button b in GetComponentsInChildren<Button>(true))
            {
                if (b.name == "ui_button_revive") _reviveButton = b;
                else if (b.name == "ui_button_leave") _leaveButton = b;
            }
            foreach (TMP_Text t in GetComponentsInChildren<TMP_Text>(true))
                if (t.name == "ui_text_revive_cost_value") _costText = t;
        }
#endif
    }
}
