using Jogger.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.ViewModels
{
   public  class ViewModelBase
    {
        private Strings strings = new Strings();

        public Strings Strings => strings;
    }
}
