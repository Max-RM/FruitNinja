using Mortar;

namespace FruitNinja;

public static class ComboChecker
{
	private static int[,] comboTypes = new int[11, 2];

	private static int numTypesInCombo = 0;

	private static string[] strings = new string[24]
	{
		"3_FRUIT", "4_FRUIT", "5_FRUIT", "6_FRUIT", "ALL_DIFFERENT", "7_FRUIT_PLUS", "ALL_APPLES", "ALL_ORANGES", "ALL_PINEAPPLES", "ALL_WATERMELONS",
		"ALL_KIWIS", "ALL_MANGOES", "ALL_STRAWBERRIES", "ALL_PEARS", "ALL_BANANAS", "ALL_LIMES", "ALL_LEMONS", "ALL_COCONUTS", "ALPHABETICAL", "FULLHOUSE",
		"2_PAIR", "3_OF_A_KIND", "4_OF_A_KIND", "PATTERN"
	};

	private static int[,] types;

	private static string[,] textureNames;

	public static string GetComboName(COMBO_TYPE starType)
	{
		return strings[(int)starType];
	}

	public static COMBO_TYPE CheckCombo(int[] fruits, int nofruits)
	{
		int mostOf;
		return CheckCombo(fruits, nofruits, out mostOf);
	}

	public static COMBO_TYPE CheckCombo(int[] fruits, int numFruits, out int mostOf)
	{
		mostOf = 0;
		numTypesInCombo = 0;
		bool flag = true;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < numFruits; i++)
		{
			bool flag2 = false;
			for (int j = 0; j < numTypesInCombo; j++)
			{
				if (comboTypes[j, 0] == fruits[i])
				{
					comboTypes[j, 1]++;
					if (comboTypes[j, 1] > num2)
					{
						num = fruits[i];
						num2 = comboTypes[j, 1];
					}
					if (j != numTypesInCombo - 1)
					{
						flag = false;
					}
					flag2 = true;
				}
			}
			if (!flag2)
			{
				comboTypes[numTypesInCombo, 0] = fruits[i];
				comboTypes[numTypesInCombo, 1] = 1;
				numTypesInCombo++;
				if (num2 == 0)
				{
					num = fruits[i];
					num2 = 1;
				}
			}
		}
		mostOf = num;
		if (numTypesInCombo == 1)
		{
			for (int k = 0; k < 13; k++)
			{
				if (fruits[0] == types[k, 0])
				{
					return (COMBO_TYPE)types[k, 1];
				}
			}
		}
		else if (numTypesInCombo == 2)
		{
			if (!flag)
			{
				bool flag3 = true;
				for (int l = 0; l < numFruits; l++)
				{
					flag3 = ((l % 2 == 0) ? (fruits[l] == comboTypes[0, 0]) : (fruits[l] == comboTypes[1, 0]));
					if (!flag3)
					{
						break;
					}
				}
				if (flag3)
				{
					return COMBO_TYPE.CT_PATTERN;
				}
			}
			if (numFruits == 5 && (comboTypes[0, 1] == 2 || comboTypes[1, 1] == 2))
			{
				return COMBO_TYPE.CT_FULLHOUSE;
			}
		}
		else
		{
			if (numTypesInCombo == 3 && numFruits == 5 && (comboTypes[0, 1] == 2 || comboTypes[1, 1] == 2 || comboTypes[1, 1] == 2))
			{
				return COMBO_TYPE.CT_2_PAIR;
			}
			if (numTypesInCombo == numFruits && numFruits >= 5)
			{
				return COMBO_TYPE.CT_ALL_DIFFERENT;
			}
		}
		if (numTypesInCombo >= 2)
		{
			COMBO_TYPE cOMBO_TYPE = COMBO_TYPE.CT_NONE;
			for (int m = 0; m < numTypesInCombo; m++)
			{
				if (comboTypes[m, 1] == 3 && cOMBO_TYPE == COMBO_TYPE.CT_NONE)
				{
					cOMBO_TYPE = COMBO_TYPE.CT_3_OF_A_KIND;
				}
				if (comboTypes[m, 1] == 4 && cOMBO_TYPE < COMBO_TYPE.CT_4_OF_A_KIND)
				{
					cOMBO_TYPE = COMBO_TYPE.CT_4_OF_A_KIND;
				}
			}
			if (cOMBO_TYPE != COMBO_TYPE.CT_NONE)
			{
				return cOMBO_TYPE;
			}
		}
		switch (numFruits)
		{
		case 3:
			return COMBO_TYPE.CT_3_FRUIT;
		case 4:
			return COMBO_TYPE.CT_4_FRUIT;
		case 5:
			return COMBO_TYPE.CT_5_FRUIT;
		case 6:
			return COMBO_TYPE.CT_6_FRUIT;
		default:
			return COMBO_TYPE.CT_7_FRUIT_PLUS;
		case 0:
		case 1:
		case 2:
			return COMBO_TYPE.CT_NONE;
		}
	}

	public static Texture GetComboStarTexture(COMBO_TYPE starType)
	{
		Texture result = null;
		if (starType > COMBO_TYPE.CT_NONE && starType < COMBO_TYPE.CT_MAX)
		{
			int num = Math.g_random.Rand32(textureNames[(int)starType, 0][0] - 48) + 1;
			result = TextureManager.GetInstance().Load(textureNames[(int)starType, num], localise: true);
		}
		return result;
	}

	static ComboChecker()
	{
		int[,] array = new int[13, 2]
		{
			{ 0, 6 },
			{ 0, 6 },
			{ 0, 7 },
			{ 0, 8 },
			{ 0, 9 },
			{ 0, 10 },
			{ 0, 11 },
			{ 0, 12 },
			{ 0, 13 },
			{ 0, 14 },
			{ 0, 15 },
			{ 0, 16 },
			{ 0, 17 }
		};
		array[0, 0] = Fruit.FruitType("apple");
		array[1, 0] = Fruit.FruitType("apple_red");
		array[2, 0] = Fruit.FruitType("orange");
		array[3, 0] = Fruit.FruitType("pineapple");
		array[4, 0] = Fruit.FruitType("watermelon");
		array[5, 0] = Fruit.FruitType("kiwi");
		array[6, 0] = Fruit.FruitType("mango");
		array[7, 0] = Fruit.FruitType("strawberry");
		array[8, 0] = Fruit.FruitType("pear");
		array[9, 0] = Fruit.FruitType("banana");
		array[10, 0] = Fruit.FruitType("lime");
		array[11, 0] = Fruit.FruitType("lemon");
		array[12, 0] = Fruit.FruitType("coconut");
		types = array;
		textureNames = new string[24, 7]
		{
			{ "2", "star_fruity.tex", "star_juicy.tex", null, null, null, null },
			{ "2", "star_yummy.tex", "star_tasty.tex", null, null, null, null },
			{ "2", "star_lush.tex", "star_delicious.tex", null, null, null, null },
			{ "2", "star_succulent.tex", "star_succulent.tex", null, null, null, null },
			{ "3", "star_fruit_salad.tex", "star_fruits_basket.tex", "star_megamix.tex", null, null, null },
			{ "2", "star_amazing.tex", "star_exquisite.tex", null, null, null, null },
			{ "1", "star_its_apples.tex", null, null, null, null, null },
			{ "1", "star_vitamin_c.tex", null, null, null, null, null },
			{ "1", "star_got_the_sweats.tex", null, null, null, null, null },
			{ "1", "star_melon_mania.tex", null, null, null, null, null },
			{ "1", "star_flightless_bird.tex", null, null, null, null, null },
			{ "1", "star_mango_smoothie.tex", null, null, null, null, null },
			{ "1", "star_full_punnet.tex", null, null, null, null, null },
			{ "1", "star_pear_tree.tex", null, null, null, null, null },
			{ "1", "star_banana_cake.tex", null, null, null, null, null },
			{ "1", "star_scurvy_cure.tex", null, null, null, null, null },
			{ "1", "star_lemon_line_up.tex", null, null, null, null, null },
			{ "1", "star_lovely_bunch.tex", null, null, null, null, null },
			{ "1", "star_alphabetic.tex", null, null, null, null, null },
			{ "1", "star_full_house.tex", null, null, null, null, null },
			{ "1", "star_two_pairs.tex", null, null, null, null, null },
			{ "1", "star_three_of_a_kind.tex", null, null, null, null, null },
			{ "1", "star_four_of_a_kind.tex", null, null, null, null, null },
			{ "1", "star_checkers.tex", null, null, null, null, null }
		};
	}
}
