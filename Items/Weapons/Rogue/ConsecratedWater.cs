using Microsoft.Xna.Framework;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class ConsecratedWater : RogueWeapon
    {
        public const int BaseDamage = 55;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Consecrated Water");
            Tooltip.SetDefault("Throws a holy flask of water that explodes into a sacred flame pillar on death\n" +
                               "The pillar is destroyed if there's no tiles below it\n" +
                               "Stealth Strike Effect: Creates three flame pillars instead of one on death");
        }

        public override void SafeSetDefaults()
        {
            item.damage = BaseDamage;
            item.width = 22;
            item.height = 24;
            item.useAnimation = 29;
            item.useTime = 29;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4.5f;
            item.rare = 6;
            item.UseSound = SoundID.Item106;
            item.autoReuse = true;
            item.value = Item.buyPrice(gold: 48); //sell price of 9 gold 60 silver
            item.shoot = ModContent.ProjectileType<ConsecratedWaterProjectile>();
            item.shootSpeed = 15f;
            item.Calamity().rogue = true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);
            float strikeValue = player.Calamity().StealthStrikeAvailable().ToInt(); //0 if false, 1 if true
            int p = Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<ConsecratedWaterProjectile>(), damage, knockBack, player.whoAmI, ai1: strikeValue);
            if (player.Calamity().StealthStrikeAvailable())
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HolyWater, 100);
            recipe.AddIngredient(ItemID.HallowedBar, 5);
            recipe.AddIngredient(ItemID.CrystalShard, 20);
            recipe.AddIngredient(ItemID.SoulofLight, 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
