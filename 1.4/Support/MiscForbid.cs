﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using Verse;

namespace OgreStack.Support
{
	// Camping Stuff: 1523058989

	internal class MiscForbid : ModDefinition
	{
		internal MiscForbid() { }

		//=====================================================================================================\\

		internal override void Init()
		{
			
		}

		//=====================================================================================================\\

		internal override bool IsNonStackable(ThingDef thing)
		{
			HashSet<string> defsToBan = new HashSet<string>() {
				// Camping Stuff : 1523058989
				// Mods that alter stack size(OgreStack, Stack XXL, etc) - tents(DeployableTent, DeployableTentMedium, DeployableTentBig, 
				// and DeployableTentLong) must have a stack size of 1.Tents in stacks(greater than one) will 
				// not deploy and instead will disappear from the stack
				"DeployableTent",
				"DeployableTentMedium",
				"DeployableTentBig",
				"DeployableTentLong",

				// DLC: BioTech
				"Genepack",
				"HumanEmbryo",
				"Xenogerm"
			};

			if (defsToBan.Contains(thing.defName))
				return true;


			return false;
		}
	}
}
