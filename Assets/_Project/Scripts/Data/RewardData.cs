using UnityEngine;

namespace FortuneWheel
{
    public enum RewardKind { Item, Cash, Coin }

    [CreateAssetMenu(fileName = "Reward", menuName = "Fortune Wheel/Reward")]
    public class RewardData : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;
        [SerializeField] private RewardKind _kind = RewardKind.Item;
        [SerializeField] private int _minAmount = 1;
        [SerializeField] private int _maxAmount = 1;

        public string DisplayName => _displayName;
        public Sprite Icon => _icon;
        public RewardKind Kind => _kind;
        public int MinAmount => _minAmount;
        public int MaxAmount => _maxAmount;
    }
}
