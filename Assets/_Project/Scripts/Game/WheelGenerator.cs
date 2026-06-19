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
        public List<WheelSlice> Generate(WheelData wheel, int amount, Random random)
        {
            var slices = new List<WheelSlice>(WheelData.SlotCount);

            for (int i = 0; i < WheelData.SlotCount; i++)
                slices.Add(new WheelSlice(PickReward(wheel, random), amount, false));

            if (wheel.HasBomb && wheel.BombReward != null)
            {
                int bombSlot = random.Next(WheelData.SlotCount);
                slices[bombSlot] = new WheelSlice(wheel.BombReward, 0, true);
            }

            return slices;
        }

        private static RewardData PickReward(WheelData wheel, Random random)
        {
            var pool = wheel.Pool;
            return pool.Count == 0 ? null : pool[random.Next(pool.Count)];
        }
    }
}
