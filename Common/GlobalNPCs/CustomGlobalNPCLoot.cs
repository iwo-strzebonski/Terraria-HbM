using TerrariaHbM.Common.ItemDropRules.DropConditions;
using TerrariaHbM.Content.Items;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaHbM.Common.GlobalNPCs
{
	// This file shows numerous examples of what you can do with the extensive NPC Loot lootable system.
	// You can find more info on the wiki: https://github.com/tModLoader/tModLoader/wiki/Basic-NPC-Drops-and-Loot-1.4
	// Despite this file being GlobalNPC, everything here can be used with a ModNPC as well! See examples of this in the Content/NPCs folder.
	public class CustomGlobalNPCLoot : GlobalNPC
	{
		// ModifyNPCLoot uses a unique system called the ItemDropDatabase, which has many different rules for many different drop use cases.
		// Here we go through all of them, and how they can be used.
		// There are tons of other examples in vanilla! In a decompiled vanilla build, GameContent/ItemDropRules/ItemDropDatabase adds item drops to every single vanilla NPC, which can be a good resource.

		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
		{
			if (!NPCID.Sets.CountsAsCritter[npc.type])
			{ // If npc is not a critter
				// Make it drop ExampleItem.
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ExampleItem>(), 1));

				// Drop an ExampleResearchPresent in journey mode with 2/7ths base chance, but only in journey mode
				npcLoot.Add(ItemDropRule.ByCondition(new ExampleJourneyModeDropCondition(), ModContent.ItemType<ExampleResearchPresent>(), chanceDenominator: 7, chanceNumerator: 2));
			}

			// We will now use the Guide to explain many of the other types of drop rules.
			if (npc.type == NPCID.Guide)
			{
				// RemoveWhere will remove any drop rule that matches the provided expression.
				// To make your own expressions to remove vanilla drop rules, you'll usually have to study the original source code that adds those rules.
				npcLoot.RemoveWhere(
					// The following expression returns true if the following conditions are met:
					rule => rule is ItemDropWithConditionRule drop // If the rule is an ItemDropWithConditionRule instance
						&& drop.itemId == ItemID.GreenCap // And that instance drops a green cap
						&& drop.condition is Conditions.NamedNPC npcNameCondition // ..And if its condition is that an npc name must match some string
						&& npcNameCondition.neededName == "Andrew" // And the condition's string is "Andrew".
				);

				npcLoot.Add(ItemDropRule.Common(ItemID.GreenCap, 1)); // In conjunction with the above removal, this makes it so a guide with any name will drop the Green Cap.
			}

			// Editing an existing drop rule
			if (npc.type == NPCID.BloodNautilus)
			{
				foreach (var rule in npcLoot.Get())
				{
					// You must study the vanilla code to know what to objects to cast to.
					if (rule is DropBasedOnExpertMode drop && drop.ruleForNormalMode is CommonDrop normalDropRule && normalDropRule.itemId == ItemID.SanguineStaff)
						normalDropRule.chanceDenominator = 3;
				}
			}
		}
	}
}
