using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class SulphuricAcidCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphuric Acid Cannon");
            Tooltip.SetDefault("Fires an acidic bubble that sticks to enemies and emits sulphuric gas");
        }

        public override void SetDefaults()
        {
            item.damage = 220;
            item.ranged = true;
            item.width = 90;
            item.height = 30;
            item.useTime = 18;
            item.useAnimation = 18;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 6f;
            item.autoReuse = true;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item95;
            item.shoot = ModContent.ProjectileType<SulphuricAcidBubble2>();
            item.shootSpeed = 16f;
            item.Calamity().postMoonLordRarity = 13;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }
    }
}
