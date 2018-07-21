using System;
using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace OgreStack.PersistentData
{
	public class OgreStackSettings : Verse.ModSettings
	{
		public OgreStackSettings() { }

		private bool isLoaded = false;

		private Dictionary<Category, string> _preSettingsWindow = null;
		
		internal Action ReModify = null;

		internal Vector2 ScrollPosition = new Vector2(0, 0);

		public bool IsDebug;

		internal void HashCurrentSettings()
		{
			_preSettingsWindow = new Dictionary<Category, string>();
			foreach (KeyValuePair<Category, CategorySetting> kvp in this.Values)
				_preSettingsWindow.Add(kvp.Key, kvp.Value.ToString());
		}

		public bool DetermineIfModifyStacksIsNeeded()
		{
			if (_preSettingsWindow == null)
			{
				//Verse.Log.Message("Restack: true (null presettings dictionary)");
				// the mod window has never been opened before
				// stacks are ogre defaults
				return false;
			}

			foreach (KeyValuePair<Category, CategorySetting> kvp in this.Values)
			{
				string before = _preSettingsWindow[kvp.Key];
				string after = kvp.Value.ToString();

				if (string.Compare(before, after, true) != 0)
				{
					//Verse.Log.Message(string.Format("Restack: true {0}:{1} => {2}",
					//	kvp.Key.ToString(),
					//	before,
					//	after
					//));

					return true;
				}
			}

			//Verse.Log.Message("Restack: false");
			return false;
		}

		private Dictionary<Category, CategorySetting> _values = null;
		public Dictionary<Category, CategorySetting> Values
		{
			get
			{
				if (_values == null)
				{
					_values = new Dictionary<Category, CategorySetting>(OgreStackMod._DEFAULTS.Keys.Count);
					foreach (KeyValuePair<Category, CategorySetting> kvp in OgreStackMod._DEFAULTS)
					{
						_values.Add(kvp.Key, new CategorySetting(
							mode: kvp.Value.Mode, 
							value: kvp.Value.Value
						));
					}
				}
				return _values;
			}
		}
		
		public override void ExposeData()
		{
			//Verse.Log.Message("Settings. ExposeData()");
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				// in save mode, it looks like it deletes any
				// existing keys in the save data
				// so each 'save' is starting from a blank slate
				//Verse.Log.Message("Scribe.mode(Saving)");
				
				foreach (KeyValuePair<Category, CategorySetting> kvp in this.Values)
				{
					Category cat = kvp.Key;
					CategorySetting setting = kvp.Value;

					if (OgreStackMod._DEFAULTS[cat].Mode != setting.Mode
						|| OgreStackMod._DEFAULTS[cat].Value != setting.Value)
					{
						string label = cat + "_";

						Scribe_Values.Look<MultiplierMode>(ref setting.Mode, label + "Mode");
						Scribe_Values.Look<float>(ref setting.Value, label + "Value");

						//Verse.Log.Message(string.Format("[Save Write] {0}:{1}:{2}",
						//	cat.ToString(),
						//	setting.Mode.ToString(),
						//	setting.Value.ToString()
						//));
					}
					else
					{
						//Verse.Log.Message(string.Format("[Save Skip] {0}:{1}:{2}",
						//	cat.ToString(),
						//	setting.Mode.ToString(),
						//	setting.Value.ToString()
						//));
					}
				}

				if (this.IsDebug)
				{
					Scribe_Values.Look<bool>(ref this.IsDebug, "IsDebugCSV");
				}

				if (this.DetermineIfModifyStacksIsNeeded())
				{
					this.ReModify();
				}
				this.HashCurrentSettings();
				
			}
			else
			{
				if (!isLoaded)
				{
					isLoaded = true;
					//Verse.Log.Message("Scribe.mode(Loading)");
					foreach (Category c in OgreStackMod._DEFAULTS.Keys)
					{
						string label = c.ToString() + "_";

						MultiplierMode mode = MultiplierMode.Fixed;
						float value = 0; // OgreStackMod._DEFAULTS[c].Value;
						Scribe_Values.Look<MultiplierMode>(ref mode, label + "Mode", MultiplierMode.Fixed);
						Scribe_Values.Look<float>(ref value, label + "Value");
						if (value > 0)
						{
							//Verse.Log.Message(string.Format("[Load Found] {0}:{1}:{2}",
							//	c.ToString(),
							//	mode,
							//	value.ToString()
							//));

							this.Values[c] = new CategorySetting(
								mode: mode,
								value: value
							);
						}
						else
						{
							this.Values[c] = new CategorySetting(
								mode: OgreStackMod._DEFAULTS[c].Mode,
								value: OgreStackMod._DEFAULTS[c].Value
							);

							//Verse.Log.Message(string.Format("[Load Missing] {0}",
							//	c.ToString()
							//));
						}
					}

					Scribe_Values.Look<bool>(ref this.IsDebug, "IsDebugCSV", false);

					this.HashCurrentSettings();
				}
				else
				{
					//Verse.Log.Message("Scribe.mode(Loading) [SKIP: Already Loaded]");
				}
			}
		}
	}
}
