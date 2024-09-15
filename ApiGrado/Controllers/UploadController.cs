using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace ApiGrado.Controllers
{
    [Route("api/upload")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly ILogger<UploadController> _logger;
        private readonly IWebHostEnvironment _environment;

        public UploadController(ILogger<UploadController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                _logger.LogInformation($"Attempting to upload file: {file.FileName}");

                var folderName = Path.Combine("ImagenesPosts");
                var pathToSave = Path.Combine(_environment.ContentRootPath, folderName);

                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"'));
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName).Replace("\\", "/");

                    Directory.CreateDirectory(pathToSave);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    _logger.LogInformation($"File uploaded successfully: {dbPath}");
                    return Ok(dbPath);
                }
                else
                {
                    _logger.LogWarning("File length is 0");
                    return BadRequest("File length is 0");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uploading file");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("ImagenesPosts/{fileName}")]
        public IActionResult GetFile(string fileName)
        {
            _logger.LogInformation($"Attempting to retrieve file: {fileName}");
            var path = Path.Combine(_environment.ContentRootPath, "ImagenesPosts", fileName);
            if (!System.IO.File.Exists(path))
            {
                _logger.LogWarning($"File not found: {path}");
                return NotFound();
            }
            return PhysicalFile(path, "application/octet-stream");
        }
    }
}
