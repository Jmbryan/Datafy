using System;

namespace Datafy.Core
{
    public static class IdGenerator
    {
        public const ulong ReserveMask  = 0xF000000000000000;
        public const ulong TimeMask     = 0x0FFFFFFFF0000000;
        public const ulong SessionMask  = 0x000000000FFFFFFF;
        public const int ReserveShift = 60;
        public const int TimeShift = 32;
        public const int SessionShift = 0;
        private static ulong s_sessionCounter = 1;

        public static ulong GenerateId()
        {
            // 4 bits reserved
            // 32 bits time (seconds)
            // 28 bits session counter @ startup (incremented by 1)

            ulong time = (TimeMask & ((ulong)DateTimeOffset.Now.ToUnixTimeSeconds() << TimeShift));
            ulong session = (SessionMask & s_sessionCounter++);

            return time | session;
        }
    }
}
