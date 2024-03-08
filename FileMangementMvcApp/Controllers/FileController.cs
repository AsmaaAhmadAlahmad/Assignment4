using Assignement3_Domain.Helper;
using AutoMapper;
using FileManagement.Data;
using FileManagement.Domain.Entities;
using FileMangementMvcApp.Models;
using FileMangementMvcApp.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.IO;
using static System.Net.WebRequestMethods;

namespace FileMangementMvcApp.Controllers
{

    [Authorize]
    public class FileController : Controller
    {
        private readonly IGenericRepository<FileUpload, FileUploadToUpdate> genericRepository;
        private readonly HelperMapper<FileUploadToAdd, FileUploadToUpdate, FileUpload> mapper;

        public FileController(IGenericRepository<FileUpload,FileUploadToUpdate> genericRepository
                              , HelperMapper<FileUploadToAdd, FileUploadToUpdate, FileUpload> mapper)
        {
            this.genericRepository = genericRepository;
            this.mapper = mapper;
        }


        // الاكشن التالية لتأخذ المستخدم لصفحة تعديل ملف 
        public async Task<IActionResult> Update(Guid id)
        {
            ViewBag.ErrorMessageInvalidExtension = TempData["ErrorMessage"];

            var file = await genericRepository.GetItemAsync(filter: f => f.Id == id);

            return View(file);
        }


        // الاكشن التالية لتأخذ المستخدم لصفحة رفع ملف 
        public IActionResult Upload()
        {
            ViewBag.MessageSelectfile = TempData["MessageSelectfile"];

            ViewBag.ErrorMessageInvalidExtension =  TempData["ErrorMessage"];

            return View();
        }

        public async Task<IActionResult> Index()
        {
            // ViewBag للحصول على الرسالة ونخزينها في TempData استخدام 
            ViewBag.ErrorMessage = TempData["ErrorMessage"];

            ViewBag.SuccessMessage = TempData["SuccessMessage"];

            
            ViewBag.Message = TempData["Message"] as string;

            // جلب كل معلومات الملفات 
            var files = await genericRepository.GetAllAsync();

            return View(files);
        }


        // ايند بوينت حذف ملف 
        [HttpPost("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var file = await genericRepository.GetItemAsync(filter: f => f.Id == id);

                string fileNameInUploadsFolder = GetFileNameFromUploadsFolder(id, file?.Name);

                await DeleteFileWithoutReturnAnyThing(id, fileNameInUploadsFolder);

                TempData["SuccessMessage"] = "File Deleted successfully";

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "an error occurred";
            }
            return RedirectToAction("Index");
        }



        // ايند بوينت رفع ملف 
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file,
                                                FileUploadToAdd fileUploadToAdd)
        {
           
            if (file != null && file.Length > 0)
            {
                try
                {
                    if (!ValidExtensions(file.FileName)) // اذا كان امتداد الملف غير مسموح به 
                    {

                        return RedirectToAction("Upload");
                    }

                    // استخراج امتداد الملف 
                    string fileExtension = Path.GetExtension(file.FileName);

                    // متابعة مع التحميل والحفظ إذا كان امتداد الملف مسموح به
                    // تحديد المجلد الذي سيتم حفظ الملف المرفوع فيه 
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

                    // انشاء المجلد اذا كان غير موجود 
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // استخراج اسم الملف الاصلي 
                    string fileName = Path.GetFileName(file.FileName);

                    // تم توليد رقم الملف هنا لأنني سأستخدمه في وضع مسار الملف 
                    // حيث استخدمت رقم الملف وليس اسمه لكي يتخزن الملف في الهارد ويكون اسمه هو رقمه
                    // مع الامتداد الخاص به وليس الاسم الذي رُفع به لأني واجهت مشكلة عندما كان الاسم الذي رُفع به
                    var fileId = Guid.NewGuid();

                    string fileIdAsString = fileId.ToString();

                    // وضع مسار الملف 
                    string filePath = Path.Combine(uploadsFolder, fileIdAsString + fileExtension);

                    // حفظ الملف في المجلد 
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // في الاوبجكت التالي لاداعي للاوتو مابر لأن خصائص الاوبجكت لايتم أخذها من اوبجكت اخر
                    //  عدا اخر خاصية
                    FileUpload fileUpload = new FileUpload
                    {
                        Id = fileId,
                        Path = filePath,
                        Name = fileName,
                        FileType = file.ContentType,
                        CreatedBy = User.Identity.Name,
                        Description = fileUploadToAdd.Description
                    };

                    // اضافة معلومات الملف للقاعدة 
                    await genericRepository.AddAsync(fileUpload);
                    await genericRepository.Save();

                    TempData["SuccessMessage"] = "File uploaded successfully";
                }

                catch (Exception)
                {
                    TempData["ErrorMessage"] = "an error occurred";
                }
            }
            else
            {
                TempData["MessageSelectfile"] = "Please select a file";
                return RedirectToAction("Upload");

            }

            return RedirectToAction("Index");
        }



