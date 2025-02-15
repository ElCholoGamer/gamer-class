﻿using GamerClass.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GamerClass
{
    public static class GamerUtils
    {
        public static List<Dust> DustExplosion(
            int points,
            Vector2 position,
            int dustType,
            float size,
            float baseRotation = -MathHelper.PiOver2,
            float dustScale = 1f,
            Color? color = null,
            int dustPerPoint = 4)
        {
            List<Dust> dusts = new List<Dust>();

            Vector2 baseDirection = baseRotation.ToRotationVector2();
            float separation = MathHelper.TwoPi / points;

            float minVelocity = size / 2;
            float maxVelocity = size;

            for (int p = 0; p < points; p++)
            {
                for (int side = -1; side <= 2; side += 2)
                {
                    Vector2 sideDirection = baseDirection.RotatedBy(side * (separation / 2f));

                    for (int d = 0; d < dustPerPoint; d++)
                    {
                        float dustRadius = (float)d / dustPerPoint;

                        Vector2 direction = Vector2.Lerp(baseDirection, sideDirection, dustRadius);
                        float dustVelocity = maxVelocity - ((maxVelocity - minVelocity) * (float)Math.Sin(dustRadius * MathHelper.PiOver2));

                        Dust dust = Dust.NewDustPerfect(position, dustType, newColor: color ?? Color.White);
                        dust.scale = dustScale;
                        dust.velocity = direction * dustVelocity;
                        dust.noGravity = true;

                        dusts.Add(dust);
                    }
                }

                baseDirection = baseDirection.RotatedBy(separation);
            }

            return dusts;
        }

        public static GamerGlobalProjectile GamerProjectile(this Projectile projectile) =>
            projectile.GetGlobalProjectile<GamerGlobalProjectile>();

        public static GamerPlayer GamerPlayer(this Player player) => player.GetModPlayer<GamerPlayer>();

        public static void DrawCentered(this ModProjectile modProj, SpriteBatch spriteBatch, Color lightColor, bool flip = true)
        {
            Texture2D texture = Main.projectileTexture[modProj.projectile.type];
            int frameHeight = texture.Height / Main.projFrames[modProj.projectile.type];

            Rectangle sourceRectangle = new Rectangle(0, modProj.projectile.frame * frameHeight, texture.Width, frameHeight);
            Color color = modProj.projectile.GetAlpha(lightColor);
            Vector2 origin = sourceRectangle.Size() / 2;
            SpriteEffects spriteEffects = (modProj.projectile.direction == 1 || !flip) ? SpriteEffects.None : SpriteEffects.FlipVertically;

            int trails = ProjectileID.Sets.TrailCacheLength[modProj.projectile.type];
            for (int i = 0; i < trails; i++)
            {
                spriteBatch.Draw(
                    texture,
                    modProj.projectile.oldPos[i] + modProj.projectile.Size / 2f - Main.screenPosition,
                    sourceRectangle,
                    color * 0.5f * (1f - ((float)i / trails)),
                    modProj.projectile.oldRot[i],
                    origin,
                    modProj.projectile.scale,
                    spriteEffects,
                    0f);
            }
            
            spriteBatch.Draw(
                texture,
                modProj.projectile.Center - Main.screenPosition,
                sourceRectangle,
                color,
                modProj.projectile.rotation,
                origin,
                modProj.projectile.scale,
                spriteEffects,
                0f);
        }

        public static void AreaDamage(int damage, float knockBack, Vector2 damageSource, Rectangle hitbox, Func<NPC, bool> predicate = null)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (
                    npc.active
                    && !npc.dontTakeDamage
                    && (predicate == null || predicate(npc))
                    && Collision.CanHitLine(damageSource, 0, 0, npc.Center, 0, 0)
                    && npc.Hitbox.Intersects(hitbox)
                    )
                {
                    int hitDirection = Math.Sign(npc.Center.X - damageSource.X);
                    npc.StrikeNPC(damage / 2, knockBack * 0.5f, hitDirection);
                }
            }
        }

        public static bool CheckAABBvCircleCollision(Vector2 objectPosition, Vector2 objectDimensions, Vector2 circleCenter, float radius)
        {
            Vector2 rectCenter = objectPosition + objectDimensions / 2;
            Vector2 circleDistance = new Vector2(Math.Abs(circleCenter.X - rectCenter.X), Math.Abs(circleCenter.Y - rectCenter.Y));
            
            if (circleDistance.X > (objectDimensions.X / 2) + radius) return false;
            if (circleDistance.Y > (objectDimensions.Y / 2) + radius) return false;

            if (circleDistance.X <= (objectDimensions.X / 2)) return true;
            if (circleDistance.Y <= (objectDimensions.Y / 2)) return true;

            return Vector2.DistanceSquared(circleDistance, objectDimensions / 2) <= Math.Pow(radius, 2);
        }
    }
}
