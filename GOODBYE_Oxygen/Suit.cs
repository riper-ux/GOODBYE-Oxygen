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
                // ��������� ����������� ���� � �������
                __result = true;
                return false; // ���������� ���������� ������������� ������
            }
            return true; // ���� ��������� �� ��������, ��������� ������������ �����
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
                    // ���������, ��������� �� �������� ���
                    if (component.amount >= component.capacity)
                    {
                        Debug.Log("Suit is already fully charged. Skipping charging process.");
                        return false; // ������ ��� �������, ����������
                    }

                    // ���� �� ��������, ��������� �� ���������
                    Debug.Log($"Charging suit: Current Amount = {component.amount}, Capacity = {component.capacity}");
                    component.amount = component.capacity;
                }

                return false; // ���������� ���������� ������������� ������
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
                // ������ ���������� true ��� �������� ���������
                __result = true;
                return false; // ���������� ���������� ������������� ������
            }
            return true;
        }
    }
}