        // ايند بوينت تعديل ملف
        [HttpPost]
        public async Task<IActionResult> Update( IFormFile file1,
                                                 FileUploadToUpdate fileUploadToUpdate)
        {
            try
            {
                var Oldfile = await genericRepository.GetItemAsync(filter: f => f.Id == fileUploadToUpdate.id);

                // هنا في حال اراد اليوزر تعديل كل الملف يعني تغيير كل الملف 
                if (file1 != null)
                {

                    if (!ValidExtensions(file1?.FileName)) // اذا كان امتداد الملف غير مسموح به 
                    {

                        return RedirectToAction("Update", new { Id = Oldfile?.Id });
                    }


                    string fileNameInUploadsFolder = GetFileNameFromUploadsFolder(Oldfile!.Id, Oldfile.Name);

                    await DeleteFileWithoutReturnAnyThing(fileUploadToUpdate.id, fileNameInUploadsFolder);

                    FileUploadToAdd fileUploadToAdd = mapper.MapperForCreate.Map<FileUploadToUpdate, FileUploadToAdd>(fileUploadToUpdate);

                    // رفع الملف الجديد 
                    await Upload(file1, fileUploadToAdd);

                    TempData["SuccessMessage"] = "File updated successfully";
                }



                // هنا في حال اراد اليوزر تعديل معلومات الملف فقط 
                else
                {
                    await genericRepository.UpdateAsync(fileUploadToUpdate.id, fileUploadToUpdate);
                    await genericRepository.Save();

                    TempData["SuccessMessage"] = "File updated successfully";
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "an error occurred";
            }

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Download(Guid fileId )
        {
            var fileName = "";
            var filePath = "";

            try
            {
                var file = await genericRepository.GetItemAsync(filter: f => f.Id == fileId);

                filePath = file?.Path;
                fileName = file?.Name;

                // uploads التأكد من أن الملف موجود في المجلد 
                if (file == null || !System.IO.File.Exists(filePath))
                {
                    TempData["ErrorMessage"] = $"Error: Could not find file with name {fileName}";
                    return RedirectToAction("Index");
                }

             
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred";
                return RedirectToAction("Index");
            }

            return PhysicalFile(filePath, "application/octet-stream", fileName);
        }


        // قمت بتعريف هذه الدالة لان الكود الذي بداخلها سأحتاجه في اكثر من مكان
        // uploads حيث هي دالة تحذف معلومات الملف من القاعدة وتحذف الملف من مجلد 
        private async Task DeleteFileWithoutReturnAnyThing(Guid id, string fileNameInUploadsFolder )

        {
            // حذف معلومات الملف من القاعدة
            await genericRepository.DeleteAsync(id);

            await genericRepository.Save();

            // جلب مسار الملف من اجل حذفه من هذا المسار بعد حذفه من قاعدة البيانات
            var filePath = Path.Combine("uploads", $"{fileNameInUploadsFolder}");

            // حذف الملف إذا كان موجودا
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        // الدالة التالية استخدمتها في اكثر من مكان والكود الذي بداخلها يتأكد بأن امتداد الملف مسموح به 
        private bool ValidExtensions(string? fileName)
        {
            // قائمة بالامتدادات المسموح بها
            List<string> allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };

            // جلب امتداد الملف
            string fileExtension = Path.GetExtension(fileName);

            if (!allowedExtensions.Contains(fileExtension!.ToLower())) // اذا كان امتداد الملف غير مسموح به 
            {

                TempData["ErrorMessage"] = "Invalid file extension";
                return false;
            }

            // عندما تعيد الدالة ترو فذلك يعني أن اسم الملف متاح
            return true;
        }

        // من أجل حذفه  uploads  في الدالة التالية يتم جلب اسم الملف الموجود في المجلد 
        // uploads لأن اسمه في مجلد
        // يختلف عن اسمه في قاعدة البيانات وهذه الدالة مستخدمة في مكانين
        private string GetFileNameFromUploadsFolder(Guid id , string? name)
        {
            return  id.ToString() + System.IO.Path.GetExtension(name);
        }

    }
}
