using System.Linq;
using Xunit;
using SabberStoneCore.Actions;
using SabberStoneCore.Config;
using SabberStoneCore.Enums;
using SabberStoneCore.Model;
using SabberStoneCore.Tasks.PlayerTasks;
using SabberStoneCore.Model.Entities;
using System.Collections.Generic;
using System;
using SabberStoneCore.Tasks.SimpleTasks;

namespace SabberStoneCoreTest.Control
{
	// dotnet test --filter FullyQualifiedName~SabberStoneCoreTest.Control.ControlTest
	public class ControlTest
	{
		[Fact]
		public void AutoNextStep()
		{
			var gameTrue =
				new Game(new GameConfig
				{
					StartPlayer = 1,
					Player1HeroClass = CardClass.PRIEST,
					Player2HeroClass = CardClass.HUNTER,
					FillDecks = true,
					AutoNextStep = true
				});
			gameTrue.StartGame();
			Assert.Equal(Step.MAIN_ACTION, gameTrue.Step);
			Assert.Equal(Step.MAIN_ACTION, gameTrue.NextStep);

			var gameFalse =
				new Game(new GameConfig
				{
					StartPlayer = 1,
					Player1HeroClass = CardClass.PRIEST,
					Player2HeroClass = CardClass.HUNTER,
					FillDecks = true,
					AutoNextStep = false
				});
			gameFalse.StartGame();
			Assert.Equal(Step.INVALID, gameFalse.Step);
			Assert.Equal(Step.BEGIN_FIRST, gameFalse.NextStep);
		}

