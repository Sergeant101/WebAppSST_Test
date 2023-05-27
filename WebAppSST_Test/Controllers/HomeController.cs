using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
//include data context db
using WebAppSST_Test.Data;
//model data table
using WebAppSST_Test.Models;
using Microsoft.EntityFrameworkCore;

// изначально конечно правильнее было бы сделать CRUD в отдельном API, но так быстрее
namespace WebAppSST_Test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // for DI context DB 
        private readonly ApplicationDbContext _context;

        // injection service and data base context
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // просто асинхронно выбираем первый совпадающий элемент по ID, в принципе можно выбирать элемент по любому другому полю
        public async Task<string> GetUser(int id)
        {

            // если пользователь найден выводим данные, если нет выводим заглушку об неудачном поиске без подробностей
            try
            {
                User user = await _context.Users.FirstAsync(u => u.ID == id);
                return user.Name + ' ' + user.SurName;
            }
            catch
            {
                _logger.LogInformation("Не удалось найти пользователя");
                return "Не удалось найти пользователя";
            }
        }

        // добавляем нового пользователя
        public string AddUser(string name, string surname)
        {
            // обычная проверка на допустимые значения
            if ((name == null) || (surname == null)) 
            {
                return "имя пользователя и фамилия не могут быть пустыми";    
            }

            // как и в предыдущем оборачиваем в обработчик ошибок чтобы избежать неожиданностей
            try
            {
                //создаём новую запись пользователя
                var newUser = new User
                {
                    Name = name,
                    SurName = surname
                };

                //помечаем пользователя как остлеживаемого на предмет изменений
                _context.Users.Add(newUser);
                //сохраняем пользователя в базе данных
                int affected = _context.SaveChanges();

                //Если всё хорошо выводим сообщение
                if (affected == 1)
                {
                    return "пользователь добавлен";
                }

                return "не удалось добавить пользователя";
                
            }
            catch
            {
                return "ошибка при добавлении пользователя";
            }
        }

        // обновление существующего пользователя
        public async Task<string> UpdateUser(string sourcesurname, string name, string surname) 
        {
            if ((name == null) || (surname == null) || (sourcesurname == null))
            {
                return "поля имя и фамилия не могут быть пустыми";
            }
            try
            {
                // почему изменяю только по первому нахождению фамилии не учитывая возможные совпадения?
                // просто демонстрирую что умею пользоваться EF
                var updateUser = await _context.Users.FirstOrDefaultAsync(u => u.SurName == sourcesurname);

                if (updateUser == null) 
                {
                    return "пользователь не найден";
                }
                // обновляем поля
                updateUser.Name = name;
                updateUser.SurName = surname;
                // сохраняем изменения
                _context.SaveChanges();

                return "изменения сохранены";
            }
            catch
            {
                return "не удалось обновить пользователя";
            }
        }

        public async Task<string> DeleteUser(string name,  string surname)
        {
            if ((name == null) || (surname == null))
            {
                return "поля имя и фамилия не могут быть пустыми";
            }

            try
            {
                // выборка по двум столбцам
                var deleteUser = await _context.Users
                    .FirstAsync(u => u.SurName == surname && u.Name == name);

                if (deleteUser == null)
                {
                    return "пользователь не найден";
                }
                _context.Users.Remove(deleteUser);
                _context.SaveChanges() ;

                return "пользователь удалён";
            }
            catch 
            {
                return "ошибка при удалении пользователя";
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}