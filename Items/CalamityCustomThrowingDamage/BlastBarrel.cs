using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class BlastBarrel : CalamityDamageItem
    {
        public const int BaseDamage = 71;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast Barrel");
            Tooltip.SetDefault("Throws a rolling barrel that explodes on wall collision\n" +
                               "Stealth Strike Effect: Bounces three times instead of dying. Various effects occur the more it bounces");
        }

        public override void SafeSetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.damage = BaseDamage;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 22;
            item.useTime = 22;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 12, 0, 0); //2 gold 40 silver sellprice
            item.rare = 4;
            item.shoot = mod.ProjectileType("BlastBarrelProjectile");
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            speedY *= 0.7f; //since the barrel is heavy
            Vector2 initialVelocity = new Vector2(speedX, speedY);

            //unitY additive is do it doesn't exploe initially
            int p = Projectile.NewProjectile(position - Vector2.UnitY * 12f, initialVelocity, type, damage, knockBack, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable())
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }
}
