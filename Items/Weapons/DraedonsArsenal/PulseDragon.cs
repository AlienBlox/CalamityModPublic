using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class PulseDragon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pulse Dragon");
            Tooltip.SetDefault("Throws two dragon heads that emit electrical fields");
        }

        public override void SetDefaults()
        {
            item.damage = 420;
            item.melee = true;
            item.width = 30;
            item.height = 10;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.knockBack = 8f;
            item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DraedonRust;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.channel = true;
            item.shoot = ModContent.ProjectileType<PulseDragonProjectile>();
            item.shootSpeed = 20f;
            item.Calamity().Chargeable = true;
            item.Calamity().ChargeMax = 190;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);
            float offsetAngle = Main.rand.NextFloat(0.2f, 0.4f);
            velocity += player.velocity;
            float velocityAngle = velocity.ToRotation();
            Projectile projectile = Projectile.NewProjectileDirect(position, velocity, type, damage, knockBack, player.whoAmI, velocityAngle, Main.rand.NextFloat(30f, PulseDragonProjectile.MaximumPossibleOutwardness));
            projectile.localAI[0] = 1f;
            projectile = Projectile.NewProjectileDirect(position, velocity.RotatedBy(offsetAngle), type, damage, knockBack, player.whoAmI, velocityAngle + offsetAngle, Main.rand.NextFloat(30f, PulseDragonProjectile.MaximumPossibleOutwardness));
            projectile.localAI[0] = -1f;
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 12);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 18);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 8);
            recipe.AddIngredient(ItemID.LunarBar, 4);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
