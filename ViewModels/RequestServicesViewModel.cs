using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using RequestServices_Ivanov.Views;

namespace RequestServices_Ivanov.ViewModels;

public class RequestServicesViewModel : ViewModelBase
{
    // Источник данных
    private readonly SourceCache<Request, int> _sourceCache = new (x => x.Idrequest); 

    // Коллекция строк которая будет преобразовываться в страницы 
    // и передаваться в другую коллекцию для отображения
    private ReadOnlyObservableCollection<Request> _pagesRequsts;

    // Команда с пустыми параметрами которая будет очищать фильтр
    public ReactiveCommand<Unit, Unit> ClearFilterCommand { get; private set;}

    // Команды с пустыми параметрами которая будет отображать представления
    // для взаимодействия с данными
    public ReactiveCommand<Unit, Unit> ShowAddOrChangeViewCommand { get; private set;}

    // Контекст базы данных
    private RequestServicesContext _database;

    // Текущий пользователь
    public User User { get; set;}

    public RequestServicesViewModel()
    {
        // Описываем команду для очистки фильтра с помощью лямбды выражения
        ClearFilterCommand = ReactiveCommand.Create(()=>
        {
            // Обьявляем контекст базы данных 
            using(var db = new RequestServicesContext())
            {
                // Очищаем источник данных
                _sourceCache.Clear();
                // В качестве выбранного статуса устанавливаем "Все варианты"
                SelectedStatus = "Все варианты";
                // Устанавливаем пустое значение для поисковой строки техники, клиента и исполнителя
                FilterEquipment = string.Empty;
                FilterClient = string.Empty;
                FilterExecutor = string.Empty;

                // Загружаем в источник новые данные
                _sourceCache.AddOrUpdate(db.Requests.ToList());
            }
        });

        // Описываем команду для показа представления для взаимодействия с данными
        //  при помощи лямбды выражения
        ShowAddOrChangeViewCommand = ReactiveCommand.Create(()=>
        {
            // Если свойство текущего представления не пусто
            if(CurrentView != null)
            {
                // Очищаем его
                CurrentView = null;
                // И скрываем панель которая отображает окно
                IsVisibilityView = !IsVisibilityView;
            } 
            // а если текущее представление пусто
            else 
            {
                // Устанавливаем в свойство текущего представления окно
                CurrentView = new AddOrChangeRequestView()
                {
                    // В его DataContext создаём новый экземпляр класса и передаём параметры
                    DataContext = new AddOrChangeRequestViewModel(true, this, User)
                };
                // Отображаем панель
                IsVisibilityView = !IsVisibilityView;
            }
        });

        // Connect создаёт объект IObservable<IChangeSet<TObject, TKey>>, представляющий изменения в исходном потоке данных.
        _sourceCache.Connect()
            //Устанавливаем функцию фильтрации, при изменении источника данных или его перезагрузке
            //будет воспроизводится функция фильтрации, берётся каждый элемен источника данных
            // запускаяется метод на проверку этой строк. Возвращаемое логическое значение из метода
            // задаёт пропускать эту строку или нет
            .Filter(filterRow => Filter(filterRow))       
            //Устанавливаем выходной элемент, передавать все данные в коллекцию строк _pagesRequsts
            .Bind(out _pagesRequsts)
            // Метод представляющий дополнительные действия после всех операций с данными
            // В нашем случае мы считаем количество строк в коллекции и обновляем коллекцию строк
            // Которая представляет данные в интерфейсе
            .Do(changes => 
            { 
                CountRows = changes.Count();
                RefreshFilterRequests(_page); 
            })
            // Функция очищает ресурсы, связанные с элементами, когда они удаляются из кеша. 
            .DisposeMany();

        // Загружаем даннные в источник
        LoadData(_page);
    }


    private int _countPage;

    #region Размер страницы
    private int _pageSize = 10;    
    public int PageSize
    {
        get => _pageSize;
        set => this.RaiseAndSetIfChanged(ref _pageSize, value);
    }
    #endregion
    
    #region Количество строк в странице
    private int _countRows;
    public int CountRows
    {
        get => _countRows;
        set => this.RaiseAndSetIfChanged(ref _countRows, value);
    }
    #endregion
    
    #region Страница
    private int _page = 1;
    public int Page
    {        
        get => _page;
        set => this.RaiseAndSetIfChanged(ref _page, value);
    }
    #endregion
    
    // Метод для переключения страницы вперёд
    public void NextPage()
    {
        // Создаём переменную и задаём значение количества страниц,
        // при этом нужно соблюдать условие, ведь деление может быть с остатком,
        // а количество страниц всегда целое число, поэтому мы просто через условие
        // прибаляем один если есть остаток и 0 если его нет
        int countPage = _countRows / 10 + 
            (_countRows % 10 == 0 ? 0 : 1);
            // Проверям чтобы текущая страница не выходила за пределы общего количества
        if(_page + 1 > countPage)
            // Если выходит за пределы то присваиваем текущей странице начальное значение
            Page = 1;
            else
            // А если нет, то прибавляем 1
            Page = _page + 1;

        // Обновляем коллекцию строк представления данных меняя страницу    
        RefreshFilterRequests(Page);
    }
    public void PrevPage()
    {
        // Делаем обратные действия
        if( 1 > _page - 1)
            Page = _countRows / 10 + 
            (_countRows % 10 == 0 ? 0 : 1);
        else
            Page = _page - 1;
            RefreshFilterRequests(Page);
    }
    private void RefreshFilterRequests(int page)
    {
        // Присваиваем коллекции для отображения данных в интерфейсе
        // новую коллекцию из пропущенных страниц и строк
        // Будет пропускаться строки вычисляемые при помощи страниц, и браться 10 строк
        FilterRequests = new ObservableCollection<Request>(
            _pagesRequsts.Skip((page - 1) * 10).Take(10));
    }
    
