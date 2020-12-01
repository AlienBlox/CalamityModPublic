using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
	public class MagicNebulaShot : BaseLaserbeamProjectile
	{
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

		private Color startingColor = new Color(119, 210, 255);
		private Color secondColor = new Color(247, 119, 255);
		public override float MaxScale => 0.7f;
		public override float MaxLaserLength => 1599.999999f;
		public override float Lifetime => 20f;
		public override Color LaserOverlayColor => CalamityUtils.ColorSwap(startingColor, secondColor, 0.9f);
		public override Color LightCastColor => LaserOverlayColor;
		public override Texture2D LaserBeginTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UltimaRayStart");
		public override Texture2D LaserMiddleTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UltimaRayMid");
		public override Texture2D LaserEndTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UltimaRayEnd");

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nebula Shot");
		}

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 22;
			projectile.friendly = true;
			projectile.magic = true;
			projectile.penetrate = -1;
			projectile.alpha = 255;
			projectile.localNPCHitCooldown = 2;
			projectile.usesLocalNPCImmunity = true;
		}

		public override bool PreAI()
		{
			// Initialization. Using the AI hook would override the base laser's code, and we don't want that.
			if (projectile.localAI[0] == 0f)
			{
				if (Main.rand.NextBool())
				{
					secondColor = new Color(119, 210, 255);
					startingColor = new Color(247, 119, 255);
				}
			}
			return true;
		}

		public override bool ShouldUpdatePosition() => false;

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.OnFire, 600);
			target.AddBuff(BuffID.Frostburn, 600);
		}
	}
}
