using DynamicData;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TipCalc.Core.Model;
using TipCalc.Core.Services;

namespace TipCalc.Core.ViewModels
{
    public class ListRightGestionVM : MvxViewModel<ShopList>
    {
        private readonly IMvxNavigationService _navigationService;

        // ShopList actuellement ouverte.
        private ShopList _shopList;

        // Permet de faire les appels API.
        public API Api = new API();

        public ListRightGestionVM(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override void Prepare(ShopList parameter)
        {
            _shopList = parameter;
        }

        public override async Task Initialize()
        {
            PageTitle = "Rights gestion";
            UsersWithRight = new ObservableCollection<User>();

            // On vérifie que UserIdRight n'est pas null.
            if (_shopList.UserIdRight == null)
                _shopList.UserIdRight = "";


            if (_shopList.UserIdRight.Length != 0)
            {
                string[] listUserRights = _shopList.UserIdRight.Split('|');

                foreach (var IdUser in listUserRights)
                {
                    // TODO : Get user by Id and create a list of them in UsersWithRight to display it.

                    // UsersWithRight.Add(Api.UsersClient.);
                }
            }
        }


        #region Commands
        #endregion


        #region Properties
        /// <summary>
        /// Titre du header de la page mobile.
        /// </summary>
        private string _pageTitle;
        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                SetProperty(ref _pageTitle, value);
            }
        }


        /// <summary>
        /// Liste des utilisateurs autre que le créateur de la liste qui y ont accès.
        /// </summary>
        private ObservableCollection<User> _usersWithRight;
        public ObservableCollection<User> UsersWithRight
        {
            get => _usersWithRight;
            set
            {
                SetProperty(ref _usersWithRight, value);
            }
        }


        /// <summary>
        /// Texte de l'Entry pour ajouter le droit d'accès à cette liste à un nouvel utilisateur.
        /// </summary>
        private string _newUserName;
        public string NewUserName
        {
            get => _newUserName;
            set
            {
                SetProperty(ref _newUserName, value);
            }
        }
        #endregion
    }
}