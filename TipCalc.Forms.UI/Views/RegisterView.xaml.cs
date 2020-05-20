using MvvmCross.Forms.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipCalc.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TipCalc.Forms.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterView : MvxContentPage<RegisterVM>
    {
        public RegisterView()
        {
            InitializeComponent();
        }
    }
}