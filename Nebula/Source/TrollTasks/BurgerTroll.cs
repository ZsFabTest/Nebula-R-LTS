using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nebula.Source.TrollTasks
{
    /// <summary>
    /// 做汉堡任务恶搞 - 奇葩的配料组合
    /// </summary>
    internal static class BurgerTroll
    {
        public static T GetRandom<T>(this T[] list)
        {
            var indexData = Random.Range(0, list.Length);
            return list[indexData];
        }

        [HarmonyPatch(typeof(BurgerMinigame))]
        private static class BurgerMinigamePatch
        {
            [HarmonyPatch(nameof(BurgerMinigame.Begin))]
            [HarmonyPostfix]
            private static void BeginPostfix(BurgerMinigame __instance)
            {
                if (!TrollTaskManager.IsEnabled) return;

                switch (Random.RandomRange(0f, 1f))
                {
                    case <= 0.50f: // 50% 随机混合
                        __instance.ExpectedToppings = new(6);
                        __instance.ExpectedToppings[0] = BurgerToppingTypes.Plate;
                        for (int i = 1; i < __instance.ExpectedToppings.Count; i++)
                        {
                            BurgerToppingTypes topping = (BurgerToppingTypes)IntRange.Next(0, 6);
                            bool set = __instance.ExpectedToppings.Count(t => t == topping) < topping switch
                            {
                                BurgerToppingTypes.TopBun => 1,
                                BurgerToppingTypes.BottomBun => 1,
                                BurgerToppingTypes.Lettuce => 3,
                                _ => 2
                            };
                            if (set) __instance.ExpectedToppings[i] = topping;
                            else i--;
                        }
                        break;
                    case <= 0.70f: // 20% 肉饼当面包
                        __instance.ExpectedToppings = new(6);
                        __instance.ExpectedToppings[0] = BurgerToppingTypes.Plate;
                        BurgerToppingTypes bun = (new BurgerToppingTypes[] { BurgerToppingTypes.Meat, BurgerToppingTypes.Onion, BurgerToppingTypes.Tomato }).GetRandom();
                        __instance.ExpectedToppings[1] = bun;
                        __instance.ExpectedToppings[5] = bun;
                        for (int i = 2; i < __instance.ExpectedToppings.Count - 1; i++)
                        {
                            BurgerToppingTypes topping = (BurgerToppingTypes)IntRange.Next(2, 6);
                            bool set = __instance.ExpectedToppings.Count(t => t == topping) < topping switch
                            {
                                BurgerToppingTypes.TopBun => 1,
                                BurgerToppingTypes.BottomBun => 1,
                                BurgerToppingTypes.Lettuce => 3,
                                _ => 2
                            };
                            if (set) __instance.ExpectedToppings[i] = topping;
                            else i--;
                        }
                        break;
                    case <= 0.90f: // 20% 生菜包裹
                        __instance.ExpectedToppings = new(6);
                        __instance.ExpectedToppings[0] = BurgerToppingTypes.Plate;
                        __instance.ExpectedToppings[1] = BurgerToppingTypes.Lettuce;
                        __instance.ExpectedToppings[5] = BurgerToppingTypes.Lettuce;
                        for (int i = 2; i < __instance.ExpectedToppings.Count - 1; i++)
                        {
                            BurgerToppingTypes topping = (new BurgerToppingTypes[] { BurgerToppingTypes.Lettuce, BurgerToppingTypes.Onion, BurgerToppingTypes.Tomato }).GetRandom();
                            bool set = __instance.ExpectedToppings.Count(t => t == topping) < topping switch
                            {
                                BurgerToppingTypes.TopBun => 1,
                                BurgerToppingTypes.BottomBun => 1,
                                BurgerToppingTypes.Lettuce => 3,
                                _ => 2
                            };
                            if (set) __instance.ExpectedToppings[i] = topping;
                            else i--;
                        }
                        break;
                    case <= 0.95f: // 5% 只有面包
                        __instance.ExpectedToppings = new(3);
                        __instance.ExpectedToppings[0] = BurgerToppingTypes.Plate;
                        __instance.ExpectedToppings[1] = BurgerToppingTypes.BottomBun;
                        __instance.ExpectedToppings[2] = BurgerToppingTypes.TopBun;
                        break;
                    case <= 1.00f: // 5% 接近正常但有1%概率最后两个交换
                        __instance.ExpectedToppings = new(6);
                        __instance.ExpectedToppings[0] = BurgerToppingTypes.Plate;
                        if (BoolRange.Next(0.1f))
                        {
                            __instance.ExpectedToppings[1] = BurgerToppingTypes.Lettuce;
                            __instance.ExpectedToppings[5] = BurgerToppingTypes.Lettuce;
                        }
                        else
                        {
                            __instance.ExpectedToppings[1] = BurgerToppingTypes.BottomBun;
                            __instance.ExpectedToppings[5] = BurgerToppingTypes.TopBun;
                        }
                        __instance.ExpectedToppings[2] = BurgerToppingTypes.Meat;
                        __instance.ExpectedToppings[3] = BurgerToppingTypes.Onion;
                        __instance.ExpectedToppings[4] = BurgerToppingTypes.Tomato;
                        if (BoolRange.Next(0.01f))
                        {
                            var temp = __instance.ExpectedToppings[3];
                            __instance.ExpectedToppings[3] = __instance.ExpectedToppings[4];
                            __instance.ExpectedToppings[4] = temp;
                        }
                        break;
                }
            }
        }
    }
}
