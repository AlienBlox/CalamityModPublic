using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ArkoftheElements : ModItem
    {
        public float Combo = 0f;
        public float Charge = 0f;

        public const float ComboLenght = 4f; //How many regular swings before the long throw happens
        public static float snapDamageMultiplier = 1.3f; //Extra damage from making the scissors snap
        public static float chargeDamageMultiplier = 1.6f; //Extra damage from charge

        const string ComboTooltip = "Performs a combo of swings, throwing the blade out every 5 swings. Releasing the mouse while the blade is out will throw the second half towards it, making the scissors snap\n" +
                "Snapping the scissors together increase their damage and empower your next two swings";

        const string ParryTooltip = "Using RMB will snip out the scissor blades in front of you. Hitting an enemy with it will parry them, granting you a small window of invulnerability\n" +
                "You can also parry projectiles and temporarily make them deal 200 less damage\n" +
                "Parrying will empower the next 10 swings of the sword, letting you use both blades at once\n" +
                "Using RMB and pressing up while the Ark is charged will release all the charges in a powerful burst of energy";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ark of the Elements");
            Tooltip.SetDefault("This line gets set in ModifyTooltips\n" +
                "This line also gets set in ModifyTooltips\n" +
                "A heavenly pair of blades infused with the essence of Terraria, powerful enough to cut through the fabric of reality");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var comboTooltip = tooltips.FirstOrDefault(x => x.Name == "Tooltip0" && x.mod == "Terraria");
            comboTooltip.text = ComboTooltip;
            comboTooltip.overrideColor = Color.IndianRed;

            var parryTooltip = tooltips.FirstOrDefault(x => x.Name == "Tooltip1" && x.mod == "Terraria");
            parryTooltip.text = ParryTooltip;
            parryTooltip.overrideColor = Color.Coral;
        }

        public override void SetDefaults()
        {
            item.width = 112;
            item.height = 172;
            item.damage = 1305;
            item.melee = true;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useAnimation = 20;
            item.useTime = 20;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 8.5f;
            item.UseSound = null;
            item.autoReuse = true;
			item.value = CalamityGlobalItem.Rarity11BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 16f;
        }

		// Terraria seems to really dislike high crit values in SetDefaults
		public override void GetWeaponCrit(Player player, ref int crit) => crit += 10;

        public override bool AltFunctionUse(Player player) => true;

        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;

            if (CanUseItem(player) && Combo != 4)
                item.channel = false;

            if (Combo == 4)
                item.channel = true;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ProjectileType<ArkoftheElementsSwungBlade>());
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                if (Charge >= 0 && player.controlUp)
                {
                    float angle = new Vector2(speedX, speedY).ToRotation();
                    Projectile.NewProjectile(player.Center + angle.ToRotationVector2() * 90f, new Vector2(speedX, speedY), ProjectileType<ArkoftheElementsSnapBlast>(), (int)(damage * Charge * 1.8f), 0, player.whoAmI, angle, 600);

                    if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 3)
                        Main.LocalPlayer.Calamity().GeneralScreenShakePower = 3;

                    Charge = 0;
                }

                else if (!Main.projectile.Any(n => n.active && n.owner == player.whoAmI && (n.type == ProjectileType<ArkoftheAncientsParryHoldout>() || n.type == ProjectileType<TrueArkoftheAncientsParryHoldout>() || n.type == ProjectileType<ArkoftheElementsParryHoldout>())))
                    Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<ArkoftheElementsParryHoldout>(), damage, 0, player.whoAmI, 0, 0);

                return false;
            }

            if (Charge > 0)
                damage = (int)(chargeDamageMultiplier * damage);
            float scissorState = Combo == ComboLenght ? 2 : Combo % 2;

            Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<ArkoftheElementsSwungBlade>(), damage, knockBack, player.whoAmI, scissorState, Charge);
            
            Combo += 1;
            if (Combo > ComboLenght)
                Combo = 0;

            


            //Shoot projectiles every upwards swing
            if (scissorState == 1f)
            {
                Vector2 throwVector = new Vector2(speedX, speedY);
                float empoweredNeedles = Charge > 0 ? 1f : 0f;
                Projectile.NewProjectile(player.Center + Vector2.Normalize(throwVector) * 20, new Vector2(speedX, speedY) * 2.8f, ProjectileType<SolarNeedle>(), (int)(damage * 0.5f), knockBack, player.whoAmI, empoweredNeedles);


                Vector2 Shift = Vector2.Normalize(new Vector2(speedX, speedY).RotatedBy(MathHelper.PiOver2)) * 20;

                Projectile.NewProjectile(player.Center + Shift, throwVector.RotatedBy(MathHelper.PiOver4 * 0.3f), ProjectileType<ElementalGlassStar>(), (int)(damage * 0.2f), knockBack, player.whoAmI);
                Projectile.NewProjectile(player.Center + Shift * 1.2f, throwVector.RotatedBy(MathHelper.PiOver4 * 0.4f) * 0.8f, ProjectileType<ElementalGlassStar>(), (int)(damage * 0.2f), knockBack, player.whoAmI);


                Projectile.NewProjectile(player.Center - Shift, throwVector.RotatedBy(-MathHelper.PiOver4 * 0.3f), ProjectileType<ElementalGlassStar>(), (int)(damage * 0.2f), knockBack, player.whoAmI);
                Projectile.NewProjectile(player.Center - Shift * 1.2f, throwVector.RotatedBy(-MathHelper.PiOver4 * 0.4f) * 0.8f, ProjectileType<ElementalGlassStar>(), (int)(damage * 0.2f), knockBack, player.whoAmI);
            }

            Charge--;
            if (Charge < 0)
                Charge = 0;

            return false;
        }

        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);

            (clone as ArkoftheElements).Charge = (item.modItem as ArkoftheElements).Charge;

            return clone;
        }
        public override ModItem Clone()
        {
            var clone = base.Clone();

            (clone as ArkoftheElements).Charge = Charge;

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
            recipe.AddIngredient(ItemType<TrueArkoftheAncients>());
            recipe.AddIngredient(ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ItemType<CoreofCalamity>(), 5);
            recipe.AddIngredient(ItemType<BarofLife>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D frontTexture = GetTexture("CalamityMod/Items/Weapons/Melee/ArkoftheElements");
            Texture2D backTexture = GetTexture("CalamityMod/Items/Weapons/Melee/ArkoftheElementsBack");

            float backLayerOpacity = (Charge > 0) ? 1f : (float)Math.Sin(Main.GlobalTime * 0.9f) * 0.2f + 0.3f;

            spriteBatch.Draw(backTexture, position, null, drawColor * backLayerOpacity, 0f, origin, scale, SpriteEffects.None, 0f); //Make the back scissor slightly transparent if the ark isnt charged
            spriteBatch.Draw(frontTexture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D frontTexture = GetTexture("CalamityMod/Items/Weapons/Melee/ArkoftheElements");
            Texture2D backTexture = GetTexture("CalamityMod/Items/Weapons/Melee/ArkoftheElementsBack");

            spriteBatch.Draw(backTexture, item.Center - Main.screenPosition, null, lightColor, rotation, item.Size * 0.5f, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(frontTexture, item.Center - Main.screenPosition, null, lightColor, rotation, item.Size * 0.5f, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Charge <= 0)
                return;

            var barBG = GetTexture("CalamityMod/ExtraTextures/GenericBarBack");
            var barFG = GetTexture("CalamityMod/ExtraTextures/GenericBarFront");

            float barScale = 3.5f;

            Vector2 drawPos = position + Vector2.UnitY * frame.Height * scale + Vector2.UnitX * (frame.Width - barBG.Width * barScale) * scale * 0.5f;
            Rectangle frameCrop = new Rectangle(0, 0, (int)(Charge / 10f * barFG.Width), barFG.Height);
            Color color = Main.hslToRgb(((float)Math.Sin(Main.GlobalTime * 0.6f) * 0.5f + 0.5f) * 0.15f, 1, 0.85f + (float)Math.Sin(Main.GlobalTime * 3f) * 0.1f);

            spriteBatch.Draw(barBG, drawPos, null, color, 0f, origin, scale * barScale, 0f, 0f);
            spriteBatch.Draw(barFG, drawPos, frameCrop, color * 0.8f, 0f, origin, scale * barScale, 0f, 0f);
        }
    }
}
