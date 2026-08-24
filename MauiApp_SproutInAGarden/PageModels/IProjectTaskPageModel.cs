using CommunityToolkit.Mvvm.Input;
using MauiApp_SproutInAGarden.Models;

namespace MauiApp_SproutInAGarden.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}