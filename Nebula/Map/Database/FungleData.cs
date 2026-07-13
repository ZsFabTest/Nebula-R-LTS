namespace Nebula.Map.Database;
public class FungleData : MapData
{
    public override IEnumerable<Tuple<GameObject, float>> AllVitals(ShipStatus shipStatus)
    {
        yield return new(shipStatus.transform.GetChild(4).GetChild(6).GetChild(4).gameObject, 0.28f);
    }

    public override IEnumerable<Tuple<GameObject, float>> AllCameras(ShipStatus shipStatus)
    {
        yield return new(shipStatus.transform.GetChild(4).GetChild(7).GetChild(6).gameObject, 0.1f);
    }

    public FungleData() : base(5, "FungleShip")
    {
        // 设置破坏任务数据
        SabotageMap[SystemTypes.Reactor] = new SabotageData(SystemTypes.Reactor, new Vector3(21.1f, -6.7f), true, true);
        SabotageMap[SystemTypes.Comms] = new SabotageData(SystemTypes.Comms, new Vector3(20.9f, 10.8f), true, false);

        //ドロップシップ
        MapPositions.Add(new Vector2(-9.2f, 13.4f));
        //カフェテリア
        MapPositions.Add(new Vector2(-19.1f, 7.0f));
        MapPositions.Add(new Vector2(-13.6f, 5.0f));
        MapPositions.Add(new Vector2(-20.5f, 6.0f));
        //カフェ下
        MapPositions.Add(new Vector2(-12.9f, 2.3f));
        MapPositions.Add(new Vector2(-21.7f, 2.41f));
        //スプラッシュゾーン
        MapPositions.Add(new Vector2(-20.2f, -0.3f));
        MapPositions.Add(new Vector2(-19.8f, -2.1f));
        MapPositions.Add(new Vector2(-16.1f, -0.1f));
        MapPositions.Add(new Vector2(-15.6f, -1.8f));
        //キャンプファイア周辺
        MapPositions.Add(new Vector2(-11.3f, 2.0f));
        MapPositions.Add(new Vector2(-0.83f, 2.4f));
        MapPositions.Add(new Vector2(-9.4f, 0.2f));
        MapPositions.Add(new Vector2(-6.9f, 0.2f));
        //スプラッシュゾーン下
        MapPositions.Add(new Vector2(-17.3f, -4.5f));
        //キッチン
        MapPositions.Add(new Vector2(-15.4f, -9.5f));
        MapPositions.Add(new Vector2(-17.4f, -7.5f));
        //キッチン・ジャングル間通路
        MapPositions.Add(new Vector2(-11.2f, -6.1f));
        MapPositions.Add(new Vector2(-5.5f, -14.8f));
        //ミーティング上
        MapPositions.Add(new Vector2(-2.8f, 2.2f));
        MapPositions.Add(new Vector2(2.2f, 1.0f));
        //ストレージ
        MapPositions.Add(new Vector2(-0.6f, 4.2f));
        MapPositions.Add(new Vector2(2.3f, 6.2f));
        MapPositions.Add(new Vector2(3.3f, 6.7f));
        //ミーティング・ドーム
        MapPositions.Add(new Vector2(-0.15f, -1.77f));
        MapPositions.Add(new Vector2(-4.65f, 1.58f));
        MapPositions.Add(new Vector2(-4.8f, -1.44f));
        //ラボ
        MapPositions.Add(new Vector2(-7.1f, -11.9f));
        MapPositions.Add(new Vector2(-4.5f, -6.8f));
        MapPositions.Add(new Vector2(-3.3f, -8.9f));
        MapPositions.Add(new Vector2(-5.4f, -10.2f));
        //ジャングル(左)
        MapPositions.Add(new Vector2(-1.44f, -13.3f));
        MapPositions.Add(new Vector2(3.8f, -12.5f));
        //ジャングル(中)
        MapPositions.Add(new Vector2(7.08f, -15.3f));
        MapPositions.Add(new Vector2(11.6f, -14.3f));
        //ジャングル(上)
        MapPositions.Add(new Vector2(2.7f, -6.0f));
        MapPositions.Add(new Vector2(12.1f, -7.3f));
        //グリーンハウス・ジャングル
        MapPositions.Add(new Vector2(13.6f, -12.1f));
        MapPositions.Add(new Vector2(6.4f, -10f));
        //ジャングル(右)
        MapPositions.Add(new Vector2(15.0f, -6.7f));
        MapPositions.Add(new Vector2(18.1f, -9.1f));
        //ジャングル(下)
        MapPositions.Add(new Vector2(14.9f, -16.3f));
        //リアクター
        MapPositions.Add(new Vector2(21.1f, -6.7f));
        //高台
        MapPositions.Add(new Vector2(15.9f, 0.4f));
        MapPositions.Add(new Vector2(15.6f, 4.3f));
        MapPositions.Add(new Vector2(19.2f, 1.78f));
        //鉱山
        MapPositions.Add(new Vector2(12.5f, 7.7f));
        MapPositions.Add(new Vector2(13.4f, 9.7f));
        //ルックアウト
        MapPositions.Add(new Vector2(6.6f, 3.8f));
        MapPositions.Add(new Vector2(8.7f, 1f));
        //梯子中間
        MapPositions.Add(new Vector2(20.1f, 7.2f));
        //コミュ
        MapPositions.Add(new Vector2(20.9f, 10.8f));
        MapPositions.Add(new Vector2(24.1f, 13.2f));
        MapPositions.Add(new Vector2(17.9f, 12.7f));

        // 设置地图比例
        MapScale = 38f;

        // SpawnCandidates
        SpawnCandidates.Add(new SpawnCandidate("jungleLabo", new Vector2(-5.42f, -9.20f), "assets/nebulaassets/SpawnCandidates/Fungle/Laboratory.png", null, 115f, useNewAssets: true));
        SpawnCandidates.Add(new SpawnCandidate("jungleReactor", new Vector2(16.75f, -6.68f), "assets/nebulaassets/SpawnCandidates/Fungle/Reactor.png", null, 115f, useNewAssets: true));
        SpawnCandidates.Add(new SpawnCandidate("jungleGreenhouse", new Vector2(9.07f, -11.00f), "assets/nebulaassets/SpawnCandidates/Fungle/Greenhouse.png", null, 115f, useNewAssets: true));
        SpawnCandidates.Add(new SpawnCandidate("beachToJungle", new Vector2(-10.25f, -10.93f), "assets/nebulaassets/SpawnCandidates/Fungle/Jungle.png", null, 115f, useNewAssets: true));
        SpawnCandidates.Add(new SpawnCandidate("campfire", new Vector2(-9.72f, 1.39f), "assets/nebulaassets/SpawnCandidates/Fungle/Campfire.png", null, 115f, useNewAssets: true));
        SpawnCandidates.Add(new SpawnCandidate("beachCafeteria", new Vector2(-16.33f, 5.10f), "assets/nebulaassets/SpawnCandidates/Fungle/Cafeteria.png", null, 115f, useNewAssets: true));
        SpawnCandidates.Add(new SpawnCandidate("beachSplashzone", new Vector2(-22.00f, -0.84f), "assets/nebulaassets/SpawnCandidates/Fungle/SplashZone.png", null, 115f, useNewAssets: true));
        SpawnCandidates.Add(new SpawnCandidate("highlandsComm", new Vector2(22.87f, 13.55f), "assets/nebulaassets/SpawnCandidates/Fungle/Comms.png", null, 115f, useNewAssets: true));
        SpawnCandidates.Add(new SpawnCandidate("beachKicken", new Vector2(-22.96f, -7.154f), "assets/nebulaassets/SpawnCandidates/Fungle/Kicken.png", null, 115f, useNewAssets: true));
        SpawnCandidates.Add(new SpawnCandidate("beachDropship", new Vector2(-8.29f, 10f), "assets/nebulaassets/SpawnCandidates/Fungle/Dropship.png", null, 115f, useNewAssets: true));
        SpawnCandidates.Add(new SpawnCandidate("lookout", new Vector2(7.751f, 1.013f), "assets/nebulaassets/SpawnCandidates/Fungle/Lookout.png", null, 115f, useNewAssets: true));
        SpawnCandidates.Add(new SpawnCandidate("miningPit", new Vector2(11.314f, 9.349f), "assets/nebulaassets/SpawnCandidates/Fungle/MiningPit.png", null, 115f, useNewAssets: true));
        SpawnCandidates.Add(new SpawnCandidate("upperEngine", new Vector2(22.627f, 3.563f), "assets/nebulaassets/SpawnCandidates/Fungle/UpperEngine.png", null, 115f, useNewAssets: true));
        SpawnCandidates.Add(new SpawnCandidate("meetingRoom", new Vector2(-1.875f, -1.944f), "assets/nebulaassets/SpawnCandidates/Fungle/MeetingRoom.png", null, 115f, useNewAssets: true));
        SpawnCandidates.Add(new SpawnCandidate("storage", new Vector2(2.417f, 4.063f), "assets/nebulaassets/SpawnCandidates/Fungle/Storage.png", null, 115f, useNewAssets: true));

        // SpawnPoints
        SpawnPoints.Add(new SpawnPointData("jungleLabo", new Vector2(-5.42f, -9.20f)));
        SpawnPoints.Add(new SpawnPointData("jungleReactor", new Vector2(16.75f, -6.68f)));
        SpawnPoints.Add(new SpawnPointData("jungleGreenhouse", new Vector2(9.07f, -11.00f)));
        SpawnPoints.Add(new SpawnPointData("beachToJungle", new Vector2(-10.25f, -10.93f)));
        SpawnPoints.Add(new SpawnPointData("campfire", new Vector2(-9.72f, 1.39f)));
        SpawnPoints.Add(new SpawnPointData("beachCafeteria", new Vector2(-16.33f, 5.10f)));
        SpawnPoints.Add(new SpawnPointData("beachSplashzone", new Vector2(-22.00f, -0.84f)));
        SpawnPoints.Add(new SpawnPointData("highlandsComm", new Vector2(22.87f, 13.55f)));
        SpawnPoints.Add(new SpawnPointData("beachKicken", new Vector2(-22.96f, -7.154f)));
        SpawnPoints.Add(new SpawnPointData("beachDropship", new Vector2(-8.29f, 10f)));
        SpawnPoints.Add(new SpawnPointData("lookout", new Vector2(7.751f, 1.013f)));
        SpawnPoints.Add(new SpawnPointData("miningPit", new Vector2(11.314f, 9.349f)));
        SpawnPoints.Add(new SpawnPointData("upperEngine", new Vector2(22.627f, 3.563f)));
        SpawnPoints.Add(new SpawnPointData("meetingRoom", new Vector2(-1.875f, -1.944f)));
        SpawnPoints.Add(new SpawnPointData("storage", new Vector2(2.417f, 4.063f)));

        // 注册任务对象位置
        RegisterObjectPosition(new Vector2(-6.5f, -10.5f), SystemTypes.Laboratory, 40f);
        RegisterObjectPosition(new Vector2(14.0f, -15.0f), SystemTypes.Greenhouse, 35f);
        RegisterObjectPosition(new Vector2(22.5f, 11.5f), SystemTypes.Comms, 30f);

        // 设置仪式房间
        RitualRooms.Add(new SystemTypes[] { SystemTypes.Laboratory });
        RitualRooms.Add(new SystemTypes[] { SystemTypes.Greenhouse });
        RitualRooms.Add(new SystemTypes[] { SystemTypes.Comms, SystemTypes.Laboratory });

        // 设置管理员房间
        AdminRooms.Add(new PointData("comms", new Vector2(22.0f, 11.0f)));
        AdminSystemTypeMap.Add(SystemTypes.Comms, 1);

        AdminRooms.Add(new PointData("lab", new Vector2(-6.5f, -11.0f)));
        AdminSystemTypeMap.Add(SystemTypes.Laboratory, 2);

        AdminRooms.Add(new PointData("highlands", new Vector2(18.0f, 1.0f)));
        AdminSystemTypeMap.Add(SystemTypes.UpperEngine, 3);

        // 经典管理员掩码
        ClassicAdminMask = 0b1010;
    }
}