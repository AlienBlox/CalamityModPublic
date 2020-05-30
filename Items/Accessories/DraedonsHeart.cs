using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class DraedonsHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draedon's Heart");
            Tooltip.SetDefault("Gives 10% increased damage while you have the absolute rage buff\n" +
                "Increases your chance of getting the absolute rage buff\n" +
                "Boosts your damage by 5% and max movement speed and acceleration by 5%\n" +
                "Rage mode does more damage\n" +
                "You gain rage over time\n" +
                "The Horror debuff lasts twice as long,\n" +
                "but it instead grants various buffs to the player\n" +
                "Receiving a hit causes you to only lose half of your max adrenaline rather than all of it\n" +
                "Standing still regenerates your life quickly and boosts your defense by 25");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 7));
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
			modPlayer.laudanum = true;
			modPlayer.stressPills = true;
            modPlayer.draedonsHeart = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<HeartofDarkness>());
            recipe.AddIngredient(ModContent.ItemType<StressPills>());
            recipe.AddIngredient(ModContent.ItemType<Laudanum>());
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 5);
            recipe.AddIngredient(ItemID.Nanites, 250);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
