using System.Collections.Generic;
using UnityEngine;

namespace FortuneWheel
{
    public enum WheelTier { Bronze, Silver, Golden }

    [CreateAssetMenu(fileName = "Wheel", menuName = "Fortune Wheel/Wheel")]
    public class WheelData : ScriptableObject
    {
        public const int SlotCount = 8;

        [SerializeField] private WheelTier _tier = WheelTier.Bronze;
        [SerializeField] private Sprite _baseSprite;
        [SerializeField] private Sprite _indicatorSprite;
        [SerializeField] private List<RewardData> _pool = new List<RewardData>();
        [SerializeField] private float _amountMultiplier = 1f;

        [Header("Bomb")]
        [SerializeField] private bool _hasBomb = true;
        [SerializeField] private RewardData _bombReward;

        public WheelTier Tier => _tier;
        public Sprite BaseSprite => _baseSprite;
        public Sprite IndicatorSprite => _indicatorSprite;
        public IReadOnlyList<RewardData> Pool => _pool;
        public float AmountMultiplier => _amountMultiplier;
        public bool HasBomb => _hasBomb;
        public RewardData BombReward => _bombReward;
    }
}
