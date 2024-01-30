using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaHbM.Common.GlobalNPCs;
using TerrariaHbM.Common.Players;

namespace TerrariaHbM.Content.Buffs
{
  public class MindMeltDebuff : ModBuff
  {

    // TODO: Create a texture for this debuff.
    // public override string Texture => TerrariaHbM.AssetPath + $"Textures/Buffs/{Name}";
    public override string Texture => TerrariaHbM.AssetPath + "Textures/Buffs/DebuffTemplate";

    public override void SetStaticDefaults()
    {
      Main.debuff[Type] = true;
      Main.pvpBuff[Type] = true;
      Main.buffNoSave[Type] = true;
      BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
      BuffID.Sets.LongerExpertDebuff[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
      player.GetModPlayer<CustomPlayer>().mindMeltDebuff = true;
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
      npc.GetGlobalNPC<DamageOverTimeGlobalNPC>().mindMeltDebuff = true;
    }
  }
}