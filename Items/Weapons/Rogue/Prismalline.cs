using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Prismalline : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismalline");
            Tooltip.SetDefault("Throws daggers that split after a while\n" +
			"Stealth strikes additionally explode into prism shards and briefly stun enemies");
        }

        public override void SafeSetDefaults()
        {
            item.width = 46;
            item.damage = 18;
            item.crit += 4;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 14;
            item.useStyle = 1;
            item.useTime = 14;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 46;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.shoot = ModContent.ProjectileType<PrismallineProj>();
            item.shootSpeed = 16f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[proj].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Crystalline>());
            recipe.AddIngredient(ModContent.ItemType<EssenceofEleum>(), 5);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
