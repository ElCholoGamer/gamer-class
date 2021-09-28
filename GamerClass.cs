using System;
using System.Runtime.CompilerServices;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using GamerClass.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GamerClass
{
    public class GamerClass : Mod
    {
        internal RamUsageBar RamUsageBar;
        private UserInterface _ramUsageBar;

        public override void Load()
        {
            IL.Terraria.NPC.NPCLoot += NPC_NPCLoot;
            IL.Terraria.Player.OpenBossBag += Player_OpenBossBag;

            RamUsageBar = new RamUsageBar();
            RamUsageBar.Activate();

            _ramUsageBar = new UserInterface();
            _ramUsageBar.SetState(RamUsageBar);
        }

        private void NPC_NPCLoot(ILContext il)
        {
            var c = new ILCursor(il);

            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdtoken(out _),
                i => i.MatchCall(typeof(RuntimeHelpers).GetMethod("InitializeArray", new Type[] { typeof(Array), typeof(RuntimeFieldHandle) })),
                i => i.MatchCall(out _),
                i => i.MatchStloc(56)))
            {
                Logger.Error("NPC_NPCLoot IL patch could not apply");
                return;
            }
            else
            {
                c.Index--;
                c.EmitDelegate<Func<int, int>>(itemId =>
                {
                    return 1;
                });
            }
        }

        private void Player_OpenBossBag(ILContext il)
        {
            var c = new ILCursor(il);

            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdtoken(out _),
                i => i.MatchCall(typeof(RuntimeHelpers).GetMethod("InitializeArray", new Type[] { typeof(Array), typeof(RuntimeFieldHandle) })),
                i => i.MatchCall(out _),
                i => i.MatchStloc(11)))
            {
                Logger.Error("Player_OpenBossBag IL patch could not apply");
                return;
            }
            else
            {
                c.Index--;
                c.EmitDelegate<Func<int, int>>(itemId =>
                {
                    return 1;
                });
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _ramUsageBar.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int resourceBarsIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (resourceBarsIndex != -1)
            {
                layers.Insert(resourceBarsIndex, new LegacyGameInterfaceLayer(
                    "GamerClass: RAM Usage",
                    delegate
                    {
                        _ramUsageBar.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}