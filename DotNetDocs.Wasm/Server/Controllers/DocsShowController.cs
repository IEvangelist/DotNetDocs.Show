using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetDocs.Services;
using DotNetDocs.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetDocs.Wasm.Server.Controllers
{
    [ApiController, Route("shows")]
    public class DocsShowController : ControllerBase
    {
        readonly IScheduleService _scheduleService;

        public DocsShowController(IScheduleService scheduleService) =>
            _scheduleService = scheduleService;

        [HttpGet, Route("/{id}")]
        public ValueTask<DocsShow?> Get([FromRoute] string id) =>
            _scheduleService.GetShowAsync(id);

        [HttpGet]
        public ValueTask<IEnumerable<DocsShow>> Get(DateTime since) =>
            _scheduleService.GetAllAsync(since);

        [HttpDelete, Route("/{id}")]
        public ValueTask<DocsShow> Delete([FromRoute] string id) =>
            _scheduleService.DeleteShowAsync(id);

        [HttpPost]
        public ValueTask<DocsShow> Create([FromBody] DocsShow show) =>
            _scheduleService.CreateShowAsync(show);

        [HttpPut]
        public ValueTask<DocsShow> Update([FromBody] DocsShow show) =>
            _scheduleService.UpdateShowAsync(show);
    }
}
