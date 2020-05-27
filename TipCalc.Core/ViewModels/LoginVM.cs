using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TipCalc.Core.Model;
using TipCalc.Core.Services;

namespace TipCalc.Core.ViewModels
{
    public class LoginVM : MvxViewModel
    {
        // Navigation
        private readonly IMvxNavigationService _navigationService;

        public LoginVM(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        // Permet de faire les appels API.
        public API Api = new API();

        public override async Task Initialize()
        {
            await base.Initialize();

            // Texte affiché en haut à gauche de la nav bar
            PageTitle = "Login";
        }


        #region Commands
        private ICommand _loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                _loginCommand = _loginCommand ?? new MvxCommand(Login);
                return _loginCommand;
            }
        }

        // Redirection sur la page RegisterVM pour pouvoir créer un compte.
        private ICommand _navigationRegisterCommand;
        public ICommand NavigationRegisterCommand
        {
            get
            {
                _navigationRegisterCommand = _navigationRegisterCommand ?? new MvxCommand(() => _navigationService.Navigate<RegisterVM>());
                return _navigationRegisterCommand;
            }
        }


        // Clic du bouton Login
        public async void Login()
        {
            User user = new User();

            try
            {
                Username =  Username.ToLower();

                // On test si l'utilisateur existe sur la bdd et si oui la validation du mdp
                user = await Api.UsersClient.LoginAsync(Username, Password);

                Console.WriteLine(user.Id + " " + user.Name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorMessage = "User not found !";
                return;
            }

            // En cas de validation du login, on redirige sur la page ListVM
            await _navigationService.Navigate<ListVM, User>(user);
            // await _navigationService.Close(this);
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


        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                RaisePropertyChanged(() => Username);
            }
        }


        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                RaisePropertyChanged(() => Password);
            }
        }


        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                RaisePropertyChanged(() => ErrorMessage);
            }
        }
        #endregion
    }
}
