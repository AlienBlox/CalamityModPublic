using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class ReaverVisage : ModItem
    {
		//Jump/Flight Boosts and Movement Speed Helm
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Visage");
            Tooltip.SetDefault("25% increased movement speed and 20% increased jump speed");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 28;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.rare = 7;
            item.defense = 13; //46
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ReaverScaleMail>() && legs.type == ModContent.ItemType<ReaverCuisses>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.reaverSpeed = true;
            modPlayer.wearingRogueArmor = true;
            player.setBonus = "Grants immunity to fall damage and allows constant jumping\n" +
                "10% increased flight time and horizontal wing speed\n" +
				"Hooks fly out and retract 10% faster\n" +
				"Reduces the cooldown of dashes";
            player.noFallDmg = true;
            player.autoJump = true;
			if (player.miscCounter % 3 == 2 && player.dashDelay > 0)
				player.dashDelay--;
        }

        public override void UpdateEquip(Player player)
        {
            player.jumpSpeedBoost += 1f;
            player.moveSpeed += 0.25f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 8);
			recipe.AddIngredient(ItemID.JungleSpores, 6);
			recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>());
			recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
