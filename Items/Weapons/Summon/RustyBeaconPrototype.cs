using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class RustyBeaconPrototype : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rusty Beacon Prototype");
            Tooltip.SetDefault("Summons a long-abandoned drone to support you\n" +
                               "Clicking on an enemy gives them a tiny prick, causing them to become aggravated\n" +
                               "The drone hovers above nearby enemies and gives them various debuffs");
        }

        public override void SetDefaults()
        {
            item.mana = 10;
            item.width = 28;
            item.height = 20;
            item.useTime = item.useAnimation = 35;
            item.useStyle = 4;
            item.noMelee = true;
            item.knockBack = 0.5f;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
            item.rare = 1;
            item.UseSound = SoundID.Item15; // Phaseblade sound effect
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<RustyDrone>();
            item.shootSpeed = 10f;
            item.summon = true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI, 0f, 1f);
            }
            return false;
        }
    }
}
