using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.NPCs
{
    public class SCalScreenShaderData : ScreenShaderData
    {
        private int SCalIndex;

        public SCalScreenShaderData(string passName)
            : base(passName)
        {
        }

        private void UpdateSCalIndex()
        {
            int SCalType = ModLoader.GetMod("CalamityMod").NPCType("SupremeCalamitas");
            if (SCalIndex >= 0 && Main.npc[SCalIndex].active && Main.npc[SCalIndex].type == SCalType)
            {
                return;
            }
            SCalIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == SCalType)
                {
                    SCalIndex = i;
                    break;
                }
            }
        }

        public override void Apply()
        {
            UpdateSCalIndex();
            if (SCalIndex != -1)
            {
                UseTargetPosition(Main.npc[SCalIndex].Center);
            }
            base.Apply();
        }
    }
}
