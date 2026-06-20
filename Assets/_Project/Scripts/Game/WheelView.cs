using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace FortuneWheel
{
    [DisallowMultipleComponent]
    public class WheelView : MonoBehaviour
    {
        [SerializeField] private WheelData _wheel;
        [SerializeField] private WheelSlotView[] _slots;
        [SerializeField] private Transform _rotor;
        [SerializeField] private Transform _indicator;
        [SerializeField, HideInInspector] private Image _baseImage;
        [SerializeField, HideInInspector] private Image _indicatorImage;
        [SerializeField] private float _spinDuration = 4f;
        [SerializeField] private int _spinTurns = 5;
        [SerializeField] private float _growthPerZone = 0.15f;

        private readonly WheelGenerator _generator = new WheelGenerator();
        private readonly System.Random _random = new System.Random();
        private List<WheelSlice> _current;
        private bool _isSpinning;
        private int _zone = 1;

        public event Action<WheelSlice, WheelSlotView> Landed;
        public bool IsSpinning => _isSpinning;

        private void Start()
        {
            if (_slots == null || _slots.Length == 0)
                _slots = GetComponentsInChildren<WheelSlotView>(true);
            if (_rotor == null) _rotor = transform.parent;

            if (_indicator != null && _rotor != null && _rotor.parent != null)
                _indicator.SetParent(_rotor.parent, true);

            ShowNewWheel();
        }

        public void ShowWheel(WheelData wheel, int zone)
        {
            _wheel = wheel;
            _zone = zone;
            ShowNewWheel();
        }

        public void ShowNewWheel()
        {
            if (_wheel == null || _slots == null) return;

            _current = _generator.Generate(_wheel, _zone, _growthPerZone, _random);

            for (int i = 0; i < _slots.Length && i < _current.Count; i++)
                _slots[i].Show(_current[i]);

            ApplySkin();
        }

        private void ApplySkin()
        {
            if (_baseImage != null && _wheel.BaseSprite != null) _baseImage.sprite = _wheel.BaseSprite;
            if (_indicatorImage != null && _wheel.IndicatorSprite != null) _indicatorImage.sprite = _wheel.IndicatorSprite;
        }

        public void Spin()
        {
            if (_isSpinning || _current == null || _slots == null || _slots.Length == 0 || _rotor == null) return;

            int index = _random.Next(_slots.Length);

            float slotAngle = _slots[index].transform.localEulerAngles.z;
            float current = _rotor.localEulerAngles.z;
            float align = Mathf.Repeat(-slotAngle - current, 360f);
            float endZ = current + _spinTurns * 360f + align;

            _isSpinning = true;
            _rotor.DOLocalRotate(new Vector3(0f, 0f, endZ), _spinDuration, RotateMode.FastBeyond360)
                  .SetEase(Ease.OutQuart)
                  .OnComplete(() =>
                  {
                      _isSpinning = false;
                      Landed?.Invoke(_current[index], _slots[index]);
                  });
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_slots == null || _slots.Length == 0)
                _slots = GetComponentsInChildren<WheelSlotView>(true);
            if (_rotor == null) _rotor = transform.parent;
            if (_indicator == null && transform.parent != null)
                _indicator = transform.parent.Find("ui_image_spin_indicator_value");
            if (_baseImage == null && transform.parent != null)
            {
                Transform t = transform.parent.Find("ui_image_spin_base_value");
                if (t != null) _baseImage = t.GetComponent<Image>();
            }
            if (_indicatorImage == null && _indicator != null)
                _indicatorImage = _indicator.GetComponent<Image>();
        }
#endif
    }
}
