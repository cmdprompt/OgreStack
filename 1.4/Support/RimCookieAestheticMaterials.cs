using System.Text.RegularExpressions;
using Verse;

namespace OgreStack.Support
{
	internal class AestheticMaterials : ModDefinition
	{
		internal AestheticMaterials() { }

		internal override void Init()
		{
			this.Define(Category.Resource, (d) => Regex.IsMatch(d.FirstThingCategory.ToString(), @"^(ExoticWood|AestheticMetals)$", RegexOptions.IgnoreCase));
		}

		internal override bool IsNonStackable(ThingDef thing)
		{
			return false;
		}
	}
}