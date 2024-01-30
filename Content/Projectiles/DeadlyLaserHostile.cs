using Terraria;

namespace TerrariaHbM.Content.Projectiles
{
  public class DeadlyLaserHostile : DeadlyLaser
  {
    public override int TimeLeft => 60;
    public override float LaserSize => 0.5f;

    public override void SetDefaults()
    {
      base.SetDefaults();

      Projectile.friendly = false;
      Projectile.hostile = true;
    }
  }
}
