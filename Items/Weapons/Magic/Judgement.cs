using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Judgement : ModItem
    {
        public const int HitsPerFlash = 300;
        public const int FlashBaseDamage = 80000;
        
        private const float SpawnAngleSpread = 0.4f * MathHelper.Pi;
        private const float SpeedRandomness = 0.08f;
        private const float Inaccuracy = 0.04f;
        private const float MinSpawnDist = 40;
        private const float MaxSpawnDist = 140;

        public static Color GetLightColor(float deviation) => new Color(1f, 0.5f + 0.35f * MathHelper.Clamp(deviation, 0f, 1f), 1f);
        public static Color GetSyncedLightColor() => GetLightColor(Main.DiscoG / 255f);
        public static Color GetRandomLightColor() => GetLightColor(Main.rand.NextFloat());

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Dance of Light");
            Tooltip.SetDefault("Barrages enemies with a hailstorm of Light Blades\n'And in a flash of light, nothing remains'");
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 42;
            item.damage = 2077;
            item.knockBack = 4f;
            item.magic = true;
            item.mana = 6;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 2;
            item.useAnimation = 8;
            item.reuseDelay = 5;
            item.UseSound = SoundID.Item105;
            item.autoReuse = true;
            item.noMelee = true;

            item.shoot = ModContent.ProjectileType<LightBlade>();
            item.shootSpeed = 14f;

            item.value = CalamityGlobalItem.Rarity16BuyPrice;
            item.Calamity().customRarity = CalamityRarity.HotPink;
            item.Calamity().devItem = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 20;

        public override Vector2? HoldoutOffset() => Vector2.Zero;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile[] pair = new Projectile[2];
            for(int i = 0; i < 2; ++i)
            {
                // Pick a random direction somewhere behind the player
                float shootAngle = (float)Math.Atan2(speedY, speedX);
                Vector2 offset = Main.rand.NextVector2Unit(MathHelper.Pi - SpawnAngleSpread, 2f * SpawnAngleSpread).RotatedBy(shootAngle);
                // Set how far away this sword is spawning
                offset *= Main.rand.NextFloat(MinSpawnDist, MaxSpawnDist);

                Vector2 spawnPos = position + offset;
                float randSpeed = Main.rand.NextFloat(1f - SpeedRandomness, 1f + SpeedRandomness);
                float randAngle = Main.rand.NextFloat(-Inaccuracy, Inaccuracy);
                Vector2 velocity = randSpeed * new Vector2(speedX, speedY).RotatedBy(randAngle);
                Projectile p = Projectile.NewProjectileDirect(spawnPos, velocity, type, damage, knockBack, player.whoAmI);
                pair[i] = p;
            }

            // Pair the swords up so they home in on each other
            pair[0].ai[1] = pair[1].whoAmI + 1f;
            pair[1].ai[1] = pair[0].whoAmI + 1f;
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SkyFracture);
            recipe.AddIngredient(ItemID.LunarFlareBook);
            recipe.AddIngredient(ModContent.ItemType<WrathoftheAncients>());
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