		[Fact]
		public void ControlFullGame()
		{
			/* This test simulates the 4th game of the Hearthstone World Championship final, which can be viewed from 46:13 at https://www.twitch.tv/videos/220978876
			 * It will be played with the point of view of tom60229, who plays a tempo rogue deck: http://www.hearthstonetopdecks.com/decks/tom60229s-tempo-rogue-hearthstone-world-championship-2017/
			 * tom60229 goes second versus Fr0zen's druid.
			 */

			List<Card> TempoRogue = new List<Card>()
			{
				Cards.FromName("Backstab"),
				Cards.FromName("Backstab"),
				Cards.FromName("Shadowstep"),
				Cards.FromName("Shadowstep"),
				Cards.FromName("Swashburglar"),
				Cards.FromName("Swashburglar"),
				Cards.FromName("Fire Fly"),
				Cards.FromName("Fire Fly"),
				Cards.FromName("Patches the Pirate"),
				Cards.FromName("Southsea Deckhand"),
				Cards.FromName("Southsea Deckhand"),
				Cards.FromName("Prince Keleseth"),
				Cards.FromName("Edwin VanCleef"),
				Cards.FromName("SI:7 Agent"),
				Cards.FromName("SI:7 Agent"),
				Cards.FromName("Southsea Captain"),
				Cards.FromName("Southsea Captain"),
				Cards.FromName("Elven Minstrel"),
				Cards.FromName("Saronite Chain Gang"),
				Cards.FromName("Saronite Chain Gang"),
				Cards.FromName("Vilespine Slayer"),
				Cards.FromName("Vilespine Slayer"),
				Cards.FromName("Cobalt Scalebane"),
				Cards.FromName("Cobalt Scalebane"),
				Cards.FromName("Leeroy Jenkins"),
				Cards.FromName("Bonemare"),
				Cards.FromName("Bonemare"),
				Cards.FromName("Corridor Creeper"),
				Cards.FromName("Corridor Creeper"),
				Cards.FromName("The Lich King"),
			};

			List<Card> UnknownDruid = new List<Card>();
			for (int i = 0; i < 30; i++)
			{
				UnknownDruid.Add(new Card()
				{
					Id = "Unknown",
					Name = "Unknown",
					Tags = new Dictionary<GameTag, int> { [GameTag.CARDTYPE] = (int)CardType.MINION },
				});
			}

			var randomController = new CustomRandomController();
			var game = new Game(
				new GameConfig()
				{
					StartPlayer = 2,
					Player1Name = "tom60229",
					Player1HeroClass = CardClass.ROGUE,
					Player1Deck = TempoRogue,
					Player2Name = "Fr0zen",
					Player2HeroClass = CardClass.DRUID,
					Player2Deck = UnknownDruid,
					FillDecks = false,
					Shuffle = false,
					SkipMulligan = false,
					AutoNextStep = false,
					Logging = false,
					History = false,
					RandomController = randomController
				});
			game.StartGame();
			Assert.Equal(30, game.Player1.DeckZone.Count);
			Assert.Equal(30, game.Player2.DeckZone.Count);

			/* MULLIGAN
			 * Fr0zen: draws 3 unknown cards; mulligans the 2nd; draws one unknown card
			 * tom60229: draws Leeroy Jenkins, Elven Minstrel, Shadowstep and Backstab; mulligans all;
			 *			 draws Backstab, Swashburglar, Edwin VanCleef and Southsea Deckhand; gets The Coin
			 */
			randomController.cardsToDraw = new List<string>(){ "Leeroy Jenkins", "Elven Minstrel", "Shadowstep", "Backstab" };
			game.BeginDraw();

			game.BeginMulligan();
			randomController.cardsToDraw = new List<string>(){ "Backstab", "Swashburglar", "Edwin VanCleef", "Southsea Deckhand" };
			game.Process(ChooseTask.Mulligan(game.Player1, new List<int>()));
			game.Process(ChooseTask.Mulligan(game.Player2, game.Player2.Choice.Choices));

			Assert.Equal(5, game.Player1.HandZone.Count);
			Assert.Equal(26, game.Player1.DeckZone.Count);
			Assert.Equal(3, game.Player2.HandZone.Count);
			Assert.Equal(27, game.Player2.DeckZone.Count);

			/* TURN 1: 30 - 30
			 * Fr0zen: draws one unknown card; plays Jade Idol; chooses to summon a 1/1 Jade Golem
			 * tom60229: draws Leeroy Jenkins; plays Swashburglar; gets Innervate; summons Patches whick attacks face
			 */
			ProcessTurnStart(game);
			Assert.Equal(4, game.Player2.HandZone.Count);
			Assert.Equal(26, game.Player2.DeckZone.Count);
			IPlayable toBePlayed = Entity.FromCard(game.Player2, Cards.FromName("Jade Idol"));
			game.Player2.HandZone.Replace(game.Player2.HandZone[1], toBePlayed);
			game.Process(PlayCardTask.Any(game.Player2, toBePlayed, null, 0, 1));
			game.MainCleanUp();
			Assert.Equal(3, game.Player2.HandZone.Count);
			Assert.Equal(1, game.Player2.BoardZone.Count);
			Assert.Equal("Jade Golem", game.Player2.BoardZone[0].Card.Name);
			ProcessTurnEnd(game);

			randomController.cardsToDraw = new List<string>() { "Leeroy Jenkins" };
			ProcessTurnStart(game);
			Assert.Equal(6, game.Player1.HandZone.Count);
			Assert.Equal(25, game.Player1.DeckZone.Count);
			randomController.cardsToPick = new List<string>() { "Innervate" };
			game.Process(PlayCardTask.Any(game.Player1, "Swashburglar", null, 0));
			game.MainCleanUp();
			Assert.Equal(6, game.Player1.HandZone.Count);
			Assert.Equal(2, game.Player1.BoardZone.Count);
			game.Process(MinionAttackTask.Any(game.Player1, game.Player1.BoardZone.Where(m => m.Card.Name == "Patches the Pirate").First(), game.Player2.Hero));
			game.MainCleanUp();
			ProcessTurnEnd(game);

			Assert.Equal(30, game.Player1.Hero.Health);
			Assert.Equal(29, game.Player2.Hero.Health);

			/* TURN 2: 29 - 30
			 * Fr0zen: draws one unknown card; uses hero power; hero attacks Patches; Jade Golem attacks the Swashburglar
			 * tom60229: draws Vilespine Slayer; plays Innervate; plays The Coin; plays Southsea Deckhand; plays Backstab on the Southsea Deckhand; plays Edwin VanCleef
			 */
			ProcessTurnStart(game);
			game.Process(HeroPowerTask.Any(game.Player2));
			game.MainCleanUp();
			Assert.Equal(1, game.Player2.Hero.Armor);
			game.Process(HeroAttackTask.Any(game.Player2, game.Player1.BoardZone.Where(m => m.Card.Name == "Patches the Pirate").First()));
			game.MainCleanUp();
			Assert.Equal(0, game.Player2.Hero.Armor);
			game.Process(MinionAttackTask.Any(game.Player2, game.Player2.BoardZone.Where(m => m.Card.Name == "Jade Golem").First(), game.Player1.BoardZone.Where(m => m.Card.Name == "Swashburglar").First()));
			game.MainCleanUp();
			Assert.Empty(game.Player1.BoardZone);
			Assert.Empty(game.Player2.BoardZone);
			ProcessTurnEnd(game);

			randomController.cardsToDraw = new List<string>() { "Vilespine Slayer" };
			ProcessTurnStart(game);
			game.Process(PlayCardTask.Any(game.Player1, "Innervate"));
			game.MainCleanUp();
			game.Process(PlayCardTask.Any(game.Player1, "The Coin"));
			game.MainCleanUp();
			game.Process(PlayCardTask.Any(game.Player1, "Southsea Deckhand", null, 0));
			game.MainCleanUp();
			game.Process(PlayCardTask.Any(game.Player1, "Backstab", game.Player1.BoardZone.Where(m => m.Card.Name == "Southsea Deckhand").First()));
			game.MainCleanUp();
			game.Process(PlayCardTask.Any(game.Player1, "Edwin VanCleef", null, 0));
			game.MainCleanUp();
			Assert.Equal(1, game.Player1.BoardZone.Count);
			Assert.Equal(10, game.Player1.BoardZone[0].AttackDamage);
			Assert.Equal(10, game.Player1.BoardZone[0].Health);
			ProcessTurnEnd(game);

			Assert.Equal(30, game.Player1.Hero.Health);
			Assert.Equal(29, game.Player2.Hero.Health);

			/* TURN 3: 29 - 30
			 * Fr0zen: draws one unknown card; plays Jade Blossom
			 * tom60229: draws Corridor Creeper; Edwin VanCleef attacks face; uses hero power; hero atacks face
			 */
			ProcessTurnStart(game);
			toBePlayed = Entity.FromCard(game.Player2, Cards.FromName("Jade Blossom"));
			game.Player2.HandZone.Replace(game.Player2.HandZone[0], toBePlayed);
			game.Process(PlayCardTask.Any(game.Player2, toBePlayed));
			game.MainCleanUp();
			Assert.Equal("Jade Golem", game.Player2.BoardZone[0].Card.Name);
			Assert.Equal(2, game.Player2.BoardZone[0].AttackDamage);
			ProcessTurnEnd(game);

			randomController.cardsToDraw = new List<string>() { "Corridor Creeper" };
			ProcessTurnStart(game);
			game.Process(MinionAttackTask.Any(game.Player1, game.Player1.BoardZone.Where(m => m.Card.Name == "Edwin VanCleef").First(), game.Player2.Hero));
			game.MainCleanUp();
			game.Process(HeroPowerTask.Any(game.Player1));
			game.MainCleanUp();
			game.Process(HeroAttackTask.Any(game.Player1, game.Player2.Hero));
			game.MainCleanUp();
			ProcessTurnEnd(game);

			Assert.Equal(30, game.Player1.Hero.Health);
			Assert.Equal(18, game.Player2.Hero.Health);

			/* TURN 4: 18 - 30
			 * Fr0zen: draws one unknown card; plays Violet Teacher; plays Mark of the Lotus; Jade Golem attacks face
			 * tom60229: draws Swashburglar; plays Swashburglar; gets Aya Blackpaw; Edwin VanCleef attacks face; hero attacks face; uses hero power
			 */
			ProcessTurnStart(game);
			toBePlayed = Entity.FromCard(game.Player2, Cards.FromName("Violet Teacher"));
			game.Player2.HandZone.Replace(game.Player2.HandZone[0], toBePlayed);
			game.Process(PlayCardTask.Any(game.Player2, toBePlayed, null, 0));
			game.MainCleanUp();
			toBePlayed = Entity.FromCard(game.Player2, Cards.FromName("Mark of the Lotus"));
			game.Player2.HandZone.Replace(game.Player2.HandZone[0], toBePlayed);
			game.Process(PlayCardTask.Any(game.Player2, toBePlayed));
			game.MainCleanUp();
			game.Process(MinionAttackTask.Any(game.Player2, game.Player2.BoardZone.Where(m => m.Card.Name == "Jade Golem").First(), game.Player1.Hero));
			game.MainCleanUp();
			ProcessTurnEnd(game);

			randomController.cardsToDraw = new List<string>() { "Swashburglar" };
			ProcessTurnStart(game);
			randomController.cardsToPick = new List<string>() { "Jade Idol" }; // Should be Aya blackpaw, but bug in RandomCardTask
			game.Process(PlayCardTask.Any(game.Player1, "Swashburglar", null, 0));
			game.MainCleanUp();
			game.Process(MinionAttackTask.Any(game.Player1, game.Player1.BoardZone.Where(m => m.Card.Name == "Edwin VanCleef").First(), game.Player2.Hero));
			game.MainCleanUp();
			game.Process(HeroAttackTask.Any(game.Player1, game.Player2.Hero));
			game.MainCleanUp();
			game.Process(HeroPowerTask.Any(game.Player1));
			game.MainCleanUp();
			ProcessTurnEnd(game);

			Assert.Equal(27, game.Player1.Hero.Health);
			Assert.Equal(7, game.Player2.Hero.Health);

			/* TURN 5: 7 - 27
			 * Fr0zen: draws one unknown card; plays Swipe on Edwin VanCleef; plays Wild Growth; Violet Teacher attacks Edwin VanCleef; 2/2 attacks Edwin VanCleef; Jade Golem attacks face
			 * tom60229: draws Southsea Captain; plays Leeroy Jenkins; Leeroy Jenkins attacks face; hero attacks face
			 */
			ProcessTurnStart(game);
			toBePlayed = Entity.FromCard(game.Player2, Cards.FromName("Swipe"));
			game.Player2.HandZone.Replace(game.Player2.HandZone[0], toBePlayed);
			game.Process(PlayCardTask.Any(game.Player2, toBePlayed, game.Player1.BoardZone.Where(m => m.Card.Name == "Edwin VanCleef").First()));
			game.MainCleanUp();
			game.Process(MinionAttackTask.Any(game.Player2, game.Player2.BoardZone.Where(m => m.Card.Name == "Violet Teacher").First(), game.Player1.BoardZone.Where(m => m.Card.Name == "Edwin VanCleef").First()));
			game.MainCleanUp();
			game.Process(MinionAttackTask.Any(game.Player2, game.Player2.BoardZone.Where(m => m.Card.Name == "Violet Apprentice").ElementAt(1), game.Player1.BoardZone.Where(m => m.Card.Name == "Edwin VanCleef").First()));
			game.MainCleanUp();
			Assert.Empty(game.Player1.BoardZone);
			game.Process(MinionAttackTask.Any(game.Player2, game.Player2.BoardZone.Where(m => m.Card.Name == "Jade Golem").First(), game.Player1.Hero));
			game.MainCleanUp();
			ProcessTurnEnd(game);

			randomController.cardsToDraw = new List<string>() { "Southsea Captain" };
			ProcessTurnStart(game);
			game.Process(PlayCardTask.Any(game.Player1, "Leeroy Jenkins", null, 0));
			game.MainCleanUp();
			game.Process(MinionAttackTask.Any(game.Player1, game.Player1.BoardZone.Where(m => m.Card.Name == "Leeroy Jenkins").First(), game.Player2.Hero));
			game.MainCleanUp();
			game.Process(HeroAttackTask.Any(game.Player1, game.Player2.Hero));
			game.MainCleanUp();

			Assert.Equal(23, game.Player1.Hero.Health);
			Assert.Equal(0, game.Player2.Hero.Health);
			Assert.Equal(Step.FINAL_WRAPUP, game.NextStep);

			/* FINAL: 0 - 23 */
			game.FinalWrapUp();
			Assert.Equal(PlayState.WON, game.Player1.PlayState);
			Assert.Equal(PlayState.LOST, game.Player2.PlayState);
		}

