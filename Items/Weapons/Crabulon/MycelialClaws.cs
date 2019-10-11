using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Crabulon
{
    public class MycelialClaws : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mycelial Claws");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.damage = 14;
            item.melee = true;
            item.useAnimation = 6;
            item.useStyle = 1;
            item.useTime = 6;
            item.useTurn = true;
            item.knockBack = 3.75f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 24;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 56);
            }
        }
    }
}
