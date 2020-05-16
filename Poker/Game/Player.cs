using Cards;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solitare.Game
{
    public class Player : INotifyPropertyChanged
    {
        [JsonProperty]
        public int Index = 0;

        [JsonProperty]
        public Guid Uuid;

        private ObservableCollection<Card> _hand;

        [JsonProperty]
        public ObservableCollection<Card> Hand
        {
            get
            {
                return this._hand;
            }

            set
            {
                if (!object.Equals(this._hand, value))
                {
                    this._hand = value;
                    this.RaisePropertyChanged("Hand");
                }
            }
        }

        private bool _isDealer;

        private bool _isOut;

        public bool IsShowingHand = false;

        [JsonProperty]
        public bool IsOut
        {
            get
            {
                return this._isOut;
            }

            set
            {
                if (!object.Equals(this._isOut, value))
                {
                    this._isOut = value;
                    this.RaisePropertyChanged("IsOut");
                }
            }
        }

        [JsonProperty]
        public bool IsDealer
        {
            get
            {
                return this._isDealer;
            }

            set
            {
                if (!object.Equals(this._isDealer, value))
                {
                    this._isDealer = value;
                    this.RaisePropertyChanged("IsDealer");
                }
            }
        }

        private string _name;

        [JsonProperty]
        public string Name
        {
            get
            {
                return this._name;
            }

            set
            {
                if (!object.Equals(this._name, value))
                {
                    this._name = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }

        //private ObservableCollection<Chip> _chips;

        //public ObservableCollection<Chip> Chips
        //{
        //    get
        //    {
        //        return this._chips;
        //    }

        //    set
        //    {
        //        if (!object.Equals(this._chips, value))
        //        {
        //            this._chips = value;
        //            this.RaisePropertyChanged("Chips");
        //        }
        //    }
        //}

        private int _points;

        [JsonProperty]
        public int Points
        {
            get
            {
                return this._points;
            }

            set
            {
                if (!object.Equals(this._points, value))
                {
                    this._points = value;
                    this.RaisePropertyChanged(nameof(Points));
                }
            }
        }

        private int _activeBet;

        private int _pendingBet;

        [JsonProperty]
        public int BetForRound
        {
            get
            {
                return this._pendingBet;
            }

            set
            {
                if (!object.Equals(this._pendingBet, value))
                {
                    this._pendingBet = value;
                    this.RaisePropertyChanged(nameof(BetForRound));
                }
            }
        }

        [JsonProperty]
        public int BetForGame
        {
            get
            {
                return this._activeBet;
            }

            set
            {
                if (!object.Equals(this._activeBet, value))
                {
                    this._activeBet = value;
                    this.RaisePropertyChanged(nameof(BetForGame));
                }
            }
        }

        public void ResetHand()
        {
            this.Hand = new ObservableCollection<Card>();
            this.Points += this.BetForGame;
            this.BetForGame = 0;
            this.IsOut = false;
        }

        public void SetIsDealer(bool isDealer)
        {
            this.IsDealer = isDealer;
        }

        //private ObservableCollection<Chip> _activeBet;

        //public ObservableCollection<Chip> ActiveBet
        //{
        //    get
        //    {
        //        return this._activeBet;
        //    }

        //    set
        //    {
        //        if (!object.Equals(this._activeBet, value))
        //        {
        //            this._activeBet = value;
        //            this.RaisePropertyChanged("ActiveBet");
        //        }
        //    }
        //}

        private bool _isFake;

        private bool _isMyTurn;

        [JsonProperty]
        public bool IsMyTurn
        {
            get
            {
                return this._isMyTurn;
            }

            set
            {
                if (!object.Equals(this._isMyTurn, value))
                {
                    this._isMyTurn = value;
                    this.RaisePropertyChanged("IsMyTurn");
                }
            }
        }


        public bool IsFake
        {
            get
            {
                return this._isFake;
            }

            set
            {
                if (!object.Equals(this._isFake, value))
                {
                    this._isFake = value;
                    this.RaisePropertyChanged("IsFake");
                }
            }
        }

        private bool _isMe;

        [JsonIgnore]
        public bool IsMe
        {
            get
            {
                return this._isMe;
            }

            set
            {
                if (!object.Equals(this._isMe, value))
                {
                    this._isMe = value;
                    this.RaisePropertyChanged("IsMe");
                }
            }
        }

        public Player()
        {

        }

        public Player(int index)
        {
            this.Index = index;
            this.Name = "No player";
            this.IsFake = true;
        }

        public Player(string name, int index, Guid uuid, bool isMe = false)
        {
            this.Index = index;
            this.IsFake = false;
            this.Name = name;
            this.IsMe = isMe;
            this.Uuid = uuid;

            if (!this.IsFake)
            {
                this.Points = 500;
            }

            var newHand = new ObservableCollection<Card>();
            this.Hand = newHand;
        }

        public void GiveCard(Card card)
        {
            var cards = this.Hand;

            if (!this.IsMe)
            {
                card.IsFaceUp = false;
            }

            cards.Add(card);
            this.Hand = cards;
        }

        public void SetHand(List<Card> cards)
        {
            this.Hand.Clear();
            foreach(var card in cards)
            {
                if (this.IsMe)
                {
                    card.IsFaceUp = true;
                }

                this.Hand.Add(card);
            }
        }

        public void HideHand()
        {
            this.IsShowingHand = false;

            foreach (var card in this.Hand)
            {
                card.IsFaceUp = false;
            }
        }

        public void ShowHand()
        {
            this.IsShowingHand = true;

            foreach (var card in this.Hand)
            {
                card.IsFaceUp = true;
            }
        }

        public void SetChips()
        {

        }

        public bool PlacePendingBet(int bet, bool deduct)
        {
            if (this.IsMyTurn && this.Points >= bet)
            {
                this.BetForRound += bet;
                if (deduct)
                {
                    this.Points -= bet;
                }

                return true;
            }

            return false;
        }

        public bool CanPlaceBet()
        {
            if (this.IsMyTurn && this.Points >= this.BetForRound)
            {
                return true;
            }

            return false;
        }

        public bool PlaceBet(bool commit = false)
        {
            if (commit)
            {
                this.BetForGame += this.BetForRound;
                this.BetForRound = 0;

                return true;
            }
            else if (CanPlaceBet())
            {
                this.Points -= this.BetForRound;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Event indicating that a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Raises the PropertyChanged event.</summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