		[Fact]
		public void DiscoverTest()
		{
			var randomController = new CustomRandomController();
			var game =
				new Game(new GameConfig
				{
					StartPlayer = 1,
					Player1HeroClass = CardClass.MAGE,
					Player2HeroClass = CardClass.HUNTER,
					AutoNextStep = false,
					RandomController = randomController
				});
			game.StartGame();
			game.Player1.BaseMana = 10;
			game.Player2.BaseMana = 10;

			ProcessTurnStart(game);
			Assert.Equal(0, game.Player1.HandZone.Count);
			randomController.cardsToDiscover = new List<string>() { "The Lich King", "Frostwolf Grunt", "Goldshire Footman"};
			IPlayable toBePlayed = Entity.FromCard(game.Player1, Cards.FromName("Stonehill Defender"));
			game.Player1.HandZone.Add(toBePlayed);
			game.Process(PlayCardTask.Any(game.Player1, toBePlayed, null, 0));
			game.MainCleanUp();
			game.Process(ChooseTask.Pick(game.Player1, game.Player1.Choice.Choices[0]));
			game.MainCleanUp();
			Assert.Equal(1, game.Player1.HandZone.Count);
			Assert.Equal("The Lich King", game.Player1.HandZone[0].Card.Name);
			ProcessTurnEnd(game);
		}

