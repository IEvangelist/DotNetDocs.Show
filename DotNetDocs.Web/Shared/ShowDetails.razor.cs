using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetDocs.Services.Models;
using DotNetDocs.Web.Extensions;
using Microsoft.AspNetCore.Components;

namespace DotNetDocs.Web.Shared
{
    public partial class ShowDetails
    {
        [Parameter]
        public DocsShow Show { get; set; } = null!;

        [Parameter]
        public int Index { get; set; }

        bool IsDivisableBy(int number) => (Index + 1) % number == 0;

        IDictionary<string, string> Tags =>
                Show.Tags
                    .Select(tag => (tag, color: ""))
                    .Concat(
                        Show.Guests
                            .Where(p => p.IsMicrosoftMvp)
                            .Select(p => (tag: p.ToMvpUrl(), color: "badge-mvp")))
                    .ToDictionary(t => t.tag, t => t.color);

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
    }
}
