
using System;
using System.Collections.Generic;
using Verse;

namespace OgreStack
{
	public class CategorySetting
	{
		public CategorySetting(MultiplierMode mode)
		{
			this.Mode = mode;
		}

		//=====================================================================================================\\

		public CategorySetting(MultiplierMode mode, float value)
		{
			this.Mode = mode;
			this.Value = value;
			this.Buffer = value.ToString();
		}

		//=====================================================================================================\\

		public CategorySetting(MultiplierMode mode, string buffer)
		{
			this.Mode = mode;
			this.Buffer = buffer;
			this.Value = float.TryParse(buffer, out this.Value) ? this.Value : 0f;
		}

		//=====================================================================================================\\

		// these need to be variables
		// beacuse they are used
		// as ref params for the menu / scribe

		public MultiplierMode Mode;
		public float Value;
		public string Buffer;

		//=====================================================================================================\\

		public void Modify(MultiplierMode mode, float value, string buffer)
		{
			this.Mode = mode;
			this.Value = value;
			this.Buffer = buffer;
		}

		//=====================================================================================================\\

		public bool ParseBuffer()
		{
			if (float.TryParse(this.Buffer, out this.Value))
			{
				return true;
			}
			else
			{
				this.Value = 0f;
				return false;
			}
		}

		//=====================================================================================================\\

		private static readonly Dictionary<string, int> _baseCounts = new Dictionary<string, int>();

		//=====================================================================================================\\

		public static int GetBaseStackLimit(ThingDef def)
		{
			if (def == null)
			{
				Verse.Log.Message("def should not be null. returning 1 for stacklimit.");
				return 1;
			}

			int count = 1;
			if (!_baseCounts.TryGetValue(def.defName, out count))
			{
				_baseCounts.Add(def.defName, def.stackLimit);
				count = def.stackLimit;
			}

			return count;
		}

		//=====================================================================================================\\

		public void ProcessThing(ThingDef def, Dictionary<string, List<Thing>> activeThings)
		{
			int baseCount = CategorySetting.GetBaseStackLimit(def);

			int stackLimit = 1;
			if (this.Mode == MultiplierMode.Fixed)
			{
				stackLimit = Math.Max(1, (int)Math.Floor(this.Value));
			}
			else
			{
				int b = Math.Max(1, baseCount);
				stackLimit = Math.Max(1, (int)Math.Floor(b * this.Value));
			}

			def.stackLimit = stackLimit;

			if (activeThings != null)
			{
				List<Thing> things = null;
				if (activeThings.TryGetValue(def.defName, out things))
				{
					foreach (Thing t in things)
					{
						if (t != null && t.def != null)
						{
							if (t.stackCount > stackLimit)
							{
								ThingDef stuff = t.Stuff == null ? null : t.Stuff;
								
								// if the stack of things is forbidden
								// make the stacks generated from it
								// also forbidden
								bool isForbidden = false;
								if (t is ThingWithComps)
								{
									RimWorld.CompForbiddable comp = (t as ThingWithComps).GetComp<RimWorld.CompForbiddable>();
									if (comp != null)
										isForbidden = comp.Forbidden;
								}

								if (t.Position != null && t.Map != null)
								{
									for (int over = t.stackCount - stackLimit; over > 0; over = over - stackLimit)
									{
										Thing remain = (stuff == null)
											? ThingMaker.MakeThing(t.def)
											: ThingMaker.MakeThing(t.def, stuff);

										remain.stackCount = Math.Min(over, stackLimit);
										remain.HitPoints = Math.Min(t.HitPoints, remain.MaxHitPoints);

										if (isForbidden)
										{
											ThingWithComps twc = remain as ThingWithComps;
											RimWorld.CompForbiddable ftwc = twc.GetComp<RimWorld.CompForbiddable>();
											if (ftwc != null)
												ftwc.Forbidden = true;

										}
										Verse.GenPlace.TryPlaceThing(remain, t.Position, t.Map, ThingPlaceMode.Near);
									}
								}

								t.stackCount = stackLimit;
							}
						}
					}
				}
			}
		}

		//=====================================================================================================\\

		public override string ToString()
		{
			return string.Format("({0}{1})",
				Mode == MultiplierMode.Fixed ? "F" : "x",
				this.Value
			);
		}
	}
}
