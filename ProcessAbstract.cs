using System;
using System.Collections.Generic;
using Verse;

namespace OgreStack
{
	internal abstract class ModDefinition
	{
		private Dictionary<Category, List<Func<ThingDef, bool>>> _definitions = new Dictionary<Category, List<Func<ThingDef, bool>>>();

		//=====================================================================================================\\

		internal ModDefinition() { }

		//=====================================================================================================\\

		protected void Define(Category category, Func<ThingDef, bool> fnDefine)
		{
			List<Func<ThingDef, bool>> fns = null;
			if (!_definitions.TryGetValue(category, out fns))
			{
				fns = new List<Func<ThingDef, bool>>();
				_definitions.Add(category, fns);
			}

			fns.Add(fnDefine);
		}

		//=====================================================================================================\\

		internal List<Func<ThingDef, bool>> GetCategoryFunctions(Category category)
		{
			List<Func<ThingDef, bool>> functions;
			return _definitions.TryGetValue(category, out functions)
				? functions
				: null;
		}

		//=====================================================================================================\\

		internal abstract void Init();
		internal abstract bool IsNonStackable(ThingDef thing);
	}
}
