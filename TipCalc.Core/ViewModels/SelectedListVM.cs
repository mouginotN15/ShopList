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
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TipCalc.Core.Model;
using TipCalc.Core.Services;
using System.Reactive;

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

        //Comparers
        SortExpressionComparer<ShopItem> ComparerAtoZ;
        SortExpressionComparer<ShopItem> ComparerZtoA;
        SortExpressionComparer<ShopItem> ComparerAtoZwithTags;
        SortExpressionComparer<ShopItem> ComparerZtoAwithTags;
        BehaviorSubject<SortExpressionComparer<ShopItem>> observableComparer;


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
                "Z - A",
                "Tag : A - Z",
                "Tag : Z - A"
            };

            // Comparer initialize
            ComparerAtoZ = SortExpressionComparer<ShopItem>.Descending(Si => Si.Checked).ThenByAscending(Si => Si.Name);
            ComparerZtoA = SortExpressionComparer<ShopItem>.Descending(Si => Si.Checked).ThenByDescending(Si => Si.Name);
            // ComparerAtoZwithTags
            // ComparerZtoAwithTags

            observableComparer = new BehaviorSubject<SortExpressionComparer<ShopItem>>(ComparerAtoZ);

            // Charge les shopItems de la shoplist dans SourceListShopItem et dans OldSourceListShopItem
            SourceListShopItem = new SourceList<ShopItem>();
            OldSourceListShopItem = new List<ShopItem>();
            await LoadItems();

            // Système de NotifiePropertyChange
            var propertyChanged = SourceListShopItem.Connect().WhenPropertyChanged(x => x.Checked).Select(_ => Unit.Default);
            ListShopItemSort = SourceListShopItem.Connect().Sort(ComparerAtoZ, resort: propertyChanged, comparerChanged: observableComparer).AsObservableList();
            ListShopItemSort.Connect().Bind(out _displayedListShopItemSort).Do((x) => { this.RaisePropertyChanged(nameof(DisplayedListShopItemSort)); }).Subscribe();

            // Si il y a un changement du paramètre "checked" sur une ligne, modifie la valeur en BDD
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            SourceListShopItem.Connect().WhenPropertyChanged(x => x.Checked).Do(x => CheckOrUncheckItem(x.Sender)).Subscribe();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            // Premier tri de la liste par ordre alaphabetique
            SelectedSort = "0";
            this.WhenPropertyChanged(x => x.SelectedSort).Subscribe((x) => SortChanged());
        }


        /// <summary>
        /// Fonction qui enregistre en BDD les modifications apporté à la selection des items de la liste. (checkbox)
        /// </summary>
        /// <param name="shopItem"></param>
        /// <returns></returns>
        public async Task CheckOrUncheckItem(ShopItem shopItem)
        {
            var compared = OldSourceListShopItem.Where(x => x.Id == shopItem.Id).FirstOrDefault();

            if (shopItem.Checked == compared.Checked)
                return;

            try
            {
                await Api.ShopItemsClient.ShopItemsPutAsync(shopItem.Id, shopItem);
                compared.Checked = shopItem.Checked;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception = " + e);
                throw;
            }
        }

        /// <summary>
        /// Charge et affiche les items de la shoplist. Est appellé lors de la création d'un item en BDD mais pas lors de la destruction.
        /// </summary>
        /// <returns></returns>
        public async Task LoadItems()
        {
            SourceListShopItem.Clear();
            OldSourceListShopItem.Clear();
            Console.WriteLine(" LOAD ---------------------------------------------------------------------- ");

            try
            {
                List<ShopItem> Temp = new List<ShopItem>(await Api.ShopItemsClient.ListAsync(_shopList.Id));

                OldSourceListShopItem.AddRange(Temp.Select(x => new ShopItem(x)));
                SourceListShopItem.AddRange(Temp);
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
            Console.WriteLine(SelectedSort);

            switch (intSortType)
            {
                // Tri par ordre alphabetique.
                case 0:
                    observableComparer.OnNext(ComparerAtoZ);
                    break;

                // Tri par ordre alphabetique inversé.
                case 1:
                    observableComparer.OnNext(ComparerZtoA);
                    break;

                // Tri par catégorie et par ordre alphabetique.
                case 2:
                    observableComparer.OnNext(ComparerAtoZwithTags);
                    break;

                // Tri par catégorie et par ordre alphabetique inversé.
                case 3:
                    observableComparer.OnNext(ComparerZtoAwithTags);
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


        private ICommand _usersRightGestionCommand;
        public ICommand UsersRightGestionCommand
        {
            get
            {
                _usersRightGestionCommand = _usersRightGestionCommand ?? new MvxCommand(UsersRightGestion);
                return _usersRightGestionCommand;
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
                ShopItem shopItem = await Api.ShopItemsClient.ShopItemsPostAsync(_shopList.Id, NewShopItemName, 00, "");
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
                await Api.ShopItemsClient.ShopItemsDeleteAsync(id);
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

        /// <summary>
        /// Navigue sur une nouvelle page pour pouvoir gérer les utilisateurs qui ont des droits d'accès à cette liste.
        /// </summary>
        /// <param name="id"></param>
        public async void UsersRightGestion()
        {
            try
            {
                await _navigationService.Navigate<ListRightGestionVM, ShopList>(_shopList);
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
        /// Old Data List de tout les shopItems
        /// </summary>
        private List<ShopItem> _oldSourceListShopItem;
        public List<ShopItem> OldSourceListShopItem
        {
            get => _oldSourceListShopItem;
            set
            {
                SetProperty(ref _oldSourceListShopItem, value);
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