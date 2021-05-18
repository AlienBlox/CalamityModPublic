using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Providence;
using CalamityMod.TileEntities;
using CalamityMod.World;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod
{
    public class CalamityNetcode
    {
        public static void HandlePacket(Mod mod, BinaryReader reader, int whoAmI)
        {
            try
            {
                CalamityModMessageType msgType = (CalamityModMessageType)reader.ReadByte();
                switch (msgType)
                {
                    //
                    // Proficiency levels
                    //

                    case CalamityModMessageType.MeleeLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 0);
                        break;
                    case CalamityModMessageType.RangedLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 1);
                        break;
                    case CalamityModMessageType.MagicLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 2);
                        break;
                    case CalamityModMessageType.SummonLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 3);
                        break;
                    case CalamityModMessageType.RogueLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 4);
                        break;
                    case CalamityModMessageType.ExactMeleeLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 0);
                        break;
                    case CalamityModMessageType.ExactRangedLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 1);
                        break;
                    case CalamityModMessageType.ExactMagicLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 2);
                        break;
                    case CalamityModMessageType.ExactSummonLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 3);
                        break;
                    case CalamityModMessageType.ExactRogueLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 4);
                        break;

                    //
                    // Core stat syncs
                    //

                    case CalamityModMessageType.MoveSpeedStatSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleMoveSpeedStat(reader);
                        break;
                    case CalamityModMessageType.DefenseDamageSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleDefenseDamage(reader);
                        break;
                    case CalamityModMessageType.RageSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleRage(reader);
                        break;
                    case CalamityModMessageType.AdrenalineSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleAdrenaline(reader);
                        break;
                    case CalamityModMessageType.DodgeCooldown:
                        Main.player[reader.ReadInt32()].Calamity().HandleDodgeCooldown(reader);
                        break;
                    case CalamityModMessageType.DeathCountSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleDeathCount(reader);
                        break;

                    //
                    // Syncs for specific bosses or entities
                    //

                    // This code has been edited to fail gracefully when trying to provide data for an invalid NPC.
                    case CalamityModMessageType.SyncCalamityNPCAIArray:
                        // Read the entire packet regardless of anything
                        byte npcIdx = reader.ReadByte();
                        float ai0 = reader.ReadSingle();
                        float ai1 = reader.ReadSingle();
                        float ai2 = reader.ReadSingle();
                        float ai3 = reader.ReadSingle();

                        // If the NPC in question isn't valid, don't do anything.
                        NPC npc = Main.npc[npcIdx];
                        if (!npc.active)
                            break;

                        CalamityGlobalNPC cgn = npc.Calamity();
                        cgn.newAI[0] = ai0;
                        cgn.newAI[1] = ai1;
                        cgn.newAI[2] = ai2;
                        cgn.newAI[3] = ai3;
                        break;
                    case CalamityModMessageType.SpawnSuperDummy:
                        int x = reader.ReadInt32();
                        int y = reader.ReadInt32();
                        // Not strictly necessary, but helps prevent unnecessary packetstorm in MP
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.NewNPC(x, y, ModContent.NPCType<SuperDummyNPC>());
                        break;
                    case CalamityModMessageType.ServersideSpawnOldDuke:
                        byte playerIndex2 = reader.ReadByte();
                        CalamityUtils.SpawnOldDuke(playerIndex2);
                        break;
                    case CalamityModMessageType.DoGCountdownSync:
                        int countdown = reader.ReadInt32();
                        CalamityWorld.DoGSecondStageCountdown = countdown;
                        break;
                    case CalamityModMessageType.ArmoredDiggerCountdownSync:
                        int countdown5 = reader.ReadInt32();
                        CalamityWorld.ArmoredDiggerSpawnCooldown = countdown5;
                        break;
                    case CalamityModMessageType.ProvidenceDyeConditionSync:
                        byte npcIndex3 = reader.ReadByte();
                        (Main.npc[npcIndex3].modNPC as Providence).hasTakenDaytimeDamage = reader.ReadBoolean();
                        break;
                    case CalamityModMessageType.PSCChallengeSync:
                        byte npcIndex4 = reader.ReadByte();
                        (Main.npc[npcIndex4].modNPC as Providence).challenge = reader.ReadBoolean();
                        break;

                    //
                    // Tile Entities
                    //

                    case CalamityModMessageType.PowerCellFactory:
                        TEPowerCellFactory.ReadSyncPacket(mod, reader);
                        break;
                    case CalamityModMessageType.ChargingStationStandard:
                        TEChargingStation.ReadSyncPacket(mod, reader);
                        break;
                    case CalamityModMessageType.ChargingStationItemChange:
                        TEChargingStation.ReadItemSyncPacket(mod, reader);
                        break;
                    case CalamityModMessageType.Turret:
                        TEBaseTurret.ReadSyncPacket(mod, reader);
                        break;
                    case CalamityModMessageType.LabHologramProjector:
                        TELabHologramProjector.ReadSyncPacket(mod, reader);
                        break;

                    //
                    // Boss Rush
                    //

                    case CalamityModMessageType.BossRushStage:
                        int stage = reader.ReadInt32();
                        BossRushEvent.BossRushStage = stage;
                        break;
                    case CalamityModMessageType.BossRushStartTimer:
                        BossRushEvent.StartTimer = reader.ReadInt32();
                        break;
                    case CalamityModMessageType.BossRushEndTimer:
                        BossRushEvent.EndTimer = reader.ReadInt32();
                        break;
                    case CalamityModMessageType.BossSpawnCountdownSync:
                        int countdown2 = reader.ReadInt32();
                        CalamityWorld.bossSpawnCountdown = countdown2;
                        break;
                    case CalamityModMessageType.BossTypeSync:
                        int type = reader.ReadInt32();
                        CalamityWorld.bossType = type;
                        break;
                    case CalamityModMessageType.BRHostileProjKillSync:
                        int countdown3 = reader.ReadInt32();
                        CalamityWorld.bossRushHostileProjKillCounter = countdown3;
                        break;
                    case CalamityModMessageType.TeleportPlayer:
                        Main.player[reader.ReadInt32()].Calamity().HandleTeleport(reader.ReadInt32(), true, whoAmI);
                        break;

                    //
                    // Acid Rain
                    //

                    case CalamityModMessageType.AcidRainSync:
                        CalamityWorld.rainingAcid = reader.ReadBoolean();
                        CalamityWorld.acidRainPoints = reader.ReadInt32();
                        CalamityWorld.timeSinceAcidRainKill = reader.ReadInt32();
                        break;
                    case CalamityModMessageType.AcidRainOldDukeSummonSync:
                        CalamityWorld.triedToSummonOldDuke = reader.ReadBoolean();
                        break;
                    case CalamityModMessageType.EncounteredOldDukeSync:
                        CalamityWorld.encounteredOldDuke = reader.ReadBoolean();
                        break;

                    //
                    // Death Mode environmental syncs
                    //

                    case CalamityModMessageType.DeathModeUnderworldTimeSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleDeathModeUnderworldTime(reader);
                        break;
                    case CalamityModMessageType.DeathModeBlizzardTimeSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleDeathModeBlizzardTime(reader);
                        break;
                    case CalamityModMessageType.DeathBossSpawnCountdownSync:
                        int countdown4 = reader.ReadInt32();
                        CalamityWorld.deathBossSpawnCooldown = countdown4;
                        break;

                    //
                    // Reforge syncs
                    //

                    case CalamityModMessageType.ItemTypeLastReforgedSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleItemTypeLastReforged(reader);
                        break;
                    case CalamityModMessageType.ReforgeTierSafetySync:
                        Main.player[reader.ReadInt32()].Calamity().HandleReforgeTierSafety(reader);
                        break;

                    //
                    // Default case: with no idea how long the packet is, we can't safely read data.
                    // Throw an exception now instead of allowing the network stream to corrupt.
                    //
                    default:
                        CalamityMod.Instance.Logger.Error($"Failed to parse Calamity packet: No Calamity packet exists with ID {msgType}.");
                        throw new Exception("Failed to parse Calamity packet: Invalid Calamity packet ID.");
                }
            }
            catch (Exception e)
            {
                if (e is EndOfStreamException eose)
                    CalamityMod.Instance.Logger.Error("Failed to parse Calamity packet: Packet was too short, missing data, or otherwise corrupt.", eose);
                else if (e is ObjectDisposedException ode)
                    CalamityMod.Instance.Logger.Error("Failed to parse Calamity packet: Packet reader disposed or destroyed.", ode);
                else if (e is IOException ioe)
                    CalamityMod.Instance.Logger.Error("Failed to parse Calamity packet: An unknown I/O error occurred.", ioe);
                else
                    throw e; // this either will crash the game or be caught by TML's packet policing
            }
        }

        public static void SyncWorld()
        {
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }
    }

    public enum CalamityModMessageType : byte
    {
        // Proficiency levels
        // TODO -- simplify proficiency netcode, there do not need to be this many separate packet types
        MeleeLevelSync,
        RangedLevelSync,
        MagicLevelSync,
        SummonLevelSync,
        RogueLevelSync,
        ExactMeleeLevelSync,
        ExactRangedLevelSync,
        ExactMagicLevelSync,
        ExactSummonLevelSync,
        ExactRogueLevelSync,

        // Core stat syncs
        MoveSpeedStatSync, // TODO -- this can't be synced every 60 frames, it needs to be synced every time the player is
        DefenseDamageSync, // TODO -- this can't be synced every 60 frames, it needs to be synced when the player gets hit, or every time it heals up
        RageSync, // TODO -- this can't be synced every 60 frames, it needs to be synced every time the player is
        AdrenalineSync, // TODO -- this can't be synced every 60 frames, it needs to be synced every time the player is
        DodgeCooldown,
        DeathCountSync, // TODO -- this is synced in numerous incorrect places, Armageddon deaths count twice, and it supposedly counts every time you log in

        // Syncs for specific bosses or entities
        SyncCalamityNPCAIArray,
        SpawnSuperDummy, // TODO -- super dummies STILL don't work in multiplayer
        ServersideSpawnOldDuke,
        DoGCountdownSync, // TODO -- this gets written in about six thousand places which all need to be individually evaluated
        ArmoredDiggerCountdownSync, // TODO -- remove this mechanic entirely
        ProvidenceDyeConditionSync, // TODO -- this packetstorms if you hit Provi with spam weapons. It should ONLY send a packet if the status changes.
        PSCChallengeSync, // TODO -- once you've failed the PSC challenge this packetstorms

        // Tile Entities
        PowerCellFactory,
        ChargingStationStandard,
        ChargingStationItemChange,
        Turret,
        LabHologramProjector,

        // Boss Rush
        BossRushStage,
        BossRushStartTimer,
        BossRushEndTimer,
        BossSpawnCountdownSync,
        BossTypeSync,
        BRHostileProjKillSync, // TODO -- Simplify this. Only one packet needs be sent: "kill all hostile projectiles for N frames".
        TeleportPlayer, // also used by Astral Arcanum.

        // Acid Rain
        AcidRainSync,
        AcidRainOldDukeSummonSync,
        EncounteredOldDukeSync,

        // Death Mode environmental syncs
        DeathModeUnderworldTimeSync, // TODO -- Rename to heat time. This should be synced every 15 frames.
        DeathModeBlizzardTimeSync, // TODO -- Rename to cold time. This should be synced every 15 frames.
        DeathBossSpawnCountdownSync, // TODO -- This currently syncs every frame and shouldn't. It only needs to sync once, when the countdown starts.

        // Reforge syncs
        ItemTypeLastReforgedSync, // TODO -- there has to be a better way to do this, but I don't know what it is
        ReforgeTierSafetySync,
    }
}
