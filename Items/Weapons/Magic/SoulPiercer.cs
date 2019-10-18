using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Magic
{
    public class SoulPiercer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Piercer");
            Tooltip.SetDefault("Casts a powerful ray that summons extra rays on enemy hits");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 185;
            item.magic = true;
            item.mana = 19;
            item.width = 64;
            item.height = 64;
            item.useTime = 18;
            item.useAnimation = 18;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 8f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item73;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SoulPiercerBeam>();
            item.shootSpeed = 6f;
            item.Calamity().postMoonLordRarity = 14;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Vector2 origin = new Vector2(32f, 30f);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/SoulPiercerGlow"), item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CosmiliteBar", 12);
            recipe.AddIngredient(null, "NightmareFuel", 6);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CosmiliteBar", 12);
            recipe.AddIngredient(null, "EndothermicEnergy", 6);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
