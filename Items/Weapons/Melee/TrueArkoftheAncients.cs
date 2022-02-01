using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TrueArkoftheAncients : ModItem
    {
        public float Combo = 1f;
        public float Charge = 0f;
        public override bool CloneNewInstances => true;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Ark of the Ancients");
            Tooltip.SetDefault("Using RMB will extend the Ark out in front of you. Hitting an enemy with it will parry them, granting you a small window of invulnerability\n" +
                "You can also parry projectiles and temporarily make them deal 160 less damage\n" +
                "Parrying will empower the next 10 swings of the sword, boosting their damage and letting them throw stronger projectiles\n" +
                "Using RMB and pressing up while the Ark is charged will release all the charges in a powerful burst of energy\n" +
                "A heavenly blade forged to vanquish all evil");
        }

        public override void SetDefaults()
        {
            item.width = item.height = 72;
            item.damage = 148;
            item.melee = true;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useAnimation = 25;
            item.useTime = 25;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 6.5f;
            item.UseSound = null;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 12f;
        }
        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ProjectileType<TrueArkoftheAncientsSwungBlade>());
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                if (Charge > 0 && player.controlUp)
                {
                    float angle = new Vector2(speedX, speedY).ToRotation();
                    Projectile.NewProjectile(player.Center + angle.ToRotationVector2() * 90f, Vector2.Zero, ProjectileType<TrueAncientBlast>(), (int)(damage * Charge * 1.8f), 0, player.whoAmI, angle, 600);

                    if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 3)
                        Main.LocalPlayer.Calamity().GeneralScreenShakePower = 3;

                    Charge = 0;
                }

                else if (!Main.projectile.Any(n => n.active && n.owner == player.whoAmI && (n.type == ProjectileType<ArkoftheAncientsParryHoldout>() || n.type == ProjectileType<TrueArkoftheAncientsParryHoldout>())))
                    Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<TrueArkoftheAncientsParryHoldout>(), damage, 0, player.whoAmI, 0, 0);

                return false;
            }

            //Failsafe
            if (Combo != -1 && Combo != 1)
                Combo = 1;

            if (Charge > 0)
                damage *= 2;
            Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<TrueArkoftheAncientsSwungBlade>(), damage, knockBack, player.whoAmI, Combo, Charge);
            Combo *= -1f;

            //Shoot an extra star projectile every upwards swing
            if (Combo == -1f)
            {
                //Only shoot the center star if theres no charge
                if (Charge == 0)
                    Projectile.NewProjectile(player.Center + Vector2.Normalize(new Vector2(speedX, speedY)) * 20, new Vector2(speedX, speedY), ProjectileType<AncientStar>(), (int)(damage * 0.1f), knockBack, player.whoAmI);

                Vector2 Shift = Vector2.Normalize(new Vector2(speedX, speedY).RotatedBy(MathHelper.PiOver2)) * 30;

                Projectile.NewProjectile(player.Center + Shift, new Vector2(speedX, speedY).RotatedBy(MathHelper.PiOver4 * 0.3f) , ProjectileType<AncientStar>(), (int)(damage * 0.1f), knockBack, player.whoAmI, Charge > 0 ? 1 : 0);
                Projectile.NewProjectile(player.Center - Shift, new Vector2(speedX, speedY).RotatedBy(-MathHelper.PiOver4 * 0.3f), ProjectileType<AncientStar>(), (int)(damage * 0.1f), knockBack, player.whoAmI, Charge > 0 ? 1 : 0);
            }


            Charge--;
            if (Charge < 0)
                Charge = 0;

            return false;
        }

        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);

            (clone as TrueArkoftheAncients).Charge = (item.modItem as TrueArkoftheAncients).Charge;

            return clone;
        }
        public override ModItem Clone()
        {
            var clone = base.Clone();

            (clone as TrueArkoftheAncients).Charge = Charge;

            return clone;
        }


        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(Charge);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            Charge = reader.ReadInt32();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<ArkoftheAncients>());
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddIngredient(ItemID.Excalibur);
            recipe.AddIngredient(ItemType<CoreofCalamity>());
            recipe.AddIngredient(ItemType<LivingShard>(), 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<ArkoftheAncients>());
            recipe.AddIngredient(ItemID.TrueExcalibur);
            recipe.AddIngredient(ItemType<CoreofCalamity>());
            recipe.AddIngredient(ItemType<LivingShard>(), 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Charge <= 0)
                return;

            var barBG = GetTexture("CalamityMod/ExtraTextures/GenericBarBack");
            var barFG = GetTexture("CalamityMod/ExtraTextures/GenericBarFront");

            Vector2 drawPos = position + Vector2.UnitY * frame.Height * scale + Vector2.UnitX * (frame.Width - barBG.Width) * scale * 0.5f;
            Rectangle frameCrop = new Rectangle(0, 0, (int)(Charge / 10f * barFG.Width), barFG.Height);
            Color color = Main.hslToRgb((Main.GlobalTime * 0.6f) % 1, 1, 0.85f + (float)Math.Sin(Main.GlobalTime * 3f) * 0.1f);

            spriteBatch.Draw(barBG, drawPos, null, color, 0f, origin, scale, 0f, 0f);
            spriteBatch.Draw(barFG, drawPos, frameCrop, color * 0.8f, 0f, origin, scale, 0f, 0f);
        }
    }
}
