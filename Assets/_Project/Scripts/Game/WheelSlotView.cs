using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FortuneWheel
{
    [DisallowMultipleComponent]
    public class WheelSlotView : MonoBehaviour
    {
        [SerializeField, HideInInspector] private Image _icon;
        [SerializeField, HideInInspector] private TMP_Text _amount;

        public void Show(WheelSlice slice)
        {
            if (_icon != null)
                _icon.sprite = slice.Reward != null ? slice.Reward.Icon : null;

            if (_amount != null)
                _amount.text = slice.IsBomb ? string.Empty : $"x{slice.Amount}";
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_icon == null) _icon = GetComponentInChildren<Image>(true);
            if (_amount == null) _amount = GetComponentInChildren<TMP_Text>(true);
        }
#endif
    }
}
