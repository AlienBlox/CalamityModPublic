﻿using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class WulfrumBattery : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public static readonly SoundStyle ExtraDropSound = new("CalamityMod/Sounds/Custom/WulfrumExtraDrop") { PitchVariance = 0.3f };

        public override void SetStaticDefaults()
        {
           
            ItemID.Sets.ExtractinatorMode[Item.type] = Item.type;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;

            //Needed for extractination
            Item.useStyle = ItemUseStyleID.HiddenAnimation;
            Item.useAnimation = 10;
            Item.useTime = 2;
            Item.consumable = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<SummonDamageClass>() += 0.07f;
            player.GetModPlayer<WulfrumBatteryPlayer>().battery = true;
        }

        //Scrappable for 3-6 wulfrum scrap or a 20% chance to get an energy core
        public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            resultType = ModContent.ItemType<WulfrumMetalScrap>();
            resultStack = Main.rand.Next(3, 6);

            if (Main.rand.NextFloat() > 0.8f)
            {
                resultStack = 1;
                resultType = ModContent.ItemType<EnergyCore>();
            }
        }
    }

    public class WulfrumBatteryPlayer : ModPlayer
    {
        public bool battery = false;
        public override void ResetEffects() => battery = false;
        public override void UpdateDead() => battery = false;
    }

    public class WulfrumBatteryGlobalProjectile : GlobalProjectile
    {
        public override void AI(Projectile projectile)
        {
            if (!projectile.npcProj && !projectile.trap && projectile.minion && !ProjectileID.Sets.MinionShot[projectile.type] && Main.player[projectile.owner].GetModPlayer<WulfrumBatteryPlayer>().battery)
            {
                float lightMult = 2f;
                if (Lighting.UpdateEveryFrame) //The light loks wayyy too bright in retro/trippy
                    lightMult *= 0.5f;

                Lighting.AddLight(projectile.Center, Color.DeepSkyBlue.ToVector3() * lightMult);



                if (Main.rand.NextBool(16))
                {
                    float size = (projectile.Hitbox.Size() / 2f).Length();

                    Dust zapDust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(1f, 1f) * size, 226, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(1f, 2.3f));
                    zapDust.noGravity = true;
                }
            }
        }
    }
}
