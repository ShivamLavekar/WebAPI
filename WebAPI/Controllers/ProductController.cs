using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext db;

        public ProductController(AppDbContext db)
        {
            this.db = db;
        }

        [HttpPost]
        [Route("AddProd")]
        public IActionResult AddNewProduct(Product p)
        {
            db.Products.Add(p);
            db.SaveChanges();
            return Ok("Product Added Successfully");
        }


        [HttpGet]
        [Route("GetProd")]
        public IActionResult FetchAllProduct()
        {
            var data = db.Products.ToList();
            return Ok(data);
        }

        [HttpDelete]
        [Route("DelProd/{id}")]
        public IActionResult DelProductById(int id)
        {
            var data = db.Products.Find(id);
            if (data == null)
            {
                return NotFound("Product not found");
            }
            db.Products.Remove(data);
            db.SaveChanges();
            return Ok("Product Deleted Successfully");
        }

        [HttpPut]
        [Route("UpdateProd/{id}")]
        public IActionResult UpdateProduct(Product p)
        {
            db.Products.Update(p);
            db.SaveChanges();
            return Ok("Product Updated Successfully");
        }

        [HttpGet]
        [Route("SearchProd/{name}")]
        public IActionResult SearchProduct(string name)
        {
            var data = db.Products.Where(x => x.Pname.Contains(name)).ToList();
            return Ok(data);
        }

        [HttpGet]
        [Route("GetProductById/{id}")]
        public IActionResult GetProductById(int id)
        {
            var data = db.Products.Find(id);
            if (data == null)
            {
                return NotFound("Product not found");
            }
            return Ok(data);
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads", file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { filePath = path });
        }

        [HttpGet]
        [Route("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var path = Path.Combine("uploads", fileName);

            if (!System.IO.File.Exists(path))
                return NotFound("File not found.");

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(path), fileName);
        }

        private string GetContentType(string path)
        {
            var types = new Dictionary<string, string>
    {
        { ".txt", "text/plain" },
        { ".pdf", "application/pdf" },
        { ".doc", "application/vnd.ms-word" },
        { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        { ".xls", "application/vnd.ms-excel" },
        { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
        { ".png", "image/png" },
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".gif", "image/gif" },
        { ".csv", "text/csv" }
    };

            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }
    }
}
