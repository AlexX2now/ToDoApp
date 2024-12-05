using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TodoApp.Models;

namespace TodoApp.Pages
{
    public class TodoModel : PageModel
    {
        private readonly TodoContext _context;

        public TodoModel(TodoContext context)
        {
            _context = context;
        }

        public List<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
        [BindProperty]
        public int FindId { get; set; }
        public int IdRec { get; set; }
        public string RusIsComp { get; set; }
        public TodoItem? FoundTodoItem { get; set; }
        public string Message { get; private set; } = "";
        public string TableHtml { get; set; }
        public async Task OnGetAsync()
        {
            TodoItems = await _context.TodoItems.ToListAsync();

            //Заполнение таблицы записями из бд
            var todoItems = await _context.TodoItems.ToListAsync();
            string tableHtml = "<table><thead><tr><th>ID</th><th>Заголовок</th><th>Выполнено</th><th>Дата создания</th></tr></thead><tbody>";
            foreach (var item in todoItems)
            {
                RusIsComp = item.IsCompleted.Value ? "Да" : "Нет";
                tableHtml += $"<tr><td>{item.Id}</td><td>{item.Title}</td><td>{RusIsComp}</td><td>{item.CreatedAt.Value.ToShortDateString()}</td></tr>";
            }
            tableHtml += "</tbody></table>";
            TableHtml = tableHtml;
        }

        public async Task<IActionResult> OnPostAsync([FromForm] string title, [FromForm] string actionBtn, [FromForm] int recToUpd)
        {
            try
            {

                if (string.IsNullOrEmpty(title))
                {
                    Message = "Не оставляйте поля пустыми";
                    TodoItems = await _context.TodoItems.ToListAsync();
                    return RedirectToPage("/Index");
                }
                else
                {
                    //В завимости от нажатой кнопки производится действие
                    switch (actionBtn)
                    {
                        //Кнопка добавления записи
                        case "addBtn":
                            TodoItem newTodoItem = new()
                            {
                                Title = title,
                                IsCompleted = false,
                                CreatedAt = DateTime.Now,
                            };

                            _context.TodoItems.Add(newTodoItem);
                            await _context.SaveChangesAsync();

                            TodoItems = await _context.TodoItems.ToListAsync();

                            return RedirectToPage("/Index");

                            //Кнопка выполнения записи
                        case "perBtn":
                            if (!IsNum(title))
                            {

                                return NotFound();
                            }
                            else
                            {
                                IdRec = int.Parse(title);

                                var neededRecToChn = _context.TodoItems.FirstOrDefault(x => x.Id == IdRec);

                                if (neededRecToChn == null)
                                {
                                    return NotFound();
                                }
                                else
                                {
                                    if (neededRecToChn.IsCompleted == true)
                                    {
                                        return NotFound();
                                    }
                                    else
                                    {
                                        neededRecToChn.IsCompleted = true;
                                        await _context.SaveChangesAsync();

                                        TodoItems = await _context.TodoItems.ToListAsync();

                                        return RedirectToPage("/Index");
                                    }
                                }
                            }

                            //Кнопка удаления записи
                        case "delBtn":
                            if (!IsNum(title))
                            {
                                return NotFound();
                            }
                            else
                            {
                                IdRec = int.Parse(title);
                                var neededRecToDel = _context.TodoItems.FirstOrDefault(x => x.Id == IdRec);

                                if (neededRecToDel == null)
                                {
                                    return NotFound();
                                }
                                else
                                {
                                    _context.TodoItems.Remove(neededRecToDel);

                                    await _context.SaveChangesAsync();

                                    TodoItems = await _context.TodoItems.ToListAsync();

                                    return RedirectToPage("/Index");
                                }
                            }

                            //Кнопка поиска записи
                        case "serBtn":
                            if (!IsNum(title))
                            {
                                return NotFound();
                            }
                            else
                            {
                                IdRec = int.Parse(title);
                                var neededRecToSrc = _context.TodoItems.FirstOrDefault(x => x.Id == IdRec);

                                if (neededRecToSrc == null)
                                {
                                    return NotFound();
                                }
                                else
                                {
                                    string tableHtml = "<h2>Нужная запись</h2><table><thead><tr><th>ID</th><th>Заголовок</th><th>Выполнено</th><th>Дата создания</th></tr></thead><tbody>";
                                    
                                    RusIsComp = neededRecToSrc.IsCompleted.Value ? "Да" : "Нет";
                                    
                                    tableHtml += $"<tr><td>{neededRecToSrc.Id}</td><td>{neededRecToSrc.Title}</td><td>{RusIsComp}</td><td>{neededRecToSrc.CreatedAt.Value.ToShortDateString()}</td></tr>";

                                    tableHtml += "</tbody></table>";
                                    TableHtml = tableHtml;
                                    return Page();
                                }
                            }

                            //Кнопка редактирования записи
                        case "updBtn":
                            var neededRecToUpd = _context.TodoItems.FirstOrDefault(x => x.Id == recToUpd);

                            if (neededRecToUpd == null)
                            {
                                return NotFound();
                            }
                            else
                            {
                                neededRecToUpd.Title = title;

                                await _context.SaveChangesAsync();

                                TodoItems = await _context.TodoItems.ToListAsync();

                                return RedirectToPage("/Index");
                            }

                            //Кнопка обновлаяющая страницу
                        case "refreshBtn":
                            TodoItems = await _context.TodoItems.ToListAsync();

                            return RedirectToPage("/Index");

                        default:
                            return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                TodoItems = await _context.TodoItems.ToListAsync();

                return RedirectToPage("/Index");
            }
        }

        //Метод проверки поля, является ли он числом
        private bool IsNum(string field)
        {
            return int.TryParse(field, out _) ? true : false;
        }
    }
}