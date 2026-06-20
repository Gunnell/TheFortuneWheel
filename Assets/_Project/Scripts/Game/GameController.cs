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
        [SerializeField] private FailWindow _failWindow;
        [SerializeField] private WheelData _normalWheel;
        [SerializeField] private WheelData _safeWheel;
        [SerializeField] private WheelData _superWheel;
        [SerializeField] private float _advanceDelay = 0.6f;
        [SerializeField] private int _baseReviveCost = 50;
        [SerializeField] private ZoneBar _zoneBar;

        private int _zone = 1;
        private int _reviveCount;

        private void OnEnable()
        {
            if (_exitButton != null) _exitButton.onClick.AddListener(OnExit);
            if (_wheel != null) _wheel.Landed += OnLanded;
            if (_failWindow != null)
            {
                _failWindow.Revive += OnRevive;
                _failWindow.Leave += OnLeave;
            }
        }

        private void OnDisable()
        {
            if (_exitButton != null) _exitButton.onClick.RemoveListener(OnExit);
            if (_wheel != null) _wheel.Landed -= OnLanded;
            if (_failWindow != null)
            {
                _failWindow.Revive -= OnRevive;
                _failWindow.Leave -= OnLeave;
            }
        }

        private void Start()
        {
            _zone = 1;
            _reviveCount = 0;
            ShowZone();
        }

        private void OnLanded(WheelSlice slice, WheelSlotView slot)
        {
            if (slice.IsBomb)
            {
                OnBomb();
                return;
            }

            CancelInvoke(nameof(AdvanceZone));
            Invoke(nameof(AdvanceZone), _advanceDelay);
        }

        private void OnBomb()
        {
            if (_failWindow == null) return;
            int cost = ReviveCost();
            _failWindow.Show(cost, _hud != null && _hud.Coin >= cost);
        }

        private int ReviveCost() => _baseReviveCost * (_reviveCount + 1);

        private void OnRevive()
        {
            if (_hud == null || !_hud.TrySpendCoin(ReviveCost())) return;

            _reviveCount++;
            if (_failWindow != null) _failWindow.Hide();
            ShowZone();
        }

        private void OnLeave()
        {
            if (_inventory != null) _inventory.Clear();
            if (_failWindow != null) _failWindow.Hide();
            _reviveCount = 0;
            _zone = 1;
            ShowZone();
        }

        private void AdvanceZone()
        {
            _zone++;
            ShowZone();
        }

        private void ShowZone()
        {
            WheelData wheel = WheelFor(Zones.Classify(_zone));
            if (_wheel != null && wheel != null) _wheel.ShowWheel(wheel, _zone);
            if (_zoneBar != null) _zoneBar.SetZone(_zone);
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
            _reviveCount = 0;
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
            if (_zoneBar == null) _zoneBar = FindObjectOfType<ZoneBar>();
            if (_failWindow == null) _failWindow = FindObjectOfType<FailWindow>();
        }
#endif
    }
}
