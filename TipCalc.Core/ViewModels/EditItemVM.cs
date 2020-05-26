using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TipCalc.Core.Model;
using TipCalc.Core.Services;
using Tuto_System.Reactive;

namespace TipCalc.Core.ViewModels
{
    public class EditItemVM : MvxViewModel<ShopItem>
    {
        // Navigation
        private readonly IMvxNavigationService _navigationService;

        // Permet de faire les appels API.
        public API Api = new API();

        public EditItemVM(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }


        // On récupère notre ShopItem transmis par la page précédente.
        public override void Prepare(ShopItem parameter)
        {
            OldShopItem = parameter;
            NewShopItem = new ShopItem(parameter);
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            // Texte affiché en haut à gauche de la nav bar
            PageTitle = "My lists";

            InitializeAllTagsList();

            // Si il y a un changement sur les tags de l'objet, l'affichage est mise à jour.
            NewShopItem.GetPropertyValues(csi => csi.Tags).Subscribe(InitializeTagList);
        }

        /// <summary>
        /// Met à jour l'affichage de la CollectionView si il y a un changement sur les tags
        /// </summary>
        /// <param name="subscribe"></param>
        void InitializeTagList(string subscribe)
        {
            if (NewShopItem.Tags != null && NewShopItem.Tags.Length > 0)
            {
                DisplayTags = new ObservableCollection<string>(NewShopItem.Tags.Split('|').ToList());
            }
        }


        void InitializeAllTagsList()
        {
            // TODO : Appel API pour obtenir tout les tags d'une liste
            List<string> TempListAllTags = new List<string>{"test1", "test2", "test3", "test4", "test5", "test6", "test7", "test8", "test9"};
            PickerSourceListAllTags = new ObservableCollection<string>(TempListAllTags);
        }


        #region Commands
        private ICommand _newTagCommand;
        public ICommand NewTagCommand
        {
            get
            {
                _newTagCommand = _newTagCommand ?? new MvxCommand(NewTag);
                return _newTagCommand;
            }
        }


        private ICommand _confirmEditShopItemCommand;
        public ICommand ConfirmEditShopItemCommand
        {
            get
            {
                _confirmEditShopItemCommand = _confirmEditShopItemCommand ?? new MvxCommand(ConfirmEditShopItem);
                return _confirmEditShopItemCommand;
            }
        }


        private ICommand _deleteTagCommand;
        public ICommand DeleteTagCommand
        {
            get
            {
                _deleteTagCommand = _deleteTagCommand ?? new MvxCommand<string>(DeleteTag);
                return _deleteTagCommand;
            }
        }


        /// <summary>
        /// Ajoute un nouveau tag au ShopItem actuellement selectionné
        /// </summary>
        public void NewTag()
        {
            if (NewShopItem.Tags != null && NewShopItem.Tags.Split('|').ToList().Contains(NewTagTexte))
            {
                return;
            }

            if (NewShopItem.Tags != null && NewShopItem.Tags.Length > 0)
                NewShopItem.Tags += "|" + NewTagTexte;
            else
                NewShopItem.Tags = NewTagTexte;
            NewTagTexte = "";
        }

        /// <summary>
        /// Permet de sauvgarder en BDD les modifications apporté au nom et au prix du ShopItem
        /// </summary>
        public async void ConfirmEditShopItem()
        {
            try
            {
                OldShopItem.Tags = NewShopItem.Tags;
                await Api.ShopItemsClient.ShopItemsPutAsync(NewShopItem.Id, NewShopItem);
                await _navigationService.Close(this);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Permet de supprimer des tags sur ce ShopItem
        /// </summary>
        /// <param name="TagToDelete"> String qui correspond au tag à supprimer. </param>
        public void DeleteTag(string TagToDelete)
        {
            List<string> Tags = NewShopItem.Tags.Split('|').ToList();

            if (Tags.Contains(TagToDelete))
                Tags.Remove(TagToDelete);

            NewShopItem.Tags = Tags[0];

            for (int i = 1; i < Tags.Count; i++)
            {
                NewShopItem.Tags += "|" + Tags[i];
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


        private ShopItem _oldShopItem;
        public ShopItem OldShopItem
        {
            get => _oldShopItem;
            set
            {
                SetProperty(ref _oldShopItem, value);
            }
        }

        /// <summary>
        /// Item en cours de modification, est supprimer si il n'y a pas de validation.
        /// </summary>
        private ShopItem _newShopItem;
        public ShopItem NewShopItem
        {
            get => _newShopItem;
            set
            {
                SetProperty(ref _newShopItem, value);
            }
        }

        /// <summary>
        /// Liste d'affichage des tags bind à la collectionView
        /// </summary>
        private ObservableCollection<string> _displayTags;
        public ObservableCollection<string> DisplayTags
        {
            get => _displayTags;
            set
            {
                SetProperty(ref _displayTags, value);
            }
        }

        /// <summary>
        /// Entry qui contient le nom du nouveau tag
        /// </summary>
        private string _newTagTexte;
        public string NewTagTexte
        {
            get => _newTagTexte;
            set
            {
                SetProperty(ref _newTagTexte, value);
            }
        }

        /// <summary>
        /// Liste de tout les tags de la ShopList
        /// </summary>
        private ObservableCollection<string> _pickerSourceListAllTags;
        public ObservableCollection<string> PickerSourceListAllTags
        {
            get => _pickerSourceListAllTags;
            set
            {
                SetProperty(ref _pickerSourceListAllTags, value);
            }
        }

        /// <summary>
        /// Index du tag actuellement selectionné dans le picker.
        /// </summary>
        private string _pickerSelectedItem;
        public string PickerSelectedItem
        {
            get => _pickerSelectedItem;
            set
            {
                SetProperty(ref _pickerSelectedItem, value);
            }
        }
        #endregion
    }
}
