using UnityEngine;

namespace FortuneWheel
{
    [DisallowMultipleComponent]
    public class WheelView : MonoBehaviour
    {
        [SerializeField] private WheelData _wheel;
        [SerializeField] private WheelSlotView[] _slots;

        private readonly WheelGenerator _generator = new WheelGenerator();
        private readonly System.Random _random = new System.Random();

        private void Start()
        {
            if (_slots == null || _slots.Length == 0)
                _slots = GetComponentsInChildren<WheelSlotView>(true);

            ShowNewWheel(1);
        }

        public void ShowNewWheel(int zone)
        {
            if (_wheel == null || _slots == null) return;

            int amount = 10 * zone;
            var slices = _generator.Generate(_wheel, amount, _random);

            for (int i = 0; i < _slots.Length && i < slices.Count; i++)
                _slots[i].Show(slices[i]);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_slots == null || _slots.Length == 0)
                _slots = GetComponentsInChildren<WheelSlotView>(true);
        }
#endif
    }
}
