﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GamerClass.Projectiles.TouhouStick
{
    public class HomingCharm : ModProjectile
    {
        private readonly float RangeSQ = (float)Math.Pow(1000, 2);
        private readonly float rotationSpeed = MathHelper.TwoPi / 180f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 32;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 420;
            projectile.scale = 0.8f;
            projectile.alpha = 255;
            projectile.GamerProjectile().gamer = true;
        }

        public int CurrentTarget
        {
            get => (int)projectile.ai[0];
            set => projectile.ai[0] = value;
        }

        public override void AI()
        {
            projectile.rotation += rotationSpeed;
            projectile.alpha = (int)MathHelper.Max(projectile.alpha - 40, 50);

            FindTarget();

            if (CurrentTarget != -1)
            {
                NPC target = Main.npc[CurrentTarget];

                float speed = 30f;
                float inertia = 12f;
                Vector2 direction = Vector2.Normalize(target.Center - projectile.Center);
                projectile.velocity = (projectile.velocity * (inertia - 1) + direction * speed) / inertia;
            }

            if (Main.rand.NextBool(16))
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.PurificationPowder);
                dust.velocity = Vector2.Normalize(projectile.velocity) * dust.velocity.Length() * 0.5f;
                dust.fadeIn *= 0.2f;
            }
        }

        private void FindTarget()
        {
            if (CurrentTarget > -1) return;

            int currentNPC = -1;
            float currentDistanceSQ = -1f;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.CanBeChasedBy()) continue;

                float distanceSQ = (npc.Center - projectile.Center).LengthSquared();
                if (distanceSQ > RangeSQ) continue;

                if (currentDistanceSQ != -1 && distanceSQ > currentDistanceSQ) continue;

                bool inSight = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
                if (!inSight) continue;

                currentNPC = i;
                currentDistanceSQ = distanceSQ;

            }

            CurrentTarget = currentNPC;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.Center);

            for (int d = 0; d < 5; d++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.PurificationPowder);
                dust.velocity *= 2f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 origin = texture.Size() / 2f;

            Color color = projectile.GetAlpha(lightColor);

            // Afterimages
            int trails = 5;
            for (int i = 1; i <= trails; i++)
            {
                int reverseIndex = trails - i + 1;
                Vector2 position = projectile.Center - projectile.velocity * reverseIndex * 0.3f;

                spriteBatch.Draw(
                    texture,
                    position - Main.screenPosition,
                    null,
                    color * (projectile.Opacity * i * 0.05f),
                    projectile.rotation - rotationSpeed * i,
                    origin,
                    projectile.scale,
                    SpriteEffects.None,
                    0f);
            }

            spriteBatch.Draw(
                texture,
                projectile.Center - Main.screenPosition,
                null,
                color * projectile.Opacity,
                projectile.rotation,
                origin,
                projectile.scale,
                SpriteEffects.None,
                0f);
            return false;
        }
    }
}
