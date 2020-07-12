using DotNetDocs.Services.Models;
using Microsoft.AspNetCore.Components;

namespace DotNetDocs.Web.Shared
{
    public partial class ShareShow
    {
        [Parameter]
        public DocsShow? Show { get; set; }
    }
}
