using HybridModelBinding;

namespace INDG.Image.Service.ApiModels
{
    public class ResizeImageRequest
    {
        [HybridBindProperty(Source.Route)]
        public string Id { get; set; }

        [HybridBindProperty(Source.Body)]
        public int Height { get; set; }
    }
}
