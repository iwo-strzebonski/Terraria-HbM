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
using Microsoft.Xna.Framework.Graphics;

namespace TerrariaHbM.Content.NPCs
{
  // [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
  [AutoloadHead]
  public class SiergiejZhukov : ModNPC
  {
    public const string ShopName = "Military Offers";
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

      NPCID.Sets.ExtraFramesCount[Type] = 9;
      NPCID.Sets.AttackFrameCount[Type] = 4;
      NPCID.Sets.DangerDetectRange[Type] = 1500;
      NPCID.Sets.AttackType[Type] = 1;
      NPCID.Sets.AttackTime[Type] = 30;
      NPCID.Sets.AttackAverageChance[Type] = 5;
      NPCID.Sets.HatOffsetY[Type] = 4;
      NPCID.Sets.ShimmerTownTransform[NPC.type] = false;

      NPCID.Sets.ShimmerTownTransform[Type] = false;

      NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
      {
        Velocity = 1f,
        Direction = 1
      };

      NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

      NPCProfile = new Profiles.StackedNPCProfile(
        new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture), Texture)
      );
    }

    public override void SetDefaults()
    {
      NPC.townNPC = true;
      NPC.friendly = true;
      NPC.width = 18;
      NPC.height = 40;
      NPC.aiStyle = 7;
      NPC.damage = 200;
      NPC.defense = 50;
      NPC.lifeMax = 1000;
      NPC.HitSound = SoundID.NPCHit1;
      NPC.DeathSound = SoundID.NPCDeath1;
      NPC.knockBackResist = 0.75f;

      AnimationType = NPCID.TacticalSkeleton;
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

        return true;

        // Player has to have either an ExampleItem or an ExampleBlock in order for the NPC to spawn
        // if (player.inventory.Any(item => item.type == ModContent.ItemType<ExampleItem>() || item.type == ModContent.ItemType<Items.Placeable.ExampleBlock>())) {
        // 	return true;
        // }
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
        "Siergiej Zhukov",
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
      if (chosenChat == Language.GetTextValue($"Mods.TerrariaHbM.Dialogue.{Name}.StandardDialogue4"))
      {
        // Main.npcChatCornerItem shows a single item in the corner, like the Angler Quest chat.
        Main.npcChatCornerItem = ModContent.ItemType<ExampleItem>();
      }

      return chosenChat;
    }

    public override void OnKill()
    {
      Main.NewText("<Colonel Siergiej Zhukov> [c/FF00FF:The Federation will remember this.]");

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
      button2 = "Classified";

      if (Main.LocalPlayer.HasItem(ItemID.Musket))
      {
        button2 = "Exchange old weapons";
      }
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shop)
    {
      if (firstButton)
      {

        shop = ShopName; // Name of the shop tab we want to open.
      }
      else
      {
        if (Main.LocalPlayer.HasItem(ItemID.Musket))
        {
          SoundEngine.PlaySound(SoundID.Item37); // Reforge/Anvil sound

          Main.npcChatText = $"I exchanged {Lang.GetItemNameValue(ItemID.Musket)} to a {Lang.GetItemNameValue(ItemID.TacticalShotgun)}. Thank you for trusting the Federation with your security.";

          int itemIndex = Main.LocalPlayer.FindItem(ItemID.Musket);
          var entitySource = NPC.GetSource_GiftOrReward();

          Main.LocalPlayer.inventory[itemIndex].TurnToAir();
          Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.TacticalShotgun);

          return;
        }
      }
    }

    // Not completely finished, but below is what the NPC will sell
    public override void AddShops()
    {
      NPCShop npcShop = new NPCShop(Type, ShopName)
        .Add(new Item(ItemID.TacticalShotgun) { shopCustomPrice = Item.buyPrice(platinum: 2) });

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
        if (NPC.IsShimmerVariant)
        {
          int value = item.shopCustomPrice ?? item.value;
          item.shopCustomPrice = value / 2;
        }
      }
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
      npcLoot.Add(ItemDropRule.Common(ItemID.MusketBall, 20, 0, 20));
      npcLoot.Add(ItemDropRule.Common(ItemID.ChlorophyteBullet, 50, 0, 10));
      npcLoot.Add(ItemDropRule.Common(ItemID.CrystalBullet, 50, 0, 10));
      npcLoot.Add(ItemDropRule.Common(ItemID.CursedBullet, 50, 0, 10));
      npcLoot.Add(ItemDropRule.Common(ItemID.ExplodingBullet, 50, 0, 10));
      npcLoot.Add(ItemDropRule.Common(ItemID.GoldenBullet, 50, 0, 10));
      npcLoot.Add(ItemDropRule.Common(ItemID.IchorBullet, 50, 0, 10));
      npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemID.TacticalShotgun));
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
      knockback = 4f;
    }

    public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
    {
      cooldown = 20;
      randExtraCooldown = 10;
    }

    public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
    {
      projType = ModContent.ProjectileType<FakeZhukovsProjectile>();
      attackDelay = 1;
    }

    public override void DrawTownAttackGun(ref Texture2D item, ref Rectangle itemFrame, ref float scale, ref int horizontalHoldoutOffset)
    {
      // item = TextureAssets.Item[ItemID.TacticalShotgun].Value;

      Main.GetItemDrawFrame(ItemID.TacticalShotgun, out item, out itemFrame);
      horizontalHoldoutOffset = (int)Main.DrawPlayerItemPos(1f, ItemID.TacticalShotgun).X - itemFrame.Width / 2;
    }

    public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
    {
      multiplier = 8f;
      randomOffset = 0f;
      // SparklingBall is not affected by gravity, so gravityCorrection is left alone.
    }

    public override void TownNPCAttackShoot(ref bool inBetweenShots)
    {
      inBetweenShots = true;
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