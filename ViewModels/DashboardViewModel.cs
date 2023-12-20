using System.Linq;

namespace RequestServices_Ivanov.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    //Объявили праватное поле для хранения промежуточных значений в свойстве LastAddRequest
    private string _lastAddRequest;
    // Объявили публичное свойство в котором будет храниться строка даты последней добавленной заявки пользователя
    public string LastAddRequest
    {
        get { return _lastAddRequest; }
        set { _lastAddRequest = value; }

    }

    //Объявили праватное поле для хранения промежуточных значений в свойстве _countAddRequest
    private string _countAddRequest;

    // Объявили публичное свойство которое будет хранить строку количество добавленных заявок пользователя
    public string CountAddRequest
    {
        get { return _countAddRequest; }
        set { _countAddRequest = value; }
    }
        
    //Объявили праватное поле для хранения промежуточных значений в свойстве CountRequestStatusWorking
    private string _countRequestStatusWorking;

    // Объявили публичное свойство для хранении строки количества заявок со статусом "В обработке" пользователя
    public string CountRequestStatusWorking
    {
        get { return _countRequestStatusWorking; }
        set { _countRequestStatusWorking = value; }
    }
    
    // Объявили конструктор с параметром экземляра пользователя
    public DashboardViewModel(User user)    
    {
        //Открыли соединение с базой данных и объявили экземпляр контекста базы данных
        //using используется чтобы в конце взаимодействия с бд не нужно было в ручную очищать ресурсы
        //using сделает это за нас 
        using(var db = new RequestServicesContext())
        {
            //Нашли и присвоили экземпляр пользователя по переданному пользователю в конструкторе
            User reqUser = db.Users.FirstOrDefault(u => u.Iduser == user.Iduser);

            // создали запрос в котором присвоили результат выборки Linq запроса
            //знаки ? нужны для провери результатов на null
            // таким образом первым проверяется regUser на null, потом результат выборки первого клиента по id пользователя,
            // а затем проверяется массив с заявками, есть ли у клиента заявки
            // Выборка происходит так, из поля regUser выбирается таблица клиентов, выбирается первый клиент, затем
            // у этого клиента берётся массив его заявок (если они есть), далее сортируем заявки по дате добавления,
            // отображается только поле с датой и выбирается первое поле, тоесть самая рання дата
            var lastRequestQuery = reqUser?.Clients.FirstOrDefault()?.Requests?
                            .OrderBy(r => r.DateAdd)
                            .Select( r => r.DateAdd.ToString("dd.MM.yyyy"))
                            .FirstOrDefault();

            // Производим выборку и переносим результат в переменную
            var countRequestQuery = reqUser?.Clients.FirstOrDefault()?.Requests?
                            .Count().ToString();

            // Производим выборку и переносим результат в переменную
            var countRequestWorkingQuery = reqUser?.Clients.FirstOrDefault()?.Requests?
                            .Where(r => r.Status == "В обработке")?.Count().ToString();
            //Далее проверяется, вернули ли результаты выборки null, и если вернули,
            //оповещаем пользоватея что выборка не удалась
            _lastAddRequest = lastRequestQuery ?? "Не найдено";
            _countAddRequest = countRequestQuery ?? "Не найдено";
            _countRequestStatusWorking = countRequestWorkingQuery ?? "Не найдено";
        }
    }
}
