using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace TranslatorMobile.UI.ViewModels;

public class SettingsViewModel: INotifyPropertyChanged
{
    private string _subscriptionKey = SecureStorage.Default.GetAsync("SubscriptionKey").Result ?? string.Empty;
    public string SubscriptionKey
    {
        get { return _subscriptionKey; }
        set
        {
            _subscriptionKey = value;
            OnPropertyChanged(nameof(SubscriptionKey));
        }
    }

    private ObservableCollection<string> _regions = new ObservableCollection<string>
    {
        "eastus",
        "westus",
        "centralus",
        "eastasia",
        "southeastasia",
        "northeurope",
        "westeurope",
        "australiaeast",
        "brazilsouth",
        "southcentralus",
        "japaneast",
        "koreacentral",
        "francecentral",
        "uksouth",
        "canadacentral",
        "centralindia",
        "southafricanorth",
        "uaecentral",
        "switzerlandnorth",
        "germanywestcentral",
        "norwayeast",
        "australiacentral",
        "australiasoutheast",
        "japanwest",
        "koreasouth",
        "francesouth",
        "ukwest",
        "canadaeast",
        "southindia",
        "southafricawest",
        "uaenorth",
        "switzerlandwest",
        "germanycentral",
        "norwaywest",
        "brazilsoutheast"
    };  
    public ObservableCollection<string> Regions
    {
        get { return _regions; }
        set
        {
            _regions = value;
            OnPropertyChanged(nameof(Regions));
        }
    }

    private string _selectedRegion = SecureStorage.Default.GetAsync("Region").Result ?? string.Empty;
    public string SelectedRegion
    {
        get { return _selectedRegion; }
        set
        {
            _selectedRegion = value;
            OnPropertyChanged(nameof(SelectedRegion));
        }
    }

    private bool _isBusy = false;
    public bool IsBusy
    {
        get { return _isBusy; }
        set
        {
            _isBusy = value;
            OnPropertyChanged(nameof(IsBusy));
        }
    }

    public ICommand SaveCommand => new Command(async () =>
    {
        IsBusy = true;

        await Task.Run(() =>
        {
            SecureStorage.Default.SetAsync("SubscriptionKey", SubscriptionKey);
            SecureStorage.Default.SetAsync("Region", SelectedRegion);
        });

        IsBusy = false;
    });

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

