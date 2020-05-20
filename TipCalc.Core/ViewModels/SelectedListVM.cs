using DynamicData;
using DynamicData.Binding;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TipCalc.Core.Model;
using TipCalc.Core.Services;

namespace TipCalc.Core.ViewModels
{
    public class SelectedListVM : MvxViewModel<Tuple<User, ShopList>>
    {
        private readonly IMvxNavigationService _navigationService;

        // ShopList actuellement ouverte.
        private ShopList _shopList;

        // Utilisateur actuellement connecté.
        private User _user;

        // Permet de faire les appels API.
        public API Api = new API();

        public SelectedListVM(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        // On récupère notre ShopList transmis par la page précédente.
        public override void Prepare(Tuple<User, ShopList> parameter)
        {
            _user = parameter.Item1;
            _shopList = parameter.Item2;
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            PageTitle = _shopList.Name;

            // Possibilité de tri de liste
            ListSort = new List<string>
            {
                "A - Z",
                "Personal sort",
                "Tag : A - Z",
                "Tag : Personal sort"
            };

            SourceListShopItem = new SourceList<ShopItem>();
            await LoadItems();

            // Premier tri de la liste par ordre alaphabetique
            SelectedSort = "0";
            SortChanged();
        }

        /// <summary>
        /// Charge et affiche les items de la shoplist. Est appellé lors de la création d'un item en BDD mais pas lors de la destruction.
        /// </summary>
        /// <returns></returns>
        public async Task LoadItems()
        {
            SourceListShopItem.Clear();

            try
            {
                SourceListShopItem.AddRange(await Api.ShopItemsClient.GetShopItemsAsync(_shopList.Id));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Si le type de tri est modifié, cette méthode est appellé pour editer la liste.
        /// </summary>
        /// <param name="sortType"></param>
        /// <returns></returns>
        public void SortChanged()
        {
            int intSortType = Convert.ToInt32(SelectedSort);

            switch (intSortType)
            {
                // Tri par ordre alphabetique.
                case 0:
                    var Comparer1 = SortExpressionComparer<ShopItem>.Descending(Si => Si.Checked).ThenByAscending(Si => Si.Name);
                    var propertyChanged = SourceListShopItem.Connect().WhenPropertyChanged(x => x.Checked).Select(_ => System.Reactive.Unit.Default);

                    ListShopItemSort = SourceListShopItem.Connect().Sort(Comparer1, resort: propertyChanged).AsObservableList();
                    ListShopItemSort.Connect().Bind(out _displayedListShopItemSort).Do((x) => { this.RaisePropertyChanged(nameof(DisplayedListShopItemSort)); }).Subscribe();
                    break;

                // Tri personnel enregistré en mémoire tel.
                case 1:
                    break;

                // Tri par catégorie et par ordre alphabetique.
                case 2:

                    //var Comparer1 = SortExpressionComparer<ShopItem>.Descending(Si => Si.Checked).ThenByAscending(Si => Si.Name);
                    //var propertyChanged = SourceListShopItem.Connect().WhenPropertyChanged(x => x.Checked).Select(_ => System.Reactive.Unit.Default);

                    //ListShopItemSort = SourceListShopItem.Connect().Sort(Comparer1, resort: propertyChanged).AsObservableList();
                    //ListShopItemSort.Connect().Bind(out _displayedListShopItemSort).Do((x) => { this.RaisePropertyChanged(nameof(DisplayedListShopItemSort)); }).Subscribe();


                    break;

                // Tri  par catégorie et personnel enregistré en mémoire tel.
                case 3:
                    break;

            }
        }


        #region Commands
        private ICommand _newShopItemCommand;
        public ICommand NewShopItemCommand
        {
            get
            {
                _newShopItemCommand = _newShopItemCommand ?? new MvxCommand(NewShopItem);
                return _newShopItemCommand;
            }
        }


        private ICommand _deleteItemListCommand;
        public ICommand DeleteItemListCommand
        {
            get
            {
                _deleteItemListCommand = _deleteItemListCommand ?? new MvxCommand<int>(DeleteItemList);
                return _deleteItemListCommand;
            }
        }


        private ICommand _navToItemEditCommand;
        public ICommand NavToItemEditCommand
        {
            get
            {
                _navToItemEditCommand = _navToItemEditCommand ?? new MvxCommand<int>(NavToItemEdit);
                return _navToItemEditCommand;
            }
        }


        /// <summary>
        /// Ajoute un nouvel item à la BDD
        /// </summary>
        public async void NewShopItem()
        {
            try
            {
                // création de la nouvelle liste et reset le champ NewListName et met à jour la liste avec la fonction LoadLists
                ShopItem shopItem = await Api.ShopItemsClient.PostShopItemAsync(_shopList.Id, NewShopItemName, 00, "");
                NewShopItemName = "";
                SourceListShopItem.Add(shopItem);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Supprime un item de la BDD
        /// </summary>
        /// <param name="id"></param>
        public async void DeleteItemList(int id)
        {
            try
            {
                // Suppression de l'item via id
                SourceListShopItem.Remove(SourceListShopItem.Items.Where(i => i.Id == id).FirstOrDefault());
                await Api.ShopItemsClient.DeleteShopItemAsync(id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Navigue sur une nouvelle page pour pouvoir editer tout les paramètres d'un ShopItem
        /// </summary>
        /// <param name="id"></param>
        public async void NavToItemEdit(int id)
        {
            try
            {
                await _navigationService.Navigate<EditItemVM, ShopItem>(SourceListShopItem.Items.Where(si => si.Id == id).FirstOrDefault());
            }
            catch (Exception e)
            {
                throw e;
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
        /// List des différents types de tri qui existe.
        /// </summary>
        private List<string> _listSort;
        public List<string> ListSort
        {
            get => _listSort;
            set
            {
                SetProperty(ref _listSort, value);
            }
        }

        /// <summary>
        /// Systeme de tri actuellement selectionné
        /// </summary>
        private string _selectedSort;
        public string SelectedSort
        {
            get => _selectedSort;
            set
            {
                SetProperty(ref _selectedSort, value);
            }
        }

        /// <summary>
        /// Data List de tout les shopItems
        /// </summary>
        private SourceList<ShopItem> _sourceListShopItem;
        public SourceList<ShopItem> SourceListShopItem
        {
            get => _sourceListShopItem;
            set
            {
                SetProperty(ref _sourceListShopItem, value);
            }
        }

        /// <summary>
        /// Est utilisé pour le tri et la détection des checkboxs (il n'y a pas de récupération d'id possible car pas de command avec les checkboxs)
        /// </summary>
        private IObservableList<ShopItem> _listShopItemSort;
        public IObservableList<ShopItem> ListShopItemSort
        {
            get => _listShopItemSort;
            set
            {
                SetProperty(ref _listShopItemSort, value);
            }
        }

        /// <summary>
        /// Affichage des ShopItems. Liste bind à la collectionView.
        /// </summary>
        private ReadOnlyObservableCollection<ShopItem> _displayedListShopItemSort;
        public ReadOnlyObservableCollection<ShopItem> DisplayedListShopItemSort => _displayedListShopItemSort;

        /// <summary>
        /// Entry pour récupérer le nom du nouvel item qui va être crée.
        /// </summary>
        private string _newShopItemName;
        public string NewShopItemName
        {
            get => _newShopItemName;
            set
            {
                SetProperty(ref _newShopItemName, value);
            }
        }

        #endregion
    }
}