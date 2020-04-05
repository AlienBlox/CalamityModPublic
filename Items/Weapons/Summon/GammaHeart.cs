using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class GammaHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gamma Heart");
            Tooltip.SetDefault("Summons a frigid entity with a head\n" +
                               "If the entity already exists, using this item again will cause it to gain more heads");
        }

        public override void SetDefaults()
        {
            item.width = 58;
            item.height = 60;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.UseSound = SoundID.Item42;
            item.summon = true;
            item.mana = 16;
            item.damage = 120;
            item.knockBack = 3f;
            item.autoReuse = true;
            item.useTime = item.useAnimation = 15;
            item.shoot = ModContent.ProjectileType<GammaHead>();
            item.shootSpeed = 10f;
            item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                Projectile.NewProjectileDirect(player.Center, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}
