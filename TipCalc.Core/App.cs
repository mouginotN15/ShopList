using MvvmCross;
using MvvmCross.ViewModels;
using TipCalc.Core.ViewModels;

namespace TipCalc.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            RegisterAppStart<LoginVM>();
        }
    }
}