using System.Text.RegularExpressions;
using Verse;
namespace OgreStack.Support
{
	internal class VanillaExpanded: ModDefinition
	{
		// cooking
		// plants

		internal VanillaExpanded() { }

		internal override void Init()
		{
			// Condiments
			// Soups?
			this.Define(Category.Food, (d) => Regex.IsMatch(d.FirstThingCategory.ToString(), @"^VCE_(Condiments|UncookedSoups)$", RegexOptions.IgnoreCase));


			// Fruits
			this.Define(Category.RawFoodPlant, (d) => string.Compare(d.FirstThingCategory.ToString(), "VCE_Fruit", true) == 0);
		}

		internal override bool IsNonStackable(ThingDef thing)
		{
			return false;
		}
	}
}
