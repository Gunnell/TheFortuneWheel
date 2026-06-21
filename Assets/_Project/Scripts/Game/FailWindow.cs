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
        [SerializeField, HideInInspector] private TMP_Text _cashText;
        [SerializeField, HideInInspector] private TMP_Text _coinText;
        [SerializeField, Range(0f, 1f)] private float _disabledReviveAlpha = 0.45f;
        [SerializeField, Range(0.5f, 1f)] private float _disabledReviveScale = 0.88f;

        private CanvasGroup _reviveGroup;

        public event Action Revive;
        public event Action Leave;

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

        public void Show(int cost, bool canAfford, int cash, int coin)
        {
            if (_costText != null) _costText.text = cost.ToString("N0");
            if (_cashText != null) _cashText.text = cash.ToString("N0");
            if (_coinText != null) _coinText.text = coin.ToString("N0");
            SetReviveAffordable(canAfford);
            gameObject.SetActive(true);
        }

        private void SetReviveAffordable(bool canAfford)
        {
            if (_reviveButton == null) return;
            _reviveButton.interactable = canAfford;
            _reviveButton.transform.localScale = Vector3.one * (canAfford ? 1f : _disabledReviveScale);

            if (_reviveGroup == null)
            {
                _reviveGroup = _reviveButton.GetComponent<CanvasGroup>();
                if (_reviveGroup == null) _reviveGroup = _reviveButton.gameObject.AddComponent<CanvasGroup>();
            }
            _reviveGroup.alpha = canAfford ? 1f : _disabledReviveAlpha;
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
                else if (b.name == "ui_button_leave" || b.name == "ui_button_giveup") _leaveButton = b;
            }
            foreach (TMP_Text t in GetComponentsInChildren<TMP_Text>(true))
            {
                if (t.name == "ui_text_revive_cost_value") _costText = t;
                else if (t.name == "ui_text_fail_cash_value") _cashText = t;
                else if (t.name == "ui_text_fail_coin_value") _coinText = t;
            }
        }
#endif
    }
}
