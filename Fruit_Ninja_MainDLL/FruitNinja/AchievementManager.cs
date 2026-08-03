using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mortar;

namespace FruitNinja;

public class AchievementManager
{
	private struct KeyRemove(uint t, uint k)
	{
		public uint type = t;

		public uint key = k;
	}

	private static object achievementsLockObject = new object();

	private static int awardMode = 0;

	private static int y;

	private static int delay;

	private static int unlockMode = 0;

	private static int uy;

	private static int udelay;

	private static int utype;

	private static int awardDelay = 30000;

	private static bool awarding = false;

	private static string[] achivementKeys = new string[20]
	{
		"Great Fruit Ninja", "Ultimate Fruit Ninja", "Fruit Fight", "Fruit Blitz", "Fruit Frenzy", "Fruit Rampage", "Fruit Annihilation", "Lucky Ninja", "Almost A Century", "Mango Magic",
		"Deja Vu", "Combo Mambo", "Moment Of Zen", "Year Of The Dragon", "The Lovely Bunch", "Its All Pear Shaped", "Wake Up", "Underachiever", "Overachiever", "Bomb Magnet"
	};

	private static int achievementIndex = 0;

	public static LinkedList<int> akeys = new LinkedList<int>();

	private static AchievementData[] str_en = new AchievementData[20]
	{
		new AchievementData("Got a critical hit with a Mango", "Mango Magic"),
		new AchievementData("Killed 4 of the same type of fruit in a row in Classic Mode", "Deja Vu"),
		new AchievementData("Sliced the secret fruit!", "Year Of The Dragon"),
		new AchievementData("Got the Lovely Bunch star in Zen Mode", "The Lovely Bunch"),
		new AchievementData("Killed 3 Pears in a row in Classic Mode", "Its All Pear Shaped"),
		new AchievementData("Got a score of less than 20 after all bonuses had been awarded", "Underachiever"),
		new AchievementData("Got a score of over 400 after bonuses", "Overachiever"),
		new AchievementData("Hit three bombs and score over 250 points after bonuses", "Bomb Magnet"),
		new AchievementData("Got 6 criticals in one round in Classic Mode", "Lucky Ninja"),
		new AchievementData("Killed 10000 Fruit Total", "Fruit Annihilation"),
		new AchievementData("Killed 5000 Fruit Total", "Fruit Rampage"),
		new AchievementData("Got a score of 200 in Classic Mode", "Ultimate Fruit Ninja"),
		new AchievementData("Got a score of 99", "Almost A Century"),
		new AchievementData("Killed 1000 Fruit Total", "Fruit Frenzy"),
		new AchievementData("Sliced 6 fruit in one combo", "Combo Mambo"),
		new AchievementData("Killed 500 Total Fruit", "Fruit Blitz"),
		new AchievementData("Achieved a score of 200 in Zen Mode", "Moment Of Zen"),
		new AchievementData("Killed 150 Fruit Total", "Fruit Fight"),
		new AchievementData("Got a score of 100 in Classic Mode", "Great Fruit Ninja"),
		new AchievementData("Got a score of 0", "Wake Up")
	};

	private static AchievementData[] str_fr = new AchievementData[20]
	{
		new AchievementData("Coup critique obtenu avec une mangue", "Mango Magic"),
		new AchievementData("4 fruits du même type éliminés à la suite en mode Classique.", "Deja Vu"),
		new AchievementData("Fruit secret tranché !", "Year Of The Dragon"),
		new AchievementData("Étoile Jolie Grappe obtenue en mode Zen", "The Lovely Bunch"),
		new AchievementData("3 poires éliminées à la suite en mode Classique", "Its All Pear Shaped"),
		new AchievementData("Score de moins de 20 obtenu après tous les bonus en mode Arcade.", "Underachiever"),
		new AchievementData("Score de plus de 400 obtenu après tous les bonus en mode Arcade.", "Overachiever"),
		new AchievementData("Trois bombes touchées et score de plus de 250 obtenu après tous les bonus en mode Arcade.", "Bomb Magnet"),
		new AchievementData("6 critiques obtenus en un round en mode Classique", "Lucky Ninja"),
		new AchievementData("10 000 fruits éliminés au total", "Fruit Annihilation"),
		new AchievementData("5000 fruits éliminés au total", "Fruit Rampage"),
		new AchievementData("Score de 200 obtenu en Mode Classique", "Ultimate Fruit Ninja"),
		new AchievementData("Score de 99 obtenu", "Almost A Century"),
		new AchievementData("1000 fruits éliminés au total", "Fruit Frenzy"),
		new AchievementData("6 fruits tranchés en une combo", "Combo Mambo"),
		new AchievementData("500 fruits éliminés au total", "Fruit Blitz"),
		new AchievementData("Score de 200 obtenu en mode Zen", "Moment Of Zen"),
		new AchievementData("150 fruits éliminés au total", "Fruit Fight"),
		new AchievementData("Score de 100 obtenu en Mode Classique", "Great Fruit Ninja"),
		new AchievementData("Score de 0 obtenu", "Wake Up")
	};

