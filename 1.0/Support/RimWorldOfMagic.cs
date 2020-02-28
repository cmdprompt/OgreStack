using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Verse;

namespace OgreStack.Support
{
	internal class RimWorldOfMagic : ModDefinition
	{
		internal static readonly Regex RX_MAGICYTE_STONE = new Regex(@"^TM_EStone_", RegexOptions.IgnoreCase);
		internal static readonly Regex RX_SCROLLS_BOOKS = new Regex(@"^TM_(SkillBooks|Scrolls)$", RegexOptions.IgnoreCase);

		internal RimWorldOfMagic() { }

		internal override void Init()
		{
			// Magicyte
			this.Define(Category.Resource, (d) => {
				return string.Compare(d.FirstThingCategory.ToString(), "TM_Magicyte", true) == 0
					&& RX_MAGICYTE_STONE.IsMatch(d.defName);
			});

			// RawMagicyte
			this.Define(Category.SmallVolumeResource, (d) => {
				return string.Compare(d.FirstThingCategory.ToString(), "TM_Magicyte", true) == 0
					&& string.Compare(d.defName, "RawMagicyte", true) == 0;
			});
		}

		internal override bool IsNonStackable(ThingDef thing)
		{
			// scrolls and books
			// stack fine in the game
			// but the Rimworld of Magic mod itself 
			// seems to consume the whole stack
			// making them unsafe to stack, unlike artifacts
			// which are safe to stack
			return RX_SCROLLS_BOOKS.IsMatch(thing.FirstThingCategory.ToString());
		}
	}
}
