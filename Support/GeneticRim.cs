using System.Text.RegularExpressions;
using Verse;

namespace OgreStack.Support
{
	internal class GeneticRim : ModDefinition
	{
		internal GeneticRim() { }

		//=====================================================================================================\\

		internal override void Init() {

			// not sure how useful stack increase on these are
			// on my playthroughs i never reached the standard 75 limit
			// on any of the serums
			this.Define(Category.Resource, (d) => Regex.IsMatch(d.FirstThingCategory.ToString(), @"^GR_(AlphaSerums|GeneticMaterial)", RegexOptions.IgnoreCase));

			// this is made from wood and i think you can make it from bodies too
			// make it stack as high as raw plant matter stacks
			this.Define(Category.PlantMatter, (d) => {
				return string.Compare(d.FirstThingCategory.ToString(), "Manufactured", true) == 0
					&& string.Compare(d.defName, "GR_OrganicPulp", true) == 0;
			});

			// Empty incubators only
			this.Define(Category.Manufactured, (d) => {
				return string.Compare(d.FirstThingCategory.ToString(), "GR_Incubators", true) == 0
					&& string.Compare(d.defName, "GR_EmptyIncubator", true) == 0;
			});
		}

		//=====================================================================================================\\

		internal override bool IsNonStackable(ThingDef thing)
		{
			return (
				// its nice to see each incubator
				// separate for the individual timer	
				string.Compare(thing.FirstThingCategory.ToString(), "GR_Incubators", true) == 0

				// empty incubators can stack fine
				// flag them as manufactured in .Init				
				&& string.Compare(thing.defName, "GR_EmptyIncubator", true) != 0
			);
		}
	}
}
