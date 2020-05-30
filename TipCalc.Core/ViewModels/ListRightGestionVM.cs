using DynamicData;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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

            await LoadSharedPeopleList();
        }


        public async Task LoadSharedPeopleList()
        {

            if (!string.IsNullOrEmpty(_shopList.UserIdRight))
            {
                string[] listUserRights = _shopList.UserIdRight.Split('|');

                foreach (string IdUser in listUserRights)
                {

                    try
                    {
                        UsersWithRight.Add(await Api.UsersClient.IdAsync(Convert.ToInt32(IdUser)));
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    
                }
            }

        }


        #region Commands

        private ICommand _newUserRightCommand;
        public ICommand NewUserRightCommand
        {
            get
            {
                _newUserRightCommand = _newUserRightCommand ?? new MvxCommand(NewUserRight);
                return _newUserRightCommand;
            }
        }


        private ICommand _deleteUserRightCommand;
        public ICommand DeleteUserRightCommand
        {
            get
            {
                _deleteUserRightCommand = _deleteUserRightCommand ?? new MvxCommand<string>(DeleteUserRight);
                return _deleteUserRightCommand;
            }
        }



        /// <summary>
        /// Ajoute un nouvel utilisateur à la shopList
        /// </summary>
        public async void NewUserRight()
        {
            User user = new User();

            try
            {
                // On test si l'utilisateur existe en BDD ou pas.
                user = await Api.UsersClient.UsersGetAsync(NewUserName);
            }
            catch (Exception e)
            {
                // Texte d'erreur
                ConfirmOrErrorEntryColorText = "Red";
                ConfirmOrErrorEntry = "Error, this username do not exist.";
                Console.WriteLine("ERROR = " + e);
                return;
            }

            // On verifie que l'utilisateur n'est pas déjà le droit d'accès à cette liste.
            if (!string.IsNullOrEmpty(user.Right))
            {
                string[] userRights = user.Right.Split('|');

                foreach (string right in userRights)
                {
                    if (_shopList.Id == Convert.ToInt32(right))
                    {
                        ConfirmOrErrorEntryColorText = "Green";
                        ConfirmOrErrorEntry = "User already have access to this list !";
                        return;
                    }
                }
            }

            // On ajoute l'id utilisateur sur la liste pour savoir facilement qui à accès à cette liste.
            if (string.IsNullOrEmpty(_shopList.UserIdRight))
            {
                _shopList.UserIdRight = user.Id.ToString();
            }
            else
            {
                _shopList.UserIdRight += "|" + user.Id.ToString();
            }

            try
            {
                // Ajout des droits d'accès à la liste en bdd pour l'utilisateur ciblé.
                await Api.UsersClient.AddRightAsync(user.Name, _shopList.Id.ToString());

                // mise à jour en BDD de l'ajout de nouveau utilisateur à la liste.
                await Api.ShopListsClient.ShopListsPutAsync(_shopList.Id, _shopList.Name, _shopList.UserIdRight);
            }
            catch (Exception e)
            {
                ConfirmOrErrorEntryColorText = "Red";
                ConfirmOrErrorEntry = "ERROR API";
                Console.WriteLine("ERROR = " + e);
                return;
            }

            UsersWithRight.Add(user);
            NewUserName = "";

            // Texte de confirmation
            ConfirmOrErrorEntryColorText = "Green";
            ConfirmOrErrorEntry = "User access confirmed !";
        }

        /// <summary>
        /// Suppression d'un utilisateur à la shopList
        /// </summary>
        public async void DeleteUserRight(string username)
        {
            User user = new User();

            try
            {
                // on test si l'utilisateur existe en bdd et si oui on le récupère.
                user = await Api.UsersClient.UsersGetAsync(username);

                // On supprime le droit de l'utilisateur.
                await Api.UsersClient.DeleteRightAsync(username, _shopList.Id.ToString());

                // On le retire de la liste d'affichage.
                UsersWithRight.Remove(UsersWithRight.Where(u => u.Name == username).FirstOrDefault());

                // On retire l'id utilisateur de la liste de droit de la shoplist.
                string[] shopListRights = _shopList.UserIdRight.Split('|');
                string newShopListUserIdRight = "";
                foreach (string right in shopListRights)
                {
                    if (right != user.Id.ToString())
                    {
                        newShopListUserIdRight += '|' + right;
                    }
                }
                _shopList.UserIdRight = newShopListUserIdRight.Substring(1);

                await Api.ShopListsClient.ShopListsPutAsync(_shopList.Id, _shopList.Name, _shopList.UserIdRight);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR = " + e);
                return;
            }
        }

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


        /// <summary>
        /// Texte qui confirme l'ajout d'une personne sur la liste ou une erreur si la personne n'existe pas.
        /// </summary>
        private string _confirmOrErrorEntry;
        public string ConfirmOrErrorEntry
        {
            get => _confirmOrErrorEntry;
            set
            {
                SetProperty(ref _confirmOrErrorEntry, value);
            }
        }


        /// <summary>
        /// Couleur rouge si c'est une erreur, couleur verte si c'est une validation
        /// </summary>
        private string _confirmOrErrorEntryColorText;
        public string ConfirmOrErrorEntryColorText
        {
            get => _confirmOrErrorEntryColorText;
            set
            {
                SetProperty(ref _confirmOrErrorEntryColorText, value);
            }
        }
        #endregion
    }
}
