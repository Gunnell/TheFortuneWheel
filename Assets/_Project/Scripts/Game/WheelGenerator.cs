using System;
using System.Collections.Generic;

namespace FortuneWheel
{
    public class WheelSlice
    {
        public WheelSlice(RewardData reward, int amount, bool isBomb)
        {
            Reward = reward;
            Amount = amount;
            IsBomb = isBomb;
        }

        public RewardData Reward { get; }
        public int Amount { get; }
        public bool IsBomb { get; }
    }

    public class WheelGenerator
    {
        public List<WheelSlice> Generate(WheelData wheel, int zone, float growthPerZone, Random random)
        {
            var pool = new List<RewardData>();
            foreach (RewardData reward in wheel.Pool)
                if (reward != null && !pool.Contains(reward)) pool.Add(reward);

            for (int i = pool.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (pool[i], pool[j]) = (pool[j], pool[i]);
            }

            var slices = new List<WheelSlice>(WheelData.SlotCount);
            for (int i = 0; i < WheelData.SlotCount; i++)
            {
                RewardData reward = pool.Count == 0 ? null : pool[i % pool.Count];
                int amount = reward != null ? RollAmount(reward, zone, growthPerZone, random) : 0;
                slices.Add(new WheelSlice(reward, amount, false));
            }

            if (wheel.HasBomb && wheel.BombReward != null)
            {
                int bombSlot = random.Next(WheelData.SlotCount);
                slices[bombSlot] = new WheelSlice(wheel.BombReward, 0, true);
            }

            return slices;
        }

        private static int RollAmount(RewardData reward, int zone, float growthPerZone, Random random)
        {
            int hi = Math.Max(reward.MinAmount, reward.MaxAmount);
            int baseRoll = random.Next(reward.MinAmount, hi + 1);
            double scaled = baseRoll * (1.0 + (zone - 1) * growthPerZone);
            return Math.Max(1, (int)Math.Round(scaled));
        }
    }
}
