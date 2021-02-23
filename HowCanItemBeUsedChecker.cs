using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Items;
using Kingmaker.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InventoryTweaks
{
    public static class HowCanItemBeUsedChecker
    {
        private static readonly Color defaultColor = Color.black;
        private static readonly Color useMagicDeviceColor = new Color(0.5f, 0.2f, 0.5f);
        private static readonly Color cannotUseColor = new Color(0.4f, 0.1f, 0.1f);

        public static string GetDescriptiveText(HowCanItemBeUsed how)
        {
            if (how == HowCanItemBeUsed.InSpellList)
                return "(spell)";
            if (how == HowCanItemBeUsed.UseMagicDevice)
                return "(umd)";
            if (how == HowCanItemBeUsed.CannotUse)
                return "(cannot)";
            return ""; // how == HowCanItemBeUsed.NotLimited
        }

        public static Color GetColor(HowCanItemBeUsed how)
        {
            if (how == HowCanItemBeUsed.InSpellList)
                return defaultColor;
            if (how == HowCanItemBeUsed.UseMagicDevice)
                return useMagicDeviceColor;
            if (how == HowCanItemBeUsed.CannotUse)
                return cannotUseColor;
            return defaultColor; // how == HowCanItemBeUsed.NotLimited
        }

        public static HowCanItemBeUsed Check(ItemEntity item, UnitEntityData unit)
        {
            var itemBlueprint = item.Blueprint as BlueprintItemEquipmentUsable;
            if (itemBlueprint == null)
                return HowCanItemBeUsed.CannotUse;

            if (itemBlueprint.Type == UsableItemType.Potion)
                return HowCanItemBeUsed.NotLimited;

            if (unit == null)
                return HowCanItemBeUsed.CannotUse;

            var unitDescriptor = unit.Descriptor;


            if (UIUtilityItem.IsItemAbilityInSpellListOfUnit(itemBlueprint, unitDescriptor))
                return HowCanItemBeUsed.InSpellList;

            if (itemBlueprint.IsUnitNeedUMDForUse(unitDescriptor) && unitDescriptor.HasUMDSkill)
                return HowCanItemBeUsed.UseMagicDevice;

            return HowCanItemBeUsed.CannotUse;
        }
    }
}
