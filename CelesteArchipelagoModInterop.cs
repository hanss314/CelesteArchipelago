using System;
using MonoMod.ModInterop;

namespace CelesteArchipelago
{
    [ModImportName("ExtendedVariantMode")]
    public static class ExtendedVariantInterop
    {
        public static Func<string, object> GetCurrentVariantValue;

        public static Action<string, int, bool> TriggerIntegerVariant;

        public static Action<string, bool, bool> TriggerBooleanVariant;

        public static Action<string, float, bool> TriggerFloatVariant;

        public static Action<string, object, bool> TriggerVariant;

        public static Action<int> SetJumpCount;

        public static Action<int> CapJumpCount;
    }
}