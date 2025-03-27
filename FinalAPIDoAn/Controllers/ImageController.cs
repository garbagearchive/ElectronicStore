using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly CloudinaryService _cloudinaryService;

    public ImagesController(CloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string folderName)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var result = await _cloudinaryService.UploadImageAsync(file, folderName);

            return Ok(new
            {
                PublicId = result.PublicId,
                Url = result.SecureUrl.ToString(),
                Format = result.Format,
                Bytes = result.Bytes
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("upload-multiple")]
    public async Task<IActionResult> UploadMultipleImages(List<IFormFile> files, [FromQuery] string folderName)
    {
        try
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files uploaded");

            var uploadTasks = files.Select(file => _cloudinaryService.UploadImageAsync(file, folderName));
            var results = await Task.WhenAll(uploadTasks);

            return Ok(results.Select(result => new {
                PublicId = result.PublicId,
                Url = result.SecureUrl.ToString(),
                Format = result.Format,
                Bytes = result.Bytes
            }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteImage([FromQuery] string publicId)
    {
        try
        {
            var result = await _cloudinaryService.DeleteImageAsync(publicId);
            return Ok(new { Result = result.Result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}