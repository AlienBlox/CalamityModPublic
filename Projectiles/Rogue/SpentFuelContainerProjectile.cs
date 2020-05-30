using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace CalamityMod.Projectiles.Rogue
{
    public class SpentFuelContainerProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SpentFuelContainer";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spent Fuel Container");
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 200;
            projectile.tileCollide = true;
            projectile.alpha = 0;
            projectile.Calamity().rogue = true;
        }
        public override void AI()
        {
            projectile.ai[0] += 1f; //arbitrary timer
            if (projectile.ai[0] > 75f)
            {
                if (projectile.velocity.Y < 10f)
                {
                    projectile.velocity.Y += 0.15f;
                }
            }
            projectile.rotation += MathHelper.ToRadians(projectile.velocity.Length());
        }
        public override void Kill(int timeLeft)
        {
            //Dust
            for (int i = 0; i < 30; i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f));
                Dust.NewDust(projectile.Center, 1, 1, (int)CalamityDusts.SulfurousSeaAcid, dspeed.X, dspeed.Y, 0, default, 1.1f);
            }

            Main.PlaySound(SoundID.Item107, projectile.Bottom);
            Point result;
            if (WorldUtils.Find(projectile.Top.ToTileCoordinates(), Searches.Chain((GenSearch)new Searches.Down(80), (GenCondition)new Conditions.IsSolid()), out result))
            {
                int proj = Projectile.NewProjectile(result.ToVector2() * 16f, Vector2.Zero, ModContent.ProjectileType<SulphuricNukesplosion>(), projectile.damage, 2f, projectile.owner);
                Main.projectile[proj].Calamity().stealthStrike = projectile.Calamity().stealthStrike;
            }
        }
    }
}
