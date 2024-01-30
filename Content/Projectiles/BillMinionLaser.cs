using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaHbM.Content.Projectiles
{
  public class BillMinionLaser : ModProjectile
  {
    // public override string Texture => TerrariaHbM.AssetPath + $"Textures/Projectiles/BillMinionLaser/BillMinionLaser";
    override public string Texture => "Terraria/Images/Projectile_" + ProjectileID.DeathLaser;


    public override void SetDefaults()
    {
      base.SetDefaults();

      Projectile.CloneDefaults(ProjectileID.DeathLaser);
      AIType = ProjectileID.DeathLaser;
      Projectile.friendly = false;
      Projectile.hostile = true;
      Projectile.DamageType = DamageClass.Magic;
    }

    public override void AI()
    {
      base.AI();

      Projectile.velocity *= 1.1f;
    }
  }
}
