using Object = UnityEngine.Object;

namespace Nebula.Source.TrollTasks
{
    /// <summary>
    /// 任务恶搞管理器 - 提供静态工具方法和全局开关检查
    /// </summary>
    public static class TrollTaskManager
    {
        /// <summary>
        /// 检查任务恶搞是否启用
        /// </summary>
        public static bool IsEnabled
        {
            get
            {
                try
                {
                    return CustomOptionHolder.enableTaskTroll != null && CustomOptionHolder.enableTaskTroll.getBool();
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 生成一个随机颜色
        /// </summary>
        public static Color RandomColor()
        {
            return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1.0f);
        }

        /// <summary>
        /// 销毁接线任务中已生成的电线节点（不销毁模板对象）
        /// </summary>
        public static void DestroyWireNodes(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                GameObject childNode = parent.GetChild(i).gameObject;
                if (!childNode.name.Contains("WireNode")) continue;
                Object.Destroy(childNode);
            }
        }

        /// <summary>
        /// 复制接线节点并设置合适的Y位置
        /// </summary>
        public static GameObject BuildWireNode(GameObject prefab, ref float positionY, int totalWires)
        {
            positionY -= 4.6f / (totalWires + 1);
            GameObject newGameObject = Object.Instantiate(prefab, prefab.transform.parent);
            newGameObject.transform.localPosition = new Vector3(
                newGameObject.transform.localPosition.x, positionY,
                newGameObject.transform.localPosition.z);
            // 隐藏基础符号（原始模版中没有这个）
            Transform baseSymbol = newGameObject.transform.FindChild("BaseSymbol");
            if (baseSymbol) baseSymbol.gameObject.active = false;
            return newGameObject;
        }
    }
}
