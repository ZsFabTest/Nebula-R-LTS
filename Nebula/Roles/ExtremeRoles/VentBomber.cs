namespace Nebula.Roles.CrewmateRoles
{
    public class VentBomber : Template.TCrewmate
    {
        public static Color RoleColor = new(29f / 255f,150f / 255f,142f / 255f);
        private Module.CustomOption bombBombCooldown;

        public override void LoadOptionData()
        {
            TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
            bombBombCooldown = CreateOption(Color.white,"bombBombCooldown",35f,5f,60f,2.5f);
            bombBombCooldown.suffix = "second";
        }

        private Vent targetVent = null!;
        private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.CloseVentButton.png", 115f, "ui.button.minekeeper.set");
        private List<Vent> vl = new List<Vent>();

        public override void MyPlayerControlUpdate()
        {
            if (!ShipStatus.Instance) return;

            Vent target = null;
            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            float closestDistance = float.MaxValue;
            //Debug.LogWarning("14 " + ShipStatus.Instance.AllVents[14].name);
            for (int i = 0; i < ShipStatus.Instance.AllVents.Length; i++)
            {
                Vent vent = ShipStatus.Instance.AllVents[i];
                //Debug.Log(vent.name);
                if (vent.GetVentData().Sealed) continue;
                float distance = Vector2.Distance(vent.transform.position, truePosition);
                //Debug.Log(vent.name + " " + distance.ToString() + " " + vent.UsableDistance.ToString());
                if (distance <= vent.UsableDistance && distance < closestDistance)
                {
                    closestDistance = distance;
                    target = vent;
                }
            }
            //try{ Debug.Log(target.name); }catch{ Debug.LogWarning("No Target"); };
            targetVent = target;
        }

        private CustomButton set;
        public override void ButtonInitialize(HudManager __instance)
        {
            if (set != null)
            {
                set.Destroy();
                vl.Clear();
            }
            set = new CustomButton(
                () => {
                    Kill(targetVent);
                    vl.Clear();
                    set.Timer = set.MaxTimer;
                },
                () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
                () => { return targetVent && PlayerControl.LocalPlayer.CanMove; },
                () => { set.Timer = set.MaxTimer; },
                buttonSprite.GetSprite(),
                Expansion.GridArrangeExpansion.GridArrangeParameter.None,
                __instance,
                Module.NebulaInputManager.abilityInput.keyCode,
                "button.label.bomb"
            ).SetTimer(CustomOptionHolder.InitialModestAbilityCoolDownOption.getFloat());
            set.MaxTimer = bombBombCooldown.getFloat();
        }

        public override void CleanUp()
        {
            if (set != null)
            {
                set.Destroy();
                set = null;
            }
            targetVent = null;
            vl.Clear();
        }

        private void Kill(Vent v)
        {
            if (!v) return;
            foreach(var vent in vl)
            {
                if (vent && vent.transform.position == v.transform.position) return;
            }
            vl.Add(v);
            foreach(var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (!p.Data.IsDead && p.inVent && Vector2.Distance(v.transform.position, p.transform.position) <= 1f)
                    Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer,p,Game.PlayerData.PlayerStatus.Dead,false,false);
            }
            Kill(v.Left);
            Kill(v.Right);
            Kill(v.Center);
        }

        public VentBomber() : base("VentBomber", "ventBomber", RoleColor, false)
        {
        }
    }
}
