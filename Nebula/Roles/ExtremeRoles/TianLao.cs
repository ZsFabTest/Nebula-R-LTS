namespace Nebula.Roles.ImpostorRoles;

public class TianLao : Template.TImpostor
{
    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.AdvancedSettings;
    }

    public override void MyPlayerControlUpdate()
    {
        if(PlayerControl.AllPlayerControls.GetFastEnumerator().Any((p) => !p.Data.IsDead && p.PlayerId != PlayerControl.LocalPlayer.PlayerId && p.GetModData().role.category == RoleCategory.Impostor && Vector2.Distance(p.transform.position,PlayerControl.LocalPlayer.transform.position) <= 2.5f))
        {
            PlayerControl.LocalPlayer.killTimer -= Time.deltaTime * 2;
        }
    }

    public TianLao() : base("TianLao", "tianLao", true)
    {
    }
}