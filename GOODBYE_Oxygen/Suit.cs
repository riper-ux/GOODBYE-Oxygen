using HarmonyLib;
using Klei.CustomSettings;

public static class Suit
{
    public static bool IsOxygenConsumptionDisabled()
    {
        return CustomGameSettings.Instance
            .GetCurrentQualitySetting("DisableOxygenConsumption").id == "Enabled";
    }

    [HarmonyPatch(typeof(SuitTank), "ConsumeGas")]
    public static class SuitTank_ConsumeGas_Patch
    {
        public static bool Prefix(ref bool __result)
        {
            if (IsOxygenConsumptionDisabled())
            {
                // Отключаем потребление газа в костюме
                __result = true;
                return false; // Пропускаем выполнение оригинального метода
            }
            return true; // Если настройка не включена, выполняем оригинальный метод
        }
    }

    [HarmonyPatch(typeof(SuitLocker), "ChargeSuit")]
    public static class SuitLocker_ChargeSuit_Patch
    {
        public static bool Prefix(SuitLocker __instance)
        {
            if (IsOxygenConsumptionDisabled())
            {
                KPrefabID storedOutfit = __instance.GetStoredOutfit();
                if (storedOutfit == null)
                    return false;

                SuitTank component = storedOutfit.GetComponent<SuitTank>();
                if (component != null)
                {
                    // Проверяем, полностью ли заполнен бак
                    if (component.amount >= component.capacity)
                    {
                        Debug.Log("Suit is already fully charged. Skipping charging process.");
                        return false; // Костюм уже заряжен, пропускаем
                    }

                    // Если не заполнен, заполняем до максимума
                    Debug.Log($"Charging suit: Current Amount = {component.amount}, Capacity = {component.capacity}");
                    component.amount = component.capacity;
                }

                return false; // Пропускаем выполнение оригинального метода
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(SuitLocker), "HasOxygen")]
    [HarmonyPatch(typeof(SuitLocker), "IsOxygenTankFull")]
    public static class SuitLocker_OxygenCheck_Patch
    {
        public static bool Prefix(ref bool __result)
        {
            if (IsOxygenConsumptionDisabled())
            {
                // Всегда возвращаем true для проверки кислорода
                __result = true;
                return false; // Пропускаем выполнение оригинального метода
            }
            return true;
        }
    }
}
