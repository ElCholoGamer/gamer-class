﻿using GamerClass.Buffs;
using GamerClass.Items;
using GamerClass.Items.Dyes;
using GamerClass.Items.Weapons;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace GamerClass
{
    public class GamerPlayer : ModPlayer
    {
        private int screenShake;
        public int ScreenShake
        {
            get => screenShake;
            set => screenShake = (int)(value * (ModContent.GetInstance<GamerConfig>().ScreenShakeIntensity / 100f));
        }

        public int jetpackBulletCooldown = 0;

        public bool glasses3D;
        public bool gameBoyVisor;
        public bool swearing;

        public bool linkArmorBonus;
        public bool friskSet;
        public bool masterChiefSet;

        public bool yuyufumo;
        public bool gamerCooldown;
        public bool inked;
        public bool haloShieldCooldown;
        public bool haloShield;

        public int maxRam;
        public int maxRam2;
        public int usedRam;
        public int ramRegenTimer;
        public float ramRegenRate;
        public float ramUsageMult;

        public float gamerDamageMult;
        public float gamerKnockback;
        public float gamerUseTimeMult;
        public int gamerCrit;

        public override void Initialize()
        {
            maxRam = 5;
        }

        public override void ResetEffects() => ResetVariables();

        public override void UpdateDead() => ResetVariables();

        private void ResetVariables()
        {
            if (screenShake > 0)
                screenShake--;

            if (jetpackBulletCooldown > 0)
                jetpackBulletCooldown--;

            glasses3D = false;
            gameBoyVisor = false;
            if (!player.dead) swearing = false;

            linkArmorBonus = false;
            friskSet = false;
            masterChiefSet = false;

            yuyufumo = false;
            gamerCooldown = false;
            inked = false;
            haloShieldCooldown = false;
            haloShield = false;

            maxRam2 = maxRam;
            ramUsageMult = 1f;

            gamerDamageMult = 1f;
            gamerKnockback = 0f;
            gamerUseTimeMult = 1f;
            gamerCrit = 0;
        }

        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (gamerCooldown)
            {
                Vector2 position = player.position;
                position.Y -= player.height / 4;

                Vector2 direction = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2 * 0.8f);

                Dust dust = Dust.NewDustDirect(position, player.width, player.height / 2, DustID.Smoke, Scale: 1.6f);
                dust.velocity = direction * 2f;

                if (Main.rand.NextBool(8))
                {
                    dust = Dust.NewDustDirect(position, player.width, (int)(player.height * 0.75f), DustID.Fire, Scale: 1.5f);
                    dust.velocity *= 2f;
                }
            }

            if (inked)
            {
                if (Main.rand.NextBool(10))
                {
                    Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.BubbleBlock, newColor: Color.Blue);
                    dust.noLight = true;
                    dust.velocity *= 0f;
                    Main.playerDrawDust.Add(dust.dustIndex);
                }

                Color newColor = Color.Lerp(new Color(r, g, b, a), Color.Blue, 0.8f);
                r = newColor.R / 255f;
                g = newColor.G / 255f;
                b = newColor.B / 255f;
                a = newColor.A / 255f;
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (!gamerCooldown)
            {
                if (usedRam > 0 && ++ramRegenTimer > 20f * ramRegenRate)
                {
                    usedRam = (int)MathHelper.Max(usedRam - 1, 0);
                    ramRegenTimer = 0;
                }
                else
                {
                    ramRegenRate *= 0.997f;
                }
            }

            if (Main.netMode != NetmodeID.Server)
            {
                if (player.dye[0].type == ModContent.ItemType<BendyDye>()
                    && player.dye[1].type == player.dye[0].type
                    && player.dye[2].type == player.dye[1].type
                    && ModContent.GetInstance<GamerConfig>().OldMovieEffect)
                {
                    GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<BendyDye>()).UseOpacity(0f);

                    if (!Filters.Scene["GamerClass:OldMovie"].IsActive())
                    {
                        Filters.Scene.Activate("GamerClass:OldMovie").GetShader()
                            .UseOpacity(1f)
                            .UseProgress(6f)
                            .UseIntensity(4f)
                            .UseColor(new Color(240, 160, 100))
                            .UseImage(mod.GetTexture("Textures/Scratches"));
                    }
                }
                else
                {
                    GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<BendyDye>()).UseOpacity(1f);

                    if (Filters.Scene["GamerClass:OldMovie"].IsActive())
                    {
                        Filters.Scene["GamerClass:OldMovie"].GetShader().UseOpacity(0f);
                        Filters.Scene.Deactivate("GamerClass:OldMovie");
                    }
                }
            }
        }

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            if (haloShield)
            {
                int shieldShader = GameShaders.Armor.GetShaderIdFromItemId(ModContent.ItemType<HaloShieldDye>());
                drawInfo.bodyArmorShader = shieldShader;
                drawInfo.headArmorShader = shieldShader;
                drawInfo.legArmorShader = shieldShader;
                drawInfo.wingShader = shieldShader;
                drawInfo.handOnShader = shieldShader;
                drawInfo.handOffShader = shieldShader;
                drawInfo.shoeShader = shieldShader;
            }
        }

        public override void PostUpdate()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                if (glasses3D)
                {
                    if (!Filters.Scene["GamerClass:Glasses3D"].IsActive())
                    {
                        Filters.Scene.Activate("GamerClass:Glasses3D").GetShader().UseOpacity(0.3f).UseProgress(18f);
                    }
                }
                else if (Filters.Scene["GamerClass:Glasses3D"].IsActive())
                {
                    Filters.Scene["GamerClass:Glasses3D"].GetShader().UseProgress(0f);
                    Filters.Scene.Deactivate("GamerClass:Glasses3D");
                }

                if (gameBoyVisor)
                {
                    if (!Filters.Scene["GamerClass:GameBoyVisor"].IsActive())
                    {
                        Filters.Scene.Activate("GamerClass:GameBoyVisor").GetShader().UseProgress(1f);
                    }
                }
                else if (Filters.Scene["GamerClass:GameBoyVisor"].IsActive())
                {
                    Filters.Scene["GamerClass:GameBoyVisor"].GetShader().UseProgress(0f);
                    Filters.Scene.Deactivate("GamerClass:GameBoyVisor");
                }
            }
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (crit && friskSet && item.modItem is GamerWeapon)
                SpawnUndertaleSoul(target.Center);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (crit && friskSet && proj.GamerProjectile().gamer)
                SpawnUndertaleSoul(target.Center);
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (masterChiefSet && GamerClass.HaloShieldHotKey.JustPressed && !haloShieldCooldown)
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/HaloShield"));

                player.AddBuff(ModContent.BuffType<HaloShield>(), 300);
                player.AddBuff(ModContent.BuffType<HaloShieldCooldown>(), 2700);
            }
        }

        public override void ModifyScreenPosition()
        {
            if (screenShake > 0)
            {
                Main.screenPosition.X += Main.rand.NextFloat(-screenShake, screenShake);
                Main.screenPosition.Y += Main.rand.NextFloat(-screenShake, screenShake);
            }
        }

        private void SpawnUndertaleSoul(Vector2 position)
        {
            var type = new WeightedRandom<int>();

            type.Add(ModContent.ItemType<RedSoul>());
            type.Add(ModContent.ItemType<BlueSoul>());
            type.Add(ModContent.ItemType<GreenSoul>());
            type.Add(ModContent.ItemType<PurpleSoul>());
            type.Add(ModContent.ItemType<YellowSoul>());
            type.Add(ModContent.ItemType<OrangeSoul>());
            type.Add(ModContent.ItemType<LightBlueSoul>());

            Item.NewItem(position, type, noBroadcast: true);
        }

        public bool ConsumeRam(int amount, int regenDelay)
        {
            if (gamerCooldown) return false;

            ramRegenTimer = -regenDelay;
            ramRegenRate = 1f;
            amount = (int)Math.Round(amount * ramUsageMult);

            if (amount <= 0) return true;

            usedRam += amount;

            if (usedRam >= maxRam2)
            {
                // RAM overheat
                usedRam = maxRam2;
                player.AddBuff(ModContent.BuffType<GamerCooldown>(), 300);

                for (int d = 0; d < 20; d++)
                {
                    bool fire = Main.rand.NextBool(3);

                    int size = 20;
                    Vector2 position = player.Center - new Vector2(1f, 1f) * (size / 2);

                    Dust dust = Dust.NewDustDirect(position, size, size, fire ? DustID.Fire : DustID.Smoke);
                    dust.noGravity = true;
                    dust.fadeIn = 2f;
                    dust.velocity *= fire ? 8f : 4f;
                    dust.scale = 1f;
                }

                for (int g = 0; g < 6; g++)
                {
                    Gore gore = Gore.NewGoreDirect(player.position, Vector2.Zero, 99, Scale: 1.25f);
                    gore.velocity *= 0.75f;
                }

                Main.PlaySound(SoundID.NPCHit53);

                return false;
            }

            return true;
        }
    }
}
