using MvvmCross.Forms.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipCalc.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TipCalc.Forms.UI.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListView : MvxContentPage<ListVM>
    {
        public ListView()
        {
            InitializeComponent();
        }
    }
}