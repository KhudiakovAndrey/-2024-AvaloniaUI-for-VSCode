using System.Reactive;
using ReactiveUI;
using RequestServices_Ivanov.Views;

namespace RequestServices_Ivanov.ViewModels;

public class NavigationWindowViewModel : ViewModelBase
{
    //Объявили приватное поле для пользователя, который зашёл в систему
    private User _user;

    //Объявили публичное свойство для пользователя, который зашёл в систему
    //И в set методе вызываем уведомлением об изменениях, на случай если пользователь поменяется
    public User User
    {
        get=>_user;
        set=>this.RaiseAndSetIfChanged(ref _user, value);
    }

    //Объявили приватное поле для текущего представления, 
    // оно будет хранить в себе отображающиеся представление(окно)
    private object  _currentView;

    //Объявили публичное поле для текущего представления, 
    //И в set методе вызываем уведомлением об изменениях
    public object  CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    //Объявление публичных команд типа ReactiveCommand<Unit, Unit>
    // где первый Unit - параметр (Unit-пустой), второй Unit - выходное значение (Unit-пустой) 
    public ReactiveCommand<Unit, Unit> DachboardCommand { get; } //Команда для смены окна на DashboardView(окно статистики)
    public ReactiveCommand<Unit, Unit> RequestServicesCommand { get; } //Команда для смены окна на RequestServicesView(окно заявок)

    //Опишем конструктор модели представления где в параметре будет пользователь
    public NavigationWindowViewModel(User user)
    {
        //Передали параметр в свойство модели
        User = user;

        //Описываем команду для установки контекста на окно статистики
        DachboardCommand = ReactiveCommand.Create(()=>
        {
            //Создали поле типа окна статистики и в его DataContext
            // сразу же создали контекст данных
            CurrentView = new DashboardView()
            {
                DataContext = new DashboardViewModel(_user)
            };
        });

        //Описываем команду для установки окна на окно заявок
        RequestServicesCommand = ReactiveCommand.Create(()=>
        {
            CurrentView = new RequestServicesView()
            {
                DataContext = new RequestServicesViewModel()
                {
                    User = _user
                }
            };
        });

        //В качестве отображаемого окна установили окно с заявками
        CurrentView = new RequestServicesView()
        {
            DataContext = new RequestServicesViewModel()
            {
                User = _user
            }
        };


    }
}
