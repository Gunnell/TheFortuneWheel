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
        [SerializeField] private float _bombIconScale = 1.5f;
        [SerializeField] private Vector2 _bombIconOffset = Vector2.zero;

        private Vector2 _iconBasePos;
        private bool _cachedBasePos;

        public Transform IconTransform => _icon != null ? _icon.transform : transform;

        private void Awake() => CacheBasePos();

        public void Show(WheelSlice slice)
        {
            if (_icon != null)
            {
                CacheBasePos();
                _icon.sprite = slice.Reward != null ? slice.Reward.Icon : null;
                _icon.rectTransform.localScale = Vector3.one * (slice.IsBomb ? _bombIconScale : 1f);
                _icon.rectTransform.anchoredPosition = slice.IsBomb ? _bombIconOffset : _iconBasePos;
            }

            if (_amount != null)
                _amount.text = slice.IsBomb ? string.Empty : $"x{slice.Amount}";
        }

        private void CacheBasePos()
        {
            if (_cachedBasePos || _icon == null) return;
            _iconBasePos = _icon.rectTransform.anchoredPosition;
            _cachedBasePos = true;
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
