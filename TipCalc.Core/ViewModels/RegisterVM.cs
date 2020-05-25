using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TipCalc.Core.Model;
using TipCalc.Core.Services;

namespace TipCalc.Core.ViewModels
{
    public class RegisterVM : MvxViewModel
    {
        // Navigation
        private readonly IMvxNavigationService _navigationService;

        // Permet de faire les appels API.
        public API Api = new API();

        public RegisterVM(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            // Texte affiché en haut à gauche de la nav bar
            PageTitle = "Register";
        }


        #region Commands
        private ICommand _validateRegisterCommand;
        public ICommand ValidateRegisterCommand
        {
            get
            {
                _validateRegisterCommand = _validateRegisterCommand ?? new MvxCommand(ValidateRegister);
                return _validateRegisterCommand;
            }
        }

        // Redirection sur la page LoginVM en cas d'annulation.
        private ICommand _navigationLoginCommand;
        public ICommand NavigationLoginCommand
        {
            get
            {
                _navigationLoginCommand = _navigationLoginCommand ?? new MvxCommand(() => _navigationService.Close(this));
                return _navigationLoginCommand;
            }
        }

        // Si l'utilisateur valide la création de son compte
        public async void ValidateRegister()
        {
            // Vérification si les champs sont vides
            if (Username == null || Password == null || ConfirmPassword == null)
            {
                ErrorMessage = "Some fields seem to be empty ^^";
                return;
            }

            // On test si la vérification est identique au mdp
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Password and confirm password are not identical.";
                return;
            }

            // Tout les Username son en minuscule et pas sensible à la casse lors du login.
            Username = Username.ToLower();

            try
            {
                // On test si l'username est déjà pris. Si non, on créer le compte.
                User user = await Api.UsersClient.UsersPostAsync(Username, Password);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                if (e.ToString().Contains("Status: 400"))
                {
                    ErrorMessage = "Username already taken";
                    return;
                }

                if (e.ToString().Contains("Status: 201"))
                {
                    ErrorMessage = "Account created !";
                    await _navigationService.Close(this);
                }
            }
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


        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                RaisePropertyChanged(() => ConfirmPassword);
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
