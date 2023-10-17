using API.DTOs.Employees;
using API.Models;
using Client.Contract;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Client.Controllers
{
    public class EmployeeController : Controller
    {

        private readonly IEmployeeRepository repository;

        public EmployeeController(IEmployeeRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IActionResult> List()
        {
            var result = await repository.Get();
            var listEmployee = new List<EmployeeDto>();
            if (result != null)
            {

                listEmployee = result.Data.Select(x => (EmployeeDto)x).ToList();
            }

            return View(listEmployee);
        }

        [HttpGet]
        public async Task<IActionResult> CreateEmployee()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeDto employeeDto)
        {
            if (ModelState.IsValid)
            {
                var result = await repository.Post(employeeDto);
                if (result.Code == 200)
                {
                    return RedirectToAction(nameof(List));
                }
                else if (result.Code == 409)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                    return View();
                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid guid)
        {
            var existingEmployee = await repository.Get(guid);

            if (existingEmployee != null)
            {
                var employeeDto = (EmployeeDto)existingEmployee.Data;
                return View(employeeDto);
            }
            else
            {
                // Handle error jika data tidak ditemukan
                return RedirectToAction("List"); // Redirect ke halaman daftar jika data tidak ditemukan
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeDto employeeDto)
        {
            if (ModelState.IsValid)
            {
                var result = await repository.Put(employeeDto.Guid, employeeDto);
                if (result != null)
                {
                    if (result.Code == 200) // Perubahan berhasil
                    {
                        return RedirectToAction(nameof(List));
                    }
                    else if (result.Code == 409) // Konflik, misalnya ada entitas dengan ID yang sama
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                        return View();
                    }
                    else
                    {
                        // Handle status kode lain sesuai kebutuhan Anda
                        // Contoh:
                        ModelState.AddModelError(string.Empty, "Terjadi kesalahan saat menyimpan perubahan.");
                        return View();
                    }
                }
                else
                {
                    // Handle ketika result adalah null, misalnya ada kesalahan saat melakukan permintaan HTTP
                    ModelState.AddModelError(string.Empty, "Terjadi kesalahan saat menyimpan perubahan.");
                    return View();
                }
            }
            return View();
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await repository.Get(id);
            var university = new EmployeeDto();
            if (result.Data?.Guid is null)
            {
                return View(university);
            }
            return View(result.Data);
        }

        [HttpPost]
        /*[ValidateAntiForgeryToken]*/
        public async Task<IActionResult> RemoveEmployee(Guid Guid)
        {
            var result = await repository.Delete(Guid);
            if (result.Code == 200)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        /*[HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Panggil fungsi Delete dari repository
            var result = await repository.Delete(id);

            if (result != null)
            {
                if (result.Code == 200) // Atau kode lain yang menunjukkan sukses
                {
                    // Penghapusan berhasil, tampilkan pesan sukses (opsional)
                    TempData["SuccessMessage"] = "Employee deleted successfully.";
                }
                else
                {
                    // Penghapusan gagal, tampilkan pesan kesalahan (opsional)
                    TempData["ErrorMessage"] = result.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete employee.";
            }

            return RedirectToAction("List"); // Redirect kembali ke daftar karyawan setelah penghapusan
        }*/


        public IActionResult Index()
        {
            return View();
        }
    }
}