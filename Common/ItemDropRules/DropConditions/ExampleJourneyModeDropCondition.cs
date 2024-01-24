using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace TerrariaHbM.Common.ItemDropRules.DropConditions
{
	// Drop condition where items drop only on Journey mode.
	public class ExampleJourneyModeDropCondition : IItemDropRuleCondition
	{
		private static LocalizedText Description;

		public ExampleJourneyModeDropCondition()
		{
			Description ??= Language.GetOrRegister("Mods.TerrariaHbM.DropConditions.JourneyMode");
		}

		public bool CanDrop(DropAttemptInfo info)
		{
			return Main.GameModeInfo.IsJourneyMode;
		}

		public bool CanShowItemDropInUI()
		{
			return true;
		}

		public string GetConditionDescription()
		{
			return Description.Value;
		}
	}
}
