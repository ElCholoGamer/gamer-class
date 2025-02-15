﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GamerClass.Projectiles.Weapons.DetachedGlove
{
    public class SpreadShot : ModProjectile
    {
        private readonly Color dustColor = new Color(1f, 0.2f, 0.2f);

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 7;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 16;
            projectile.friendly = true;
            projectile.timeLeft = 10;
            projectile.frame = Main.rand.Next(Main.projFrames[projectile.type]);
            projectile.alpha = 255;
            projectile.GamerProjectile().gamer = true;
        }

        public bool LongerLife => projectile.ai[0] == 1f;

        public override void AI()
        {
            if (projectile.GamerProjectile().FirstTick && LongerLife)
            {
                projectile.timeLeft += 6;
            }

            projectile.rotation = projectile.velocity.ToRotation();
            projectile.alpha = (int)MathHelper.Max(projectile.alpha - 100, 0);

            if (++projectile.frameCounter > 2)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }

            if (Main.rand.NextBool(12))
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.BubbleBurst_White, newColor: dustColor);
                dust.noGravity = true;
                dust.velocity = projectile.velocity * 0.06f;
            }

            Lighting.AddLight(projectile.Center, Color.DarkRed.ToVector3());
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 10, 0.6f);
            GamerUtils.DustExplosion(7, projectile.Center, DustID.BubbleBurst_White, 3f, baseRotation: Main.rand.NextFloat(MathHelper.Pi), color: dustColor);

            if (Main.rand.NextBool(3))
            {
                int dusts = 45;
                float separation = MathHelper.TwoPi / dusts;
                float speed = Main.rand.NextFloat(2.5f, 3f);

                for (int d = 0; d < dusts; d++)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.Center, DustID.BubbleBurst_White, newColor: dustColor);
                    dust.velocity = Vector2.UnitY.RotatedBy(d * separation) * speed;
                    dust.noGravity = true;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            this.DrawCentered(spriteBatch, lightColor, flip: false);
            return false;
        }
    }
}
