// <copyright file="item_sheepstick.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    public class item_scythe_of_sun : RangedAbility, IHasTargetModifier
    {
        public item_scythe_of_sun(Item item)
            : base(item)
        {
        }

        public string TargetModifierName { get; } = "modifier_item_scythe_of_sun_hex";
    }
}
