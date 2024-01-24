using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

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
		}

		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(silver: 1, copper: 50);
			Item.maxStack = 9999;
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
	}
}
