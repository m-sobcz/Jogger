using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Jogger.Behaviors
{
    public class TextBoxAutoScroll : Behavior<TextBox>
    {
        private TextBox textBox;
        protected override void OnAttached()
        {
            base.OnAttached();
            this.textBox = base.AssociatedObject;
            this.textBox.TextChanged += TextBox_TextChanged;
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.textBox.ScrollToEnd();          
        }
    }

}
