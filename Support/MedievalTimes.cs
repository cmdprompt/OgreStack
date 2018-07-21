using System.Text.RegularExpressions;
using Verse;

namespace OgreStack.Support
{
	internal class MedievalTimes : ModDefinition
	{
		internal MedievalTimes() { }

		//=====================================================================================================\\

		internal override void Init()
		{
			// TradeGoods
			// (crafting tools, trigger mechanism, storagepot, nanite vial, wood pitch glue)
			this.Define(Category.Resource, (d) => {
				return string.Compare(d.FirstThingCategory.ToString(), "TradeGoods", true) == 0
					&& Regex.IsMatch(d.defName, @"^MedTimes_");
			});

			// ExplosivePot / Quicklime
			// these are flagged as manufactured
			// but they are used as fuel for the catapult/murderhole
			// so they fit better in the mortarshell category
			this.Define(Category.MortarShell, (d) => Regex.IsMatch(d.defName, @"^MedTimes_Resource_(ExplosivePot|Quicklime)", RegexOptions.IgnoreCase));
			
		}

		//=====================================================================================================\\

		internal override bool IsNonStackable(ThingDef thing)
		{
			// no idea on this one.
			// medtimes WeaponsMelee_(Leg|Nan) stacks fine, but they look like they
			// are supposed to be melee weapon parts for other items made in the 
			// infinity forge. but you can equip the weapons even though they have
			// no HP or item quality ... treat them as if they did though and dont stack them

			return Regex.IsMatch(thing.FirstThingCategory.ToString(), @"^WeaponsMelee_(Leg|Nan)", RegexOptions.IgnoreCase);
		}
	}
}