	private static AchievementData[] str_de = new AchievementData[20]
	{
		new AchievementData("Einen kritischen Treffer bei einer Mango erzielt", "Mango Magic"),
		new AchievementData("4 Früchte gleicher Art in einer Runde im Klassik-Modus zerstört", "Deja Vu"),
		new AchievementData("Die geheime Frucht zerteilt!", "Year Of The Dragon"),
		new AchievementData("Den \"Süßer Haufen\"-Stern im Zen-Modus erhalten", "The Lovely Bunch"),
		new AchievementData("3 Birnen in einer Runde im Klassik-Modus zerstört", "Its All Pear Shaped"),
		new AchievementData("Weniger als 20 Punkte zusammen mit allen Boni im Arcade-Modus erzielt", "Underachiever"),
		new AchievementData("Mehr als 400 Punkte zusammen mit allen Boni im Arcade-Modus erzielt", "Overachiever"),
		new AchievementData("Drei Bomben getroffen und mehr als 250 Punkte zusammen mit allen Boni im Arcade-Modus erzielt", "Bomb Magnet"),
		new AchievementData("6 kritische Treffer in einer Runde im Klassik-Modus erzielt", "Lucky Ninja"),
		new AchievementData("Insgesamt 10.000 Früchte zerstört", "Fruit Annihilation"),
		new AchievementData("Insgesamt 5000 Früchte zerstört", "Fruit Rampage"),
		new AchievementData("200 Punkte im Klassik-Modus erzielt", "Ultimate Fruit Ninja"),
		new AchievementData("99 Punkte erzielt", "Almost A Century"),
		new AchievementData("Insgesamt 1000 Früchte zerstört", "Fruit Frenzy"),
		new AchievementData("6 Früchte in einer Combo zerteilt", "Combo Mambo"),
		new AchievementData("Insgesamt 500 Früchte zerstört", "Fruit Blitz"),
		new AchievementData("200 Punkte im Zen-Modus erzielt", "Moment Of Zen"),
		new AchievementData("Insgesamt 150 Früchte zerstört", "Fruit Fight"),
		new AchievementData("100 Punkte im Klassik-Modus erzielt", "Great Fruit Ninja"),
		new AchievementData("0 Punkt erzielt", "Wake Up")
	};

	private static AchievementData[] str_it = new AchievementData[20]
	{
		new AchievementData("Hai ottenuto un colpo critico con un mango", "Mango Magic"),
		new AchievementData("Hai ucciso 4 frutti dello stesso tipo di fila, in modalità Classica.", "Deja Vu"),
		new AchievementData("Hai affettato il frutto segreto!", "Year Of The Dragon"),
		new AchievementData("Hai ottenuto la stella Un bel mucchio in modalità Zen", "The Lovely Bunch"),
		new AchievementData("Hai ucciso 3 pere di fila in modalità Classica", "Its All Pear Shaped"),
		new AchievementData("Hai ottenuto un punteggio inferiore a 20 dopo tutti i bonus in modalità Arcade.", "Underachiever"),
		new AchievementData("Hai ottenuto un punteggio inferiore a 400 dopo tutti i bonus in modalità Arcade.", "Overachiever"),
		new AchievementData("Hai colpito tre bombe e ottenuto più di 250 dopo tutti i bonus in modalità Arcade.", "Bomb Magnet"),
		new AchievementData("Hai ottenuto 6 critiche in un turno in modalità Classica", "Lucky Ninja"),
		new AchievementData("Hai ucciso 10000 frutti in totale", "Fruit Annihilation"),
		new AchievementData("Hai ucciso 5000 frutti in totale", "Fruit Rampage"),
		new AchievementData("Hai ottenuto un punteggio pari a 200 in modalità Classica", "Ultimate Fruit Ninja"),
		new AchievementData("Hai ottenuto un punteggio pari a 99", "Almost A Century"),
		new AchievementData("Hai ucciso 1000 frutti in totale", "Fruit Frenzy"),
		new AchievementData("Hai affettato 6 frutti in una combo", "Combo Mambo"),
		new AchievementData("Hai ucciso 500 frutti in totale", "Fruit Blitz"),
		new AchievementData("Hai ottenuto un punteggio pari a 200 in modalità Zen", "Moment Of Zen"),
		new AchievementData("Hai ucciso 150 frutti in totale", "Fruit Fight"),
		new AchievementData("Hai ottenuto un punteggio pari a 100 in modalità Classica", "Great Fruit Ninja"),
		new AchievementData("Hai ottenuto un punteggio pari a 0", "Wake Up")
	};

	private static AchievementData[] str_es = new AchievementData[20]
	{
		new AchievementData("Has conseguido un impacto crítico con un mango", "Mango Magic"),
		new AchievementData("Has matado 4 frutas seguidas del mismo tipo en el modo Clásico", "Deja Vu"),
		new AchievementData("¡Has cortado la fruta secreta!", "Year Of The Dragon"),
		new AchievementData("Has conseguido la estrella de Un grupo encantador en el modo Zen", "The Lovely Bunch"),
		new AchievementData("Has matado 3 peras seguidas en el modo Clásico", "Its All Pear Shaped"),
		new AchievementData("Has conseguido una puntuación inferior a 20 tras todas las bonificaciones en el modo Arcade", "Underachiever"),
		new AchievementData("Has conseguido una puntuación superior a 400 tras todas las bonificaciones en el modo Arcade", "Overachiever"),
		new AchievementData("Hit tres bombas y puntuación de más de 250 puntos después de las bonificaciones", "Bomb Magnet"),
		new AchievementData("Has conseguido 6 críticos en un asalto en el modo Clásico", "Lucky Ninja"),
		new AchievementData("Has matado 10000 frutas en total", "Fruit Annihilation"),
		new AchievementData("Has matado 5000 frutas en total", "Fruit Rampage"),
		new AchievementData("Has conseguido una puntuación de 200 en el modo Clásico", "Ultimate Fruit Ninja"),
		new AchievementData("Has conseguido una puntuación de 99", "Almost A Century"),
		new AchievementData("Has matado 1000 frutas en total", "Fruit Frenzy"),
		new AchievementData("Has cortado 6 frutas en un combo", "Combo Mambo"),
		new AchievementData("Has matado 500 frutas en total", "Fruit Blitz"),
		new AchievementData("Has conseguido una puntuación de 200 en el modo Zen", "Moment Of Zen"),
		new AchievementData("Has matado 150 frutas en total", "Fruit Fight"),
		new AchievementData("Has conseguido una puntuación de 100 en el modo Clásico", "Great Fruit Ninja"),
		new AchievementData("Has conseguido una puntuación de 0", "Wake Up")
	};

