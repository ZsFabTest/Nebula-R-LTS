using UnityEngine;

namespace Nebula.Source.TrollTasks
{
    /// <summary>
    /// 任务恶搞游戏数据管理器 - 驱动需要每帧更新的功能
    /// </summary>
    public class TrollGameData : MonoBehaviour
    {
        public TrollGameData(System.IntPtr ptr) : base(ptr) { }

        public void Start()
        {
        }

        public void Update()
        {
            if (!TrollTaskManager.IsEnabled) return;
            VentAutoTroll.Update();
        }

        public void LateUpdate()
        {
            if (!TrollTaskManager.IsEnabled) return;
            LateTask.Update(Time.fixedDeltaTime / 2);
        }
    }
}
