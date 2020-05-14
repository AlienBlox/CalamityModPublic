using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class PrismaticBreaker : ModItem
    {
        private int alpha = 50;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismatic Breaker");
            Tooltip.SetDefault("Seems to belong to a certain magical girl. Radiates with intense cosmic energy.\n" +
                "Fire to charge for a powerful rainbow laser\n" +
				"Right click to instead swing the sword and fire rainbow colored waves\n" +
				"The sword is boosted by both melee and ranged damage");
			Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 400;
            item.crit += 8;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.useTime = 15;
            item.useTurn = true;
            item.melee = true;
            item.knockBack = 7f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.width = 50;
            item.height = 50;
            item.shoot = ModContent.ProjectileType<PrismaticBeam>();
            item.shootSpeed = 14f;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Dedicated;
        }

		//Cancel out normal melee damage boosts and replace it with the average of melee and ranged damage boosts
		//all damage boosts should still apply
        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
			float damageMult = (player.meleeDamage + player.rangedDamage - 2f) / 2f;
            add += damageMult - player.meleeDamage + 1f;
		}

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
			//item.width and then item.height - 2f
            Vector2 origin = new Vector2(25f, 23f);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Items/Weapons/Melee/PrismaticBreakerGlow"), item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
				Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<PrismaticWave>(), damage * 2, knockBack, player.whoAmI, 0f, 0f);
            }
			else
			{
                Projectile.NewProjectile(position.X, position.Y, speedX * 0.5f, speedY * 0.5f, type, damage, knockBack, player.whoAmI, 0f, 0f);
			}
            return false;
        }

		public override bool AltFunctionUse(Player player) => true;

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				item.UseSound = SoundID.Item1;
				item.useStyle = 1;
				item.useTurn = true;
				item.autoReuse = true;
				item.noMelee = false;
				item.channel = false;
			}
			else
			{
				item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/CrystylCharge");
				item.useStyle = 5;
				item.useTurn = false;
				item.autoReuse = false;
				item.noMelee = true;
				item.channel = true;
			}
			return base.CanUseItem(player);
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
            {
				Color color = Utils.SelectRandom(Main.rand, new Color[]
				{
					new Color(255, 0, 0, alpha), //Red
					new Color(255, 128, 0, alpha), //Orange
					new Color(255, 255, 0, alpha), //Yellow
					new Color(128, 255, 0, alpha), //Lime
					new Color(0, 255, 0, alpha), //Green
					new Color(0, 255, 128, alpha), //Turquoise
					new Color(0, 255, 255, alpha), //Cyan
					new Color(0, 128, 255, alpha), //Light Blue
					new Color(0, 0, 255, alpha), //Blue
					new Color(128, 0, 255, alpha), //Purple
					new Color(255, 0, 255, alpha), //Fuschia
					new Color(255, 0, 128, alpha) //Hot Pink
				});
                Dust rainbow = Main.dust[Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 267, 0f, 0f, alpha, color, 0.8f)];
                rainbow.noGravity = true;
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
            target.AddBuff(BuffID.CursedInferno, 120);
            target.AddBuff(BuffID.Frostburn, 120);
            target.AddBuff(BuffID.OnFire, 120);
            target.AddBuff(BuffID.Ichor, 120);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
            target.AddBuff(BuffID.CursedInferno, 120);
            target.AddBuff(BuffID.Frostburn, 120);
            target.AddBuff(BuffID.OnFire, 120);
            target.AddBuff(BuffID.Ichor, 120);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmicRainbow>());
            recipe.AddIngredient(ModContent.ItemType<SolsticeClaymore>());
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 3);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
