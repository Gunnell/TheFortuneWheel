using UnityEngine;

namespace FortuneWheel
{
    [CreateAssetMenu(fileName = "Reward", menuName = "Fortune Wheel/Reward")]
    public class RewardData : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;

        public string DisplayName => _displayName;
        public Sprite Icon => _icon;
    }
}
