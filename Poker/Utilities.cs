using Cards;
using System;
using System.Collections.Generic;

namespace Solitare
{
    public class Utilities
    {
        public static Stack<Card> Shuffle()
        {
            List<Card> unshuffledDeck = new List<Card>();

            // create list of the cards unshuffled.
            for (int i = 0; i < 52; i++)
            {
                // You can cast an int to an enum.  Unless you explictly specify the enum values,
                // the first enum value is 0, the next is 1, and so forth.
                Suit suit = (Suit)(i / 13);

                // '%' is the modulus operator.  Gives you the remainder of the division.
                int rank = (i % 13) + 1;

                unshuffledDeck.Add(new Card(suit, rank, true));
            }

            // randomly shuffle them into a Stack<Card> that will be the deck.
            Stack<Card> deck = new Stack<Card>();
            Random randcard = new Random();
            for (int i = 0; i < 52; i++)
            {
                // randomly pick a card from what remains in the list.
                int cardIndex = randcard.Next(unshuffledDeck.Count);

                // add it to the deck and remove it from the unshuffled list
                deck.Push(unshuffledDeck[cardIndex]);
                unshuffledDeck.RemoveAt(cardIndex);
            }

            return deck;
        }
    }
}
