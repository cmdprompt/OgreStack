using System.Text.RegularExpressions;
using Verse;

namespace OgreStack.Support
{
	internal class VegetableGarden : ModDefinition
	{
		internal VegetableGarden() { }

		//=====================================================================================================\\

		internal override void Init()
		{
			// PlantFood
			// FruitFoodRaw, Cooking Supplies
			this.Define(Category.RawFoodPlant, (d) => Regex.IsMatch(d.FirstThingCategory.ToString(), @"^(FruitFoodRaw|CookingSupplies)", RegexOptions.IgnoreCase));

			// FoodForAnimals
			this.Define(Category.FoodForAnimal, (d) => string.Compare(d.FirstThingCategory.ToString(), "AnimalFeed", true) == 0);

			// Food
			// SweetMeals (tea, coffee, fruitdrink)
			this.Define(Category.Food, (d) => string.Compare(d.FirstThingCategory.ToString(), "SweetMeals", true) == 0);
		}

		//=====================================================================================================\\

		internal override bool IsNonStackable(ThingDef thing)
		{
			return false;
		}
	}
}
