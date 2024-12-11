using HarmonyLib;
using Klei.CustomSettings;

public static class Pathes
{
    public static bool IsOxygenConsumptionDisabled()
    {
        return CustomGameSettings.Instance
            .GetCurrentQualitySetting("DisableOxygenConsumption").id == "Enabled";
    }

    [HarmonyPatch(typeof(OxygenBreather), "OnSpawn")]
    public static class OxygenBreather_OnSpawn_Patch
    {
        public static void Postfix(OxygenBreather __instance)
        {
            if (IsOxygenConsumptionDisabled())
            {
                // ������� ������� ����������� ��������� � ��������� CO2
                KSelectable component = __instance.GetComponent<KSelectable>();
                if (component != null)
                {
                    component.RemoveStatusItem(Db.Get().DuplicantStatusItems.BreathingO2);
                    component.RemoveStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2);
                }
            }
        }
    }

    [HarmonyPatch(typeof(SuffocationMonitor), "InitializeStates")]
    public static class SuffocationMonitor_Patch
    {
        public static bool Prefix(ref StateMachine.BaseState default_state, SuffocationMonitor __instance)
        {
            if (IsOxygenConsumptionDisabled())
            {
                // ����������� ��������� �� "�������������"
                default_state = __instance.satisfied;
                __instance.root.TagTransition(GameTags.Minions.Models.Standard, __instance.satisfied, false)
                                .TagTransition(GameTags.Dead, __instance.dead, false);
                __instance.dead.DoNothing();
                return false; // ������������� ����������� �������������
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(SuitSuffocationMonitor), "InitializeStates")]
    public static class SuitSuffocationMonitor_Patch
    {
        public static bool Prefix(ref StateMachine.BaseState default_state, SuitSuffocationMonitor __instance)
        {
            if (IsOxygenConsumptionDisabled())
            {
                // ����������� ��������� �� "�������������"
                default_state = __instance.satisfied;
                __instance.root.TagTransition(GameTags.Minions.Models.Standard, __instance.satisfied, false);
                return false; // ������������� ����������� �������������
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(OxygenBreather), "Sim200ms")]
    public static class OxygenBreather_Sim200ms_Patch
    {
        public static bool Prefix(OxygenBreather __instance, float dt)
        {
            if (IsOxygenConsumptionDisabled())
            {
                // ��������� ������ ���������� ��������� ��� ���������
                UnityEngine.Debug.Log("Sim200ms logic disabled for OxygenBreather.");
                return false; // ���������� false, ����� ������������� ���������� ������������� ������
            }
            return true;
        }
    }
}
