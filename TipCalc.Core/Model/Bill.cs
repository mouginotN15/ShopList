using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace TipCalc.Core.Model
{
    public class Bill : MvxViewModel
    {
        /// <summary>
        /// Id unique de cette facture.
        /// </summary>
        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// UserId de l'utilisateur qui a crée cette facture.
        /// </summary>
        private int _ownerId;
        public int OwnerId
        {
            get => _ownerId;
            set => SetProperty(ref _ownerId, value);
        }

        /// <summary>
        /// ListId qui a généré cette facture.
        /// </summary>
        private int _listId;
        public int ListId
        {
            get => _listId;
            set => SetProperty(ref _listId, value);
        }

        /// <summary>
        /// Date de création de la facture.
        /// </summary>
        private DateTime _creationDate;
        public DateTime CreationDate
        {
            get => _creationDate;
            set => SetProperty(ref _creationDate, value);
        }

        /// <summary>
        /// Nombre d'item sur la facture (avec la multiplication des quantités)
        /// </summary>
        private int _nbOfItemBought;
        public int NbOfItemBought
        {
            get => _nbOfItemBought;
            set => SetProperty(ref _nbOfItemBought, value);
        }

        /// <summary>
        /// Prix total. (item * quantity * price)
        /// </summary>
        private double _totalPrice;
        public double TotalPrice
        {
            get => _totalPrice;
            set => SetProperty(ref _totalPrice, value);
        }

        /// <summary>
        /// En cas de suppression de la facture.
        /// </summary>
        private bool _deleted;
        public bool Deleted
        {
            get => _deleted;
            set => SetProperty(ref _deleted, value);
        }
    }
}
