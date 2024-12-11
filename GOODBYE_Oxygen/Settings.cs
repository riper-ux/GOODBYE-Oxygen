using Klei.CustomSettings;
using HarmonyLib;

// Определение настройки отключения потребления кислорода
public static class CustomGameSettingConfigs
{
    public static readonly ToggleSettingConfig DisableOxygenConsumption = new ToggleSettingConfig(
        id: "DisableOxygenConsumption",
        label: "Enable Oxygen Consumption",
        tooltip: "Toggle to enable oxygen consumption for duplicants.",
        off_level: new SettingLevel("Enabled", "Enabled", "Unchecked: Oxygen consumption is disabled.", 0, null),
        on_level: new SettingLevel("Disabled", "Disabled", "Checked: Oxygen consumption is enabled.", 0, null),
        default_level_id: "Enabled",
        nosweat_default_level_id: "Enabled"
    );
}

// Патч для добавления настройки в интерфейс
[HarmonyPatch(typeof(CustomGameSettings), "OnPrefabInit")]
public class CustomGameSettings_OnPrefabInit_Patch
{
    public static void Postfix(CustomGameSettings __instance)
    {
        __instance.AddQualitySettingConfig(CustomGameSettingConfigs.DisableOxygenConsumption);
    }
}
