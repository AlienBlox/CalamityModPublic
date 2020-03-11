using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class PlaguePrincessBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Virili");
            Description.SetDefault("It’s a shame you can’t hug her");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<PlaguePrincess>()] > 0)
            {
                modPlayer.virili = true;
            }
            if (!modPlayer.virili)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
            {
                player.buffTime[buffIndex] = 18000;
            }
        }
    }
}
