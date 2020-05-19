﻿namespace Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Olive.Mvc;

    public class SharedActionsController : BaseController
    {
        readonly IFileRequestService FileRequestService;

        public SharedActionsController(IFileRequestService fileRequestService)
        {
            FileRequestService = fileRequestService;
        }

        [Route("error")]
        public ActionResult Error() => View("error");

        [Route("error/404")]
        public ActionResult NotFound404() => View("error-404");
        [HttpPost, Authorize, Route("upload")]
        public async Task<IActionResult> UploadTempFileToServer(IFormFile[] files)
        {
            return Json(await FileRequestService.TempSaveUploadedFile(files[0]));
        }

        [Route("file")]
        public async Task<ActionResult> DownloadFile()
        {
            var path = Request.QueryString.ToString().TrimStart('?');
            var accessor = await FileAccessor.Create(path, User);

            if (accessor.Blob.IsMedia())
                return await RangeFileContentResult.From(accessor.Blob);
            else return await File(accessor.Blob);
        }

        [Route("temp-file/{key}")]
        public Task<ActionResult> DownloadTempFile(string key) => FileRequestService.Download(key);
    }
}