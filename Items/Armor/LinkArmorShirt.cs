﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GamerClass.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class LinkArmorShirt : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Hero's Shirt");
            Tooltip.SetDefault("Increases gamer damage by 6%");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 20;
            item.value = Item.sellPrice(silver: 90);
            item.rare = ItemRarityID.Orange;
            item.defense = 6;
        }

        public override void UpdateEquip(Player player)
        {
            player.GamerPlayer().gamerDamageMult += 0.06f;
        }
    }
}
