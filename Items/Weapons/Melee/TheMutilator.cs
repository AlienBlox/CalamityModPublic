using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheMutilator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Mutilator");
            Tooltip.SetDefault("Striking an enemy below 20% life will trigger a bloodsplosion\n" +
                               "Bloodsplosions cause hearts to drop that can be picked up to heal you");
        }

        public override void SetDefaults()
        {
            item.width = 90;
            item.height = 90;
            item.damage = 950;
            item.melee = true;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 18;
            item.useTurn = true;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shootSpeed = 10f;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= (target.lifeMax * 0.2f) && target.canGhostHeal)
            {
                int heartDrop = Main.rand.Next(1, 3);
                for (int i = 0; i < heartDrop; i++)
                {
                    Item.NewItem((int)target.position.X, (int)target.position.Y, target.width, target.height, 58, 1, false, 0, false, false);
                }
                Main.PlaySound(SoundID.Item14, target.position);
                target.position.X += (float)(target.width / 2);
                target.position.Y += (float)(target.height / 2);
                target.position.X -= (float)(target.width / 2);
                target.position.Y -= (float)(target.height / 2);
                for (int num621 = 0; num621 < 30; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 50; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 5, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BloodstoneCore>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
