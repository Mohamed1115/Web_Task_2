using Microsoft.AspNetCore.Mvc;
using Task2.Models;
using Task2.Repositories;

namespace Task2.Controllers;

public class TodoController:Controller
{
    private readonly GenericRepository<Todo> _repo;

    public TodoController(GenericRepository<Todo> repo)
    {
        _repo = repo;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cn)
     {
        
         var todo = await _repo.GetAllAsync(cn);
         return View(todo);
     }

    [HttpPost]
    public async Task<IActionResult> Creat(CancellationToken cn, Todo to,IFormFile? file)
    {
        if (file != null)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","Files" ,fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            to.File = fileName;
        }
        
        await _repo.CreateAsync(to, cn);
        return RedirectToAction(nameof(GetAll));
    }

    [HttpGet]
    public IActionResult Creat_view()
    {
        return View("Creat");
    }

    [HttpGet]
    public IActionResult User_view()
    {
        return View("User");
    }

    [HttpPost]
    public IActionResult User(string? name)
    {
        var options = new CookieOptions
        {
            MaxAge = TimeSpan.FromHours(24)
        };
        if (!string.IsNullOrEmpty(name))
        {
            Response.Cookies.Append("name", name, options);
        }
        
        return RedirectToAction(nameof(GetAll));
    }

      [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken cn)
        {
            var todo = await _repo.GetByIdAsync(id,cn);
            
            if (todo == null)
            {
                return NotFound();
            }

            return View(todo);
        }

        // POST: Todo/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormFile collection, CancellationToken cn)
        {
            try
            {
                var todo = await _repo.GetByIdAsync(id,cn);
                
                if (todo == null)
                {
                    return NotFound();
                }

                // حذف الملف المرفق إذا كان موجود
                if (!string.IsNullOrEmpty(todo.File))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "Files", todo.File);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                await _repo.DeleteAsync(todo, cn);
                

                TempData["SuccessMessage"] = "تم حذف المهمة بنجاح";
                return RedirectToAction("GetAll");
            }
            catch
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء حذف المهمة";
                return View();
            }
        }

        // GET: Todo/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cn)
        {
            var todo = await _repo.GetByIdAsync(id,cn);
            
            
            if (todo == null)
            {
                return NotFound();
            }
        
            return View(todo);
        }
        
        // POST: Todo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Todo todo, IFormFile? FileUpload, CancellationToken cn)
        {
            try
            {
                
                    var existingTodo = await _repo.GetByIdAsync(todo.Id,cn);
                    
                    if (existingTodo == null)
                    {
                        return NotFound();
                    }
        
                    // تحديث البيانات
                    existingTodo.Title = todo.Title;
                    existingTodo.Description = todo.Description;
                    existingTodo.Status = todo.Status;
                    existingTodo.DeadLine = todo.DeadLine;
        
                    // معالجة الملف الجديد إذا تم رفع واحد
                    if (FileUpload != null && FileUpload.Length > 0)
                    {
                        // حذف الملف القديم
                        if (!string.IsNullOrEmpty(existingTodo.File))
                        {
                            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "Files", existingTodo.File);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
        
                        // حفظ الملف الجديد
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(FileUpload.FileName);
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "Files", fileName);
        
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            FileUpload.CopyTo(stream);
                        }
        
                        existingTodo.File = fileName;
                    }


                    await _repo.UpdateAsync(existingTodo, cn);
        
                    TempData["SuccessMessage"] = "تم تحديث المهمة بنجاح";
                    return RedirectToAction("GetAll");
                
        
                return View(todo);
            }
            catch
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحديث المهمة";
                return View(todo);
            }
        }
       
    
    
    


}