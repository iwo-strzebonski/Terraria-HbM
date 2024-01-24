using TerrariaHbM.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaHbM.Content.Projectiles
{
  public class DeadlyLaser : ModProjectile
  {
    // override public string Texture => TerrariaHbM.AssetPath + $"Textures/Projectiles/{Name}";
    // override public string Texture => ModContent.Request<Texture2D>("Terraria/Projectile_" + ProjectileID.PhantasmalDeathray);
    override public string Texture => "Terraria/Images/Projectile_" + ProjectileID.PhantasmalDeathray;

    public override void SetDefaults()
    {
      // Projectile.CloneDefaults(ProjectileID.PhantasmalDeathray);
      Projectile.CloneDefaults(ProjectileID.DeathLaser);

      // AIType = ProjectileID.PhantasmalDeathray;
      AIType = ProjectileID.DeathLaser;
      Projectile.friendly = true;
      Projectile.hostile = false;
      Projectile.DamageType = DamageClass.Magic;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
      SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
      return base.OnTileCollide(oldVelocity);
    }

    public override void OnKill(int timeLeft)
    {
      for (int k = 0; k < 5; k++)
      {
        Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>(), Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
      }
      SoundEngine.PlaySound(SoundID.Item25, Projectile.position);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
      Projectile.ai[0] += 0.1f;
      Projectile.velocity *= 0.75f;
    }
  }
}
