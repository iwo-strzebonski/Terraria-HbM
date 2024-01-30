using TerrariaHbM.Common.Configs;
using TerrariaHbM.Content.Dusts;
using TerrariaHbM.Content.EmoteBubbles;
using TerrariaHbM.Content.Items;
using TerrariaHbM.Content.Items.Accessories;
using TerrariaHbM.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using TerrariaHbM.Content.Buffs;
using System.Linq;

namespace TerrariaHbM.Content.NPCs
{
	// [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
	[AutoloadHead]
	public class YoseiKamisato : ModNPC
	{
		public const string ShopName = "Yosei's Minor Trades";
		public int NumberOfTimesTalkedTo = 0;

		private static int ShimmerHeadIndex;
		private static Profiles.StackedNPCProfile NPCProfile;

		public override string Texture => TerrariaHbM.AssetPath + $"Textures/NPCs/{Name}/{Name}";


		public override void LoadData(TagCompound tag)
		{
			NumberOfTimesTalkedTo = tag.GetInt("numberOfTimesTalkedTo");
		}

		public override void SaveData(TagCompound tag)
		{
			tag["numberOfTimesTalkedTo"] = NumberOfTimesTalkedTo;
		}


		public override void Load()
		{
			// Adds our Shimmer Head to the NPCHeadLoader.
			// ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
		}

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 25; // The total amount of frames the NPC has

			NPCID.Sets.ExtraFramesCount[Type] = 9; // Generally for Town NPCs, but this is how the NPC does extra things such as sitting in a chair and talking to other NPCs. This is the remaining frames after the walking frames.
			NPCID.Sets.AttackFrameCount[Type] = 4; // The amount of frames in the attacking animation.
			NPCID.Sets.DangerDetectRange[Type] = 600; // The amount of pixels away from the center of the NPC that it tries to attack enemies.
			NPCID.Sets.AttackType[Type] = 2; // The type of attack the Town NPC performs. 0 = throwing, 1 = shooting, 2 = magic, 3 = melee
			NPCID.Sets.AttackTime[Type] = 40; // The amount of time it takes for the NPC's attack animation to be over once it starts.
			NPCID.Sets.AttackAverageChance[Type] = 30; // The denominator for the chance for a Town NPC to attack. Lower numbers make the Town NPC appear more aggressive.
			NPCID.Sets.HatOffsetY[Type] = 4; // For when a party is active, the party hat spawns at a Y offset.
			NPCID.Sets.ShimmerTownTransform[NPC.type] = false; // This set says that the Town NPC has a Shimmered form. Otherwise, the Town NPC will become transparent when touching Shimmer like other enemies.

			NPCID.Sets.ShimmerTownTransform[Type] = false; // Allows for this NPC to have a different texture after touching the Shimmer liquid.

			// Connects this NPC with a custom emote.
			// This makes it when the NPC is in the world, other NPCs will "talk about him".
			// By setting this you don't have to override the PickEmote method for the emote to appear.
			//NPCID.Sets.FaceEmote[Type] = ModContent.EmoteBubbleType<SuspiciosYellowTriangleEmote>();

			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Velocity = 1f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
				Direction = 1 // -1 is left and 1 is right. NPCs are drawn facing the left by default but SuspiciousYellowTriangle will be drawn facing the right
											// Rotation = MathHelper.ToRadians(180) // You can also change the rotation of an NPC. Rotation is measured in radians
											// If you want to see an example of manually modifying these when the NPC is drawn, see PreDraw
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

