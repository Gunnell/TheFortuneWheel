using TMPro;
using UnityEngine;

namespace FortuneWheel
{
    [DisallowMultipleComponent]
    public class CurrencyHud : MonoBehaviour
    {
        [SerializeField, HideInInspector] private TMP_Text _cashText;
        [SerializeField, HideInInspector] private TMP_Text _coinText;

        private int _cash;
        private int _coin;

        public int Cash => _cash;
        public int Coin => _coin;

        private void Awake() => Refresh();

        public void Add(RewardKind kind, int amount)
        {
            if (amount <= 0) return;

            if (kind == RewardKind.Cash) _cash += amount;
            else if (kind == RewardKind.Coin) _coin += amount;
            else return;

            Refresh();
        }

        private void Refresh()
        {
            if (_cashText != null) _cashText.text = _cash.ToString("N0");
            if (_coinText != null) _coinText.text = _coin.ToString("N0");
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_cashText != null && _coinText != null) return;

            foreach (TMP_Text t in GetComponentsInChildren<TMP_Text>(true))
            {
                if (_cashText == null && t.name == "ui_text_cash_value") _cashText = t;
                if (_coinText == null && t.name == "ui_text_coin_value") _coinText = t;
            }
        }
#endif
    }
}
