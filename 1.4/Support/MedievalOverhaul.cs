using Verse;
using System.Collections.Generic;

namespace OgreStack.Support
{
	internal class MedievalOverhaul : ModDefinition
	{
		private static readonly HashSet<string> WOOD_CATEGORY = new HashSet<string>()
		{
			"DankPyon_Wood",
			"DankPyon_RawWood"
		};

		private static readonly HashSet<string> WOOD_DEFS = new HashSet<string>()
		{
			"DankPyon_AncientWoodLog",
			"DankPyon_DryadWoodLog"
		};

		internal override void Init()
		{
			this.Define(Category.Resource, (d) => {
				return WOOD_CATEGORY.Contains(d.FirstThingCategory.ToString())
					|| WOOD_DEFS.Contains(d.defName);
			});
		}

		internal override bool IsNonStackable(ThingDef thing)
		{
			return false;
		}
	}
}