	private static AchievementData[] str1_en = new AchievementData[20]
	{
		new AchievementData("Get a critical hit with a Mango", "Mango Magic"),
		new AchievementData("Kill 4 of the same type of fruit in a row in Classic Mode", "Deja Vu"),
		new AchievementData("Slice the secret fruit!", "Year Of The Dragon"),
		new AchievementData("Get the Lovely Bunch star in Zen Mode", "The Lovely Bunch"),
		new AchievementData("Kill 3 Pears in a row in Classic Mode", "Its All Pear Shaped"),
		new AchievementData("Get a score of less than 20 after all bonuses have been awarded", "Underachiever"),
		new AchievementData("Get a score of over 400 after bonuses", "Overachiever"),
		new AchievementData("Hit three bombs and score over 250 points after bonuses", "Bomb Magnet"),
		new AchievementData("Get 6 criticals in one round in Classic Mode", "Lucky Ninja"),
		new AchievementData("Kill 10000 Fruit Total", "Fruit Annihilation"),
		new AchievementData("Kill 5000 Fruit Total", "Fruit Rampage"),
		new AchievementData("Get a score of 200 in Classic Mode", "Ultimate Fruit Ninja"),
		new AchievementData("Get a score of 99", "Almost A Century"),
		new AchievementData("Kill 1000 Fruit Total", "Fruit Frenzy"),
		new AchievementData("Slice 6 fruit in one combo", "Combo Mambo"),
		new AchievementData("Kill 500 Total Fruit", "Fruit Blitz"),
		new AchievementData("Achieve a score of 200 in Zen Mode", "Moment Of Zen"),
		new AchievementData("Kill 150 Fruit Total", "Fruit Fight"),
		new AchievementData("Get a score of 100 in Classic Mode", "Great Fruit Ninja"),
		new AchievementData("Get a score of 0", "Wake Up")
	};

	private static AchievementData[] str1_fr = new AchievementData[20]
	{
		new AchievementData("Obtenez un coup critique avec une mangue", "Mango Magic"),
		new AchievementData("Éliminez 4 fruits du même type à la suite en mode Classique.", "Deja Vu"),
		new AchievementData("Tranchez le fruit secret !", "Year Of The Dragon"),
		new AchievementData("Obtenez l'étoile Jolie Grappe en mode Zen", "The Lovely Bunch"),
		new AchievementData("Éliminez 3 poires à la suite en mode Classique", "Its All Pear Shaped"),
		new AchievementData("Obtenez un score de moins de 20 après tous les bonus en mode Arcade.", "Underachiever"),
		new AchievementData("Obtenez un score de plus de 400 après tous les bonus en mode Arcade.", "Overachiever"),
		new AchievementData("Touchez trois bombes et obtenez un score de plus de 250 après tous les bonus en mode Arcade.", "Bomb Magnet"),
		new AchievementData("Obtenez 6 critiques en un round en mode Classique", "Lucky Ninja"),
		new AchievementData("Éliminez 10 000 fruits au total", "Fruit Annihilation"),
		new AchievementData("Éliminez 5000 fruits au total", "Fruit Rampage"),
		new AchievementData("Obtenez un score de 200 en Mode Classique", "Ultimate Fruit Ninja"),
		new AchievementData("Obtenez un score de 99", "Almost A Century"),
		new AchievementData("Éliminez 1000 fruits au total", "Fruit Frenzy"),
		new AchievementData("Tranchez 6 fruits en une combo", "Combo Mambo"),
		new AchievementData("Éliminez 500 fruits au total", "Fruit Blitz"),
		new AchievementData("Obtenez un score de 200 en mode Zen", "Moment Of Zen"),
		new AchievementData("Éliminez 150 fruits au total", "Fruit Fight"),
		new AchievementData("Obtenez un score de 100 en Mode Classique", "Great Fruit Ninja"),
		new AchievementData("Obtenez un score de 0", "Wake Up")
	};

