using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaHbM.Content.Projectiles
{
  // This is a simple example of how to spawn multiple projectiles for Town NPCs when they shoot.
  public class FakeZhukovsProjectile : ModProjectile
  {
    override public string Texture => "Terraria/Images/Projectile_" + ProjectileID.Bullet;

    public virtual int TimeLeft => 0;


    private readonly int[] projectiles = new int[] { ProjectileID.ChlorophyteBullet, ProjectileID.CrystalBullet, ProjectileID.CursedBullet, ProjectileID.ExplosiveBullet, ProjectileID.GoldenBullet, ProjectileID.IchorBullet };


    public override void SetDefaults()
    {
      base.SetDefaults();

      Projectile.CloneDefaults(ProjectileID.Bullet);
      AIType = ProjectileID.Bullet;
      Projectile.friendly = true;
      Projectile.hostile = false;
      Projectile.DamageType = DamageClass.Ranged;
      Projectile.timeLeft = TimeLeft;

      Main.rand.NextFromList(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 });
    }

    public override void OnSpawn(IEntitySource source)
    {
      base.OnSpawn(source);

      for (int i = 0; i < projectiles.Length; i++)
      {
        Vector2 position = Projectile.position;
        Vector2 velocity = Projectile.velocity;

        velocity = velocity.RotatedByRandom(MathHelper.ToRadians(Main.rand.NextFloat(-30, 30)));

        // Spawn the Projectile. As the shotgun-like effect is created by a non-player owned Projectile, we have to verify the NetMode.
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
          Projectile.NewProjectile(source, position, velocity, projectiles[i], Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
      }
    }
  }
}
