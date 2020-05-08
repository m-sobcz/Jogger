using Jogger.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.ViewModels
{
   public  class ViewModelBase : ObservedObject
    {
        private Strings strings = new Strings();
        public ShowInfo showInfo = new ShowInfo();
        public Strings Strings => strings;
    }
}
