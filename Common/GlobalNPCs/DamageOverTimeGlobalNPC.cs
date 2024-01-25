


using Terraria;
using Terraria.ModLoader;

namespace TerrariaHbM.Common.GlobalNPCs
{
  internal class DamageOverTimeGlobalNPC : GlobalNPC
  {
    public override bool InstancePerEntity => true;
    public bool mindMeltDebuff;

    public override void ResetEffects(NPC npc)
    {
      mindMeltDebuff = false;
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
      if (mindMeltDebuff)
      {
        if (npc.lifeRegen > 0)
        {
          npc.lifeRegen = 0;
        }

        npc.lifeRegen -= 200;
      }
    }
  }
}
