using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace TipCalc.Core.Model
{
    public class ShopList : MvxViewModel
    {
        /// <summary>
        /// Id de la liste. Principalement utilisé pour transmettre les droits ou ajouter/supprimer/Editer un ShopItem.
        /// </summary>
        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// Nom de la liste.
        /// </summary>
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// UserId de l'utilisateur qui a crée cette liste.
        /// </summary>
        private int _ownerId;
        public int OwnerId
        {
            get => _ownerId;
            set => SetProperty(ref _ownerId, value);
        }

        /// <summary>
        /// Tout les Id des utilisateurs qui on le droit d'accès à cette liste.
        /// </summary>
        private string _userIdRight;
        public string UserIdRight
        {
            get => _userIdRight;
            set => SetProperty(ref _userIdRight, value);
        }

        /// <summary>
        /// Date de création de la liste par un utilisateur.
        /// </summary>
        private DateTime _creationDate;
        public DateTime CreationDate
        {
            get => _creationDate;
            set => SetProperty(ref _creationDate, value);
        }

        /// <summary>
        /// En cas de suppression de la liste.
        /// </summary>
        private bool _deleted;
        public bool Deleted
        {
            get => _deleted;
            set => SetProperty(ref _deleted, value);
        }
    }
}
