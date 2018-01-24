using System;
using System.Collections.Generic;
using System.Linq;
using SabberStoneCore.Enums;
using SabberStoneCore.Model.Entities;
using SabberStoneCore.Model.Zones;
using SabberStoneCore.Tasks.SimpleTasks;

namespace SabberStoneCore.Model
{
    public interface IRandomController {
        Card PickAdaptChoices(EntityType type, IEntity source, IEntity target, List<Card> cards);
        string PickBasicTotem(string[] totems);
		Card PickCard(RandomCardTask task, IEntity source, IEntity target, List<Card> cards);
        Card PickDiscoverChoices(DiscoverType type, IEntity source, IEntity target, List<Card> cards);
		IPlayable PickDraw(Controller c);
        string PickEntourage(IEntity source, IEntity target, string[] cards);
		CardClass PickHeroClass(IEntity source, IEntity target, CardClass[] classes);
		IPlayable PickJoust(Controller c);
		Card PickMinion(RandomMinionTask task, IEntity source, IEntity target, List<Card> cards);
		Card PickMinionNumber(int cost, IEntity source, IEntity target, List<Card> cards);
		Card PickPotionSpell(IEntity source, IEntity target, List<Card> cards);
		Minion PickRecruit(IEntity source, IEntity target, List<Minion> minions);
		Card PickReplace(IZone zone, IEntity source, IEntity target, List<Card> cards);
        Card PickSpell(IEntity source, IEntity target, Card[] card);
		IPlayable PickTarget(EntityType type, IEntity source, IEntity target, List<IPlayable> entities);
		Card PickTransformMinion(IEntity source, IEntity target, List<Card> cards);

		// Only used outside of the main game loop or in unused classes
		string UnusedPickCardAsString(List<string> cards);
        Card PickCard(string context, List<Card> cards);

		// Others
		int CoinFlip(IEntity source, IEntity target);
		int RandomDamage(int amount, IEntity source, IEntity target);
		int GetNumber(int min, int max, IEntity source, IEntity target);
		List<IPlayable> SortSummonCopy(IEntity source, IEntity target, List<IPlayable> entities);
	}

    public class RandomController: IRandomController
    {
        public Card PickAdaptChoices(EntityType type, IEntity source, IEntity target, List<Card> cards) {
            return Util.Choose<Card>(cards);
        }

		public string PickBasicTotem(string[] totems) => Util.Choose<string>(totems);

		public Card PickCard(RandomCardTask task, IEntity source, IEntity target, List<Card> cards) {
            return Util.Choose<Card>(cards);
        }

        public Card PickDiscoverChoices(DiscoverType type, IEntity source, IEntity target, List<Card> cards)
        {
            return Util.Choose<Card>(cards);
        }

		public IPlayable PickDraw(Controller c)
		{
            return c.DeckZone.TopCard;
		}

		public string PickEntourage(IEntity source, IEntity target, string[] cards)
		{
			return Util.Choose<string>(cards);
		}

		public CardClass PickHeroClass(IEntity source, IEntity target, CardClass[] classes)
		{
			return Util.RandomElement(classes);
		}

		public IPlayable PickJoust(Controller c)
		{
			return c.DeckZone.Random;
		}

		public Card PickMinion(RandomMinionTask task, IEntity source, IEntity target, List<Card> cards)
		{
			return Util.Choose<Card>(cards);
		}

		public Card PickMinionNumber(int cost, IEntity source, IEntity target, List<Card> cards)
		{
			return Util.Choose<Card>(cards);
		}

		public Card PickPotionSpell(IEntity source, IEntity target, List<Card> cards)
        {
            return Util.Choose<Card>(cards);
        }

		public Minion PickRecruit(IEntity source, IEntity target, List<Minion> minions)
		{
			return Util.Choose<Minion>(minions);
		}

		public Card PickReplace(IZone zone, IEntity source, IEntity target, List<Card> cards)
		{
			return Util.Choose<Card>(cards);
		}

        public Card PickSpell(IEntity source, IEntity target, Card[] cards) {
            return Util.Choose<Card>(cards);
        }

		public IPlayable PickTarget(EntityType type, IEntity source, IEntity target, List<IPlayable> entities) {
            return Util.Choose<IPlayable>(entities);
        }

		public Card PickTransformMinion(IEntity source, IEntity target, List<Card> cards)
		{
			return Util.Choose<Card>(cards);
		}

		public string UnusedPickCardAsString(List<string> cards) => Util.Choose<string>(cards);
        public Card PickCard(string context, List<Card> cards) => Util.Choose<Card>(cards);

		public int CoinFlip(IEntity source, IEntity target)
		{
			return Util.Random.Next(0, 2);
		}

		public int RandomDamage(int amount, IEntity source, IEntity target)
		{
			return Util.Random.Next(0, amount + 1);
		}

		public int GetNumber(int min, int max, IEntity source, IEntity target)
		{
			return Util.Random.Next(min, max + 1);
		}

		public List<IPlayable> SortSummonCopy(IEntity source, IEntity target, List<IPlayable> entities)
		{
			return entities.OrderBy(x => Util.Random.Next()).ToList();
		}
	}
}
