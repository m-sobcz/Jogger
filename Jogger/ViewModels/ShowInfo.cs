using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.ViewModels
{
    public class ShowInfo : IShowInfo
    {
        public event ShowInfoEventHandler ShowInformation;
        public delegate void ShowInfoEventHandler(object sender, string text, string caption);
        public void Show(string text, string caption = "---")
        {
            ShowInformation?.Invoke(this, text, caption);
        }
    }
}
