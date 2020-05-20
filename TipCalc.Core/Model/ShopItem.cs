using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace TipCalc.Core.Model
{
    public class ShopItem : MvxViewModel
    {
        /// <summary>
        /// Id de l'item.
        /// </summary>
        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// Nom de l'item.
        /// </summary>
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// ListId qui contient cette item.
        /// </summary>
        private int _listId;
        public int ListId
        {
            get => _listId;
            set => SetProperty(ref _listId, value);
        }

        /// <summary>
        /// Prix de l'item ajouté. Non obligatoire.
        /// </summary>
        private double _price;
        public double Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        /// <summary>
        /// Url de l'image de l'item. Non obligatoire.
        /// </summary>
        private string _picture;
        public string Picture
        {
            get => _picture;
            set => SetProperty(ref _picture, value);
        }

        /// <summary>
        /// Liste de tags attaché à se produit. Utilisé pour le rechercher. (Des mots séparés par le caractère " | ") Non obligatoire.
        /// </summary>
        private string _tags;
        public string Tags
        {
            get => _tags;
            set => SetProperty(ref _tags, value);
        }

        /// <summary>
        /// Permet de connaitre le statut de la checkbox de l'item.
        /// </summary>
        private bool _checked;
        public bool Checked
        {
            get => _checked;
            set => SetProperty(ref _checked, value);
        }

        /// <summary>
        /// En cas de suppression de l'item.
        /// </summary>
        private bool _deleted;
        public bool Deleted
        {
            get => _deleted;
            set => SetProperty(ref _deleted, value);
        }

        // Constructeur
        public ShopItem( ShopItem shopItem)
        {
            Id = shopItem.Id;
            Name = shopItem.Name;
            ListId = shopItem.ListId;
            Price = shopItem.Price;
            Picture = shopItem.Picture;
            Tags = shopItem.Tags;
            Checked = shopItem.Checked;
            Deleted = shopItem.Deleted;
        }

        public ShopItem() { }
    }
}