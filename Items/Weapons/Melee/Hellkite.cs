﻿using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Hellkite : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/WulfrumScrewdriverScrewHit") { Volume = 0.6f };
        public static float PitchSound = 1;
        public override void SetDefaults()
        {
            Item.width = 84;
            Item.height = 84;
            Item.scale = 1f;
            Item.damage = 180;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (!Main.zenithWorld)
                if (Main.rand.NextBool(4))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 174);
        }

        // NOTE: Modded item variants don't seem to be supported as of now, so this is probably temporary (also there is no way around modifiers screwing rarity/value in some way)
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage) => damage += (Main.zenithWorld ? (1369f / 180f - 1f) : 0f);
        public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback) => knockback += (Main.zenithWorld ? 1.5f : 0f);
        public override float UseSpeedMultiplier(Player player) => Main.zenithWorld ? 2.5f : 1f;
        public override void ModifyItemScale(Player player, ref float scale) => scale *= (Main.zenithWorld ? 2.5f : 1f);

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.zenithWorld)
            {
                SoundEngine.PlaySound(UseSound with { Pitch = PitchSound * Main.rand.NextFloat(0.01f, 0.03f) });
            }
            else
            {
                target.AddBuff(BuffID.OnFire3, 300);
                int onHitDamage = player.CalcIntDamage<MeleeDamageClass>(Item.damage);
                player.ApplyDamageToNPC(target, onHitDamage, 0f, 0, false);
                float num50 = 1.7f;
                float num51 = 0.8f;
                float num52 = 2f;
                Vector2 value3 = (target.rotation - 1.57079637f).ToRotationVector2();
                Vector2 value4 = value3 * target.velocity.Length();
                SoundEngine.PlaySound(SoundID.Item14, target.Center);
                int num3;
                for (int num53 = 0; num53 < 40; num53 = num3 + 1)
                {
                    int num54 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 174, 0f, 0f, 200, default, num50);
                    Dust dust = Main.dust[num54];
                    dust.position = target.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
                    dust.noGravity = true;
                    dust.velocity.Y -= 6f;
                    dust.velocity *= 3f;
                    dust.velocity += value4 * Main.rand.NextFloat();
                    num54 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 174, 0f, 0f, 100, default, num51);
                    dust.position = target.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
                    dust.velocity.Y -= 6f;
                    dust.velocity *= 2f;
                    dust.noGravity = true;
                    dust.fadeIn = 1f;
                    dust.color = Color.Crimson * 0.5f;
                    dust.velocity += value4 * Main.rand.NextFloat();
                    num3 = num53;
                }
                for (int num55 = 0; num55 < 20; num55 = num3 + 1)
                {
                    int num56 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 174, 0f, 0f, 0, default, num52);
                    Dust dust = Main.dust[num56];
                    dust.position = target.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)target.velocity.ToRotation(), default) * (float)target.width / 3f;
                    dust.noGravity = true;
                    dust.velocity.Y -= 6f;
                    dust.velocity *= 0.5f;
                    dust.velocity += value4 * (0.6f + 0.6f * Main.rand.NextFloat());
                    num3 = num55;
                }
            }
        }

        public override void ModifyTooltips(List<TooltipLine> list) => list.FindAndReplace("[GFB]", this.GetLocalizedValue(Main.zenithWorld ? "TooltipGFB" : "TooltipNormal"));

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.OnFire3, 300);
            SoundEngine.PlaySound(SoundID.Item14, target.Center);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
            AddCondition(Condition.ZenithWorld).
            AddIngredient(ItemID.TitaniumSword).
            AddIngredient<NightmareFuel>(12).
            AddTile<CosmicAnvil>().
            Register();

            CreateRecipe().
            AddCondition(Condition.NotZenithWorld).
            AddIngredient(ItemID.FieryGreatsword).
            AddIngredient<PerennialBar>(8).
            AddTile(TileID.MythrilAnvil).
            Register();
        }
    }
}
