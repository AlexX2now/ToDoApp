using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TodoApp.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            //������� �� �������� � ����� ����������
            return RedirectToPage("/Todo");
        }
    }
}
