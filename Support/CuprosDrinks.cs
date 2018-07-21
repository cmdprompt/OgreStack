using System.Text.RegularExpressions;
using Verse;

namespace OgreStack.Support
{
	internal class CuprosDrinks : ModDefinition
	{
		internal CuprosDrinks() { }

		//=====================================================================================================\\

		internal override void Init()
		{
			// Food
			// Alcohol, Drinks, Energy, Sodas
			this.Define(Category.Food, (d) => Regex.IsMatch(d.FirstThingCategory.ToString(), @"^CPD_(Alcohol|Drinks|Energy|Sodas)", RegexOptions.IgnoreCase));

			// Manufactured
			// Crates
			this.Define(Category.Manufactured, (d) => {
				bool isMatch = string.Compare(d.FirstThingCategory.ToString(), "Manufactured", true) == 0
					&& Regex.IsMatch(d.defName, @"^CPD_");

				if (isMatch)
				{
					// the manufactured CDP items are 25 or 2
					// with crates at 2. Set all items to 25
					// for the scalar associated with Manufactured
					d.stackLimit = 25;
				}

				return isMatch;
			});
		}

		//=====================================================================================================\\

		internal override bool IsNonStackable(ThingDef thing)
		{
			return false;
		}
	}
}
