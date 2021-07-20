using System.Text.RegularExpressions;
using Verse;
namespace OgreStack.Support
{
	internal class TiberiumRim: ModDefinition
	{
		internal TiberiumRim() { }

		//=====================================================================================================\\

		internal override void Init()
		{
			// i dont know what the tiberium wood is used for
			this.Define(Category.Resource, (d) => string.Compare(d.defName, "TiberiumPlantTree", true) == 0);

			// There are books
			// But they don't appear to be really useful
			// outside of initially to build a few buildings
			this.Define(Category.Resource, (d) => Regex.IsMatch(d.defName, "^Book(Riparius|Rutila|Vinifera)$", RegexOptions.IgnoreCase));


			// this is from cutting the
			// tiberium crystals that spawn or veins
			// or grass/berries
			// some of this is used to make drugs
			// stack them as high as you would Hops or Smokeleaf leaves
			this.Define(Category.PlantMatter, (d) => {
				return string.Compare(d.FirstThingCategory.ToString(), "TiberiumItems", true) == 0
					&& Regex.IsMatch(d.defName, @"^(TiberiumPlant)|(TiberiumRaw)$", RegexOptions.IgnoreCase);
			});


			// Tiberium Fuel
			this.Define(Category.Manufactured, (d) => {
				bool isFuel = string.Compare(d.defName, "TiberiumFuel", true) == 0;
				if (isFuel) {
					// scale this fuel just like chemfuel
					// by setting its base to the same
					d.stackLimit = 400;
				}
				return isFuel;
			});
		}

		//=====================================================================================================\\

		internal override bool IsNonStackable(ThingDef thing)
		{
			return false;
		}
	}
}
