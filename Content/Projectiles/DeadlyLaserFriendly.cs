using Terraria;

namespace TerrariaHbM.Content.Projectiles
{
  public class DeadlyLaserFriendly : DeadlyLaser
  {

    public override void SetDefaults()
    {
      base.SetDefaults();

      Projectile.friendly = true;
      Projectile.hostile = false;
    }
  }
}
