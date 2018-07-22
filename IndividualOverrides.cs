using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OgreStack
{
	internal class IndividualOverrides
	{
		private XDocument _overrides = null;

		private static Dictionary<string, int> _itemLevelOverrides = null;// new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		private static HashSet<string> _categoryNoStackOverrides = null; // new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		private static List<KeyValuePair<string, int>> _itemLevelOverridesOrdered = null;
		private static readonly object _lock = new object();

		//=====================================================================================================\\

		internal IndividualOverrides()
		{
			lock (_lock)
			{
				if (_itemLevelOverrides == null || _categoryNoStackOverrides == null)
				{
					_overrides = PersistentData.DataUtil.GetUserOverrides();

					if (_overrides != null)
					{
						_itemLevelOverrides = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
						_categoryNoStackOverrides = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
						_itemLevelOverridesOrdered = new List<KeyValuePair<string, int>>();

						XElement item = _overrides.Root.Element("IndividualItemOverrides");
						if (item != null)
						{
							IEnumerable<XElement> items = item.Elements("item");
							if (items != null && items.Any())
							{
								foreach (XElement def in items)
								{
									addItemLevelOverride(def);
								}
							}
						}

						XElement catRoot = _overrides.Root.Element("BanItemsFromStackingByCategory");
						if (catRoot != null)
						{
							IEnumerable<XElement> categories = catRoot.Elements("category");
							if (categories != null && categories.Any())
							{
								foreach (XElement cat in categories)
								{
									Dictionary<string, string> attributes = getAttributePairs(cat);
									string category = string.Empty;
									if (attributes.TryGetValue("defName", out category))
									{
										category = category.Trim();
										if (!string.IsNullOrEmpty(category))
										{
											_categoryNoStackOverrides.Add(category);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		//=====================================================================================================\\

		private static Dictionary<string, string> getAttributePairs(XElement element)
		{
			Dictionary<string, string> rv = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			if (element.HasAttributes)
			{
				foreach (XAttribute a in element.Attributes())
				{
					rv.Add(a.Name.ToString(), a.Value);
				}
			}

			return rv;
		}

		//=====================================================================================================\\

		private void addItemLevelOverride(XElement def)
		{
			Dictionary<string, string> data = getAttributePairs(def);
			string defName = string.Empty;
			string sLimit = string.Empty;
			if (data.TryGetValue("defName", out defName))
			{
				defName = defName.Trim();
				if (data.TryGetValue("stackLimit", out sLimit))
				{
					int limit = -1;
					if (Int32.TryParse(sLimit, out limit)
						&& (limit > 0)
						&& (!string.IsNullOrEmpty(defName))
						)
					{

						// last one in wins
						if (_itemLevelOverrides.ContainsKey(defName))
						{
							_itemLevelOverrides.Remove(defName);
							for (int i = _itemLevelOverridesOrdered.Count - 1; i > -1; i--)
							{
								if (string.Compare(_itemLevelOverridesOrdered[i].Key, defName, true) == 0)
								{
									_itemLevelOverridesOrdered.RemoveAt(i);
								}
							}
						}

						_itemLevelOverrides.Add(defName, limit);
						_itemLevelOverridesOrdered.Add(new KeyValuePair<string, int>(defName, limit));
					}
				}
			}
		}

		//=====================================================================================================\\

		internal List<KeyValuePair<string, int>> ViewInternalOverrides()
		{
			List<KeyValuePair<string, int>> rv = new List<KeyValuePair<string, int>>();
			if (_itemLevelOverridesOrdered != null && _itemLevelOverridesOrdered.Count > 0)
			{
				foreach (KeyValuePair<string, int> kvp in _itemLevelOverridesOrdered)
				{
					rv.Add(new KeyValuePair<string, int>(kvp.Key, kvp.Value));
				}
			}
			return rv;
		}

		//=====================================================================================================\\

		internal int GetItemLevelOverride(string defName)
		{
			int stackLimit = 0;
			if (_itemLevelOverrides != null)
			{
				return _itemLevelOverrides.TryGetValue(defName, out stackLimit)
					? stackLimit
					: 0;
			}
			return stackLimit;
		}

		//=====================================================================================================\\

		internal bool IsCategoryBanned(string category)
		{
			if (string.IsNullOrEmpty(category))
				return false;

			if (_categoryNoStackOverrides == null)
				return false;

			return _categoryNoStackOverrides.Contains<string>(category);
		}

		//=====================================================================================================\\

		internal static string GenerateDefaultOverridesXml()
		{
			#region XML
			return @"<?xml version=""1.0"" encoding=""utf-8""?>
<OgreStack>
	<IndividualItemOverrides>
		<!--
			* This lets you target items by their DefName.
			* Set the stackLimit directly
			
			* Items adjusted here will ignore any additional
			* processing rules in OgreStack
			
			* FORMAT: <item defName=""<string:DefName>"" stackLimit=""<int:desiredStackLimit>"" />
			* Example: set the stack limit for unfertilized chicken eggs to 2000
			
			* <item name=""EggChickenUnfertilized"" stackLimit=""2000"" />
		-->
		
		<item defName=""Silver"" stackLimit=""25000"" />
		<item defName=""Beer"" stackLimit=""500"" />
		<item defName=""Jade"" stackLimit=""15000"" />
		<item defName=""Wort"" stackLimit=""2000"" />
		<item defName=""AIPersonaCore"" stackLimit=""1"" />
		<item defName=""TechprofSubpersonaCore"" stackLimit=""1"" />
	</IndividualItemOverrides>

	<BanItemsFromStackingByCategory>
		<!--
		 * Lets you ban items that identifies itself
		 * in a category named in this section
		 
		 * Items adjusted here will ignore additional
		 * processing rules in OgreStack
		 
		 * FORMAT: <category name=""<string:Category>"" />
		 * Example: Ban stacking for any egg that is fertilized 
		 * that categorizes itself 'EggsFertilized'
		 
		 * <category name=""EggsFertilized"" />
		-->
	</BanItemsFromStackingByCategory>
</OgreStack>";
			#endregion XML
		}
	}
}
