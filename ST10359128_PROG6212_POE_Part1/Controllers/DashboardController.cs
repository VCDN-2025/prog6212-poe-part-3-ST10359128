using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ST10359128_PROG6212_POE_Part1.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace ST10359128_PROG6212_POE_Part1.Controllers
{
    public class DashboardController : Controller
    {
        private static readonly List<Claim> claims = new();

        public IActionResult Index()
        {
            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View(claims);
        }

        [HttpGet]
        public IActionResult Submit()
        {
            return View(new Claim());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(Claim claim, IFormFile document)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "⚠️ Please fill in all required fields below.";
                return View(claim);
            }

            // handle file
            if (document != null && document.Length > 0)
            {
                var ext = Path.GetExtension(document.FileName).ToLower();
                var allowed = new[] { ".pdf", ".docx", ".xlsx" };

                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("FileName", "Only PDF, DOCX, and XLSX are allowed.");
                    ViewBag.Message = "⚠️ Please upload a valid document.";
                    return View(claim);
                }

                if (document.Length > 5_000_000)
                {
                    ModelState.AddModelError("FileName", "File too large (max 5 MB).");
                    ViewBag.Message = "⚠️ File too large.";
                    return View(claim);
                }

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, document.FileName);
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    document.CopyTo(fs);
                }
                claim.FileName = document.FileName;
            }

            claim.Id = claims.Count + 1;
            claim.Status = "Pending";
            claim.Date = DateTime.Now;
            claims.Add(claim);

            TempData["SuccessMessage"] = "✅ Claim submitted successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ManagerView() => View(claims);

        [HttpPost]
        public IActionResult UpdateStatus(int id, string actionType)
        {
            var c = claims.FirstOrDefault(x => x.Id == id);
            if (c != null)
                c.Status = actionType == "Approve" ? "Approved" : "Rejected";

            return RedirectToAction("ManagerView");
        }
    }
}
