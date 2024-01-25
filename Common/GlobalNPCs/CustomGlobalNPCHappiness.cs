using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaHbM.Common.GlobalNPCs
{
	public class CustomGlobalNPCHappiness : GlobalNPC
	{
		public override void SetStaticDefaults()
		{
			int SuspiciosYellowTriangleType = ModContent.NPCType<Content.NPCs.SuspiciousYellowTriangle>(); // Get SuspiciousYellowTriangle's type
			var guideHappiness = NPCHappiness.Get(NPCID.Guide); // Get the key into The Guide's happiness

			guideHappiness.SetNPCAffection(SuspiciosYellowTriangleType, AffectionLevel.Love); // Make the Guide love SuspiciousYellowTriangle!

			// guideHappiness.SetBiomeAffection<ExampleSurfaceBiome>(AffectionLevel.Love);  // Make the Guide love ExampleSurfaceBiome!
		}
	}
}
