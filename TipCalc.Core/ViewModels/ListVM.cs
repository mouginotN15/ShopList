using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using TipCalc.Core.Model;
using TipCalc.Core.Services;

namespace TipCalc.Core.ViewModels
{
    public class ListVM : MvxViewModel<User>
    {
        // Navigation
        private readonly IMvxNavigationService _navigationService;

        // Permet de faire les appels API.
        public API Api = new API();

        // Utilisateur actuellement connecté.
        private User _user;


        public ListVM(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }


        // On récupère notre utilisateur transmis par la page précédente.
        public override void Prepare(User parameter)
        {
            _user = parameter;
        }


        public override async Task Initialize()
        {
            await base.Initialize();

            // Texte affiché en haut à gauche de la nav bar
            PageTitle = "My lists";

            await LoadLists().ConfigureAwait(false);
        }


        // Charge et affiche les listes de l'utilisateur
        public async Task LoadLists()
        {
            ListShopList.Clear();

            try
            {
                ObservableCollection<ShopList> convert = new ObservableCollection<ShopList>((await Api.ShopListsClient.UserAsync(_user.Id)).ToList());
                ListShopList = convert;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Timer pour faire redevenir les boutons normaux.
        public void DelayAction()
        {
            Timer timer = new Timer{Interval = 2000, Enabled = true};

            timer.Elapsed += delegate
            {
                Console.WriteLine("DELEGATE TIMER");
                ButtonDeleteVisibility = true;
                ButtonConfirmDeleteVisibility = false;
                timer.Stop();
            };
        }

        #region Commands
        private ICommand _newListCommand;
        public ICommand NewListCommand
        {
            get
            {
                _newListCommand = _newListCommand ?? new MvxCommand(NewList);
                return _newListCommand;
            }
        }


        private ICommand _deleteShopListCommand;
        public ICommand DeleteShopListCommand
        {
            get
            {
                _deleteShopListCommand = _deleteShopListCommand ?? new MvxCommand<int>(DeleteShopList);
                return _deleteShopListCommand;
            }
        }


        private ICommand _clickedShopListCommand;
        public ICommand ClickedShopListCommand
        {
            get
            {
                _clickedShopListCommand = _clickedShopListCommand ?? new MvxCommand(ClickedShopList);
                return _clickedShopListCommand;
            }
        }


        private ICommand _deleteConfirmVisibilityCommand;
        public ICommand DeleteConfirmVisibilityCommand
        {
            get
            {
                _deleteConfirmVisibilityCommand = _deleteConfirmVisibilityCommand ?? new MvxCommand(DeleteConfirmVisibility);
                return _deleteConfirmVisibilityCommand;
            }
        }

        
        public async void NewList()
        {
            try
            {
                // création de la nouvelle liste et reset le champ NewListName et met à jour la liste avec la fonction LoadLists
                await Api.ShopListsClient.ShopListsPostAsync(_user.Id, NewListName);
                NewListName = "";
                await LoadLists();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async void DeleteShopList(int id)
        {
            try
            {
                ListShopList.Remove(ListShopList.Where(i => i.Id == id).First());
                await Api.ShopListsClient.ShopListsDeleteAsync(id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async void ClickedShopList()
        {
            if (SelectedShopList == null)
            {
                return;
            }
            
            try
            {
                Tuple<User, ShopList> ProfilAndShopList = new Tuple<User, ShopList>(_user, SelectedShopList);
                await _navigationService.Navigate<SelectedListVM, Tuple<User, ShopList>>(ProfilAndShopList);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteConfirmVisibility()
        {
            ButtonDeleteVisibility = false;
            ButtonConfirmDeleteVisibility = true;

            DelayAction();
        }

        #endregion


        #region Properties
        private string _pageTitle;
        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                _pageTitle = value;
                RaisePropertyChanged(() => PageTitle);
            }
        }


        private string _newListName;
        public string NewListName
        {
            get => _newListName;
            set
            {
                _newListName = value;
                RaisePropertyChanged(() => NewListName);
            }
        }


        private ShopList _selectedShopList;
        public ShopList SelectedShopList
        {
            get => _selectedShopList;
            set
            {
                _selectedShopList = value;
                RaisePropertyChanged(() => SelectedShopList);
            }
        }


        private ObservableCollection<ShopList> _listShopList = new ObservableCollection<ShopList>();
        public ObservableCollection<ShopList> ListShopList
        {
            get => _listShopList;
            set
            {
                _listShopList = value;
                RaisePropertyChanged(() => ListShopList);
            }
        }


        private bool _buttonDeleteVisibility = true;
        public bool ButtonDeleteVisibility
        {
            get => _buttonDeleteVisibility;
            set
            {
                _buttonDeleteVisibility = value;
                RaisePropertyChanged(() => ButtonDeleteVisibility);
            }
        }


        private bool _buttonConfirmDeleteVisibility = false;
        public bool ButtonConfirmDeleteVisibility
        {
            get => _buttonConfirmDeleteVisibility;
            set
            {
                _buttonConfirmDeleteVisibility = value;
                RaisePropertyChanged(() => ButtonConfirmDeleteVisibility);
            }
        }

        #endregion
    }
}