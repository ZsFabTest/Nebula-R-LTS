using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace Nebula.Source.TrollTasks
{
    /// <summary>
    /// 解锁歧管任务恶搞 - 所有按钮使用相同贴图
    /// </summary>
    internal static class UnlockManifoldTroll
    {
        private static Sprite _cachedSprite;

        private static Sprite GetManifoldSprite()
        {
            if (_cachedSprite) return _cachedSprite;

            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string streamPath = assembly.GetManifestResourceNames()
                    .FirstOrDefault(x => x.EndsWith("UnlockManifold.png"));

                if (streamPath == null) return null;

                Stream stream = assembly.GetManifestResourceStream(streamPath);
                byte[] textureBytes = new byte[stream.Length];
                stream.Read(textureBytes, 0, (int)stream.Length);
                stream.Close();

                Texture2D texture = new(2, 2, TextureFormat.ARGB32, false);
                Il2CppStructArray<byte> il2CPPArray = textureBytes;
                ImageConversion.LoadImage(texture, il2CPPArray);
                _cachedSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f), 100f);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"TrollTasks: Failed to load UnlockManifold sprite: {e.Message}");
            }
            return _cachedSprite;
        }

        [HarmonyPatch(typeof(UnlockManifoldsMinigame))]
        private static class UnlockManifoldsMinigamePatch
        {
            [HarmonyPatch(nameof(UnlockManifoldsMinigame.Begin))]
            [HarmonyPrefix]
            private static void BeginPrefix(UnlockManifoldsMinigame __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;
                Sprite sprite = GetManifoldSprite();
                if (!sprite) return;
                foreach (SpriteRenderer button in __instance.Buttons)
                    button.sprite = sprite;
            }
        }
    }
}