	private static AchievementData[] str1_de = new AchievementData[20]
	{
		new AchievementData("Erziele einen kritischen Treffer bei einer Mango", "Mango Magic"),
		new AchievementData("Zerstöre 4 Früchte gleicher Art in einer Runde im Klassik-Modus", "Deja Vu"),
		new AchievementData("Zerteile die geheime Frucht!", "Year Of The Dragon"),
		new AchievementData("Erhalte den \"Süßer Haufen\"-Stern im Zen-Modus", "The Lovely Bunch"),
		new AchievementData("Zerstöre 3 Birnen in einer Runde im Klassik-Modus", "Its All Pear Shaped"),
		new AchievementData("Erziele weniger als 20 Punkte zusammen mit allen Boni im Arcade-Modus", "Underachiever"),
		new AchievementData("Erziele mehr als 400 Punkte zusammen mit allen Boni im Arcade-Modus", "Overachiever"),
		new AchievementData("Triff drei Bomben und erziele mehr als 250 Punkte zusammen mit allen Boni im Arcade-Modus", "Bomb Magnet"),
		new AchievementData("Erziele 6 kritische Treffer in einer Runde im Klassik-Modus", "Lucky Ninja"),
		new AchievementData("Zerstöre insgesamt 10.000 Früchte", "Fruit Annihilation"),
		new AchievementData("Zerstöre insgesamt 5000 Früchte", "Fruit Rampage"),
		new AchievementData("Erziele 200 Punkte im Klassik-Modus", "Ultimate Fruit Ninja"),
		new AchievementData("Erziele 99 Punkte", "Almost A Century"),
		new AchievementData("Zerstöre insgesamt 1000 Früchte", "Fruit Frenzy"),
		new AchievementData("Zerteile 6 Früchte in einer Combo", "Combo Mambo"),
		new AchievementData("Zerstöre insgesamt 500 Früchte", "Fruit Blitz"),
		new AchievementData("Erziele 200 Punkte im Zen-Modus", "Moment Of Zen"),
		new AchievementData("Zerstöre insgesamt 150 Früchte", "Fruit Fight"),
		new AchievementData("Erziele 100 Punkte im Klassik-Modus", "Great Fruit Ninja"),
		new AchievementData("Erziele 0 Punkte", "Wake Up")
	};

	private static AchievementData[] str1_it = new AchievementData[20]
	{
		new AchievementData("Ottieni un colpo critico con un mango", "Mango Magic"),
		new AchievementData("Uccidi 4 frutti dello stesso tipo di fila, in modalità Classica.", "Deja Vu"),
		new AchievementData("Affetta il frutto segreto!", "Year Of The Dragon"),
		new AchievementData("Ottieni la stella Un bel mucchio in modalità Zen", "The Lovely Bunch"),
		new AchievementData("Uccidi 3 pere di fila in modalità Classica", "Its All Pear Shaped"),
		new AchievementData("Ottieni un punteggio inferiore a 20 dopo tutti i bonus in modalità Arcade.", "Underachiever"),
		new AchievementData("Ottieni un punteggio inferiore a 400 dopo tutti i bonus in modalità Arcade.", "Overachiever"),
		new AchievementData("Colpisci tre bombe e ottieni più di 250 dopo tutti i bonus in modalità Arcade.", "Bomb Magnet"),
		new AchievementData("Ottieni 6 critiche in un turno in modalità Classica", "Lucky Ninja"),
		new AchievementData("Uccidi 10000 frutti in totale", "Fruit Annihilation"),
		new AchievementData("Uccidi 5000 frutti in totale", "Fruit Rampage"),
		new AchievementData("Ottieni un punteggio pari a 200 in modalità Classica", "Ultimate Fruit Ninja"),
		new AchievementData("Ottieni un punteggio pari a 99", "Almost A Century"),
		new AchievementData("Uccidi 1000 frutti in totale", "Fruit Frenzy"),
		new AchievementData("Affetta 6 frutti in una combo", "Combo Mambo"),
		new AchievementData("Uccidi 500 frutti in totale", "Fruit Blitz"),
		new AchievementData("Ottieni un punteggio pari a 200 in modalità Zen", "Moment Of Zen"),
		new AchievementData("Uccidi 150 frutti in totale", "Fruit Fight"),
		new AchievementData("Ottieni un punteggio pari a 100 in modalità Classica", "Great Fruit Ninja"),
		new AchievementData("Ottieni un punteggio pari a 0", "Wake Up")
	};

	private static AchievementData[] str1_es = new AchievementData[20]
	{
		new AchievementData("Consigue un impacto crítico con un mango", "Mango Magic"),
		new AchievementData("Mata 4 frutas seguidas del mismo tipo en el modo Clásico", "Deja Vu"),
		new AchievementData("¡Corta la fruta secreta!", "Year Of The Dragon"),
		new AchievementData("Consigue la estrella de Un grupo encantador en el modo Zen", "The Lovely Bunch"),
		new AchievementData("Mata 3 peras seguidas en modo Clásico", "Its All Pear Shaped"),
		new AchievementData("Consigue una puntuación inferior a 20 tras todas las bonificaciones en el modo Arcade", "Underachiever"),
		new AchievementData("Consigue una puntuación superior a 400 tras todas las bonificaciones en el modo Arcade", "Overachiever"),
		new AchievementData("Hit tres bombas y puntuación de más de 250 puntos después de las bonificaciones", "Bomb Magnet"),
		new AchievementData("Consigue 6 críticos en un asalto en modo Clásico", "Lucky Ninja"),
		new AchievementData("Mata 10000 frutas en total", "Fruit Annihilation"),
		new AchievementData("Mata 5000 frutas en total", "Fruit Rampage"),
		new AchievementData("Consigue una puntuación de 200 en el modo Clásico", "Ultimate Fruit Ninja"),
		new AchievementData("Consigue una puntuación de 99", "Almost A Century"),
		new AchievementData("Mata 1000 frutas en total", "Fruit Frenzy"),
		new AchievementData("Corta 6 frutas en un combo", "Combo Mambo"),
		new AchievementData("Mata 500 frutas en total", "Fruit Blitz"),
		new AchievementData("Consigue una puntuación de 200 en el modo Zen", "Moment Of Zen"),
		new AchievementData("Mata 150 frutas en total", "Fruit Fight"),
		new AchievementData("Consigue una puntuación de 100 en el modo Clásico", "Great Fruit Ninja"),
		new AchievementData("Consigue una puntuación de 0", "Wake Up")
	};

