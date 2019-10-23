using System.Text.RegularExpressions;
using Verse;

namespace OgreStack.Support
{
	internal class Ammunition : ModDefinition
	{
		// 1892397131
		internal Ammunition() { }

		//=====================================================================================================\\

		internal override void Init()
		{
			// ammo cases
			// must be in Category 'Ammunition'
			// and defName must have 'ammunition' in it
			this.Define(Category.Manufactured, (d) => {
				return
					string.Compare("Ammunition", d.FirstThingCategory.ToString(), true) == 0
					&& Regex.IsMatch(d.defName, @"ammunition", RegexOptions.IgnoreCase);
			});
		}

		//=====================================================================================================\\

		internal override bool IsNonStackable(ThingDef thing)
		{
			return false;
		}
	}
}
