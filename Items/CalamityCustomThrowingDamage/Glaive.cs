using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class Glaive : CalamityDamageItem
    {
        public static int BaseDamage = 23;
        public static float Knockback = 3f;
        public static float Speed = 10f;
        public static float StealthSpeedMult = 1.8f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glaive");
            Tooltip.SetDefault(@"Stacks up to 3
Stealth strikes are super fast and pierce infinitely");
        }

        public override void SafeSetDefaults()
        {
            item.damage = BaseDamage;
            item.crit = 4;
            item.Calamity().rogue = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.width = 1;
            item.height = 1;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.knockBack = Knockback;
            item.value = Item.buyPrice(0, 3, 0, 0);
            item.rare = 3;
            item.UseSound = SoundID.Item1;
            item.maxStack = 3;

            item.shootSpeed = Speed;
            item.shoot = mod.ProjectileType("GlaiveProj");
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float ai1 = 0f;
            if (player.Calamity().StealthStrikeAvailable())
            {
                speedX *= StealthSpeedMult;
                speedY *= StealthSpeedMult;
                ai1 = 1f;
            }

            int p = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, ai1);
            if (player.Calamity().StealthStrikeAvailable())
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[item.shoot] < item.stack;
        }

    }
}
