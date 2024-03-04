using TranslatorMobile.Worker;

namespace TranslatorMobile.UI.Views;

public partial class TranslationPage : ContentPage
{
    public bool IsTerminalOpen { get; set; } = false;

    public bool IsRecording { get; set; } = false;

    private CancellationTokenSource _cts;

    private readonly TranslationRecognizerWorker _worker;

    private KeyValuePair<string, string>[] _translations =
    {
        new KeyValuePair<string, string>("en-US", "English"),
        new KeyValuePair<string, string>("ja-JP", "Japanese"),
    };

    public TranslationPage(TranslationRecognizerWorker worker)
	{
		InitializeComponent();
        _worker = worker;
        BindingContext = _worker;

        SourceLangPicker.SelectedIndex = 0;
        TargetLangPicker.SelectedIndex = 1;
	}

    private async Task<PermissionStatus> CheckAndRequestMicrophonePermission()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Microphone>();

        if(status == PermissionStatus.Granted)
        {
            return status;
        }

        status = await Permissions.RequestAsync<Permissions.Microphone>();

        return status;
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        await CheckAndRequestMicrophonePermission();
    }

    private async void RecordStartButton_Clicked(object sender, EventArgs e)
    {
        var sourceLanguage = _translations[SourceLangPicker.SelectedIndex].Key;
        var targetLanguage = _translations[TargetLangPicker.SelectedIndex].Key;

        var subscriptionKey = await SecureStorage.Default.GetAsync("SubscriptionKey");
        var region = await SecureStorage.Default.GetAsync("Region");
        var endpointUrl = new Uri($"wss://{region}.stt.speech.microsoft.com/speech/universal/v2");

        if (!IsRecording)
        {
            IsRecording = true;
            RecordStartButton.Source = "pause.png";

            _cts = new CancellationTokenSource();

            var translator = new Translator(endpointUrl, subscriptionKey, sourceLanguage, targetLanguage);

            try
            {
                await translator.MultiLingualTranslation(_worker, _cts.Token);
            }
            catch (Exception ex)
            {
                _worker.LogText += $"Error: {ex.Message}\n";
            }
            finally
            {
                _cts.Cancel();

                _worker.LogText += "Translation has been stopped.\n";

                IsRecording = false;
                RecordStartButton.Source = "microphone.png";
            }
        }
        else
        {
            _cts.Cancel();

            IsRecording = false;
            RecordStartButton.Source = "microphone.png";
        }
    }

    private void OpenTerminalButton_Clicked(object sender, EventArgs e)
    {
        if (IsTerminalOpen)
        {
            TerminalRow.Height = 0;
            IsTerminalOpen = false;
            OpenTerminalButton.Text = "Open Terminal";
        }
        else
        {
            TerminalRow.Height = 300;
            IsTerminalOpen = true;
            OpenTerminalButton.Text = "Close Terminal";
        }
    }

    private void SourceLangPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if(SourceLangPicker.SelectedIndex == TargetLangPicker.SelectedIndex)
        {
            TargetLangPicker.SelectedIndex = (TargetLangPicker.SelectedIndex + 1) % _translations.Length;
        } 
    }

    private void TargetLangPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if(TargetLangPicker.SelectedIndex == SourceLangPicker.SelectedIndex)
        {
            SourceLangPicker.SelectedIndex = (SourceLangPicker.SelectedIndex + 1) % _translations.Length;
        }
    }

    private void ClearTerminalButton_Clicked(object sender, EventArgs e)
    {
        _worker.LogText = string.Empty;
    }
}