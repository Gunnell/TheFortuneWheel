using UnityEngine;
using UnityEngine.UI;

namespace FortuneWheel
{
    [DisallowMultipleComponent]
    public class WheelController : MonoBehaviour
    {
        [SerializeField] private Button _spinButton;
        [SerializeField] private WheelView _wheel;

        private void OnEnable()
        {
            if (_spinButton != null) _spinButton.onClick.AddListener(OnSpinClicked);
        }

        private void OnDisable()
        {
            if (_spinButton != null) _spinButton.onClick.RemoveListener(OnSpinClicked);
        }

        private void OnSpinClicked()
        {
            if (_wheel != null) _wheel.Spin();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_spinButton == null) _spinButton = GetComponentInChildren<Button>(true);
            if (_wheel == null) _wheel = GetComponentInChildren<WheelView>(true);
        }
#endif
    }
}
