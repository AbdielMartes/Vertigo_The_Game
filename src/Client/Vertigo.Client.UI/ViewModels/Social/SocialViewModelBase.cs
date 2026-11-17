using CommunityToolkit.Mvvm.ComponentModel;
using Vertigo.Client.UI.ViewModels.Utilities;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;

namespace Vertigo.Client.UI.ViewModels.Social;

public abstract class SocialViewModelBase : ObservableObject
{
    protected readonly SocialOperation Operation;
    protected readonly ITextLocalizer Text;
    private string _errorMessage = string.Empty;
    private bool _isBusy;

    protected SocialViewModelBase(SocialOperation operation, ITextLocalizer text)
    {
        Operation = operation;
        Text = text;
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        protected set => SetProperty(ref _errorMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        protected set => SetProperty(ref _isBusy, value);
    }

    public void ClearError() => ErrorMessage = string.Empty;
}
