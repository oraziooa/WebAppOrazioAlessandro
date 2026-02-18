using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;

namespace WebAppOrazioAlessandro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaceDetectionController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> DetectFace(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("Immagine non valida");

            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);

            var imageBytes = memoryStream.ToArray();

            var mat = Cv2.ImDecode(imageBytes, ImreadModes.Color);

            if (mat.Empty())
                return BadRequest("Errore lettura immagine");

            var cascadePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "haarcascade_frontalface_default.xml");

            var cascade = new CascadeClassifier(cascadePath);

            var faces = cascade.DetectMultiScale(mat);

            return Ok(new
            {
                FaceDetected = faces.Length > 0,
                FacesCount = faces.Length
            });
        }
    }
}
