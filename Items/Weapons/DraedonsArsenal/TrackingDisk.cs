using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class TrackingDisk : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tracking Disk");
            Tooltip.SetDefault("Releases a flying disk that fires lasers at nearby enemies\n" +
                               "Stealth Strikes allow the disk to fire multiple larger lasers at different targets.");
        }
        public override void SafeSetDefaults()
        {
            item.damage = 25;
            item.Calamity().rogue = true;

            item.width = 30;
            item.height = 34;
            item.useTime = 42;
            item.useAnimation = 42;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = false;
            item.knockBack = 3f;

            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DraedonRust;

            item.noUseGraphic = true;

            item.UseSound = SoundID.Item1;
            item.autoReuse = true;

            item.shoot = ModContent.ProjectileType<TrackingDiskProjectile>();
            item.shootSpeed = 10f;

            item.Calamity().Chargeable = true;
            item.Calamity().ChargeMax = 50;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectileDirect(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f).Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 7);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 4);
            recipe.AddIngredient(ItemID.MeteoriteBar, 4);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
