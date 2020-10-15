using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetDocs.Services.Models;
using DotNetDocs.Web.Extensions;
using DotNetDocs.Web.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DotNetDocs.Web.Shared
{
    public partial class ShowDetails
    {
        [Inject]
        public IJSRuntime JavaScript { get; set; } = null!;

        [Parameter]
        public DocsShow Show { get; set; } = null!;

        [Parameter]
        public int Index { get; set; }

        bool IsDivisableBy(int number) => (Index + 1) % number == 0;

        IDictionary<string, string> Tags => Show.ToShowTags();

        string ShowDateTimeString => Show.ToDateString();

        readonly IDictionary<string, string> _fileExtensionToMimeTypeMap =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [".jpg"] = "image/jpeg",
                [".jpeg"] = "image/jpeg",
                [".png"] = "image/png",
                [".svg"] = "image/svg+xml"
            };

        string? ToMimeType(string filePath) =>
            _fileExtensionToMimeTypeMap.TryGetValue(
                Path.GetExtension(filePath ?? ""), out string? mimeType)
                ? mimeType
                : null;

        protected override async Task OnAfterRenderAsync(bool firstRender) =>
            await JavaScript.LoadTwitterImagesAsync();
    }
}
