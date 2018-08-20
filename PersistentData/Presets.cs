using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OgreStack.PersistentData
{
	public static class Presets
	{
		private static List<Preset> _OGRE_PRESETS;
		private static List<Preset> _SCALAR_PRESETS;
		private static Dictionary<string, Preset> _REFERENCE;

		static Presets()
		{
			_OGRE_PRESETS = new List<Preset>()
			{
				new Preset("OgreStack.Presets.OgreDefault")
				{
					Modify = (a) => {
						foreach (Category c in OgreStackMod._DEFAULTS.Keys)
						{
							a.Values[c].Value = OgreStackMod._DEFAULTS[c].Value;
							a.Values[c].Mode = OgreStackMod._DEFAULTS[c].Mode;
							a.Values[c].Buffer = OgreStackMod._DEFAULTS[c].Buffer;
						}
					}
				},
				new Preset("OgreStack.Presets.OgreMid")
				{
					Modify = (a) => {
						a.Values[Category.SmallVolumeResource].Modify(MultiplierMode.Scalar, 20.0f, "20");
						a.Values[Category.Resource].Modify(MultiplierMode.Fixed, 750.0f, "750");
						a.Values[Category.RawFoodMeat].Modify(MultiplierMode.Fixed, 750.0f, "750");
						a.Values[Category.RawFoodPlant].Modify(MultiplierMode.Fixed, 750.0f, "750");
						a.Values[Category.PlantMatter].Modify(MultiplierMode.Fixed, 1000.0f, "1000");
						a.Values[Category.Meal].Modify(MultiplierMode.Fixed, 100.0f, "100");
						a.Values[Category.FoodForAnimal].Modify(MultiplierMode.Fixed, 1500.0f, "1500");
						a.Values[Category.Food].Modify(MultiplierMode.Fixed, 750.0f, "750");
						a.Values[Category.Item].Modify(MultiplierMode.Fixed, 10.0f, "10");
						a.Values[Category.BodyPartOrImplant].Modify(MultiplierMode.Fixed, 5.0f, "5");
						a.Values[Category.Leather].Modify(MultiplierMode.Fixed, 750.0f, "750");
						a.Values[Category.Textile].Modify(MultiplierMode.Fixed, 750.0f, "750");
						a.Values[Category.StoneBlock].Modify(MultiplierMode.Fixed, 1500.0f, "1500");
						a.Values[Category.Manufactured].Modify(MultiplierMode.Scalar, 10.0f, "10");
						a.Values[Category.Drug].Modify(MultiplierMode.Fixed, 2000.0f, "2000");
						a.Values[Category.Medicine].Modify(MultiplierMode.Fixed, 50.0f, "50");
						a.Values[Category.MortarShell].Modify(MultiplierMode.Fixed, 25.0f, "25");
						a.Values[Category.Artifact].Modify(MultiplierMode.Fixed, 10.0f, "10");
						a.Values[Category.Other].Modify(MultiplierMode.Scalar, 5.0f, "5");
					}
				},
				new Preset("OgreStack.Presets.OgreLow") {
					Modify = (a) => {
						a.Values[Category.SmallVolumeResource].Modify(MultiplierMode.Scalar, 15.0f, "15");
						a.Values[Category.Resource].Modify(MultiplierMode.Fixed, 500.0f, "500");
						a.Values[Category.RawFoodMeat].Modify(MultiplierMode.Fixed, 500.0f, "500");
						a.Values[Category.RawFoodPlant].Modify(MultiplierMode.Fixed, 500.0f, "500");
						a.Values[Category.PlantMatter].Modify(MultiplierMode.Fixed, 750.0f, "750");
						a.Values[Category.Meal].Modify(MultiplierMode.Fixed, 50.0f, "50");
						a.Values[Category.FoodForAnimal].Modify(MultiplierMode.Fixed, 1000.0f, "1000");
						a.Values[Category.Food].Modify(MultiplierMode.Fixed, 500.0f, "500");
						a.Values[Category.Item].Modify(MultiplierMode.Fixed, 5.0f, "5");
						a.Values[Category.BodyPartOrImplant].Modify(MultiplierMode.Fixed, 5.0f, "5");
						a.Values[Category.Leather].Modify(MultiplierMode.Fixed, 500.0f, "500");
						a.Values[Category.Textile].Modify(MultiplierMode.Fixed, 500.0f, "500");
						a.Values[Category.StoneBlock].Modify(MultiplierMode.Fixed, 1000.0f, "1000");
						a.Values[Category.Manufactured].Modify(MultiplierMode.Scalar, 5.0f, "5");
						a.Values[Category.Drug].Modify(MultiplierMode.Fixed, 1000.0f, "1000");
						a.Values[Category.Medicine].Modify(MultiplierMode.Fixed, 35.0f, "35");
						a.Values[Category.MortarShell].Modify(MultiplierMode.Fixed, 25.0f, "25");
						a.Values[Category.Artifact].Modify(MultiplierMode.Fixed, 5.0f, "5");
						a.Values[Category.Other].Modify(MultiplierMode.Scalar, 5.0f, "5");
					}
				}
			};

			_SCALAR_PRESETS = new List<Preset>();

			for (int i = 1; i < 11; i++)
			{
				int scaler = i;
				_SCALAR_PRESETS.Add(new Preset("OgreStack.Presets.Scalar." + i + "x")
				{
					Modify = (a) =>
					{
						foreach (Category c in OgreStackMod._DEFAULTS.Keys)
						{
							float f = float.Parse(scaler.ToString());
							a.Values[c].Modify(MultiplierMode.Scalar, f, scaler.ToString());
						}
					}
				});
			}

			_REFERENCE = new Dictionary<string, Preset>(StringComparer.OrdinalIgnoreCase);

			foreach (Preset p in _OGRE_PRESETS)
				_REFERENCE.Add(p.NameKey, p);

			foreach (Preset p in _SCALAR_PRESETS)
				_REFERENCE.Add(p.NameKey, p);
		}

		public static List<Preset> GetOgrePresets()
		{
			return _OGRE_PRESETS;
		}

		public static List<Preset> GetScalarPresets()
		{
			return _SCALAR_PRESETS;
		}

		public static Dictionary<string, Preset> GetPresetReferences()
		{
			return _REFERENCE;
		}
	}
}
