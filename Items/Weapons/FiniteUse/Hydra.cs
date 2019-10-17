using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class Hydra : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydra");
            Tooltip.SetDefault("Uses Explosive Shotgun Shells\n" +
                "Does more damage to everything\n" +
                "Can be used once per boss battle");
        }

        public override void SetDefaults()
        {
            item.damage = 120;
            item.width = 66;
            item.height = 30;
            item.useTime = 33;
            item.useAnimation = 33;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 10f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Hydra");
            item.autoReuse = true;
            item.shootSpeed = 12f;
            item.shoot = ModContent.ProjectileType<Projectiles.ExplosiveShellBullet>();
            item.useAmmo = ModContent.ItemType<ExplosiveShells>();
            if (CalamityPlayer.areThereAnyDamnBosses)
            {
                item.Calamity().timesUsed = 1;
            }
        }

        public override bool OnPickup(Player player)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
            {
                item.Calamity().timesUsed = 1;
            }
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            return item.Calamity().timesUsed < 1;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override void UpdateInventory(Player player)
        {
            if (!CalamityPlayer.areThereAnyDamnBosses)
            {
                item.Calamity().timesUsed = 0;
            }
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 15; ++index)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-65, 66) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-65, 66) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            }
            if (CalamityPlayer.areThereAnyDamnBosses)
            {
                for (int i = 0; i < 58; i++)
                {
                    if (player.inventory[i].type == item.type)
                    {
                        player.inventory[i].Calamity().timesUsed++;
                    }
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Shotgun);
            recipe.AddIngredient(ItemID.IronBar, 20);
            recipe.anyIronBar = true;
            recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddIngredient(ItemID.Ectoplasm, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
