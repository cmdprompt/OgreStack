using Verse;
using System.Text.RegularExpressions;

namespace OgreStack.Support
{
	internal class AlphaGenes : ModDefinition
	{
		internal AlphaGenes() { }

		internal override void Init()
		{
			
		}

		internal override bool IsNonStackable(ThingDef thing)
		{
			if (string.Compare(thing.FirstThingCategory.ToString(), "AG_GeneTools", true) == 0)
				return true;

			if (Regex.IsMatch(thing.defName, @"^AG_(Alphapack|Mixedpack|HumanEgg)$"))
				return true;

			return false;
		}
	}
}