		[Fact]
		public void RandomSpellTest()
		{
			var randomController = new CustomRandomController();
			var game =
				new Game(new GameConfig
				{
					StartPlayer = 1,
					Player1HeroClass = CardClass.HUNTER,
					Player2HeroClass = CardClass.MAGE,
					AutoNextStep = false,
					RandomController = randomController
				});
			game.StartGame();
			game.Player1.BaseMana = 10;
			game.Player2.BaseMana = 10;

			ProcessTurnStart(game);
			IPlayable toBePlayed = Entity.FromCard(game.Player1, Cards.FromName("Alleycat"));
			game.Player1.HandZone.Add(toBePlayed);
			game.Process(PlayCardTask.Any(game.Player1, toBePlayed, null, 0));
			game.MainCleanUp();
			ProcessTurnEnd(game);

			Assert.Equal(29, game.Player1.Hero.Health); // -1 for fatigue
			Assert.Equal(2, game.Player1.BoardZone.Count);

			ProcessTurnStart(game);
			toBePlayed = Entity.FromCard(game.Player2, Cards.FromName("Arcane Missiles"));
			game.Player2.HandZone.Add(toBePlayed);
			game.Process(PlayCardTask.Any(game.Player2, toBePlayed));
			game.MainCleanUp();
			ProcessTurnEnd(game);

			Assert.Equal(28, game.Player1.Hero.Health);
			Assert.Equal(0, game.Player1.BoardZone.Count);
		}

