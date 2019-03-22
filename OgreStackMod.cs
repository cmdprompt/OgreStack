using System;
using System.Collections.Generic;
using Verse;
using OgreStack.PersistentData;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace OgreStack
{
	public class OgreStackMod : Verse.Mod
	{
		public OgreStackSettings settings;
		private Dictionary<string, List<Thing>> _activeThings = null;

		//=====================================================================================================\\

		public OgreStackMod(ModContentPack content) : base(content)
		{
			//Verse.Log.Message("No Impact No Idea");
			this.settings = base.GetSettings<OgreStackSettings>();

			this.settings.ReModify = () =>
			{
				Verse.Log.Message("[OgreStack]: Remodify From Settings Change");

				_activeThings = new Dictionary<string, List<Thing>>();
				List<Map> maps = Verse.Find.Maps;
				// this can be done from the game load screen
				// with no active save loaded, which will
				// make maps null
				if (maps != null)
				{
					foreach (Map m in maps)
					{
						if (m != null && m.listerThings != null)
						{
							List<Thing> things = m.listerThings.AllThings;
							foreach (Thing t in things)
							{
								if (t != null && t.def != null && !string.IsNullOrEmpty(t.def.defName) && this.isStackIncreaseAllowed(t.def))
								{
									if (!_activeThings.ContainsKey(t.def.defName))
										_activeThings.Add(t.def.defName, new List<Thing>());

									_activeThings[t.def.defName].Add(t);
								}
							}
						}
					}
				}
				this.ModifyStackSizes();

				_activeThings.Clear();
				_activeThings = null;
			};

			// this appears to be a technique to attempt
			// processing after other mods have loaded
			// its not a guarantee though

			Verse.LongEventHandler.QueueLongEvent(() => {
				this.ModifyStackSizes();
			}, "OgreStack_Init", false, null);
		}

		//=====================================================================================================\\

		public override string SettingsCategory()
		{
			return Translator.Translate("OgreStack.ModName");
		}

		//=====================================================================================================\\

		public override void DoSettingsWindowContents(Rect rect)
		{
			Color headerColor = new Color(
				r: 1.0f,
				g: 0.9725490f,
				b: 0.239215686f,
				a: 1.0f
			);
			GUIStyleState headerState = new GUIStyleState() {
				textColor = headerColor
			};
			GUIStyle headerStyle = new GUIStyle(Text.CurFontStyle);
			headerStyle.fontStyle = FontStyle.Bold;
			headerStyle.normal = headerState;

			GUIStyle hdStyle = new GUIStyle(Text.CurFontStyle);
			hdStyle.alignment = TextAnchor.MiddleLeft;
			hdStyle.fontSize = 11;
			hdStyle.fontStyle = FontStyle.Bold;
			hdStyle.normal = new GUIStyleState() {
				textColor = new Color(
					r: 1.0f,
					g: 0.74117647f,
					b: 0.2392156862f,
					a: 1.0f
				)
			};
			hdStyle.padding = new RectOffset(4, 0, 0, 0);

			Listing_Standard listing = new Listing_Standard(GameFont.Small);
			Listing_Standard listingPresets = new Listing_Standard(GameFont.Small);
			List<KeyValuePair<string, int>> overrides = new IndividualOverrides().ViewInternalOverrides();

			// 3 headers
			// 2 table headers
			// 2 spacers
			// 23 ish listing.regulars
			// individualoverrides.count
			float height
				= (3 * (Verse.Text.LineHeight + 2)) // include 2 for bottom border
				+ (2 * (hdStyle.lineHeight)) // table headers
				+ (2 * 15) // spacer hight 15
				+ (23 * (Verse.Text.LineHeight + 7)) // divider line is 7 in regular listing entries
				+ (overrides.Count * (Verse.Text.LineHeight + 3)) // divider line is 3 in overrides
				+ 40; // for fudge making

			Rect mainRect = new Rect(
				x: 0,
				y: 0,
				width: rect.width - 25, //25 for scrollbar
				height: height
			);

			Widgets.BeginScrollView(rect, ref this.settings.ScrollPosition, mainRect, true);

			Rect mainLeft = mainRect.LeftPart(0.78f);
			Rect mainRight = mainRect.RightPart(0.20f);

			
			// PRESETS
			listingPresets.ColumnWidth = mainRight.width;
			listingPresets.Begin(mainRight);
			Rect section = listingPresets.GetRect(Verse.Text.LineHeight + 2);
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.Box(section, Translator.Translate("OgreStack.SectionHeader.Presets"), headerStyle);
			Text.Anchor = anchor;

			Color color = GUI.color;
			GUI.color = Color.grey;
			Widgets.DrawLineHorizontal(section.x, section.y + section.height - 1, section.width);
			Widgets.DrawLineHorizontal(section.x, section.y + section.height - 2, section.width);
			GUI.color = color;

			// Ogre
			section = listingPresets.GetRect(hdStyle.lineHeight);
			GUI.Box(section, Translator.Translate("OgreStack.Presets.Section.Ogre"), hdStyle);

			section = listingPresets.GetRect(7);
			color = GUI.color;
			GUI.color = Color.grey;
			Widgets.DrawLineHorizontal(section.x, section.y + 3, section.width);
			GUI.color = color;

			foreach (Preset p in Presets.GetOgrePresets())
			{
				section = listingPresets.GetRect(Verse.Text.LineHeight);
				GUI.SetNextControlName(p.NameKey);
				if (Widgets.ButtonText(section, Translator.Translate(p.NameKey)))
					p.Modify(this.settings);

				section = listingPresets.GetRect(1);
			}

			listingPresets.GetRect(7);

			// Scalar
			section = listingPresets.GetRect(hdStyle.lineHeight);
			GUI.Box(section, Translator.Translate("OgreStack.Presets.Section.Scalar"), hdStyle);

			section = listingPresets.GetRect(7);
			color = GUI.color;
			GUI.color = Color.grey;
			Widgets.DrawLineHorizontal(section.x, section.y + 3, section.width);
			GUI.color = color;

			foreach(Preset p in Presets.GetScalarPresets())
			{
				section = listingPresets.GetRect(Verse.Text.LineHeight);
				GUI.SetNextControlName(p.NameKey);
				if (Widgets.ButtonText(section, Translator.Translate(p.NameKey)))
					p.Modify(this.settings);

				section = listingPresets.GetRect(1);
			}

			listingPresets.End();


			// SETTINGS
			listing.ColumnWidth = mainLeft.width;
			listing.Begin(mainLeft);

			section = listing.GetRect(Verse.Text.LineHeight + 2);
			anchor = Text.Anchor;
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.Box(section, Translator.Translate("OgreStack.SectionHeader.Settings"), headerStyle);
			Text.Anchor = anchor;

			color = GUI.color;
			GUI.color = Color.grey;
			Widgets.DrawLineHorizontal(section.x, section.y + section.height - 1, section.width);
			Widgets.DrawLineHorizontal(section.x, section.y + section.height - 2, section.width);
			GUI.color = color;

			Rect header = listing.GetRect(hdStyle.lineHeight);
			GUI.Box(GenUI.LeftPart(header, 0.6f), Translator.Translate("OgreStack.Settings.Category"), hdStyle);
			Rect headerInputs = GenUI.RightPart(header, 0.4f);
			Rect boxModeHd = GenUI.LeftHalf(headerInputs);
			GUI.Box(boxModeHd, Translator.Translate("OgreStack.Settings.Mode"), hdStyle);
			GUI.Box(GenUI.RightHalf(headerInputs), Translator.Translate("OgreStack.Settings.Value"), hdStyle);

			TooltipHandler.TipRegion(boxModeHd, Translator.Translate("OgreStack.Settings.Mode.Desc"));

			List<Category> allCats = new List<Category>(OgreStackMod._PROCESSING_ORDER);
			allCats.Add(Category.Other);

			foreach (Category c in allCats)
			{
				// separator line
				Rect divider = listing.GetRect(7);
				color = GUI.color;
				GUI.color = Color.grey;
				Widgets.DrawLineHorizontal(divider.x, divider.y + 3, divider.width);
				GUI.color = color;


				// Container for row
				Rect container = listing.GetRect(Verse.Text.LineHeight);
				Widgets.DrawHighlightIfMouseover(container);

				// Category
				Rect boxCategory = GenUI.LeftPart(container, 0.6f);
				anchor = Text.Anchor;
				Text.Anchor = TextAnchor.MiddleLeft;
				Widgets.Label(boxCategory, Translator.Translate("OgreStack.Settings." + c.ToString() + ".Title"));
				Text.Anchor = anchor;

				TooltipHandler.TipRegion(boxCategory, Translator.Translate("OgreStack.Settings." + c.ToString() + ".Desc"));


				// MODE
				Rect boxInputs = GenUI.RightPart(container, 0.4f);
				//Rect boxMode = GenUI.LeftHalf(boxInputs);
				Rect boxMode = GenUI.LeftPart(boxInputs, 0.49f);
				Rect boxValue = GenUI.Rounded(GenUI.RightHalf(boxInputs));

				Widgets.Dropdown<MultiplierMode, MultiplierMode>(
					rect: boxMode,

					// no idea what target
					// and getPayload are for/do
					target: MultiplierMode.Fixed,
					getPayload: (s) => {
						return s;
					},

					// This appears to update automatically
					buttonLabel: Translator.Translate("OgreStack.Settings.Mode." + this.settings.Values[c].Mode.ToString()),
					menuGenerator: (s) => {
						List<Widgets.DropdownMenuElement<MultiplierMode>> rv = new List<Widgets.DropdownMenuElement<MultiplierMode>>();
						rv.Add(new Widgets.DropdownMenuElement<MultiplierMode>() {
							option = new FloatMenuOption(Translator.Translate("OgreStack.Settings.Mode.Scalar"), () => {
								this.settings.Values[c].Mode = MultiplierMode.Scalar;
							}),
							payload = MultiplierMode.Scalar
						});

						rv.Add(new Widgets.DropdownMenuElement<MultiplierMode>() {
							option = new FloatMenuOption(Translator.Translate("OgreStack.Settings.Mode.Fixed"), () => {
								this.settings.Values[c].Mode = MultiplierMode.Fixed;
							}),
							payload = MultiplierMode.Fixed
						});
						return rv;
					}
				);

				// VALUE
				this.settings.Values[c].Buffer = Widgets.TextField(boxValue, this.settings.Values[c].Buffer);

				if (!this.settings.Values[c].ParseBuffer())
				{
					color = GUI.color;
					GUI.color = new Color(0.662745f, 0f, 0f);
					Widgets.DrawBox(GenUI.Rounded(boxValue), 2);
					GUI.color = color;
				}
			}

			Rect space = listing.GetRect(15);

			section = listing.GetRect(Verse.Text.LineHeight + 2);
			anchor = Text.Anchor;
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.Box(section, Translator.Translate("OgreStack.SectionHeader.SingleThingDefTargeting"), headerStyle);
			Text.Anchor = anchor;

			color = GUI.color;
			GUI.color = Color.grey;
			Widgets.DrawLineHorizontal(section.x, section.y + section.height - 1, section.width);
			Widgets.DrawLineHorizontal(section.x, section.y + section.height - 2, section.width);
			GUI.color = color;

			anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(listing.GetRect(Verse.Text.LineHeight * 3), Translator.Translate("OgreStack.Settings.SingleThingDefTargeting.Desc", DataUtil.GenerateFilePath(DataUtil._OVERRIDES_FILE_NAME).Replace("/", "\\")));
			Text.Anchor = anchor;

			header = listing.GetRect(hdStyle.lineHeight);
			anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;

			GUI.Box(GenUI.LeftPart(header, 0.35f), Translator.Translate("OgreStack.Settings.DefName"), hdStyle);
			GUI.Box(GenUI.RightPart(header, 0.65f), Translator.Translate("OgreStack.Settings.StackLimit"), hdStyle);
			Text.Anchor = anchor;


			foreach (KeyValuePair<string, int> kvp in overrides)
			{
				string defName = kvp.Key;
				int stackLimit = kvp.Value;

				// separator line
				Rect divider = listing.GetRect(3);
				color = GUI.color;
				GUI.color = Color.grey;
				Widgets.DrawLineHorizontal(divider.x, divider.y + 1, divider.width);
				GUI.color = color;

				// Container for row
				Rect container = listing.GetRect(Verse.Text.LineHeight);
				Widgets.DrawHighlightIfMouseover(container);

				// DefName
				Rect boxDefName = GenUI.LeftPart(container, 0.35f);
				anchor = Text.Anchor;
				Text.Anchor = TextAnchor.MiddleLeft;
				Widgets.Label(boxDefName, defName);
				Text.Anchor = anchor;

				// StackLimit
				Rect boxStackLimit = GenUI.RightPart(container, 0.65f);
				anchor = Text.Anchor;
				Text.Anchor = TextAnchor.MiddleLeft;
				Widgets.Label(boxStackLimit, stackLimit.ToString());
				Text.Anchor = anchor;

				// Show The Rule
				// * ends up text wrapping, so its not really
				// * very helpful IMO
				TooltipHandler.TipRegion(container, "From Rule:\n<item defName=\"" + defName + "\" stackLimit=\"" + stackLimit.ToString() + "\" />");
			}




			// DEBUG
			space = listing.GetRect(15);
			section = listing.GetRect(Verse.Text.LineHeight + 2);
			anchor = Text.Anchor;
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.Box(section, Translator.Translate("OgreStack.SectionHeader.Debug"), headerStyle);
			Text.Anchor = anchor;

			color = GUI.color;
			GUI.color = Color.grey;
			Widgets.DrawLineHorizontal(section.x, section.y + section.height - 1, section.width);
			Widgets.DrawLineHorizontal(section.x, section.y + section.height - 2, section.width);
			GUI.color = color;

			Rect oContainer = listing.GetRect(Verse.Text.LineHeight);
			Widgets.DrawHighlightIfMouseover(oContainer);
			Rect oLeft = GenUI.LeftPart(oContainer, 0.8f);
			anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(oLeft, Translator.Translate("OgreStack.Settings.CSV.Title"));
			Text.Anchor = anchor;

			TooltipHandler.TipRegion(oLeft, Translator.Translate("OgreStack.Settings.CSV.Desc", DataUtil.GenerateFilePath("OgreStack_DefsList.csv").Replace("/", "\\")));

			Rect oRight = GenUI.RightPart(oContainer, 0.2f);
			Widgets.Checkbox(oRight.x + oRight.width - 26, oRight.y, ref this.settings.IsDebug);

			//oContainer = listing.GetRect(Verse.Text.LineHeight);
			//GUI.SetNextControlName("preset_test");
			//if (Widgets.ButtonText(oContainer, "Test"))
			//{
			//	this.settings.Values[Category.SmallVolumeResource].Buffer = "999";
			//	if (this.settings.Values[Category.SmallVolumeResource].Mode == MultiplierMode.Fixed)
			//	{
			//		this.settings.Values[Category.SmallVolumeResource].Mode = MultiplierMode.Scalar;
			//	}
			//	else
			//	{
			//		this.settings.Values[Category.SmallVolumeResource].Mode = MultiplierMode.Fixed;
			//	}
			//}

			//TooltipHandler.TipRegion(oContainer, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed porttitor rhoncus lacus, sed condimentum odio pretium sed. Pellentesque luctus velit id magna efficitur interdum. Duis nec dictum ante. Morbi urna nibh, ullamcorper id blandit viverra, molestie et risus. In hac habitasse platea dictumst. Suspendisse ut dictum velit, in vestibulum erat. Proin vel ultrices ante, eget tristique lorem. Donec scelerisque rutrum fermentum. Vivamus pulvinar nec augue a convallis. Donec ut tellus lorem. Maecenas at pharetra libero, a iaculis risus. Sed dui tellus, euismod sit amet egestas vitae, convallis at eros. Quisque sed placerat quam, vel luctus erat. Suspendisse leo tellus, porta in laoreet in, feugiat quis mi. Suspendisse fermentum aliquam metus id sollicitudin. Curabitur consectetur lacus ac dolor scelerisque vestibulum.");
			listing.End();
			Widgets.EndScrollView();
		}

		//=====================================================================================================\\

		private static readonly List<Category> _PROCESSING_ORDER = new List<Category>()
		{
			Category.SmallVolumeResource,
			Category.Resource,
			Category.RawFoodMeat,
			Category.RawFoodPlant,
			Category.PlantMatter,
			Category.Meal,
			Category.FoodForAnimal,
			Category.Food,
			Category.Item,
			Category.BodyPartOrImplant,
			Category.Leather,
			Category.Textile,
			Category.StoneBlock,
			Category.Manufactured,
			Category.Drug,
			Category.Medicine,
			Category.MortarShell,
			Category.Artifact
		};

		//=====================================================================================================\\

		public static readonly Dictionary<Category, CategorySetting> _DEFAULTS = new Dictionary<Category, CategorySetting>()
		{
			{ Category.SmallVolumeResource, new CategorySetting(MultiplierMode.Scalar, 30) },
			{ Category.Resource, new CategorySetting(MultiplierMode.Fixed, 1000) },
			{ Category.RawFoodMeat, new CategorySetting(MultiplierMode.Fixed, 1000) },
			{ Category.RawFoodPlant, new CategorySetting(MultiplierMode.Fixed, 1000) },
			{ Category.PlantMatter, new CategorySetting(MultiplierMode.Fixed, 2000) },
			{ Category.Meal, new CategorySetting(MultiplierMode.Fixed, 150) },
			{ Category.FoodForAnimal, new CategorySetting(MultiplierMode.Fixed, 2000) },
			{ Category.Food, new CategorySetting(MultiplierMode.Fixed, 1000) },
			{ Category.Item, new CategorySetting(MultiplierMode.Fixed, 20) },
			{ Category.BodyPartOrImplant, new CategorySetting(MultiplierMode.Fixed, 5) },
			{ Category.Leather, new CategorySetting(MultiplierMode.Fixed, 1000) },
			{ Category.Textile, new CategorySetting(MultiplierMode.Fixed, 1000) },
			{ Category.StoneBlock, new CategorySetting(MultiplierMode.Fixed, 2500) },
			{ Category.Manufactured, new CategorySetting(MultiplierMode.Scalar, 10) },
			{ Category.Drug, new CategorySetting(MultiplierMode.Fixed, 4000) },
			{ Category.Medicine, new CategorySetting(MultiplierMode.Fixed, 75) },
			{ Category.MortarShell, new CategorySetting(MultiplierMode.Fixed, 25) },
			{ Category.Artifact, new CategorySetting(MultiplierMode.Fixed, 10) },

			{ Category.Other, new CategorySetting(MultiplierMode.Scalar, 10) }
		};

		//=====================================================================================================\\

		private List<OgreStack.ModDefinition> getSupportedMods()
		{
			List<ModDefinition> mods = new List<ModDefinition>() {
				new Support.VegetableGarden(),
				new Support.CuprosDrinks(),
				new Support.TiberiumRim(),
				new Support.MedievalTimes(),
				new Support.GeneticRim(),
				new Support.ExpandedWoodworking(),

				new Support.MiscForbid(),

				// Base Game Defs
				new Support.RimWorld()
			};

			foreach (ModDefinition m in mods)
				m.Init();

			return mods;
		}

		//=====================================================================================================\\

		private static readonly HashSet<string> _CATEGORIES_BANNED = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Chunks",
			"Furniture",
			"StoneChunks",
			"WeaponsMelee"
		};

		//=====================================================================================================\\

		private bool isStackIncreaseAllowed(ThingDef d)
		{
			if (d == null || d.thingCategories == null || !(d.thingCategories.Count > 0) || d.FirstThingCategory == null)
				return false;

			bool stackable = d.IsStuff
				|| d.isTechHediff
				|| ((d.category == ThingCategory.Item)
					&& !d.isUnfinishedThing
					&& !d.IsCorpse
					&& !d.destroyOnDrop
					&& !d.IsRangedWeapon
					&& !d.IsApparel
					&& !d.Minifiable
					&& !d.IsArt
					&& !d.IsBed
				);

			if (stackable)
			{
				stackable = !_CATEGORIES_BANNED.Contains(d.FirstThingCategory.ToString());
			}

			if (stackable)
			{
				// check comps
				if (d.comps != null && d.comps.Count > 0)
				{
					foreach (CompProperties x in d.comps)
					{
						// things that have quality on them ( legendary, excellent, etc)
						// would be confusing if they were stacked in game since the 
						// UI wouldn't show all the different qualities of the stacked items
						if (x.compClass == typeof(RimWorld.CompQuality))
						{
							stackable = false;
						}
					}
				}
			}

			return stackable;
		}

		//=====================================================================================================\\

		internal void ModifyStackSizes()
		{
			List<ModDefinition> mods = this.getSupportedMods();
			List<Category> processingOrder = new List<Category>(_PROCESSING_ORDER);

			// going to break nested loops using -1 as a flag
			// reverse the order of things so we can go backwards
			mods.Reverse();
			processingOrder.Reverse();

			List<string[]> csvData = null;

			IndividualOverrides overrides = new IndividualOverrides();
			bool writeCSV = this.settings.IsDebug;

			if (writeCSV) { csvData = new List<string[]>(); };

			Dictionary<Category, bool> parsedSettings = _PROCESSING_ORDER
				.ToDictionary(x => x, y => this.settings.Values[y].ParseBuffer());

			foreach (ThingDef thing in DefDatabase<ThingDef>.AllDefs)
			{
				if (isStackIncreaseAllowed(thing))
				{
					//string startingLimit = thing.stackLimit.ToString();
					string startingLimit = CategorySetting.GetBaseStackLimit(thing).ToString();

					// unless altered in a match function
					// its whatever it started out as
					string limitAfterMatchProcessed = string.Empty + startingLimit;

					// category for CSV
					string category = "Other " + this.settings.Values[Category.Other].ToString();
					string support = string.Empty;

					bool match = false;

					// check for overrides first
					int itemOverride = overrides.GetItemLevelOverride(thing.defName);
					if (itemOverride > 0)
					{
						match = true;
						thing.stackLimit = itemOverride;
						category = "UserOverride.SetStackLimit(" + thing.defName + ": " + itemOverride + ")";
						support = "Overrides.xml";
					}

					// category level local override check
					if (overrides.IsCategoryBanned(thing.FirstThingCategory.ToString()))
					{
						match = true;
						thing.stackLimit = 1;
						category = "UserOverride.BanStacking(Category: " + thing.FirstThingCategory.ToString() + ")";
						support = "Overrides.xml";
					}

					// cascaded through other mod tweeks
					// and vanilla rimworld 
					if (!match)
					{
						for (int m = mods.Count - 1; m > -1; m--)
						{
							if (mods[m].IsNonStackable(thing))
							{
								match = true;
								category = "StackChangeForbidden";
								support = mods[m].GetType().ToString();
								break;
							}
							for (int c = processingOrder.Count - 1; c > -1; c--)
							{
								List<Func<ThingDef, bool>> fns = mods[m].GetCategoryFunctions(processingOrder[c]);
								if (fns != null)
								{
									foreach (Func<ThingDef, bool> fn in fns)
									{
										match = fn.Invoke(thing);

										limitAfterMatchProcessed = thing.stackLimit.ToString(); // for CSV
										if (match)
										{
											if (parsedSettings[processingOrder[c]])
											{
												// successful parse from 
												// the settings menu
												this.settings.Values[processingOrder[c]].ProcessThing(thing, _activeThings);
											}

											category = processingOrder[c].ToString() + " " + this.settings.Values[processingOrder[c]].ToString();

											support = mods[m].GetType().ToString();

											m = c = -1; // break all loops
											break;
										}
									}
								}
							}
						}
					}

					if (!match)
					{
						// still stackable, but not specifically
						// targeted. use the 'other' setting

						this.settings.Values[Category.Other].ProcessThing(thing, _activeThings);
						support = "OgreStack.Other";
					}

					if (thing.stackLimit > 1)
					{
						thing.drawGUIOverlay = true;
						if (thing.resourceReadoutPriority == ResourceCountPriority.Uncounted)
						{
							// these appear to be all the legs/arms/bionics
							// in 'uncounted', but they need to be counted in order
							// for bills to work correctly for the do until 'X'
							// setting priority to middle is enough to get it flagged
							// but the Defs in ResourceCounter looks to be populated aready
							thing.resourceReadoutPriority = ResourceCountPriority.Middle;
						}
					}

					if (writeCSV)
					{
						csvData.Add(new string[] {
							thing.defName,
							thing.FirstThingCategory.ToString(),
							startingLimit,
							limitAfterMatchProcessed,
							category,
							thing.stackLimit.ToString(),
							support
						});
					}
				}
			}

			// this clears the resource counter dictionary
			// and re adds all the defs that should be countable
			// some ones that were not countable, are countable now
			// like stacked bionics
			RimWorld.ResourceCounter.ResetDefs();

			if (writeCSV)
			{
				csvData = csvData
					.OrderBy(x => x[1]) // category
					.ThenBy(x => x[0])  // defname
					.ToList<string[]>();

				// Make a Header Row
				csvData.Insert(0, new string[] { "DefName", "RimWorldCategory", "DefaultStackBase", "AlteredBase", "OgreStackCategory(FixedBase)", "FinalStackLimit", "Support" });

				try
				{
					DataUtil.WriteFisherPriceCSV(csvData, "OgreStack_DefsList.csv");
					Verse.Log.Message("[OgreStack]: Write Defs CSV => [" + DataUtil.GenerateFilePath("OgreStack_DefsList.csv").Replace("/", "\\") + "]");
				}
				catch (System.IO.IOException ex)
				{
					if (Regex.IsMatch(ex.Message, "sharing violation", RegexOptions.IgnoreCase))
					{
						Verse.Log.Warning("[OgreStack]: Cannot Write 'OgreStack_DefsList.csv'. Make sure you don't have the file open so OgreStack can overwrite it.");
						Verse.Log.Warning(ex.Message);
					}
				}

				csvData.Clear();
				csvData = null;
			}

			Verse.Log.Message("[OgreStack]: Modify Stack Sizes Complete");
		}
	}
}
