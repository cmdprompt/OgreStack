using OgreStack.PersistentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OgreStack
{
	public class Preset
	{
		public Preset(string key)
		{
			this.NameKey = key;
		}

		public string NameKey { get; private set; }

		public Action<OgreStackSettings> Modify { get; set; }
	}
}