			// Set Example Person's biome and neighbor preferences with the NPCHappiness hook. You can add happiness text and remarks with localization (See an example in TerrariaHbM/Localization/en-US.lang).
			// NOTE: The following code uses chaining - a style that works due to the fact that the SetXAffection methods return the same NPCHappiness instance they're called on.
			NPC.Happiness
				.SetBiomeAffection<ForestBiome>(AffectionLevel.Like) // Example Person prefers the forest.
				.SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike) // Example Person dislikes the snow.
				.SetNPCAffection(NPCID.Dryad, AffectionLevel.Love) // Loves living near the dryad.
				.SetNPCAffection(NPCID.Guide, AffectionLevel.Like) // Likes living near the guide.
				.SetNPCAffection(NPCID.Merchant, AffectionLevel.Dislike) // Dislikes living near the merchant.
				.SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Hate) // Hates living near the demolitionist.
			; // < Mind the semicolon!

			// This creates a "profile" for SuspiciousYellowTriangle, which allows for different textures during a party and/or while the NPC is shimmered.
			NPCProfile = new Profiles.StackedNPCProfile(
				new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture), Texture)
			//new Profiles.DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex, Texture + "_Shimmer_Party")
			);

		}

		public override void SetDefaults()
		{
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.width = 18;
			NPC.height = 40;
			NPC.aiStyle = 7;
			NPC.damage = 20;
			NPC.defense = 10;
			NPC.lifeMax = 250;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.8f;

			AnimationType = NPCID.Guide; // This NPC will copy the Guide's animation
		}

		public override bool CanTownNPCSpawn(int numTownNPCs)
		{ // Requirements for the town NPC to spawn.
			for (int k = 0; k < Main.maxPlayers; k++)
			{
				Player player = Main.player[k];

				if (!player.active)
				{
					continue;
				}

				if (player.inventory.Any(item => item.type == ItemID.WaterBolt))
				{
					return true;
				}
			}

			return false;
		}


		public override ITownNPCProfile TownNPCProfile()
		{
			return NPCProfile;
		}

		public override List<string> SetNPCNameList()
		{
			return new List<string>()
			{
				"Kamisato Yosei"
			};
		}

		public override string GetChat()
		{
			NumberOfTimesTalkedTo++;

			WeightedRandom<string> chat = new WeightedRandom<string>();

			AddBaseDialogues(ref chat);
			AddSpecialDialogues(ref chat);

			string chosenChat = chat; // chat is implicitly cast to a string. This is where the random choice is made.

			// Here is some additional logic based on the chosen chat line. In this case, we want to display an item in the corner for StandardDialogue4.
			if (chosenChat == Language.GetTextValue($"Mods.TerrariaHbM.Dialogue.{Name}.StandardDialogue1"))
			{
				// Main.npcChatCornerItem shows a single item in the corner, like the Angler Quest chat.
				Main.npcChatCornerItem = ModContent.ItemType<ExampleItem>();
			}

			return chosenChat;
		}

		public override void OnKill()
		{
			Main.NewText("Yosei didn't bounce.");

			base.OnKill();
		}

		private void AddBaseDialogues(ref WeightedRandom<string> chat)
		{
			// These are things that the NPC has a chance of telling you when you talk to it.
			chat.Add(Language.GetTextValue($"Mods.TerrariaHbM.Dialogue.{Name}.StandardDialogue1"));
			chat.Add(Language.GetTextValue($"Mods.TerrariaHbM.Dialogue.{Name}.StandardDialogue2"));
			chat.Add(Language.GetTextValue($"Mods.TerrariaHbM.Dialogue.{Name}.StandardDialogue3"));
			chat.Add(Language.GetTextValue($"Mods.TerrariaHbM.Dialogue.{Name}.StandardDialogue4"));
			chat.Add(Language.GetTextValue($"Mods.TerrariaHbM.Dialogue.{Name}.CommonDialogue"), 5.0);
			chat.Add(Language.GetTextValue($"Mods.TerrariaHbM.Dialogue.{Name}.RareDialogue"), 0.1);

			if (NumberOfTimesTalkedTo >= 10)
			{
				//This counter is linked to a single instance of the NPC, so if SuspiciousYellowTriangle is killed, the counter will reset.
				chat.Add(Language.GetTextValue($"Mods.TerrariaHbM.Dialogue.{Name}.TalkALot"));
			}
		}

		// TODO: Add more special dialogues
		private void AddSpecialDialogues(ref WeightedRandom<string> chat)
		{
			int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
			if (partyGirl >= 0 && Main.rand.NextBool(4))
			{
				chat.Add(Language.GetTextValue($"Mods.TerrariaHbM.Dialogue.{Name}.PartyGirlDialogue", Main.npc[partyGirl].GivenName));
			}
		}

		public override void SetChatButtons(ref string button, ref string button2)
		{ // What the chat buttons are when you open up the chat UI
			button = Language.GetTextValue("LegacyInterface.28");
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shop)
		{
			if (firstButton)
			{
				// We want 3 different functionalities for chat buttons, so we use HasItem to change button 1 between a shop and upgrade action.

				shop = ShopName; // Name of the shop tab we want to open.
			}
		}

		// Not completely finished, but below is what the NPC will sell
		public override void AddShops()
		{
			var npcShop = new NPCShop(Type, ShopName)
				.Add<ExampleItem>();

			if (ModContent.TryFind("SummonersAssociation/BloodTalisman", out ModItem bloodTalisman))
			{
				npcShop.Add(bloodTalisman.Type);
			}

			npcShop.Register(); // Name of this shop tab
		}

		public override void ModifyActiveShop(string shopName, Item[] items)
		{
			foreach (Item item in items)
			{
				// Skip 'air' items and null items.
				if (item == null || item.type == ItemID.None)
				{
					continue;
				}

				// If NPC is shimmered then reduce all prices by 50%.
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			// npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ExampleCostume>()));
		}

		// Make this Town NPC teleport to the King and/or Queen statue when triggered. Return toKingStatue for only King Statues. Return !toKingStatue for only Queen Statues. Return true for both.
		public override bool CanGoToStatue(bool toKingStatue) => toKingStatue;

		// Make something happen when the npc teleports to a statue. Since this method only runs server side, any visual effects like dusts or gores have to be synced across all clients manually.
		public override void OnGoToStatue(bool toKingStatue)
		{
			if (Main.netMode == NetmodeID.Server)
			{
				ModPacket packet = Mod.GetPacket();
				// packet.Write((byte)TerrariaHbM.MessageType.ExampleTeleportToStatue);
				packet.Write((byte)NPC.whoAmI);
				packet.Send();
			}
			else
			{
				StatueTeleport();
			}
		}

		// Create a square of pixels around the NPC on teleport.
		public void StatueTeleport()
		{
			for (int i = 0; i < 30; i++)
			{
				Vector2 position = Main.rand.NextVector2Square(-20, 21);
				if (Math.Abs(position.X) > Math.Abs(position.Y))
				{
					position.X = Math.Sign(position.X) * 20;
				}
				else
				{
					position.Y = Math.Sign(position.Y) * 20;
				}

				Dust.NewDustPerfect(NPC.Center + position, ModContent.DustType<Sparkle>(), Vector2.Zero).noGravity = true;
			}
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = NPC.damage / 2;
			knockback = 2f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 360;
			randExtraCooldown = 120;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ProjectileID.WaterBolt;
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 16f;
			randomOffset = 0.5f;
		}

		// Let the NPC "talk about" minion boss
		public override int? PickEmote(Player closestPlayer, List<int> emoteList, WorldUIAnchor otherAnchor)
		{
			// By default this NPC will have a chance to use the Minion Boss Emote even if Minion Boss is not downed yet
			// int type = ModContent.EmoteBubbleType<MinionBossEmote>();
			int type = 0;
			// If the NPC is talking to the Demolitionist, it will be more likely to react with angry emote
			if (otherAnchor.entity is NPC { type: NPCID.Demolitionist })
			{
				type = EmoteID.EmotionAnger;
			}

			// Make the selection more likely by adding it to the list multiple times
			for (int i = 0; i < 4; i++)
			{
				emoteList.Add(type);
			}

			// Use this or return null if you don't want to override the emote selection totally
			return base.PickEmote(closestPlayer, emoteList, otherAnchor);
		}
	}
}