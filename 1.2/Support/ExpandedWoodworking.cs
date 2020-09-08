using System.Text.RegularExpressions;
using Verse;

namespace OgreStack.Support
{
	internal class ExpandedWoodworking : ModDefinition
	{
		internal ExpandedWoodworking() { }

		internal override void Init()
		{
			this.Define(Category.Resource, (d) => Regex.IsMatch(d.FirstThingCategory.ToString(), @"^(WoodLumber|WoodTypes)$", RegexOptions.IgnoreCase));
		}

		internal override bool IsNonStackable(ThingDef thing)
		{
			return false;
		}
	}
}
