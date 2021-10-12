﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Other
{
    public class BrimstoneOrb : ModNPC
    {
        public ref float Time => ref npc.ai[0];
        public Player Owner
        {
            get
            {
                if (npc.target >= 255 || npc.target < 0)
                    npc.TargetClosest();
                return Main.player[npc.target];
            }
        }
        public override void SetStaticDefaults() => DisplayName.SetDefault("Brimstone Orb");

        public override void SetDefaults()
        {
            npc.width = npc.height = 28;
            npc.damage = 0;
            npc.defense = 0;
            npc.lifeMax = 181445;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.value = 0f;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCDeath7;
            npc.knockBackResist = 0f;
            npc.netAlways = true;
            npc.aiStyle = -1;
            npc.canGhostHeal = false;
            npc.Calamity().DoesNotGenerateRage = true;
            npc.Calamity().DoesNotDisappearInBossRush = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale) => npc.lifeMax = 181445;

        public override void AI()
        {
            npc.Opacity = Utils.InverseLerp(0f, 15f, Time, true);
            npc.velocity = Vector2.Zero;

            if (Main.myPlayer == npc.target)
            {
                // Disappear if the player no longer has the enchanted weapon.
                if (!Owner.Calamity().lecherousOrbEnchant)
                {
                    npc.active = false;
                    npc.netUpdate = true;
                }

                // Notify the owner that the orb has indeed spawned.
                Owner.Calamity().awaitingLecherousOrbSpawn = false;

                Vector2 destination = Vector2.Lerp(Owner.Center, Main.MouseWorld, 0.625f);
                npc.Center = Vector2.Lerp(npc.Center, destination, 0.035f).MoveTowards(destination, 8f);
                if (npc.WithinRange(destination, 5f))
                    npc.Center = destination;

                if (!npc.WithinRange(destination, 2000f))
                    npc.Center = Owner.Center;

                bool wasNotAtDestinationBefore = Vector2.Distance(npc.position + npc.Size * 0.5f, destination) < 0.1f && Vector2.Distance(npc.oldPosition + npc.Size * 0.5f, destination) > 0.1f;
                if (Vector2.Distance(npc.position, npc.oldPosition) > 30f || wasNotAtDestinationBefore)
                    npc.SyncMotionToServer();
            }

            Time++;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 3; i++)
            {
                Dust magic = Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(12f, 12f), 264);
                magic.color = Color.Red;
                magic.velocity = Main.rand.NextVector2Circular(3f, 3f);
                magic.fadeIn = 0.9f;
                magic.scale = 1.3f;
                magic.noGravity = true;
            }

            if (npc.life <= 0)
            {
                for (int i = 0; i < 15; i++)
                {
                    Dust magic = Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(12f, 12f), 264);
                    magic.color = Color.Red;
                    magic.velocity = Main.rand.NextVector2Circular(6f, 6f);
                    magic.fadeIn = 1.25f;
                    magic.scale = Main.rand.NextFloat(1.2f, 1.56f);
                    magic.noGravity = true;
                }
            }
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (Main.myPlayer == npc.target)
                npc.SyncMotionToServer();

            return base.StrikeNPC(ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override void NPCLoot()
        {
            int heartsToGive = (int)MathHelper.Lerp(0f, 7f, Utils.InverseLerp(45f, 540f, Time, true));
            for (int i = 0; i < heartsToGive; i++)
                DropHelper.DropItem(npc, ItemID.Heart);
        }

        public override Color? GetAlpha(Color drawColor) => Color.White * npc.Opacity;
    }
}
