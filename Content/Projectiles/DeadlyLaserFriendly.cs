using Terraria;

namespace TerrariaHbM.Content.Projectiles
{
  public class DeadlyLaserFriendly : DeadlyLaser
  {
    public override float LaserSize => 0.125f;

    public override void SetDefaults()
    {
      base.SetDefaults();

      Projectile.friendly = true;
      Projectile.hostile = false;
    }
  }
}
