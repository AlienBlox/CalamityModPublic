using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class PlasmaGrenade : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Grenade");
            Tooltip.SetDefault("Throws a grenade that explodes into plasma on collision\n" +
                               "Stealth strikes explode violently on collision into a vaporizing blast");
        }

        public override void SafeSetDefaults()
        {
            item.width = 22;
            item.height = 28;
            item.damage = 5000;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.consumable = true;
            item.maxStack = 999;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = item.useTime = 22;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;

            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.RareVariant;

            item.shoot = ModContent.ProjectileType<PlasmaGrenadeProjectile>();
            item.shootSpeed = 19f;
            item.Calamity().rogue = true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile grenade = Projectile.NewProjectileDirect(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            grenade.Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 1);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 1);
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 1);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this, 800);
            recipe.AddRecipe();
        }
    }
}
