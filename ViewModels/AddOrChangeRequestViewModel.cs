using System;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using ReactiveUI;

namespace RequestServices_Ivanov.ViewModels;

public class AddOrChangeRequestViewModel : ViewModelBase
{
    // Приватное поле контекста базы данных
    private RequestServicesContext db;
    // Главное представление модели, вызываем всегда от RequestServicesViewModel
    private RequestServicesViewModel _mainVM;
    // Изменяемая заявка, если она есть
    private Request _changeRequest;
    private User _user;   
    public User User => _user;

    // Конструктор с параметрами, в обязательно обновите параметры, на те которые вам нужны
    public AddOrChangeRequestViewModel(bool addOrChange, RequestServicesViewModel mainVM, User user, Request changeRequest = null)
    {
        // Переносим параметры в приватные поля класса
        db = new RequestServicesContext();
        _mainVM = mainVM;
        _addOrChange = addOrChange;
        _user = user;

        // Заполняем поля для отображения списка элементов в выборе, при добавлении данных
        Executors = new ObservableCollection<Executor>(db.Executors.ToList());
        Clients = new ObservableCollection<Client>(db.Clients.ToList());
        Equipments = new ObservableCollection<Equipment>(db.Equipments.ToList());

        // Проверяем изменяемую заявку на null
        if(changeRequest == null)
        {
            // Если пустая , то ставим пустые поля во всех вводимых полях

            // SelectedExecutors нужен для динамического отображения выбранных дополнительных специалистов к заявке
            SelectedExecutors = new ObservableCollection<Executor>();
            _priority = "0";
            _selectedStatus = "В обработке";
            
        }else
        {
            // А если нет, то заполняем поля данными из перенесённой заявки
            SelectedExecutors = new ObservableCollection<Executor>(db.ReleaseRequests.Where(r=>r.Idrequest==changeRequest.Idrequest).Select(r=>r.IdexecutorNavigation).ToList());
            
            // ListSelectExecutors нужен для отображения специалистов, которых можно добавить к заявке
            ListSelectExecutors = new ObservableCollection<Executor>(db.Executors.Where(e => !SelectedExecutors.Contains(e)).ToList());
            _priority = changeRequest.Priority;
            _selectedStatus = changeRequest.Status;
            _changeRequest = db.Requests.FirstOrDefault(r=>r.Idrequest == changeRequest.Idrequest);
            SelectecClient = _changeRequest.IdclientNavigation;
            SelectedEquipment = _changeRequest.IdequipmentNavigation;
            SelectedExecutor = _changeRequest.IdexecutorNavigation;
        }
    }

    #region Вимидомсть панели с динамическим отображением выбранных дополнительных специалистов к заявке SelectExecutor
    private bool _isVisibilitySelectExecutor = false;
    public bool IsVisibilitySelectExecutor 
    {
        get => _isVisibilitySelectExecutor;
        set => this.RaiseAndSetIfChanged(ref _isVisibilitySelectExecutor, value);
    }
    #endregion

    //Массив для отображения выриантов статуса
    public string[] Statuses => new string[] { "В работе", "Выполнено", "В обработке"};
    
    
    #region Текущий выбранный статус
    private string _selectedStatus;
    public string SelectedStatus
    {
        get => _selectedStatus;
        set 
        {
            this.RaiseAndSetIfChanged(ref _selectedStatus, value);
        }                
    }
    #endregion

    #region Признак добавления или изменения
    private bool _addOrChange;
    public bool AddOrChange 
    {
        get => _addOrChange;
        set => this.RaiseAndSetIfChanged(ref _addOrChange, value);
    }
    #endregion

    #region Список специалистов, которых можно добавить к заявке
    private ObservableCollection<Executor> _listSelectExecutors;
    public ObservableCollection<Executor> ListSelectExecutors
    {
        get => _listSelectExecutors;
        set => this.RaiseAndSetIfChanged(ref _listSelectExecutors, value); 
    }

    #endregion

    #region Список специаслистов, добавленные к заявке
    private ObservableCollection<Executor> _selectedExecutors;
    public ObservableCollection<Executor> SelectedExecutors
    {
        get => _selectedExecutors;
        set => this.RaiseAndSetIfChanged(ref _selectedExecutors, value); 
    }
    #endregion
    
    #region Приоритет
    private string _priority;
    public string Priority
    {
        get => _priority;
        set => this.RaiseAndSetIfChanged(ref _priority, value);
    }  
    #endregion
    
    #region Главный исполнитель
    private ObservableCollection<Executor> _executors;
    public ObservableCollection<Executor> Executors 
    { 
        get => _executors;
        set => _executors = value;
    }
    #endregion
    
    #region Клиент
    private ObservableCollection<Client> _clients;
    public ObservableCollection<Client> Clients 
    {
        get => _clients;
        set => _clients = value;
    }

    #endregion
    
    #region Техника
    private ObservableCollection<Equipment> _equipments;
    public ObservableCollection<Equipment> Equipments
    {
        get => _equipments;
        set => _equipments = value;
    }
    #endregion
    
    #region Выбранная техника
    private Equipment _selectedEquipment;
    public Equipment SelectedEquipment
    {
        get => _selectedEquipment;
        set => this.RaiseAndSetIfChanged(ref _selectedEquipment, value);
    }
    #endregion 
    
    #region Выбранный специалист
    private Executor _selectedExecutor;
    public Executor SelectedExecutor
    {
        get => _selectedExecutor;
        set => this.RaiseAndSetIfChanged(ref _selectedExecutor, value);
    }

