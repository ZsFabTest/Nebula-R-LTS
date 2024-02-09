namespace Nebula.Patches;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
public class MeetingStart
{
    public static DateTime MeetingStartTime = DateTime.MinValue;
    public static void Prefix(MeetingHud __instance)
    {
        MeetingStartTime = DateTime.UtcNow;
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
public static class AddChat
{
    public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer)
    {
        if (__instance != HudManager.Instance.Chat) return true;
        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null) return true;
        bool shouldSeeMessage = localPlayer.Data.IsDead || sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
        try{
            shouldSeeMessage = shouldSeeMessage || localPlayer.GetModData().extraRole.Contains(Roles.Roles.Lover) || localPlayer.GetModData().extraRole.Contains(Roles.Roles.Trilemma);
            shouldSeeMessage = shouldSeeMessage && Roles.Roles.Lover.hasPrivateChatOption.getBool();
        }catch{  }
        if (DateTime.UtcNow - MeetingStart.MeetingStartTime < TimeSpan.FromSeconds(1))
        {
            return shouldSeeMessage;
        }
        return MeetingHud.Instance != null || LobbyBehaviour.Instance != null || shouldSeeMessage;
    }
}

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class EnableChat
{
    public static void Postfix(HudManager __instance)
    {
        var localPlayer = PlayerControl.LocalPlayer;
        
        try{
            if ((localPlayer.GetModData().extraRole.Contains(Roles.Roles.Lover) || localPlayer.GetModData().extraRole.Contains(Roles.Roles.Trilemma)) && !__instance.Chat.isActiveAndEnabled && Roles.Roles.Lover.hasPrivateChatOption.getBool())
                __instance.Chat.SetVisible(true);
        }catch{  }

        try{
            if (localPlayer.GetModData().role == Roles.Roles.Resurrectionist && !Roles.Roles.Resurrectionist.hasRevived)
                __instance.Chat.SetVisible(false);
        }catch{  }
    }
}
