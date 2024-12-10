using HarmonyLib;

[HarmonyPatch(typeof(OxygenBreather), "OnSpawn")]
public static class OxygenBreather_OnSpawn_Patch
{
    public static void Postfix(OxygenBreather __instance)
    {
        // Убираем статусы потребления кислорода и выделения CO2
        KSelectable component = __instance.GetComponent<KSelectable>();
        if (component != null)
        {
            component.RemoveStatusItem(Db.Get().DuplicantStatusItems.BreathingO2);
            component.RemoveStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2);
        }
    }
}

[HarmonyPatch(typeof(SuffocationMonitor), "InitializeStates")]
public static class SuffocationMonitor_Patch
{
    public static bool Prefix(ref StateMachine.BaseState default_state, SuffocationMonitor __instance)
    {
        // Переключаем состояние на "удовлетворено"
        default_state = __instance.satisfied;
        __instance.root.TagTransition(GameTags.Minions.Models.Standard, __instance.satisfied, false)
                        .TagTransition(GameTags.Dead, __instance.dead, false);
        __instance.dead.DoNothing();
        return false; // Останавливаем стандартную инициализацию
    }
}

[HarmonyPatch(typeof(SuitSuffocationMonitor), "InitializeStates")]
public static class SuitSuffocationMonitor_Patch
{
    public static bool Prefix(ref StateMachine.BaseState default_state, SuitSuffocationMonitor __instance)
    {
        // Переключаем состояние на "удовлетворено"
        default_state = __instance.satisfied;
        __instance.root.TagTransition(GameTags.Minions.Models.Standard, __instance.satisfied, false);
        return false; // Останавливаем стандартную инициализацию
    }
}
[HarmonyPatch(typeof(OxygenBreather), "Sim200ms")]
public static class OxygenBreather_Sim200ms_Patch
{
    public static bool Prefix(OxygenBreather __instance, float dt)
    {
        // Отключаем логику обновления симуляции для кислорода
        UnityEngine.Debug.Log("Sim200ms logic disabled for OxygenBreather.");
        return false; // Возвращаем false, чтобы предотвратить выполнение оригинального метода
    }
}
