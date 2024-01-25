using Terraria.ModLoader;

namespace TerrariaHbM.Common.Players
{
  public class CustomPlayer : ModPlayer
  {
    public bool mindMeltDebuff;

    public override void ResetEffects()
    {
      mindMeltDebuff = false;
    }

    public override void UpdateDead()
    {
      mindMeltDebuff = false;
    }

    public override void UpdateBadLifeRegen()
    {
      if (mindMeltDebuff)
      {
        // These lines zero out any positive lifeRegen. This is expected for all bad life regeneration debuffs.
        if (Player.lifeRegen > 0)
        {
          Player.lifeRegen = 0;
        }

        // lifeRegenTime is the number of frames until the player's health regen kicks in and actually adds health to the player.
        // Here, we want to decrease the delay before the bad life regen kicks in.
        Player.lifeRegenTime = 0;
        // lifeRegen is the amount of health to subtract from the player's health once the lifeRegenTime has counted down.
        Player.lifeRegen -= 200;
      }
    }
  }
}
