using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace TipCalc.Core.Model
{
    public class User : MvxViewModel
    {
        /// <summary>
        /// Id de l'utilisateur. Princpalement utilisé pour identifier un utilisateur unique lors de requête.
        /// </summary>
        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// Identifiant de connexion. Sert aussi de pseudo au sein de l'application.
        /// </summary>
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// Mot de passe de l'utilisateur pour se connecter à son compte.
        /// </summary>
        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        /// <summary>
        /// Chaine de caractère qui permet de stocker à quelles listes un utilisateur à le droit d'avoir accès. (Le nom/Id des listes est à la suite, séparé par le caractère " | " )
        /// </summary>
        private string _right;
        public string Right
        {
            get => _right;
            set => SetProperty(ref _right, value);
        }
    }
}
