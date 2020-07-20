using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class SilvaMaskedCap : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silva Masked Cap");
            Tooltip.SetDefault("13% increased magic damage and critical strike chance\n" +
                "+100 max mana and 19% reduced mana usage");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 22;
            item.value = Item.buyPrice(0, 90, 0, 0);
            item.defense = 21; //110
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SilvaArmor>() && legs.type == ModContent.ItemType<SilvaLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.silvaSet = true;
            modPlayer.silvaMage = true;
            player.setBonus = "All projectiles spawn healing leaf orbs on enemy hits\n" +
                "Max run speed and acceleration boosted by 5%\n" +
                "If you are reduced to 1 HP you will not die from any further damage for 10 seconds\n" +
                "If you get reduced to 1 HP again while this effect is active you will lose 100 max life\n" +
                "This effect only triggers once per life and if you are reduced to 400 max life the invincibility effect will stop\n" +
                "Your max life will return to normal if you die\n" +
                "Magic projectiles have a 10% chance to cause a massive explosion on enemy hits\n" +
                "After the silva invulnerability time your magic weapons will do 10% more damage";
        }

        public override void UpdateEquip(Player player)
        {
            player.manaCost *= 0.81f;
            player.magicDamage += 0.13f;
            player.magicCrit += 13;
            player.statManaMax2 += 100;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 5);
            recipe.AddIngredient(ModContent.ItemType<EffulgentFeather>(), 5);
            recipe.AddRecipeGroup("AnyGoldBar", 5);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 6);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 14);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 14);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
