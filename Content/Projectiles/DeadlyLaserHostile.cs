using Terraria;

namespace TerrariaHbM.Content.Projectiles
{
  public class DeadlyLaserHostile : DeadlyLaser
  {

    public override void SetDefaults()
    {
      base.SetDefaults();

      Projectile.friendly = false;
      Projectile.hostile = true;
    }
  }
}
