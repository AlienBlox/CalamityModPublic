﻿using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    class ColdDivinity : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cold Divinity");
            Tooltip.SetDefault("Legendary Drop\n" +
                "Summons the power of the ancient ice castle\n" +
                               "For each minion slot used, you will gain an additional orbiting shield spike\n" +
                               "These spikes accelerate rapidly towards a nearby enemy to inflict heavy damage\n" +
                               "They take some time to regenerate after launching themselves at the target, however\n" +
                               "On right click, summons a duplicate ring around the targeted enemy, which slowly converges before exploding\n" +
                               "Revengeance Drop");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 169;
            item.mana = 20;
            item.width = 52;
            item.height = 50;
            item.useTime = item.useAnimation = 25;
            item.useStyle = 4;
            item.noMelee = true;
            item.knockBack = 4.5f;
            item.value = Item.buyPrice(0, 48, 0, 0);
            item.rare = 5;
            item.Calamity().customRarity = CalamityRarity.ItemSpecific;
            item.UseSound = SoundID.Item30;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ColdDivinityPointyThing>();
            item.shootSpeed = 10f;
            item.summon = true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            damage = (int)(damage * player.MinionDamage());
            float totalMinionSlots = 0f;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].minion && Main.projectile[i].owner == player.whoAmI)
                {
                    totalMinionSlots += Main.projectile[i].minionSlots;
                }
            }
            if (player.altFunctionUse != 2 && totalMinionSlots < player.maxMinions)
            {
                player.AddBuff(ModContent.BuffType<ColdDivinityBuff>(), 120, true);
                position = Main.MouseWorld;
                Projectile.NewProjectile(position, Vector2.Zero, type, damage, knockBack, player.whoAmI);
                int pointyThingCount = 0;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == type && Main.projectile[i].owner == player.whoAmI)
                    {
                        if (!(Main.projectile[i].modProjectile as ColdDivinityPointyThing).circlingPlayer)
                            continue;
                        pointyThingCount++;
                    }
                }
                float angleVariance = MathHelper.TwoPi / pointyThingCount;
                float angle = 0f;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == type && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].ai[1] == 0f)
                    {
                        if (!(Main.projectile[i].modProjectile as ColdDivinityPointyThing).circlingPlayer)
                            continue;
                        Main.projectile[i].ai[0] = angle;
                        Main.projectile[i].netUpdate = true;
                        angle += angleVariance;
                        for (int j = 0; j < 22; j++)
                        {
                            Dust dust = Dust.NewDustDirect(Main.projectile[i].position, Main.projectile[i].width, Main.projectile[i].height, DustID.Ice);
                            dust.velocity = Vector2.UnitY * Main.rand.NextFloat(3f, 5.5f) * Main.rand.NextBool(2).ToDirectionInt();
                            dust.noGravity = true;
                        }
                    }
                }
            }
            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return base.AltFunctionUse(player);
        }
    }

}
