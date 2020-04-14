using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Projectiles.Magic;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.TownNPCs
{
    [AutoloadHead]
    public class FAP : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Drunk Princess");

            Main.npcFrameCount[npc.type] = 25;
            NPCID.Sets.ExtraFramesCount[npc.type] = 9;
            NPCID.Sets.AttackFrameCount[npc.type] = 4;
            NPCID.Sets.DangerDetectRange[npc.type] = 400;
            NPCID.Sets.AttackType[npc.type] = 0;
            NPCID.Sets.AttackTime[npc.type] = 60;
            NPCID.Sets.AttackAverageChance[npc.type] = 15;
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
            npc.DeathSound = SoundID.NPCDeath6;
            npc.knockBackResist = 0.5f;
            animationType = NPCID.Guide;
        }

		public override void AI()
		{
			if (!CalamityWorld.spawnedCirrus)
			{
				CalamityWorld.spawnedCirrus = true;
			}
		}

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player player = Main.player[k];
                if (player.active)
                {
                    for (int j = 0; j < player.inventory.Length; j++)
                    {
                        if (player.inventory[j].type == ModContent.ItemType<FabsolsVodka>())
                        {
                            return Main.hardMode;
                        }
                    }
                }
            }
            return CalamityWorld.spawnedCirrus;
        }

        public override string TownNPCName()
        {
            return "Cirrus";
        }

        public override string GetChat()
        {
            if (CalamityWorld.bossRushActive)
                return "We expect you to turn in a perfect performance!";

            if (npc.homeless)
            {
                if (Main.rand.NextBool(2))
                    return "I could smell my vodka from MILES away!";
                else
                    return "Have any spare rooms available? Preferably candle-lit with a hefty supply of booze.";
            }

            if (Main.bloodMoon)
            {
                int random = Main.rand.Next(4);
                if (random == 0)
                {
                    return "Hey! Nice night. I'm gonna make some Bloody Marys. Celery included. Want one?";
                }
                else if (random == 1)
                {
                    return "More blood for the blood gods!";
                }
                else if (random == 2)
                {
                    return "Everyone else is so rude tonight. If they don't get over it soon I'll break down their doors and make them!";
                }
                else
                {
                    Main.player[Main.myPlayer].Hurt(PlayerDeathReason.ByCustomReason(Main.player[Main.myPlayer].name + " was slapped too hard."), Main.player[Main.myPlayer].statLife / 2, -Main.player[Main.myPlayer].direction, false, false, false, -1);
                    return "Being drunk I have no moral compass atm.";
                }
            }

            if (CalamityGlobalNPC.AnyBossNPCS())
                return "Nothard/10, if I fight bosses I wanna feel like screaming 'OH YES DADDY!' while I die repeatedly.";

            IList<string> dialogue = new List<string>();

            if (Main.dayTime)
            {
                dialogue.Add("Like I always say, when you're drunk you can put up with a lot more.");
                dialogue.Add("I'm literally balls drunk off my sass right now.");
                dialogue.Add("I'm either laughing because I'm drunk or because I've lost my mind. Probably both.");
                dialogue.Add("When I'm drunk I'm way happier...at least until the talking worms start to appear.");
                dialogue.Add("I should reprogram the whole game while drunk and send it back to the testers.");
                dialogue.Add("What a great day, might just drink so much that I get poisoned again.");
            }
            else
            {
                dialogue.Add("Now I want to get alcohol, first drinks are on me!");
                dialogue.Add("Here's a challenge...take a drink whenever you take a hit. Oh wait, you'd die.");
                dialogue.Add("Well I was planning to light some candles in order to relax...ah well, time to program while drunk.");
                dialogue.Add("Yes, everyone knows the mechworm is buggy.");
                dialogue.Add("That's west, " + Main.player[Main.myPlayer].name + ". You're fired again.");
                dialogue.Add("Are you sure you're 21? ...alright, fine, but don't tell anyone I sold you this.");
            }

            dialogue.Add("Drink something that turns you into a magical flying unicorn so you can be super gay.");
            dialogue.Add("Did anyone ever tell you that large assets cause back pain? Well, they were right.");

            if (BirthdayParty.PartyIsUp)
                dialogue.Add("You'll always find me at parties where booze is involved...well, you'll always find booze where I'm involved.");

            if (Main.invasionType == 4)
                dialogue.Add("Shoot down the space invaders! Sexy time will be postponed if we are invaded by UFOs!");

            if (CalamityWorld.downedCryogen)
                dialogue.Add("God I can't wait to beat on some ice again!");

            if (CalamityWorld.downedLeviathan)
                dialogue.Add("Only things I am attracted to are fish women, women, men who look like women, and that's it.");

            if (NPC.downedMoonlord)
            {
                dialogue.Add("I'll always be watching.");
                dialogue.Add("Why did one creature need that many tentacles? ...actually, don't answer that.");
            }

			if (CalamityWorld.rainingAcid)
                dialogue.Add("There's chemicals in the water...and it's turning the frogs gay!");

            if (CalamityWorld.downedPolterghast)
                dialogue.Add("I saw a ghost down by the old train tracks once flailing wildly at the lily pads, those were the days.");

            if (CalamityWorld.downedDoG)
                dialogue.Add("I hear it's amazing when the famous purple-stuffed worm out in flap-jaw space with the tuning fork does a raw blink on Hara-kiri rock. I need scissors! 61!");

            int tavernKeep = NPC.FindFirstNPC(NPCID.DD2Bartender);
            if (tavernKeep != -1)
            {
                dialogue.Add("Tell " + Main.npc[tavernKeep].GivenName + " to stop calling me. He's not wanted.");
                dialogue.Add("My booze will always be better than " + Main.npc[tavernKeep].GivenName + "'s and nobody can convince me otherwise.");
            }

            int dryad = NPC.FindFirstNPC(NPCID.Dryad);
            if (dryad != -1)
                dialogue.Add(Main.npc[dryad].GivenName + " is cool too but she'd outlive me.");

            int permadong = NPC.FindFirstNPC(ModContent.NPCType<DILF>());
            if (permadong != -1)
                dialogue.Add("I never realized how well-endowed " + Main.npc[permadong].GivenName + " was. It had to be the largest icicle I had ever seen.");

            int waifu = NPC.FindFirstNPC(NPCID.Stylist);
            if (waifu != -1)
            {
                dialogue.Add("You still can't stop me from selling alcohol and trying to move in with " + Main.npc[waifu].GivenName + ".");
                dialogue.Add("I love it when " + Main.npc[waifu].GivenName + "'s hands get sticky from all that...wax.");
                dialogue.Add(Main.npc[waifu].GivenName + " works wonders for my hair...among other things.");
            }

            if (Main.player[Main.myPlayer].Calamity().chibii)
                dialogue.Add("Is that a toy? Looks like something I'd carry around if I was 5 years old.");

            if ((Main.player[Main.myPlayer].Calamity().sirenBoobs && !Main.player[Main.myPlayer].Calamity().sirenBoobsHide) ||
                (Main.player[Main.myPlayer].Calamity().sirenBoobsAlt && !Main.player[Main.myPlayer].Calamity().sirenBoobsAltHide))
                dialogue.Add("Nice...scales...did it get hot in here?");

            if (Main.player[Main.myPlayer].Calamity().fabsolVodka)
                dialogue.Add("Oh yeah now you're drinking the good stuff! Do you like it? I created the recipe by mixing fairy dust, crystals, and other magical crap.");

            if (Main.player[Main.myPlayer].Calamity().fab)
            {
                dialogue.Add("...so, you're riding me huh? That's not weird at all.");
                dialogue.Add("Are you coming on to me?");
                dialogue.Add("If I was a magical horse in this reality I'd be out in space swirling cocktails as I watch space worms battle for my enjoyment.");
            }

			IList<string> donorList = new List<string>(CalamityMod.donatorList);
			string[] donors = new string[12];
			for (int i = 0; i < 12; i++)
			{
				donors[i] = donorList[Main.rand.Next(donorList.Count)];
				donorList.Remove(donors[i]);
			}

			dialogue.Add("Hey " + donors[0] + ", " + donors[1] + ", " + donors[2] + ", " + donors[3] + ", " + donors[4] + ", " + donors[5] + ", " + donors[6] +
				", " + donors[7] + ", " + donors[8] + ", " + donors[9] + ", " + donors[10] + " and " + donors[11] + "! You're all pretty good! ...wait, who are you again?");

            return dialogue[Main.rand.Next(dialogue.Count)];
        }

        public string Death()
        {
            return "You have failed " + Main.player[Main.myPlayer].Calamity().deathCount +
                (Main.player[Main.myPlayer].Calamity().deathCount == 1 ? " time." : " times.");
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = "Death Count";
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
				Main.LocalPlayer.Calamity().newCirrusInventory = false;
				shop = true;
            }
            else
            {
                shop = false;
                Main.npcChatText = Death();
            }
        }

        public override void SetupShop(Chest shop, ref int nextSlot) //charges 50% extra than the original item value
        {
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<GrapeBeer>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 1, 0, 0);
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<RedWine>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 3, 0, 0);
            nextSlot++;

			shop.item[nextSlot].SetDefaults(ModContent.ItemType<Whiskey>());
			shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 5, 0, 0);
			nextSlot++;

			shop.item[nextSlot].SetDefaults(ModContent.ItemType<Rum>());
			shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 7, 50, 0);
			nextSlot++;

			shop.item[nextSlot].SetDefaults(ModContent.ItemType<Tequila>());
			shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 7, 50, 0);
			nextSlot++;

			shop.item[nextSlot].SetDefaults(ModContent.ItemType<Fireball>());
			shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 10, 0, 0);
			nextSlot++;

			shop.item[nextSlot].SetDefaults(ModContent.ItemType<FabsolsVodka>());
			shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 15, 0, 0);
			nextSlot++;

			if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
			{
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<Vodka>());
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 5, 0, 0);
				nextSlot++;

				shop.item[nextSlot].SetDefaults(ModContent.ItemType<Screwdriver>());
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 25, 0, 0);
				nextSlot++;

				shop.item[nextSlot].SetDefaults(ModContent.ItemType<WhiteWine>());
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 25, 0, 0);
				nextSlot++;
			}

			if (NPC.downedPlantBoss)
			{
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<EvergreenGin>());
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 25, 0, 0);
				nextSlot++;

				shop.item[nextSlot].SetDefaults(ModContent.ItemType<CaribbeanRum>());
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 30, 0, 0);
				nextSlot++;

				shop.item[nextSlot].SetDefaults(ModContent.ItemType<Margarita>());
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 35, 0, 0);
				nextSlot++;
			}

            if (CalamityWorld.downedAstrageldon)
            {
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<Everclear>());
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 10, 0, 0);
				nextSlot++;

				if (Main.bloodMoon)
				{
					shop.item[nextSlot].SetDefaults(ModContent.ItemType<BloodyMary>());
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 15, 0, 0);
					nextSlot++;
				}

				if (!Main.dayTime)
				{
					shop.item[nextSlot].SetDefaults(ModContent.ItemType<StarBeamRye>());
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 20, 0, 0);
					nextSlot++;
				}
			}

			if (NPC.downedGolemBoss)
			{
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<Moonshine>());
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 5, 0, 0);
				nextSlot++;

				shop.item[nextSlot].SetDefaults(ModContent.ItemType<MoscowMule>());
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 25, 0, 0);
				nextSlot++;

				shop.item[nextSlot].SetDefaults(ModContent.ItemType<CinnamonRoll>());
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 25, 0, 0);
				nextSlot++;

				shop.item[nextSlot].SetDefaults(ModContent.ItemType<TequilaSunrise>());
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 30, 0, 0);
				nextSlot++;
			}

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<BlueCandle>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(2, 0, 0, 0);
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<PinkCandle>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(2, 0, 0, 0);
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<PurpleCandle>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(2, 0, 0, 0);
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<YellowCandle>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(2, 0, 0, 0);
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<OddMushroom>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(3, 0, 0, 0);
            nextSlot++;
        }

        // Make this Town NPC teleport to the Queen statue when triggered.
        public override bool CanGoToStatue(bool toKingStatue) => !toKingStatue;

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 15;
            knockback = 2f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 180;
            randExtraCooldown = 60;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<FabRay>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 2f;
        }
    }
}
