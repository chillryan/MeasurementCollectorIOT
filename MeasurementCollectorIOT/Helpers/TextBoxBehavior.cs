using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MeasurementCollectorIOT.Helpers
{
	public static class TextBoxBehavior
	{
		public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(TextBoxBehavior), new PropertyMetadata(null));

		public static readonly DependencyProperty CommandKeyProperty = DependencyProperty.RegisterAttached("CommandKey", typeof(string), typeof(TextBoxBehavior), new PropertyMetadata(default(string), OnCommandKeyChanged));

		public static ICommand GetCommand(DependencyObject obj) => (ICommand)obj.GetValue(CommandProperty);

		public static string GetCommandKey(DependencyObject obj) => (string)obj.GetValue(CommandKeyProperty);

		public static void SetCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(CommandProperty, value);
		}

		public static void SetCommandKey(DependencyObject obj, string value)
		{
			obj.SetValue(CommandKeyProperty, value);
		}

		private static void OnCommandKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TextBox)
			{
				var textBox = (TextBox)d;

				if (e.NewValue != null && e.NewValue.ToString().Length > 0)
				{
					textBox.KeyDown += TextBox_KeyDown;
				}
				else
				{
					textBox.KeyDown -= TextBox_KeyDown;
				}
			}
		}

		private static void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
		{
			var textBox = (TextBox)sender;

			if (e.Key.ToString() == textBox.GetValue(CommandKeyProperty).ToString())
			{
				e.Handled = true;

				ICommand command = GetCommand(textBox);

				command.Execute(textBox.Text);
			}
		}
	}
}
