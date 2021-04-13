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
            Tooltip.SetDefault("Each grenade contains a heavily condensed and heated unit of plasma. Use with care.\n" +
                               "Throws a grenade that explodes into plasma on collision\n" +
                               "Stealth strikes explode violently on collision into a vaporizing blast\n" +
                               "Deals more damage against enemies with high defenses");
        }

        public override void SafeSetDefaults()
        {
            CalamityGlobalItem modItem = item.Calamity();

            item.width = 22;
            item.height = 28;
            item.damage = 1312;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.consumable = true;
            item.maxStack = 999;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = item.useTime = 27;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;

            item.value = Item.sellPrice(silver: 10);
            item.rare = ItemRarityID.Purple;
            modItem.customRarity = CalamityRarity.DraedonRust;

            item.shoot = ModContent.ProjectileType<PlasmaGrenadeProjectile>();
            item.shootSpeed = 14f;
            modItem.rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
			if (proj.WithinBounds(Main.maxProjectiles))
				Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 5);
			recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>());
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this, 999);
            recipe.AddRecipe();
        }
    }
}
