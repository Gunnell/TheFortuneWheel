namespace FortuneWheel
{
    public enum ZoneType { Normal, Safe, Super }

    public static class Zones
    {
        public static ZoneType Classify(int zone)
        {
            if (zone <= 1) return ZoneType.Safe;
            if (zone % 30 == 0) return ZoneType.Super;
            if (zone % 5 == 0) return ZoneType.Safe;
            return ZoneType.Normal;
        }
    }
}