		private void ProcessTurnStart(Game game)
		{
			game.MainReady();
			game.MainStartTriggers();
			game.MainRessources();
			game.MainDraw();
			game.MainStart();
		}

		private void ProcessTurnEnd(Game game)
		{
			game.MainEnd();
			game.MainNext();
		}
	}
	

	internal class CustomRandomController : ThrowRandomController
	{
		internal List<string> cardsToDraw = new List<string>();
		internal List<string> cardsToPick = new List<string>();
		internal List<string> cardsToDiscover = new List<string>();

		public override IPlayable PickDraw(Controller c)
		{
			if (c.PlayerId == 1)
			{
				IPlayable cardDrawn = c.DeckZone.GetAll.Where(ip => ip.Card.Name == cardsToDraw[0]).First();
				cardsToDraw.RemoveAt(0);
				return cardDrawn;
			} else
			{
				return c.DeckZone[0];
			}
		}

		public override Card PickCard(RandomCardTask task, IEntity source, IEntity target, List<Card> cards)
		{
			Card cardPicked = cards.Where(card => card.Name == cardsToPick[0]).First();
			cardsToPick.RemoveAt(0);
			return cardPicked;
		}

		public override Card PickDiscoverChoices(DiscoverType type, IEntity source, IEntity target, List<Card> cards)
		{
			Card cardPicked = cards.Where(card => card.Name == cardsToDiscover[0]).First();
			cardsToDiscover.RemoveAt(0);
			return cardPicked;
		}

		public override IPlayable PickTarget(EntityType type, IEntity source, IEntity target, List<IPlayable> entities)
		{
			return entities[entities.Count - 1];
		}
	}
}
