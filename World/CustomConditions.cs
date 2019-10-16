﻿
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.World.Generation;

namespace CalamityMod.World
{
    public static class CustomConditions
    {
        public class RandomChance : GenCondition
        {
            private float oneInThisValue;

            public RandomChance(float chance)
            {
                oneInThisValue = chance;
            }

            protected override bool CheckValidity(int x, int y)
            {
                return _random.NextFloat(oneInThisValue) <= 1f;
            }
        }
        public class IsNotTouchingAir : GenCondition
        {
            private bool _useDiagonals;
            public IsNotTouchingAir(bool diagonals)
            {
                _useDiagonals = diagonals;
            }
            protected override bool CheckValidity(int x, int y)
            {
                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = y - 1; j <= y + 1; j++)
                    {
                        if (j != y && i != x && !_useDiagonals)
                            continue;
                        if (!WorldGen.InWorld(i, j))
                            continue;
                        if (!_tiles[i, j].active())
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
    }
}
