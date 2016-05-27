using MeasurementCollectorIOT.ViewModels;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MeasurementCollectorIOT.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();
		}

		private MainPageViewModel ViewModel => (MainPageViewModel)this.DataContext;

		private void txtDataEntry_KeyDown(object sender, KeyRoutedEventArgs e)
		{
			if (e.Key == VirtualKey.Enter)
			{
				ViewModel.AcceptMeasurementEntry.Execute(txtDataEntry.Text);
				txtDataEntry.Text = string.Empty;
			}
		}
	}
}
