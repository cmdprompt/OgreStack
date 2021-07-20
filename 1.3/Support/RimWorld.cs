using Verse;
using System.Text.RegularExpressions;

namespace OgreStack.Support
{
	internal class RimWorld : ModDefinition
	{
		internal RimWorld() { }

		//=====================================================================================================\\

		internal override void Init()
		{
			// Small Resource
			this.Define(Category.SmallVolumeResource, (d) => {
				return string.Compare(d.FirstThingCategory.ToString(), "ResourcesRaw", true) == 0
					&& d.smallVolume;
			});

			// Resource
			this.Define(Category.Resource, (d) => string.Compare(d.FirstThingCategory.ToString(), "ResourcesRaw", true) == 0);

			// RawMeat
			this.Define(Category.RawFoodMeat, (d) => Regex.IsMatch(d.FirstThingCategory.ToString(), @"^(AnimalProductRaw|Eggs|MeatRaw)", RegexOptions.IgnoreCase));

			// RawVeggies
			this.Define(Category.RawFoodPlant, (d) => string.Compare(d.FirstThingCategory.ToString(), "PlantFoodRaw", true) == 0);

			// Plant Matter
			this.Define(Category.PlantMatter, (d) => string.Compare(d.FirstThingCategory.ToString(), "PlantMatter", true) == 0);

			// Meals
			this.Define(Category.Meal, (d) => string.Compare(d.FirstThingCategory.ToString(), "FoodMeals", true) == 0);

			// Probably Animal Food
			this.Define(Category.FoodForAnimal, (d) => {
				return string.Compare(d.FirstThingCategory.ToString(), "Foods", true) == 0
					&& d.ingestible != null	
					&& d.ingestible.optimalityOffsetFeedingAnimals > 0;
			});

			// Food (pemmican, chocolate)
			this.Define(Category.Food, (d) => string.Compare(d.FirstThingCategory.ToString(), "Foods", true) == 0);

			// Item
			this.Define(Category.Item, (d) => string.Compare(d.FirstThingCategory.ToString(), "Items", true) == 0);

			// BodyPart
			this.Define(Category.BodyPartOrImplant, (d) => d.isTechHediff);

			// Leather
			this.Define(Category.Leather, (d) => string.Compare(d.FirstThingCategory.ToString(), "Leathers", true) == 0);

			// Textile
			this.Define(Category.Textile, (d) => string.Compare(d.FirstThingCategory.ToString(), "Textiles", true) == 0);

			// Stone Block
			this.Define(Category.StoneBlock, (d) => string.Compare(d.FirstThingCategory.ToString(), "StoneBlocks", true) == 0);

			// Manufactured
			this.Define(Category.Manufactured, (d) => string.Compare(d.FirstThingCategory.ToString(), "Manufactured", true) == 0);

			// Drug
			this.Define(Category.Drug, (d) => string.Compare(d.FirstThingCategory.ToString(), "Drugs", true) == 0);

			// Medicine
			this.Define(Category.Medicine, (d) => string.Compare(d.FirstThingCategory.ToString(), "Medicine", true) == 0);

			// Mortar Shells
			this.Define(Category.MortarShell, (d) => string.Compare(d.FirstThingCategory.ToString(), "MortarShells", true) == 0);

			// Artifacts
			this.Define(Category.Artifact, (d) => string.Compare(d.FirstThingCategory.ToString(), "Artifacts", true) == 0);
		}

		//=====================================================================================================\\

		internal override bool IsNonStackable(ThingDef thing)
		{
			return false;
		}
	}
}
