using System.Text.RegularExpressions;
using Verse;

namespace OgreStack.Support
{
	// Camping Stuff: 1523058989

	internal class MiscForbid : ModDefinition
	{
		// Camping Stuff: 1523058989
		private static readonly Regex RX_TENT = new Regex(@"^DeployableTent", RegexOptions.IgnoreCase);

		internal MiscForbid() { }

		//=====================================================================================================\\

		internal override void Init()
		{
			
		}

		//=====================================================================================================\\

		internal override bool IsNonStackable(ThingDef thing)
		{
			// Camping Stuff : 1523058989
			// Mods that alter stack size(OgreStack, Stack XXL, etc) - tents(DeployableTent, DeployableTentMedium, DeployableTentBig, 
			// and DeployableTentLong) must have a stack size of 1.Tents in stacks(greater than one) will 
			// not deploy and instead will disappear from the stack

			if (RX_TENT.IsMatch(thing.defName))
				return true;


			// Biotech DLC
			if (string.Compare(thing.defName, "Genepack", true) == 0) { return false; }
			if (string.Compare(thing.defName, "HumanEmbryo", true) == 0) { return false; }

			return false;
		}
	}
}
