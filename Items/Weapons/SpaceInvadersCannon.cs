﻿using GamerClass.Projectiles.Weapons.DustyLaserCannon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GamerClass.Items.Weapons
{
    public class SpaceInvadersCannon : GamerWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Old Joystick");
            Tooltip.SetDefault("'There's some sort of martian scribble on it'");
        }

        public override void SafeSetDefaults()
        {
            // TODO: rarity and value
            item.noMelee = true;
            item.damage = 25;
            item.knockBack = 2f;
            item.useTime = item.useAnimation = 40;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/SpaceInvadersCannon");
            item.shoot = ModContent.ProjectileType<ShoddyBeam>();
            item.shootSpeed = 20;

            ramUsage = 2;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 12f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.Granite, 30);
            recipe.AddIngredient(ItemID.StoneBlock, 10);
            recipe.AddIngredient(ItemID.WhiteString);

            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