	private static uint[] unlockTypeHashes = new uint[10]
	{
		StringFunctions.StringHash("TOTAL"),
		StringFunctions.StringHash("SCORE"),
		StringFunctions.StringHash("SCORE_UNSULLIED"),
		StringFunctions.StringHash("END_SCORE"),
		StringFunctions.StringHash("SPECIFIC"),
		StringFunctions.StringHash("CONSECUTIVE"),
		StringFunctions.StringHash("CONSECUTIVE_ANY"),
		StringFunctions.StringHash("COMBO"),
		StringFunctions.StringHash("COMBO_STAR"),
		StringFunctions.StringHash("SPECIFIC_ORDER")
	};

	private static LinkedList<KeyRemove> keys = new LinkedList<KeyRemove>();

	public Dictionary<uint, AchievementInfo> achievementInfo = new Dictionary<uint, AchievementInfo>();

	public Dictionary<uint, AchievementInfo>[] achievementTypeList = ArrayInit.CreateFilledArray<Dictionary<uint, AchievementInfo>>(10);

	private static AchievementManager instance;

	private static uint scoreID = 0u;

	public static int MAX_ORDER_LIST_TYPES => 10;

	public static void Update(GameTime game)
	{
	}

	private static string GetTitle()
	{
		return Game.game_work.language switch
		{
			StringTableUtils.Language.LANGUAGE_FRENCH => "Tu as gagné un succès !", 
			StringTableUtils.Language.LANGUAGE_ITALIAN => "Hai vinto un obiettivo!", 
			StringTableUtils.Language.LANGUAGE_GERMAN => "Du hast einen Erfolg erzielt!", 
			StringTableUtils.Language.LANGUAGE_SPANISH => "¡Has conseguido un logro!", 
			_ => "You've earned an Achievement!", 
		};
	}

	private static string GetLine1()
	{
		return Game.game_work.language switch
		{
			StringTableUtils.Language.LANGUAGE_FRENCH => "Déverrouille le jeu complet pour conserver ce succès.", 
			StringTableUtils.Language.LANGUAGE_ITALIAN => "Sblocca il gioco completo per conservare questo obiettivo.", 
			StringTableUtils.Language.LANGUAGE_GERMAN => "Schalte jetzt das vollständige Spiel frei, um den Erfolg zu behalten.", 
			StringTableUtils.Language.LANGUAGE_SPANISH => "Desbloquea el juego completo para conservar este logro.", 
			_ => "Unlock the Full Game now to keep this Achievement.", 
		};
	}

