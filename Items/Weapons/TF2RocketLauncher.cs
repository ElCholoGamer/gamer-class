﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GamerClass.Items.Weapons
{
    public class TF2RocketLauncher : GamerWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soldier's Rocket Launcher");
            Tooltip.SetDefault("'Great against red spies in the base'");
        }

        public override void SafeSetDefaults()
        {
            // TODO: rarity, value and balance
            item.width = item.height = 52;
            item.noMelee = true;
            item.damage = 150;
            item.knockBack = 4f;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item11;
            item.useTime = item.useAnimation = 30;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Projectiles.Weapons.TF2Rocket>();
            item.shootSpeed = 20f;

            ramUsage = 3;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);
            Vector2 frontDirection = Vector2.Normalize(velocity);

            Vector2 muzzleOffset = frontDirection * 24f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            for (int d = 0; d < 6; d++)
            {
                Vector2 direction = frontDirection.RotatedByRandom(MathHelper.PiOver4 * 0.7f);

                Dust dust = Dust.NewDustPerfect(position + frontDirection * 28f, DustID.Smoke, Scale: 1.5f);
                dust.velocity = direction * Main.rand.NextFloat(4f, 8f);
                dust.noGravity = true;
            }

            return true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-55f, 2f);
    }
}
