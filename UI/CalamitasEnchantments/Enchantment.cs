using System;
using Terraria;

namespace CalamityMod.UI.CalamitasEnchants
{
	public struct Enchantment
	{
		public string Name;
		public string Description;
		public Action<Item> CreationEffect;
		public Action<Player> HoldEffect;

		// This is internal because CanBeAppliedTo should be used over this field directly.
		// Using that has the benefit of null checks and is objectively superior to using the raw delegate.
		internal Predicate<Item> ApplyRequirement;
		public Enchantment(string name, string description, Action<Item> creationEffect, Action<Player> holdEffect, Predicate<Item> requirement)
		{
			Name = name;
			Description = description;
			CreationEffect = creationEffect;
			HoldEffect = holdEffect;
			ApplyRequirement = requirement;
		}
		public Enchantment(string name, string description, Action<Player> holdEffect, Predicate<Item> requirement)
		{
			Name = name;
			Description = description;
			CreationEffect = null;
			HoldEffect = holdEffect;
			ApplyRequirement = requirement;
		}
		public Enchantment(string name, string description, Action<Item> creationEffect, Predicate<Item> requirement)
		{
			Name = name;
			Description = description;
			CreationEffect = creationEffect;
			HoldEffect = null;
			ApplyRequirement = requirement;
		}

		public bool CanBeAppliedTo(Item item)
		{
			// "Empty" items can never be enchanted.
			if (item is null || item.IsAir)
				return false;

			// If there is no requirement, always return true, bar the above requirement.
			if (ApplyRequirement is null)
				return true;

			// Otherwise return what the defined requirement says is needed.
			return ApplyRequirement(item);
		}
	}
}