	public static bool UpdatePretendAchievements(SpriteBatch batch)
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		switch (awardMode)
		{
		case 1:
			y += 4;
			if (y > 0)
			{
				y = 0;
				awardMode++;
			}
			break;
		case 2:
			if (++delay > 90)
			{
				awardMode++;
			}
			break;
		case 3:
			y -= 4;
			if (y < -60)
			{
				y = -60;
				awardMode = 0;
				TheGame.instance.DoUpsell(exitGame: false);
			}
			break;
		}
		if (awardMode > 0)
		{
			batch.Begin();
			batch.Draw(TheGame.bobsCousinHairyMaclary, new Rectangle(0, y, 800, 60), new Color(127, 127, 127, 127));
			batch.DrawString(TheGame.instance.font3, GetTitle(), new Vector2(10f, (float)(y + 10)), Color.White);
			batch.DrawString(TheGame.instance.font3, GetLine1(), new Vector2(10f, (float)(y + 30)), Color.White);
			batch.End();
		}
		return awardMode > 0;
	}

	public static void AwardPretendAchievement(string achievementKey)
	{
		int hashCode = achievementKey.GetHashCode();
		if (akeys.Count > 0)
		{
			foreach (int akey in akeys)
			{
				if (akey == hashCode)
				{
					return;
				}
			}
			akeys.AddLast(hashCode);
		}
		else
		{
			akeys.AddLast(hashCode);
		}
		if (awardMode == 0)
		{
			awardMode = 1;
			y = -60;
			delay = 0;
		}
	}

	public static void ShowUnlockDropdown(int type)
	{
		if (unlockMode == 0)
		{
			unlockMode = 1;
			uy = -60;
			udelay = 0;
			utype = type;
		}
	}

	public static void UpdateUnlockDropdown(SpriteBatch batch)
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		switch (unlockMode)
		{
		case 1:
			uy += 4;
			if (uy > 0)
			{
				uy = 0;
				unlockMode++;
			}
			break;
		case 2:
			if (++udelay > 90)
			{
				unlockMode++;
			}
			break;
		case 3:
			uy -= 4;
			if (uy < -60)
			{
				uy = -60;
				unlockMode = 0;
				TheGame.instance.DoUpsell(exitGame: false);
			}
			break;
		}
		if (unlockMode > 0)
		{
			batch.Begin();
			batch.Draw(TheGame.bobsCousinHairyMaclary, new Rectangle(0, uy, 800, 60), new Color(127, 127, 127, 127));
			if (utype == 0)
			{
				batch.DrawString(TheGame.instance.font3, TheGame.instance.stringTable.GetString(61), new Vector2(10f, (float)(uy + 20)), Color.White);
			}
			else
			{
				batch.DrawString(TheGame.instance.font3, TheGame.instance.stringTable.GetString(60), new Vector2(10f, (float)(uy + 20)), Color.White);
			}
			batch.End();
		}
	}

	public static void GetAchievementsCallback(IAsyncResult result)
	{
	}

	public static string GetDesc(string achievementKey)
	{
		string result = null;
		switch (Game.game_work.language)
		{
		default:
		{
			AchievementData[] array2 = str_en;
			foreach (AchievementData achievementData2 in array2)
			{
				if (achievementData2.key == achievementKey)
				{
					result = achievementData2.desc;
					break;
				}
			}
			break;
		}
		case StringTableUtils.Language.LANGUAGE_FRENCH:
		{
			AchievementData[] array4 = str_fr;
			foreach (AchievementData achievementData4 in array4)
			{
				if (achievementData4.key == achievementKey)
				{
					result = achievementData4.desc;
					break;
				}
			}
			break;
		}
		case StringTableUtils.Language.LANGUAGE_ITALIAN:
		{
			AchievementData[] array5 = str_it;
			foreach (AchievementData achievementData5 in array5)
			{
				if (achievementData5.key == achievementKey)
				{
					result = achievementData5.desc;
					break;
				}
			}
			break;
		}
		case StringTableUtils.Language.LANGUAGE_GERMAN:
		{
			AchievementData[] array3 = str_de;
			foreach (AchievementData achievementData3 in array3)
			{
				if (achievementData3.key == achievementKey)
				{
					result = achievementData3.desc;
					break;
				}
			}
			break;
		}
		case StringTableUtils.Language.LANGUAGE_SPANISH:
		{
			AchievementData[] array = str_es;
			foreach (AchievementData achievementData in array)
			{
				if (achievementData.key == achievementKey)
				{
					result = achievementData.desc;
					break;
				}
			}
			break;
		}
		}
		return result;
	}

	public static string GetHow(string achievementKey)
	{
		string result = null;
		switch (Game.game_work.language)
		{
		default:
		{
			AchievementData[] array2 = str1_en;
			foreach (AchievementData achievementData2 in array2)
			{
				if (achievementData2.key == achievementKey)
				{
					result = achievementData2.desc;
					break;
				}
			}
			break;
		}
		case StringTableUtils.Language.LANGUAGE_FRENCH:
		{
			AchievementData[] array4 = str1_fr;
			foreach (AchievementData achievementData4 in array4)
			{
				if (achievementData4.key == achievementKey)
				{
					result = achievementData4.desc;
					break;
				}
			}
			break;
		}
		case StringTableUtils.Language.LANGUAGE_ITALIAN:
		{
			AchievementData[] array5 = str1_it;
			foreach (AchievementData achievementData5 in array5)
			{
				if (achievementData5.key == achievementKey)
				{
					result = achievementData5.desc;
					break;
				}
			}
			break;
		}
		case StringTableUtils.Language.LANGUAGE_GERMAN:
		{
			AchievementData[] array3 = str1_de;
			foreach (AchievementData achievementData3 in array3)
			{
				if (achievementData3.key == achievementKey)
				{
					result = achievementData3.desc;
					break;
				}
			}
			break;
		}
		case StringTableUtils.Language.LANGUAGE_SPANISH:
		{
			AchievementData[] array = str1_es;
			foreach (AchievementData achievementData in array)
			{
				if (achievementData.key == achievementKey)
				{
					result = achievementData.desc;
					break;
				}
			}
			break;
		}
		}
		return result;
	}

	public static int GetIndexForKey(string key)
	{
		return 0;
	}

	public static string GetDescription(int index)
	{
		return "...";
	}

	public static string GetKey(int index)
	{
		return null;
	}

	public static Texture2D GetPicture(int index)
	{
		return null;
	}

	public static int GetGamerScore(int index)
	{
		return 0;
	}

	public static bool Unlocked(int index)
	{
		return false;
	}

	private static int FindAchievementIndex(string key)
	{
		int num = 0;
		string[] array = achivementKeys;
		foreach (string text in array)
		{
			if (text == key)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public static void AwardAchievementWP7(string achievementKey)
	{
	}

	public static void AwardAchievementWP7ByIndex(int index)
	{
	}

	private static void AwardAchievementCallback(IAsyncResult result)
	{
		awarding = false;
	}

	public static AchievementManager GetInstance()
	{
		if (instance == null)
		{
			instance = new AchievementManager();
		}
		return instance;
	}

	private string Attribute(XElement element, string name)
	{
		return element.Attribute(name)?.Value;
	}

	public void LoadAchievementInfo()
	{
		this.achievementInfo.Clear();
		NotificationControl.s_banner = TextureManager.GetInstance().Load("texturesWP7/achievment_banner.tex");
		NotificationControl.s_unlockBanner = TextureManager.GetInstance().Load("texturesWP7/hud_unlocked_dialog.tex");
		XDocument xDocument = MortarXml.Load("xmlwp7/achievementList.xml");
		if (xDocument != null)
		{
			XElement element = xDocument.FirstChildElement("achievementManagerFile");
			for (element = element.FirstChildElement("achievement"); element != null; element = element.NextSiblingElement("achievement"))
			{
				string text = Attribute(element, "id");
				uint num = StringFunctions.StringHash(text);
				if (!Game.game_work.saveData.IsAchievementUnlocked(num))
				{
					AchievementInfo achievementInfo = new AchievementInfo();
					achievementInfo.id = text;
					achievementInfo.idHash = num;
					text = Attribute(element, "name");
					achievementInfo.name = text;
					this.achievementInfo[num] = achievementInfo;
					element.QueryIntAttribute("score", ref achievementInfo.score);
					element.QueryIntAttribute("total", ref achievementInfo.total);
					int value = 0;
					element.QueryIntAttribute("isGameOver", ref value);
					achievementInfo.isGameOver = value == 1;
					text = "achievements/" + Attribute(element, "texture");
					achievementInfo.texture = TextureManager.GetInstance().Load(text);
					text = Attribute(element, "mode");
					achievementInfo.modeMask = 0u;
					if (text != null)
					{
						string[] array = text.Split(',');
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = array[i].Trim();
							Game.GAME_MODE mode = Initialise.ParseGameMode(StringFunctions.StringHash(array[i]));
							achievementInfo.modeMask |= Initialise.GetModeBitMask(mode);
						}
					}
					else
					{
						achievementInfo.modeMask = Initialise.GetModeBitMask(Game.GAME_MODE.GM_CLASSIC);
					}
					achievementInfo.type = AchievementUnlockType.UNLOCK_TYPE_MAX;
					text = Attribute(element, "type");
					uint num2 = StringFunctions.StringHash(text);
					for (uint num3 = 0u; num3 < 10; num3++)
					{
						if (num2 != unlockTypeHashes[num3])
						{
							continue;
						}
						achievementInfo.type = (AchievementUnlockType)num3;
						switch (num3)
						{
						case 9u:
							achievementTypeList[num3][num] = achievementInfo;
							text = Attribute(element, "specific_type");
							achievementInfo.specificOrder = new SpecificOrder(text);
							break;
						case 1u:
						case 2u:
							achievementTypeList[num3][scoreID] = achievementInfo;
							scoreID++;
							break;
						default:
							achievementTypeList[num3][(uint)achievementInfo.total] = achievementInfo;
							text = Attribute(element, "specific_type");
							if (text != null)
							{
								achievementInfo.specificOrder = new SpecificOrder(text);
							}
							break;
						case 4u:
						case 5u:
						case 8u:
						{
							text = Attribute(element, "specific_type");
							uint key = StringFunctions.StringHash(text);
							achievementTypeList[num3][key] = achievementInfo;
							break;
						}
						}
						break;
					}
				}
			}
			return;
		}
		throw new Exception("Achievements xml is missing");
	}

	public void UnLoadAchievementInfo()
	{
		throw new MissingMethodException();
	}

	public bool UnlockTotalFruitAchievement(int total)
	{
		bool result = false;
		foreach (KeyValuePair<uint, AchievementInfo> item in achievementTypeList[0])
		{
			if (total >= item.Value.total && QueAchievement(item.Value, item.Key))
			{
				result = true;
			}
		}
		return result;
	}

	public bool UnlockScoreAchievement(int score)
	{
		bool result = false;
		foreach (KeyValuePair<uint, AchievementInfo> item in achievementTypeList[1])
		{
			if (score >= item.Value.total && (item.Value.modeMask & Initialise.GetModeBitMask(Game.game_work.gameMode)) != 0 && QueAchievement(item.Value, item.Key))
			{
				result = true;
			}
		}
		return result;
	}

	public bool UnlockScoreUnsulliedAchievement(int score)
	{
		bool result = false;
		foreach (KeyValuePair<uint, AchievementInfo> item in achievementTypeList[2])
		{
			if (score >= item.Value.total && (item.Value.modeMask & Initialise.GetModeBitMask(Game.game_work.gameMode)) != 0 && QueAchievement(item.Value, item.Key))
			{
				result = true;
			}
		}
		return result;
	}

	public bool UnlockSpecificFruitAchievement(int total, uint hash)
	{
		AchievementInfo achievementInfo = null;
		uint key = 0u;
		foreach (KeyValuePair<uint, AchievementInfo> item in achievementTypeList[4])
		{
			if (item.Key == hash)
			{
				achievementInfo = item.Value;
				key = item.Key;
				break;
			}
		}
		if (achievementInfo != null && total >= achievementInfo.total && (achievementInfo.modeMask & Initialise.GetModeBitMask(Game.game_work.gameMode)) != 0)
		{
			return QueAchievement(achievementInfo, key);
		}
		return false;
	}

	public bool UnlockEndScoreAchievement(int total, int pb)
	{
		bool result = false;
		keys.Clear();
		foreach (KeyValuePair<uint, AchievementInfo> item in achievementTypeList[3])
		{
			if ((total == item.Value.total || (item.Value.total < 0 && total == pb)) && (item.Value.modeMask & Initialise.GetModeBitMask(Game.game_work.gameMode)) != 0 && QueAchievement(item.Value, item.Key))
			{
				result = true;
			}
		}
		foreach (KeyRemove key in keys)
		{
			achievementTypeList[key.type].Remove(key.key);
		}
		return result;
	}

	public bool UnlockConsecutiveAchievement(int total, uint hash)
	{
		AchievementInfo achievementInfo = null;
		uint key = 0u;
		foreach (KeyValuePair<uint, AchievementInfo> item in achievementTypeList[5])
		{
			if (item.Key == hash)
			{
				achievementInfo = item.Value;
				key = item.Key;
				break;
			}
		}
		bool flag = false;
		if (achievementInfo != null && total >= achievementInfo.total && (achievementInfo.modeMask & Initialise.GetModeBitMask(Game.game_work.gameMode)) != 0)
		{
			flag = QueAchievement(achievementInfo, key) || flag;
		}
		achievementInfo = null;
		foreach (KeyValuePair<uint, AchievementInfo> item2 in achievementTypeList[6])
		{
			if (item2.Value.total == total)
			{
				achievementInfo = item2.Value;
				key = item2.Key;
				break;
			}
		}
		if (achievementInfo != null && (achievementInfo.modeMask & Initialise.GetModeBitMask(Game.game_work.gameMode)) != 0)
		{
			flag = QueAchievement(achievementInfo, key) || flag;
		}
		return false;
	}

	public bool UnlockComboAchievement(int total)
	{
		return UnlockComboAchievement(total, null);
	}

	public bool UnlockComboAchievement(int total, int[] fruitInCombo)
	{
		bool result = false;
		foreach (KeyValuePair<uint, AchievementInfo> item in achievementTypeList[7])
		{
			if (total < item.Value.total || (item.Value.modeMask & Initialise.GetModeBitMask(Game.game_work.gameMode)) == 0)
			{
				continue;
			}
			bool flag = true;
			if (item.Value.specificOrder != null)
			{
				int firstFruitTypeHash = (int)item.Value.specificOrder.GetFirstFruitTypeHash();
				int num = 0;
				for (int i = 0; i < total; i++)
				{
					if (Fruit.FruitTypeHash(fruitInCombo[i]) == firstFruitTypeHash)
					{
						num++;
					}
				}
				if (num < item.Value.total)
				{
					flag = false;
				}
			}
			if (flag && item.Value.isGameOver)
			{
				flag = false;
				if (Game.game_work.timeControl != null && total >= 3 && Game.game_work.timeControl.GetTime() <= 0f)
				{
					flag = true;
				}
			}
			if (flag && QueAchievement(item.Value, item.Key))
			{
				result = true;
			}
		}
		return result;
	}

	public bool UnlockComboStarAchievement(int total, uint nameHash)
	{
		AchievementInfo achievementInfo = null;
		uint key = 0u;
		foreach (KeyValuePair<uint, AchievementInfo> item in achievementTypeList[8])
		{
			if (item.Key == nameHash)
			{
				achievementInfo = item.Value;
				key = item.Key;
				break;
			}
		}
		if (achievementInfo != null && total >= achievementInfo.total && (achievementInfo.modeMask & Initialise.GetModeBitMask(Game.game_work.gameMode)) != 0)
		{
			return QueAchievement(achievementInfo, key);
		}
		return false;
	}

	public bool UnlockSpecificOrderAchievement(uint nameHash)
	{
		bool result = false;
		foreach (KeyValuePair<uint, AchievementInfo> item in achievementTypeList[9])
		{
			AchievementInfo value = item.Value;
			if (value.specificOrder != null && value.specificOrder.Check(nameHash) && QueAchievement(item.Value, item.Key))
			{
				result = true;
			}
		}
		return result;
	}

	private bool QueAchievement(AchievementInfo achievement, uint key)
	{
		if (achievement != null)
		{
			bool flag = false;
			if (Mortar.Math.BETWEEN(achievement.id[0], 48, 57))
			{
				if (NetworkManager.GetInstance().UserHasEnabledNetwork() && Game.game_work.saveData.AddToQue(achievement.id, achievement.idHash))
				{
					flag = true;
				}
			}
			else if (Game.game_work.saveData.AddToQue(achievement.id, achievement.idHash))
			{
				flag = true;
			}
			if (flag)
			{
				keys.AddLast(new KeyRemove((uint)achievement.type, key));
			}
		}
		return false;
	}

	public bool UnlockedAchievement(uint idHash, HUD hud)
	{
		AchievementInfo value = null;
		if (achievementInfo.TryGetValue(idHash, out value))
		{
			if (string.Compare(value.name, "UNLOCKED NEW BLADE") == 0 || string.Compare(value.name, "UNLOCKED NEW BACKGROUND") == 0)
			{
				uint hashCode = (uint)value.id.GetHashCode();
				for (int i = 0; i < Game.MAX_SYSTEM_ACHIEVEMENTS; i++)
				{
					if (hashCode == TheGame.settings.achievements[i])
					{
						return true;
					}
				}
				for (int j = 0; j < Game.MAX_SYSTEM_ACHIEVEMENTS; j++)
				{
					if (TheGame.settings.achievements[j] == 0)
					{
						TheGame.settings.achievements[j] = hashCode;
						break;
					}
				}
				string text = ((string.Compare(value.name, "UNLOCKED NEW BLADE") == 0) ? TheGame.instance.stringTable.GetString(61) : ((string.Compare(value.name, "UNLOCKED NEW BACKGROUND") != 0) ? value.name : TheGame.instance.stringTable.GetString(60)));
				NotificationControl notificationControl = new NotificationControl(text, value.score, value.texture, NotificationControl.NotificationType.ITEM_UNLOCK);
				notificationControl.Init();
				hud.AddNotification(notificationControl);
				TheGame.SaveConfig();
			}
			return true;
		}
		return false;
	}

	public bool AchievementExists(uint idHash)
	{
		foreach (KeyValuePair<uint, AchievementInfo> item in achievementInfo)
		{
			if (item.Value.idHash == idHash)
			{
				return true;
			}
		}
		return false;
	}
}
