using System.Collections.Generic;
using Verse;

namespace OgreStack.Support
{
	internal class MiscForbid : ModDefinition
	{
		internal MiscForbid() { }

		//=====================================================================================================\\

		private static readonly HashSet<string> DEFS_TO_BAN = new HashSet<string>() {
			// Camping Stuff : 1523058989
			// tents(DeployableTent, DeployableTentMedium, DeployableTentBig, 
			// and DeployableTentLong) must have a stack size of 1.Tents in stacks(greater than one) will 
			// not deploy and instead will disappear from the stack
			"DeployableTent",
			"DeployableTentMedium",
			"DeployableTentBig",
			"DeployableTentLong",

			// DLC: BioTech
			"Genepack",
			"HumanEmbryo",
			"Xenogerm",

			// Vanilla Books Expanded: 2193152410
			"VBE_Blueprint",
			"VBE_Map",
			"VBE_Newspaper"
		};

		//=====================================================================================================\\

		internal override void Init()
		{
			
		}

		//=====================================================================================================\\

		internal override bool IsNonStackable(ThingDef thing)
		{
			if (DEFS_TO_BAN.Contains(thing.defName))
				return true;

			return false;
		}
	}
}