    #region Текущее представление и отображение панели сбоку
    private Object _currentView;
    public Object CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }
    private bool _isVisibilityView = false;
    public bool IsVisibilityView
    {
        get => _isVisibilityView;
        set => this.RaiseAndSetIfChanged(ref _isVisibilityView, value);
    }

    #endregion
    
    // Массив для привязки к полю со списком представляющие Статусы
    public string[] Statuses => new string[] { "Все варианты", "В обработке", "В работе", "Выполнено"};

    #region Выбранный статус
    private string _selectedStatus = "Все варианты";
    public string SelectedStatus
    {
        get => _selectedStatus;
        set 
        {
            this.RaiseAndSetIfChanged(ref _selectedStatus, value);
            ApplyFilter();
        }                
    }
    #endregion
    
    #region Техника для фильтра
    private string _filterEquipment = string.Empty;
    public string FilterEquipment
    {
        get => _filterEquipment;
        set 
        {
            this.RaiseAndSetIfChanged(ref _filterEquipment, value);
            ApplyFilter();
        }
    }
    #endregion
    
    #region Клиент для фильтра
    private string _filterClient = string.Empty;
    public string FilterClient
    {
        get => _filterClient; 
        set 
        {
            this.RaiseAndSetIfChanged(ref _filterClient, value);
            ApplyFilter();
        }
    }
    
    #endregion
   
    #region Исполнитель для фильтра
    private string _filterExecutor = string.Empty;
    public string FilterExecutor
    {
        get => _filterExecutor; 
        set 
        {
            this.RaiseAndSetIfChanged(ref _filterExecutor, value);
            ApplyFilter();
        }
    }
    
    #endregion
   
    #region Коллекция строк заявок
    private ObservableCollection<Request> _filterRequests;
    public ObservableCollection<Request> FilterRequests 
    {
        get => _filterRequests;
        set => this.RaiseAndSetIfChanged(ref _filterRequests, value);
    }  
    #endregion
    
    //Метод загрузки данных в источник
    public void LoadData(int page)
    {         
        // Очищаем источник от устарвеших значений
        _sourceCache.Clear();
        // В глобальную переменную добавляем новый контекст базы
        _database = new RequestServicesContext();
        // Общее количество строк обновляем
        CountRows = _database.Requests.Count();
        // Добавляем новые данные в источник данных
        _sourceCache.AddOrUpdate(_database.Requests.ToList());
        // Переносим данные из источника в коллекцию строк которая к которой привязана таблица
        RefreshFilterRequests(page);
    }

    //Функция фильтрация возвращающее логическое значение,
    // нужно ли отобразить строку из параметра
    private bool Filter(Request request)
    {
        // Описывается процесс анализа входной строки из парамета по его полям.
        // Мы заранее переводим все строковые поля в нижний регистр чтобы пользователю было удобно искать данные

        // условие по полю "Техника", проводится поиск техники по части строки которая берётся из свойства FilterEquipment
        bool equipmentMatch = request.IdequipmentNavigation.Title.ToLower().Contains(FilterEquipment.ToLower()); 
        // условие по полю "Клиент", проводится поиск клиента по части ФИО которая берётся из свойства FilterClient
        bool clientMatch = request.IdclientNavigation.IduserNavigation.Fio.ToLower().Contains(FilterClient.ToLower()); 
        // условие по полю "Исполнитель", проводится поиск исполнителя по части его ФИО которая берётся из свойства FilterExecutor
        bool executorMatch = request.IdexecutorNavigation.IduserNavigation.Fio.ToLower().Contains(FilterExecutor.ToLower()); 
        // условие по полю "Статус", проводится по выбранной строке из ComboBox и если у нас указаны все варианты, то поиск не проводится
        // и автоматически все строки пропускаются
        bool statusMatch = _selectedStatus == "Все варианты" || _selectedStatus == null ? true : request.Status == _selectedStatus;

        // Возвращается значение логической операции где мы возвращаем true только когда все элементы true 
        return equipmentMatch && clientMatch && executorMatch && statusMatch;
    }

    private void ApplyFilter()
    {
        _sourceCache.Refresh(); // Перезапускаем фильтрацию
    }


    public void Change(Request changeRequest)
    {
        // Проверяем чтобы выбранный элемент не был пустым, и если он всё таки null, то выходим из метода
        if(changeRequest == null)
            return;

        // В текущее представление переносим созданный экземпляр класса
        // И в его DataContext создаём экземпляр где передаём параметры в конструктор
        CurrentView = new AddOrChangeRequestView()
        {
            DataContext = new AddOrChangeRequestViewModel(false, this, User, changeRequest)
        };
        // Устанавливаем видимость панели с разделителем чтобы он был виден
        IsVisibilityView = true;
    }

    public void DeleteRequest(Request deleteRequest)
    {
        //Проверяем элемент на null
        if(deleteRequest == null)
            return;
        //Удаляем строку из контекста данных
        _database.Requests.Remove(deleteRequest);
        //Передаём данные из контекста в БД и созраняем изменения
        _database.SaveChanges();
        // Загружаем новые данные с учётом выбранной страницы
        LoadData(_page);
    }
}
