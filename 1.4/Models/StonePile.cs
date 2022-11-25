using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Windows.Speech;
using Verse;

namespace OgreStack.Models
{
	public class StonePile: Building_Storage
	{
		public StonePile() { }

		public override string GetInspectString()
		{
			StringBuilder sb = new StringBuilder();
			if (base.Spawned)
			{
				if(this.storageGroup != null)
				{
					sb.Append("LinkedStorageSettings".Translate())
						.Append(": ")
						.Append("NumBuildings".Translate(this.storageGroup.MemberCount).CapitalizeFirst());
				}
				if (this.slotGroup.HeldThings.Any<Thing>())
				{
					sb.Append("StoresThings".Translate()).Append(":\n");
					Dictionary<string, int> count = new Dictionary<string, int>();
					foreach (Thing thing in this.slotGroup.HeldThings)
					{
						if (count.ContainsKey(thing.LabelShortCap))
						{
							++count[thing.LabelShortCap];
						}
						else
						{
							count[thing.LabelShortCap] = 1;
						}
					}
					IEnumerable<string> text = count.OrderBy(x => x.Key)
						.Select(x => x.Value + "x " + x.Key);
					sb.Append(string.Join("\n", text));
				}
			}
			return sb.ToString();
		}
	}
}
