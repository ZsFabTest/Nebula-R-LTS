namespace Nebula.Map.Editors;

class FungleEditor : MapEditor
{
    public FungleEditor() : base(5)
    {
    }

    public override void MapCustomize()
    {
        if (CustomOptionHolder.mapOptions.getBool())
        {
        }
    }

    public override void ModifySabotage()
    {
        if (CustomOptionHolder.SabotageOption.getBool())
        {
            ShipStatus.Instance.Systems[SystemTypes.MushroomMixupSabotage].Cast<MushroomMixupSabotageSystem>().secondsForAutoHeal = CustomOptionHolder.FungleMushroomMixupTimeLimitOption.getFloat();
            ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>().ReactorDuration = CustomOptionHolder.FungleReactorTimeLimitOption.getFloat();
        }
    }

    public override void MinimapOptimizeForJailer(Transform romeNames, MapCountOverlay countOverlay, InfectedOverlay infectedOverlay)
    {
        for (int i = 0; i < infectedOverlay.transform.childCount; i++)
            infectedOverlay.transform.GetChild(i).transform.localScale *= 0.8f;


        romeNames.GetChild(13).localPosition += new Vector3(0f, 0.1f, 0f);

        infectedOverlay.transform.GetChild(4).localPosition += new Vector3(0.07f, 0.2f, 0f);

        countOverlay.transform.GetChild(0).localPosition += new Vector3(0f, -0.75f, 0f);
        countOverlay.transform.GetChild(1).localPosition += new Vector3(0f, -0.75f, 0f);
        countOverlay.transform.GetChild(2).localPosition += new Vector3(0f, -0.75f, 0f);
        countOverlay.transform.GetChild(3).localPosition += new Vector3(0f, -0.75f, 0f);
        countOverlay.transform.GetChild(4).localPosition += new Vector3(0f, -0.75f, 0f);
        countOverlay.transform.GetChild(5).localPosition += new Vector3(0f, -0.75f, 0f);
        countOverlay.transform.GetChild(9).localPosition += new Vector3(0f, -0.55f, 0f);
        countOverlay.transform.GetChild(10).localPosition += new Vector3(0f, -0.75f, 0f);
        countOverlay.transform.GetChild(12).localPosition += new Vector3(0f, -0.75f, 0f);
        countOverlay.transform.GetChild(13).localPosition += new Vector3(0f, -0.42f, 0f);

        foreach (var c in countOverlay.CountAreas) c.YOffset *= -1f;

    }

}