    #endregion
    
    #region Выбранный клиент
    private Client _selectecClient;
    public Client SelectecClient
    {
        get => _selectecClient;
        set => this.RaiseAndSetIfChanged(ref _selectecClient, value);
    }

    #endregion
    
    
    // Метод для взаимодействия с данными
    public void ChangeData(string comment)
    {
        // Если мы признак добавления
        if(_addOrChange)
        {
            // Создаём новый экземпляр заявки
            Request newRequest = new Request()
            {
                // Дату ставим текущую
                DateAdd = DateTime.Now,
                // Приоритет нулевой
                Priority = "0",
                // Введённые данные пользователя
                Idequipment = _selectedEquipment.Idequipment,
                Idclient = _selectecClient.Idclient,
                Idexecutor = _selectedExecutor.Idexecutor,
                Status = _selectedStatus,
                Comment = comment            
            };
            try // Если во время добавления заявки возникло исключение
            {
                // Создаём контекст базы данных
                using(var db = new RequestServicesContext())
                {
                    // Добавляем новую строку
                    db.Requests.Add(newRequest);
                    // Сохраняем изменения
                    db.SaveChanges();
                    // Загружаем новые данные в таблицу с заявками
                    _mainVM.LoadData(_mainVM.Page);
                    // Закрываем представление
                    CloseView();
                }            
            }
            catch (Exception ex)
            {
                // В дебаг консоль выводим ошибку
                Debug.WriteLine(ex.Message);                
            }
        }
        // Если изменяем заявку
        else 
        {
            // В приватное поле записываем новые данные, чтобы сохранить ID
            _changeRequest.Priority = _priority;
            _changeRequest.Idequipment = _selectedEquipment.Idequipment;
            _changeRequest.Idclient = _selectecClient.Idclient;
            _changeRequest.Idexecutor = _selectedExecutor.Idexecutor;
            _changeRequest.Status = _selectedStatus;
            _changeRequest.Comment = comment;
            try
            {
                using(var db = new RequestServicesContext())
                {
                    // Обновляем строку
                    db.Requests.Update(_changeRequest);
                    // Сохраняем данные
                    db.SaveChanges();
                    // После обновления заявки, нужно проверить есть ли дополнительные специалиста у заявки
                    if(_changeRequest.ReleaseRequests.Count != 0)
                    // Если есть
                    {
                        // То удаляем все строки из таблицы ReleaseRequests у этой заявки
                        foreach(var releaseRequest in _changeRequest.ReleaseRequests)
                        {
                            db.ReleaseRequests.Remove(releaseRequest);
                        }
                        // Сохраняем значения
                        db.SaveChanges();
                        // Добавлем изменённый список дополнительных специалистов
                        foreach(var executor in SelectedExecutors)
                        {
                            ReleaseRequest releaseRequest = new ReleaseRequest()
                            {
                                Idexecutor = executor.Idexecutor,
                                Idrequest = _changeRequest.Idrequest
                            };
                            db.ReleaseRequests.Add(releaseRequest);                            
                        }

                    }
                    // А если дополнительных специалистов изначально не было
                    if(_changeRequest.ReleaseRequests.Count == 0)
                    {                        
                        // То сразу добавляем дополнительных специалистов
                        foreach(var executor in SelectedExecutors)
                        {
                            ReleaseRequest releaseRequest = new ReleaseRequest()
                            {
                                Idexecutor = executor.Idexecutor,
                                Idrequest = _changeRequest.Idrequest
                            };
                            db.ReleaseRequests.Add(releaseRequest);                            
                        }
                    }
                    // Обновляем данные
                    db.SaveChanges();
                    // Загружаем новые данные в таблицу с заявками
                    _mainVM.LoadData(_mainVM.Page);
                    // Закрываем представление
                    CloseView();
                }            
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);                
            }
                      
        }
    }

    public void CloseView()
    {
        // У свойства родительского представления модели обнуляем текущее отображаемое представление
        _mainVM.CurrentView = null;
        // И закрываем панель 
        _mainVM.IsVisibilityView = !_mainVM.IsVisibilityView;
    }

    // Метод для изменения вимидомсти панели с динамическим отображением выбранных дополнительных специалистов к заявке SelectExecutor 
    // Для этого используется лямбда выражение, ведь у нас происходит только одна строка действия
    public void ShowSelectExecutorView() => IsVisibilitySelectExecutor = !IsVisibilitySelectExecutor;
    
    // Метод происходящий при выборе дополнительного специалиста из коллекции ListSelectExecutors
    public void SelectExecutor(int Idexecutor)
    {
        // Находим экзепляр специалиста из бд по его id из параметра
        Executor executor = db.Executors.FirstOrDefault(e=>e.Idexecutor==Idexecutor); 
        // Добавлем его к колекции выбранных доп. специалистов
        SelectedExecutors.Add(executor);
        // Удаляем из коллекции возможных доп. специалистов
        // чтобы убрать возможность повтороного выбора
        ListSelectExecutors.Remove(executor);
    }

    // Метод просходящий при удалении доп. специалиста из выбранных
    public void DeleteSelectExecutor(int Idexecutor)
    {
        // Находим экзепляр специалиста из бд по его id из параметра
        Executor executor = db.Executors.FirstOrDefault(e=>e.Idexecutor==Idexecutor); 
        // Удаляем доп. специалиста из коллекции выбранных доп. специалистов
        SelectedExecutors.Remove(executor);
        // Добавляем доп. специалиста к коллекции возможных доп. специалистов
        ListSelectExecutors.Add(executor);
    }

}
