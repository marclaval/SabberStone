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
        Card PickSpell(IEntity source, IEntity target, Card[] cards);
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
        public virtual Card PickAdaptChoices(EntityType type, IEntity source, IEntity target, List<Card> cards) {
            return Util.Choose<Card>(cards);
        }

		public string PickBasicTotem(string[] totems) => Util.Choose<string>(totems);

		public virtual Card PickCard(RandomCardTask task, IEntity source, IEntity target, List<Card> cards) {
            return Util.Choose<Card>(cards);
        }

        public virtual Card PickDiscoverChoices(DiscoverType type, IEntity source, IEntity target, List<Card> cards)
        {
            return Util.Choose<Card>(cards);
        }

		public virtual IPlayable PickDraw(Controller c)
		{
            return c.DeckZone.TopCard;
		}

		public string PickEntourage(IEntity source, IEntity target, string[] cards)
		{
			return Util.Choose<string>(cards);
		}

		public virtual CardClass PickHeroClass(IEntity source, IEntity target, CardClass[] classes)
		{
			return Util.RandomElement(classes);
		}

		public virtual IPlayable PickJoust(Controller c)
		{
			return c.DeckZone.Random;
		}

		public virtual Card PickMinion(RandomMinionTask task, IEntity source, IEntity target, List<Card> cards)
		{
			return Util.Choose<Card>(cards);
		}

		public virtual Card PickMinionNumber(int cost, IEntity source, IEntity target, List<Card> cards)
		{
			return Util.Choose<Card>(cards);
		}

		public virtual Card PickPotionSpell(IEntity source, IEntity target, List<Card> cards)
        {
            return Util.Choose<Card>(cards);
        }

		public virtual Minion PickRecruit(IEntity source, IEntity target, List<Minion> minions)
		{
			return Util.Choose<Minion>(minions);
		}

		public virtual Card PickReplace(IZone zone, IEntity source, IEntity target, List<Card> cards)
		{
			return Util.Choose<Card>(cards);
		}

        public virtual Card PickSpell(IEntity source, IEntity target, Card[] cards) {
            return Util.Choose<Card>(cards);
        }

		public IPlayable PickTarget(EntityType type, IEntity source, IEntity target, List<IPlayable> entities) {
            return Util.Choose<IPlayable>(entities);
        }

		public virtual Card PickTransformMinion(IEntity source, IEntity target, List<Card> cards)
		{
			return Util.Choose<Card>(cards);
		}

		public virtual string UnusedPickCardAsString(List<string> cards) => Util.Choose<string>(cards);
        public virtual Card PickCard(string context, List<Card> cards) => Util.Choose<Card>(cards);

		public virtual int CoinFlip(IEntity source, IEntity target)
		{
			return Util.Random.Next(0, 2);
		}

		public virtual int RandomDamage(int amount, IEntity source, IEntity target)
		{
			return Util.Random.Next(0, amount + 1);
		}

		public virtual int GetNumber(int min, int max, IEntity source, IEntity target)
		{
			return Util.Random.Next(min, max + 1);
		}

		public virtual List<IPlayable> SortSummonCopy(IEntity source, IEntity target, List<IPlayable> entities)
		{
			return entities.OrderBy(x => Util.Random.Next()).ToList();
		}
	}

	public class ThrowRandomController : IRandomController
	{
		public virtual Card PickAdaptChoices(EntityType type, IEntity source, IEntity target, List<Card> cards) => throw new Exception("PickAdaptChoices not implemented");
        public virtual string PickBasicTotem(string[] totems) => throw new Exception("PickBasicTotem not implemented");
		public virtual Card PickCard(RandomCardTask task, IEntity source, IEntity target, List<Card> cards) => throw new Exception("PickCard not implemented");
		public virtual Card PickDiscoverChoices(DiscoverType type, IEntity source, IEntity target, List<Card> cards) => throw new Exception("PickDiscoverChoices not implemented");
		public virtual IPlayable PickDraw(Controller c) => throw new Exception("PickDraw not implemented");
        public virtual string PickEntourage(IEntity source, IEntity target, string[] cards) => throw new Exception("PickEntourage not implemented");
		public virtual CardClass PickHeroClass(IEntity source, IEntity target, CardClass[] classes) => throw new Exception("PickHeroClass not implemented");
		public virtual IPlayable PickJoust(Controller c) => throw new Exception("PickJoust not implemented");
		public virtual Card PickMinion(RandomMinionTask task, IEntity source, IEntity target, List<Card> cards) => throw new Exception("PickMinion not implemented");
		public virtual Card PickMinionNumber(int cost, IEntity source, IEntity target, List<Card> cards) => throw new Exception("PickMinionNumber not implemented");
		public virtual Card PickPotionSpell(IEntity source, IEntity target, List<Card> cards) => throw new Exception("PickPotionSpell not implemented");
		public virtual Minion PickRecruit(IEntity source, IEntity target, List<Minion> minions) => throw new Exception("PickRecruit not implemented");
		public virtual Card PickReplace(IZone zone, IEntity source, IEntity target, List<Card> cards) => throw new Exception("PickReplace not implemented");
        public virtual Card PickSpell(IEntity source, IEntity target, Card[] card) => throw new Exception("PickSpell not implemented");
		public virtual IPlayable PickTarget(EntityType type, IEntity source, IEntity target, List<IPlayable> entities) => throw new Exception("PickTarget not implemented");
		public virtual Card PickTransformMinion(IEntity source, IEntity target, List<Card> cards) => throw new Exception("PickTransformMinion not implemented");
		public virtual string UnusedPickCardAsString(List<string> cards) => throw new Exception("UnusedPickCardAsString not implemented");
		public virtual Card PickCard(string context, List<Card> cards) => throw new Exception("PickCard2 not implemented");
		public virtual int CoinFlip(IEntity source, IEntity target) => throw new Exception("CoinFlip not implemented");
		public virtual int RandomDamage(int amount, IEntity source, IEntity target) => throw new Exception("RandomDamage not implemented");
		public virtual int GetNumber(int min, int max, IEntity source, IEntity target) => throw new Exception("GetNumber not implemented");
		public virtual List<IPlayable> SortSummonCopy(IEntity source, IEntity target, List<IPlayable> entities) => throw new Exception("SortSummonCopy not implemented");
    }
}
