using System.ComponentModel;

namespace TranslatorMobile.Worker;

public class TranslationRecognizerWorker : TranslationRecognizerWorkerBase, INotifyPropertyChanged
{
    private string _recognizedText = string.Empty;
    public string RecognizedText 
    {
        get => _recognizedText;
        set
        {
            if(_recognizedText != value)
            {
                _recognizedText = value;
                OnPropertyChanged(nameof(RecognizedText));
            }
        }
    }

    private string _translatedText = string.Empty;
    public string TranslatedText 
    { 
        get => _translatedText;
        set
        {
            if(_translatedText != value)
            {
                _translatedText = value;
                OnPropertyChanged(nameof(TranslatedText));
            }
        }
    }

    private string _logText = string.Empty;
    public string LogText 
    {
        get => _logText;
        set
        {
            if(_logText != value)
            {
                _logText = value;
                OnPropertyChanged(nameof(LogText));
            }
        } 
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override void OnRecognizing(TranslationRecognitionEventArgs e)
    {
        LogText += "...";
    }

    public override void OnRecognized(TranslationRecognitionEventArgs e)
    {
        LogText += "\n\n";

        var result = e.Result;
        if(result.Reason == ResultReason.TranslatedSpeech)
        {
            LogText += $"TRANSLATED: {result.Text} \n";
            RecognizedText = result.Text;

            foreach (var element in result.Translations)
            {
                LogText += $"TRANSLATED: {element.Key} -> {element.Value}\n";
                TranslatedText = element.Value;
            }
            //TODO: ファイル出力処理を追加する
        }
        else if(result.Reason == ResultReason.RecognizedSpeech)
        {
            LogText += $"RECOGNISED: {result.Text}\n\tSpeech is not translated. \n";
            RecognizedText = result.Text;
        }
        else if (result.Reason == ResultReason.NoMatch)
        {
            LogText += $"NOMACH: Speech could not be recognized.\n";
        }

        LogText += $"\n";
    }

    public override void OnCanceled(TranslationRecognitionCanceledEventArgs e)
    {
        LogText += $"CANCELED: Reason={e.Reason}\n";

        if(e.Reason == CancellationReason.Error)
        {
            LogText += $"CANCELED: ErrorCode={e.ErrorCode}\n";
            LogText += $"CANCELED: ErrorDetails={e.ErrorDetails}\n";
            LogText += $"CANCELED: Did you update the subscription info?\n";
        }

        LogText += $"\n";
    }

    public override void OnSpeechStartDetected(RecognitionEventArgs e)
    {
        LogText += $"Speech start detected: {e.SessionId}\n";
    }

    public override void OnSpeechEndDetected(RecognitionEventArgs e)
    {
        LogText += $"Speech end detected: {e.SessionId}\n";
    }

    public override void OnSessionStarted(SessionEventArgs e)
    {
        LogText += $"Session started event: {e.SessionId}\n";
    }

    public override void OnSessionStopped(SessionEventArgs e)
    {
        LogText += $"Session stopped event: {e.SessionId}\nStop translation.\n";
    }
}
