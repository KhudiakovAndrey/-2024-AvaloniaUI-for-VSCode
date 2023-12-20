using Avalonia.Controls;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using RequestServices_Ivanov.Views;

namespace RequestServices_Ivanov.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    //Объявили приватное поле логина чтобы использовать его как буфер для публичного свойства
    private string _login;
    //Объявили публичное свойство Логин который позднее привяжем к 
    // текстовому полю чтобы передавать актуальное значение
    public string Login
    {
        get => _login;
        //в методе set создаём уведомление для измненения, чтобы обновить все привязанные поля 
        // к этому свойству
        set => this.RaiseAndSetIfChanged(ref _login, value);
    }

    //Объявили приватное поле пароля чтобы использовать его как буфер для публичного свойства
    private string _password;

    //Объявили публичное свойство Пароль который позднее привяжем к 
    // текстовому полю чтобы передавать актуальное значение
    public string Password
    {
        get => _password;
        //в методе set создаём уведомление для измненения, чтобы обновить все привязанные поля 
        // к этому свойству
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    //Объявили публичный асинхронный метод для авторизации, позднее привяжем к кнопке Войти
    public async void CheckLogin(Window mainWindow)
    {
        //Объявили экземпляр базы данных
        using(var db = new RequestServicesContext())
        {
            //Объявили экземпляр пользователя и присвоили из бд первого пользователя по результату
            // поиска по логину и паролю и выводим в отдельный поток заставив код дожидаться завершения перебора
            var user = await db.Users.FirstOrDefaultAsync(u=>u.Login == Login && u.Password == Password);
            //Проверяем, нашёлся ли какой пользовать
            if(user == null)
            {
                //Если не нашёлся, значит логин и пароль неправильный, обнуляем поля
                Login = Password = string.Empty;                        
                return;
            }
            NavigationWindow window = new NavigationWindow()
            {
                DataContext = new NavigationWindowViewModel(user)
            };
            window.Show();

            mainWindow?.Close();
            return;
        }
    }
}
