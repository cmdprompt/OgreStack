using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Verse;

namespace OgreStack.Support
{
	// collection of mods
	// make up RimCusine2
	// https://steamcommunity.com/workshop/filedetails/?id=1833695733
	internal class RimCuisine2 : ModDefinition
	{
		internal RimCuisine2() { }

		//=====================================================================================================\\

		internal static HashSet<string> BROKEN_MEALS_SHOULD_BE_FOOD = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"CaramelBalls",
			"HotCocoa",
			"HotTea",
			"RC2_Hardtack"
		};

		//=====================================================================================================\\

		internal override void Init()
		{
			// Food (like pemmican) you can eat more than one
			// at a time (ie they are not like meals)
			// Cheese, processed meat, processesed vegetables
			// ice cream, joy drinks
			this.Define(Category.Food, (d) => {
				string c = d.FirstThingCategory.ToString();

				return Regex.IsMatch(c, @"^RC2_(Vegetables|Meat|AnimalProduct|Fruit)Processed$", RegexOptions.IgnoreCase)
					|| Regex.IsMatch(c, @"^RC2_(Drink|Food)MealsJoy$", RegexOptions.IgnoreCase)
					|| (Regex.IsMatch(c, @"^FoodMeals$", RegexOptions.IgnoreCase) && BROKEN_MEALS_SHOULD_BE_FOOD.Contains(d.defName));
			});

			// best fits for rawfood plant
			this.Define(Category.RawFoodPlant, (d) => {
				string c = d.FirstThingCategory.ToString();

				return Regex.IsMatch(c, @"^RC2_(Fruits|Grains|Vegetables|Sweets)Raw$", RegexOptions.IgnoreCase)

					// this stuff can be eaten raw, but it gives a debuff
					// like if you ate raw corn, and it looks like its supposed to be used as
					// raw ingredients for other dishes
					|| Regex.IsMatch(c, @"^RC2_(Grains|Sweets)Processed$");
			});
			// Drugs
			// rimworld treats beer as a drug
			// does not have its own category for alcohol
			this.Define(Category.Drug, (d) => Regex.IsMatch(d.FirstThingCategory.ToString(), @"^RC2_Alcohol$", RegexOptions.IgnoreCase));

			// Manufactured
			// precursors for alcohol ( like wort )
			this.Define(Category.Manufactured, (d) => Regex.IsMatch(d.FirstThingCategory.ToString(), @"^RC2_AlcoholPrecursors$", RegexOptions.IgnoreCase));

		}

		//=====================================================================================================\\

		internal override bool IsNonStackable(ThingDef thing)
		{
			return false;
		}
	}
}
