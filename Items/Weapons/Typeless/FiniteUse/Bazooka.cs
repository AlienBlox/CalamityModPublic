using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Typeless.FiniteUse;
using CalamityMod.Items.Ammo.FiniteUse;

namespace CalamityMod.Items.Weapons.Typeless.FiniteUse
{
    public class Bazooka : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bazooka");
            Tooltip.SetDefault("Uses Grenade Shells\n" +
                "Does more damage to inorganic enemies\n" +
                "Can be used twice per boss battle");
        }

        public override void SetDefaults()
        {
            item.damage = 500;
            item.width = 66;
            item.height = 26;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 10f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/BazookaFull");
            item.autoReuse = true;
            item.shootSpeed = 12f;
            item.shoot = ModContent.ProjectileType<GrenadeRound>();
            item.useAmmo = ModContent.ItemType<GrenadeRounds>();
            if (CalamityPlayer.areThereAnyDamnBosses)
            {
                item.Calamity().timesUsed = 2;
            }
        }

        public override bool OnPickup(Player player)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
            {
                item.Calamity().timesUsed = 2;
            }
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            return item.Calamity().timesUsed < 2;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override void UpdateInventory(Player player)
        {
            if (!CalamityPlayer.areThereAnyDamnBosses)
            {
                item.Calamity().timesUsed = 0;
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
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
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.IronBar, 20);
            recipe.anyIronBar = true;
            recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddIngredient(ItemID.AdamantiteBar, 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.IronBar, 20);
            recipe.anyIronBar = true;
            recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddIngredient(ItemID.TitaniumBar, 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
