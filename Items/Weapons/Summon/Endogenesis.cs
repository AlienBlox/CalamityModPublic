﻿using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class Endogenesis : ModItem
    {
        //Cooper be like cool

        public static int AttackMode = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Endogenesis");
            Tooltip.SetDefault("Summons an ascended ice construct to protect you \n" +
                               "Changes attack modes by resummoning or reusing the staff \n" +
                               "The first mode makes it shoot sweeping lasers aimed at the enemy \n" +
                               "The second mode sacrifices its limbs to shoot out homing projectiles \n" +
                               "The third mode allows it to agressively tackle its enemies \n" +
                               "The fourth mode makes the limbs function as endothermic flamethrowers \n" +
                               "Requires 10 minion slots to be summoned \n" +
                               "There can only be one \n" +
                               "[c/B0FBFF:Ice puns not included]"); //Icy no problems with that
        }

        public override void SetDefaults()
        {
            item.width = 80;
            item.height = 80;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.UseSound = SoundID.Item78;

            item.summon = true;
            item.mana = 80;
            item.damage = 7000;
            item.knockBack = 4f;
            item.crit += 18;
            item.autoReuse = true;
            item.useTime = 36;
            item.useAnimation = 36;
            item.shoot = ModContent.ProjectileType<EndoCooperBody>();
            item.shootSpeed = 10f;

            item.value = Item.buyPrice(5, 0, 0, 0);
            item.Calamity().postMoonLordRarity = 16;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.maxMinions < 10f)
                return false;
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                player.itemTime = item.useTime;
                Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
                vector2.X = Main.mouseX + Main.screenPosition.X;
                vector2.Y = Main.mouseY + Main.screenPosition.Y;
                for (int x = 0; x < Main.projectile.Length; x++)
                {
                    Projectile projectile = Main.projectile[x];
                    if (projectile.active && projectile.owner == player.whoAmI && (projectile.type == ModContent.ProjectileType<EndoCooperBody>() || projectile.type == ModContent.ProjectileType<EndoCooperLimbs>() || projectile.type == ModContent.ProjectileType<EndoBeam>()))
                    {
                        projectile.Kill();
                    }
                }
                float dmgMult = 1f;
				if (AttackMode == 0) //lasers
					dmgMult = 0.65f;
				if (AttackMode == 1) //icicles
					dmgMult = 1f;
				if (AttackMode == 2) //melee
					dmgMult = 0.95f;
				if (AttackMode == 3) //flamethrower
					dmgMult = 0.9f;
                int body = Projectile.NewProjectile(vector2.X, vector2.Y, 0f, 0f, type, (int)(damage * dmgMult), knockBack, player.whoAmI, AttackMode, 0f);
                int limbs = Projectile.NewProjectile(vector2.X, vector2.Y, 0f, 0f, ModContent.ProjectileType<EndoCooperLimbs>(), (int)(damage * dmgMult), knockBack, player.whoAmI, AttackMode, body);
                Main.projectile[body].ai[1] = limbs;
                AttackMode++;
                if (AttackMode > 3)
                    AttackMode = 0;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CryogenicStaff>());
            recipe.AddIngredient(ItemID.BlizzardStaff);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 99);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CoreofEleum>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
