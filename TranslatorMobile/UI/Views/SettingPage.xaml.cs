using TranslatorMobile.UI.ViewModels;

namespace TranslatorMobile.UI.Views;

public partial class SettingPage : ContentPage
{
	public string SubscriptionKey { get; set; }
	public string Region { get; set; }


	public SettingPage()
	{
		InitializeComponent();
		BindingContext = new SettingsViewModel();
	}
}