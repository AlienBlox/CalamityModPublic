using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Ammo;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.TownNPCs
{
	[AutoloadHead]
    public class DILF : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Archmage");

            Main.npcFrameCount[npc.type] = 25;
            NPCID.Sets.ExtraFramesCount[npc.type] = 9;
            NPCID.Sets.AttackFrameCount[npc.type] = 4;
            NPCID.Sets.DangerDetectRange[npc.type] = 700;
            NPCID.Sets.AttackType[npc.type] = 0;
            NPCID.Sets.AttackTime[npc.type] = 90;
            NPCID.Sets.AttackAverageChance[npc.type] = 30;
        }

        public override void SetDefaults()
        {
            npc.townNPC = true;
            npc.friendly = true;
            npc.lavaImmune = true;
            npc.width = 18;
            npc.height = 40;
            npc.aiStyle = 7;
            npc.damage = 10;
            npc.defense = 15;
            npc.lifeMax = 20000;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.8f;
            animationType = NPCID.Guide;
        }

		public override void AI()
		{
			if (!CalamityWorld.foundHomePermafrost && !npc.homeless)
			{
				CalamityWorld.foundHomePermafrost = true;
			}
		}

        public override bool CanTownNPCSpawn(int numTownNPCs, int money) => CalamityWorld.downedCryogen;

        public override string TownNPCName() => "Permafrost";

        public override string GetChat()
        {
            if (npc.homeless && !CalamityWorld.foundHomePermafrost)
            {
                if (Main.rand.NextBool(2))
                    return "I have not seen such a sky in decades. Who are you, to so brazenly march against that Tyrant?";
                else
                    return "I... deeply appreciate you rescuing me from being trapped within my frozen castle. It's been many, many years.";
            }

            IList<string> dialogue = new List<string>();

            if (Main.dayTime && !Main.player[Main.myPlayer].ZoneSnow)
            {
                dialogue.Add("The sun beats down harshly upon my creations here. If you would allow me to conjure a blizzard every now and then...");
                dialogue.Add("I must admit, I’m not quite used to this weather. It's far too warm for my tastes...");
            }
            else
            {
                dialogue.Add("Nightfall is a good time for practicing magic. We mages often rely on celestial bodies and their fragments to enhance our mana.");
                dialogue.Add("Necromancy was never a field I found interesting. Why utilize the rotting corpses of people, when you could form far more elegant servants of ice?");
            }

            dialogue.Add("The tundra’s unnatural state is not all my doing. Decades ago, I came across it and amplified the climate with my magic.");
            dialogue.Add("If you have a request, make it quick. I am in the process of weaving a spell, which requires great focus.");
            dialogue.Add("You have the makings of a gifted mage. Tell me, what do you think of ice magic?");
            dialogue.Add("Flowers and the like don’t hold a candle to the beauty of intricately formed ice.");

            if (BirthdayParty.PartyIsUp)
                dialogue.Add("Sometimes... I feel like all I'm good for during these events is making ice cubes and slushies.");

            if (Main.bloodMoon)
            {
                dialogue.Add("If your blood were to thoroughly freeze, it would be quite fatal.");
                dialogue.Add("The undead which roam tonight are still monsters of blood and guts, but they seem... fresher.");
            }

            if (NPC.downedMoonlord)
            {
                dialogue.Add("It is shocking, to see you have come so far. I wish you the best of luck on your future endeavours.");
                dialogue.Add("You, having bested so many beings, even deities, I wonder if I have anything left to offer you.");
            }

            return dialogue[Main.rand.Next(dialogue.Count)];
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
			if (firstButton)
			{
				Main.LocalPlayer.Calamity().newPermafrostInventory = false;
				shop = true;
			}
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
			shop.item[nextSlot].SetDefaults(ItemID.WarmthPotion);
			shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 2, 0, 0);
            if (Main.LocalPlayer.discount)
              shop.item[nextSlot].shopCustomPrice = (int)(shop.item[nextSlot].shopCustomPrice * 0.8);
			nextSlot++;
			shop.item[nextSlot].SetDefaults(ModContent.ItemType<ColdheartIcicle>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<FrostbiteBlaster>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<IcicleTrident>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<IceStar>());
            nextSlot++;
			if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
			{
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<ArcticBearPaw>());
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<CryogenicStaff>());
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<FrostyFlare>());
				nextSlot++;
			}
			if (NPC.downedChristmasIceQueen && NPC.downedChristmasTree && NPC.downedChristmasSantank)
			{
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<AbsoluteZero>());
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<EternalBlizzard>());
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<WintersFury>());
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<IcyBullet>());
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<IcicleArrow>());
				nextSlot++;
			}
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<PermafrostsConcoction>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ItemID.SuperManaPotion);
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<DeliciousMeat>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Popo>());
			shop.item[nextSlot].shopCustomPrice = Item.buyPrice(5, 0, 0, 0);
            if (Main.LocalPlayer.discount)
              shop.item[nextSlot].shopCustomPrice = (int)(shop.item[nextSlot].shopCustomPrice * 0.8);
			nextSlot++;
            if (Main.LocalPlayer.HasItem(ModContent.ItemType<IceBarrage>()))
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<BloodRune>());
                nextSlot++;
            }
        }

        // Make this Town NPC teleport to the King statue when triggered.
        public override bool CanGoToStatue(bool toKingStatue) => toKingStatue;

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 9f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 10;
            randExtraCooldown = 50;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<DarkIce>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 2f;
        }
    }
}
