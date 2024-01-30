using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TerrariaHbM.Content.Bosses.BillCipher;
using TerrariaHbM.Content.NPCs;

namespace TerrariaHbM.Content.Items
{
	/// <summary>
	/// This item showcases one of the ways for you to do something when an item is bought from an NPC with a shop.
	/// </summary>
	public class OminousPlushie : ModItem
	{
		public static LocalizedText DeathMessage { get; private set; }
		public override string Texture => TerrariaHbM.AssetPath + "Textures/Items/OminousPlushie";

		public override void SetStaticDefaults()
		{
			// See the localization files for more info! (Localization/en-US.hjson)
			DeathMessage = this.GetLocalization(nameof(DeathMessage));
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
		}

		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.rare = ItemRarityID.Quest;
			Item.value = Item.buyPrice(silver: 1, copper: 50);
			Item.maxStack = 9999;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = true;
		}

		// Note that alternatively, you can use the ModPlayer.PostBuyItem hook to achieve the same functionality!
		public override void OnCreated(ItemCreationContext context)
		{
			if (context is not BuyItemCreationContext buyContext)
			{
				return;
			}

			SoundStyle thunder = SoundID.Thunder;
			thunder.Variants = new int[] { 4 };
			SoundEngine.PlaySound(thunder, buyContext.VendorNPC.position);

			// For fun, we'll give the buying player a 50% chance to die whenever they buy this item from an NPC.
			// if (!Main.rand.NextBool())
			// {
			// 	return;
			// }

			// This is only ever called on the local client, so the local player will do.
			Player player = Main.LocalPlayer;
			player.KillMe(PlayerDeathReason.ByCustomReason(DeathMessage.Format(player.name)), 9999, 0);
			Main.NewText("Bill Cipher: [c/FFFF00:HAHAHAHA! You seriously thought that nothing will happen?.]");
		}


		public override bool CanUseItem(Player player)
		{
			// If you decide to use the below UseItem code, you have to include !NPC.AnyNPCs(id), as this is also the check the server does when receiving MessageID.SpawnBoss.
			// If you want more constraints for the summon item, combine them as boolean expressions:
			//    return !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<MinionBossBody>()); would mean "not daytime and no MinionBossBody currently alive"
			return !NPC.AnyNPCs(ModContent.NPCType<BillCipherBoss>());
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				// If the player using the item is the client
				// (explicitly excluded serverside here)
				SoundEngine.PlaySound(SoundID.Roar, player.position);

				int type = ModContent.NPCType<BillCipherBoss>();
				bool townNPCfound = false;

				// We also want to kill town NPCs, so we'll loop through every NPC in the world
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];

					if (npc.active && npc.type == ModContent.NPCType<SuspiciousYellowTriangle>())
					{
						// If we find one, we kill it and spawn the boss
						townNPCfound = true;

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							npc.StrikeInstantKill();
						}
						else
						{
							NetMessage.SendData(MessageID.DamageNPC, npc.whoAmI, number: npc.lifeMax, number2: 1f, number3: 0f, number4: 0f);
						}
					}
				}

				if (!townNPCfound)
				{
					// If we didn't find Bill Cipher NPC, don't spawn the boss
					return false;
				}

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					// If the player is not in multiplayer, spawn directly
					NPC.SpawnOnPlayer(player.whoAmI, type);
				}
				else
				{
					// If the player is in multiplayer, request a spawn
					// This will only work if NPCID.Sets.MPAllowedEnemies[type] is true, which we set in MinionBossBody
					NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
				}
			}

			return true;
		}

	}
}
