using System.Text;

namespace FamTec.Server.Helpers
{
    public static class UniqueCodeHelpers
    {
        private static long _lastTicks = DateTime.UtcNow.Ticks -1;
        private static readonly object _lock = new();

        public static string RandomCode()
        {
            long ticks;
            lock(_lock)
            {
                ticks = DateTime.UtcNow.Ticks;
                if (ticks <= _lastTicks)
                    ticks = _lastTicks + 1;
                _lastTicks = ticks;
            }
            return ToBase26(ticks);
        }

        private static string ToBase26(long value)
        {
            if (value <= 0)
                return "A";

            var sb = new StringBuilder();
            while (value > 0)
            {
                int idx = (int)(value % 26);
                sb.Insert(0, (char)('A' + idx));
                value /= 26;
            }
            return sb.ToString();
        }
    }
}
