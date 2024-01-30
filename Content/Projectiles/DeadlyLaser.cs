/**
 * Based on Calamity's Universe Splitter
 * https://github.com/CalamityTeam/CalamityModPublic/blob/1.4.4/Projectiles/Summon/UniverseSplitterHugeBeam.cs
 */

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Enums;
using TerrariaHbM.Content.Buffs;

namespace TerrariaHbM.Content.Projectiles
{
  public class DeadlyLaser : ModProjectile
  {
    override public string Texture => TerrariaHbM.AssetPath + $"Textures/Projectiles/DeadlyLaser/DeadlyLaser";


    public const int TotalFadeoutTime = 25;
    public const float MaximumLength = 3000f;
    public virtual int TimeLeft => 180;
    public virtual float LaserSize => 1f;


    public override void SetDefaults()
    {
      Projectile.width = Projectile.height = 14;
      Projectile.penetrate = -1;
      Projectile.tileCollide = false;
      Projectile.timeLeft = TimeLeft;
      Projectile.usesLocalNPCImmunity = true;
      Projectile.ignoreWater = true;
      Projectile.localNPCHitCooldown = 6;
      Projectile.DamageType = DamageClass.Magic;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
      writer.Write(Projectile.localAI[0]);
      writer.Write(Projectile.localAI[1]);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
      Projectile.localAI[0] = reader.ReadSingle();
      Projectile.localAI[1] = reader.ReadSingle();
    }

    public override void AI()
    {
      Projectile.velocity = (Projectile.velocity.ToRotation() + Projectile.ai[0]).ToRotationVector2();
      Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

      Projectile.localAI[0]++;

      if (Projectile.localAI[0] < TotalFadeoutTime)
      {
        Projectile.scale = MathHelper.Lerp(0.01f, LaserSize, Projectile.localAI[0] / TotalFadeoutTime);
      }

      if (Projectile.timeLeft < TotalFadeoutTime)
      {
        Projectile.scale = MathHelper.Lerp(LaserSize, 0f, 1f - (Projectile.timeLeft / (float)TotalFadeoutTime));
      }

      Vector2 samplingPoint = Projectile.Top;

      float[] samples = new float[12];

      float determinedLength = 0f;
      Collision.LaserScan(samplingPoint, Projectile.velocity, Projectile.width * Projectile.scale, MaximumLength, samples);

      for (int i = 0; i < samples.Length; i++)
      {
        determinedLength += samples[i];
      }

      determinedLength /= samples.Length;

      determinedLength = MathHelper.Clamp(determinedLength, MaximumLength * 0.3f, MaximumLength);

      float lerpDelta = 0.5f;
      Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], determinedLength - 20f, lerpDelta);

      if (Projectile.localAI[0] < 30f)
      {
        Projectile.localAI[1] = MathHelper.Lerp(72f, Projectile.localAI[1], Projectile.localAI[0] / 30f);
      }
      Vector2 beamEndPosition = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 6f);

      if (Projectile.localAI[0] >= 30f &&
        Projectile.localAI[0] <= 35f)
      {
        if (Projectile.localAI[0] == 30f)
          SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

        for (int i = 0; i < 75; i++)
        {
          Dust dust = Dust.NewDustPerfect(beamEndPosition, 269);
          dust.velocity = Main.rand.NextVector2Circular(16f, 11f);
          dust.velocity = dust.velocity.SafeNormalize(Vector2.UnitY) * new Vector2(5f, 3.5f);
          dust.scale = Main.rand.NextFloat(1.2f, 1.5f);
          dust.noGravity = true;

          dust = Dust.NewDustPerfect(beamEndPosition, 269);
          dust.velocity = (i / 75f * MathHelper.TwoPi).ToRotationVector2().RotatedByRandom(0.2f) * new Vector2(16f, 11f) * 1.3f;
          dust.scale = Main.rand.NextFloat(1.4f, 1.75f);
          dust.noGravity = true;
        }
      }

      // Draw acid green light across the laser
      DelegateMethods.v3_1 = new Vector3(0.62f, 0.94f, 0.38f);
      Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], Projectile.width * Projectile.scale, DelegateMethods.CastLight);
    }

    public override bool PreDraw(ref Color lightColor)
    {
      if (Projectile.velocity == Vector2.Zero)
      {
        return false;
      }

      // TODO: Create a texture for this projectile.
      Texture2D laserTailTexture = ModContent.Request<Texture2D>(Texture + "BeamEnd").Value;
      Texture2D laserBodyTexture = ModContent.Request<Texture2D>(Texture + "BeamMid").Value;

      float laserLength = Projectile.localAI[1];
      Color drawColor = new Color(1f, 1f, 1f) * 0.9f;

      // Laser body logic

      laserLength -= laserTailTexture.Height / 2 * Projectile.scale;
      Vector2 centerDelta = Projectile.Center - 76f * Projectile.velocity * Projectile.scale;
      centerDelta += Projectile.velocity * Projectile.scale * laserTailTexture.Height / 2f;

      if (laserLength > 0f)
      {
        float laserLengthDelta = 0f;
        Rectangle sourceRectangle = new Rectangle(0, 76 * (Projectile.timeLeft / 3 % 5), laserBodyTexture.Width, 76);

        while (laserLengthDelta + 1f < laserLength)
        {
          if (laserLength - laserLengthDelta < sourceRectangle.Height)
          {
            sourceRectangle.Height = (int)(laserLength - laserLengthDelta);
          }

          Main.EntitySpriteDraw(
            laserBodyTexture,
            centerDelta - Main.screenPosition,
            new Rectangle?(sourceRectangle),
            drawColor,
            Projectile.rotation,
            new Vector2(sourceRectangle.Width / 2f, 0f),
            Projectile.scale,
            SpriteEffects.None,
            0
          );

          laserLengthDelta += sourceRectangle.Height * Projectile.scale;
          centerDelta += Projectile.velocity * sourceRectangle.Height * Projectile.scale;
          sourceRectangle.Y += (int)(76 * Projectile.scale);

          if (sourceRectangle.Y + sourceRectangle.Height > laserBodyTexture.Height)
          {
            sourceRectangle.Y = 0;
          }
        }
      }

      // Laser tail logic
      centerDelta += Projectile.velocity * Projectile.scale * 38f;
      Rectangle tailFrameRectangle = new Rectangle(0, 76 * (Projectile.timeLeft / 3 % 5), laserTailTexture.Width, 76);
      Main.EntitySpriteDraw(laserTailTexture, centerDelta - Main.screenPosition, tailFrameRectangle, drawColor, Projectile.rotation, new Vector2(148f, 76f) / 2f, Projectile.scale, SpriteEffects.None, 0);

      return false;
    }

    public override bool ShouldUpdatePosition() => false;

    public override void CutTiles()
    {
      DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
      Vector2 unit = Projectile.velocity;
      Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, DelegateMethods.CutTiles);
    }

    public override bool? CanCutTiles() => true;

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
      if (projHitbox.Intersects(targetHitbox))
      {
        return true;
      }
      float value = 0f;

      if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * (Projectile.localAI[1] + 76f), 22f * Projectile.scale, ref value))
      {
        return true;
      }
      return false;
    }

    // In Terraria, 1 second = 60 ticks. In comparison, in Minecraft 1 second = 20 ticks.
    public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<MindMeltDebuff>(), 5 * 60);
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<MindMeltDebuff>(), 5 * 60);

    // public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Electrified, 300);
    // public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Electrified, 300);
  }
}
