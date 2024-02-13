using System;
using System.Collections.Generic;
using System.Text;
using Unity.IL2CPP.CompilerServices;

namespace Nebula.Roles.CrewmateRoles
{
    public class Philosopher : Template.TCrewmate
    {
        public static Color RoleColor = new(240f / 255f,255f / 255f,240f / 255f);

        private double[] allPlayerProcess = new double[256];

        private Module.CustomOption rangeOfGettingRole;
        private Module.CustomOption requireTimeOption;

        public override void LoadOptionData()
        {
            TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
            rangeOfGettingRole = CreateOption(Color.white,"rangeOfGettingRole",2f,0.5f,10f,0.5f);
            rangeOfGettingRole.suffix = "cross";
            requireTimeOption = CreateOption(Color.white,"requireTime",60f,5f,180f,5f);
            requireTimeOption.suffix = "second";
        }

        public override void Initialize(PlayerControl __instance)
        {
            for(int i = 0;i < 127; i++) { allPlayerProcess[i] = 0f; }
        }

        public override void MyPlayerControlUpdate()
        {
            foreach(var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p.PlayerId == PlayerControl.LocalPlayer.PlayerId) continue;
                if (p.Data.IsDead)
                {
                    Game.GameData.data.AllPlayers[p.PlayerId].RoleInfo = "";
                    continue;
                }
                if (Vector2.Distance(PlayerControl.LocalPlayer.transform.position,p.transform.position) <= rangeOfGettingRole.getFloat() * 0.75f)
                {
                    allPlayerProcess[p.PlayerId] += Time.deltaTime;
                }

                if (allPlayerProcess[p.PlayerId] >= requireTimeOption.getFloat())
                {
                    Game.GameData.data.AllPlayers[p.PlayerId].RoleInfo = Helpers.cs(p.GetModData().role.Color, Language.Language.GetString("role." + p.GetModData().role.LocalizeName + ".name"));
                }
                else Game.GameData.data.AllPlayers[p.PlayerId].RoleInfo = allPlayerProcess[p.PlayerId].ToString();
            }
        }

        public override void CleanUp()
        {
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p.PlayerId == PlayerControl.LocalPlayer.PlayerId) continue;
                Game.GameData.data.AllPlayers[p.PlayerId].RoleInfo = "";
            }
        }

        public Philosopher() : base("Philosopher","philosopher",RoleColor,false)
        {
        }
    }
}
