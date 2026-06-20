using UnityEngine;
using UnityEngine.UI;

namespace FortuneWheel
{
    [DisallowMultipleComponent]
    public class GameController : MonoBehaviour
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private WheelView _wheel;
        [SerializeField] private Inventory _inventory;
        [SerializeField] private CurrencyHud _hud;
        [SerializeField] private WheelData _normalWheel;
        [SerializeField] private WheelData _safeWheel;
        [SerializeField] private WheelData _superWheel;
        [SerializeField] private float _advanceDelay = 0.6f;

        private int _zone = 1;

        private void OnEnable()
        {
            if (_exitButton != null) _exitButton.onClick.AddListener(OnExit);
            if (_wheel != null) _wheel.Landed += OnLanded;
        }

        private void OnDisable()
        {
            if (_exitButton != null) _exitButton.onClick.RemoveListener(OnExit);
            if (_wheel != null) _wheel.Landed -= OnLanded;
        }

        private void Start()
        {
            _zone = 1;
            ShowZone();
        }

        private void OnLanded(WheelSlice slice, WheelSlotView slot)
        {
            CancelInvoke(nameof(AdvanceZone));
            Invoke(nameof(AdvanceZone), _advanceDelay);
        }

        private void AdvanceZone()
        {
            _zone++;
            ShowZone();
        }

        private void ShowZone()
        {
            WheelData wheel = WheelFor(Zones.Classify(_zone));
            if (_wheel != null && wheel != null) _wheel.ShowWheel(wheel);
        }

        private WheelData WheelFor(ZoneType type)
        {
            switch (type)
            {
                case ZoneType.Super: return _superWheel;
                case ZoneType.Safe: return _safeWheel;
                default: return _normalWheel;
            }
        }

        private void OnExit()
        {
            if (_wheel == null || _wheel.IsSpinning) return;

            if (_inventory != null) _inventory.CollectInto(_hud);
            CancelInvoke(nameof(AdvanceZone));
            _zone = 1;
            ShowZone();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_exitButton == null) _exitButton = GetComponentInChildren<Button>(true);
            if (_wheel == null) _wheel = FindObjectOfType<WheelView>();
            if (_inventory == null) _inventory = FindObjectOfType<Inventory>();
            if (_hud == null) _hud = FindObjectOfType<CurrencyHud>();
        }
#endif
    }
}
