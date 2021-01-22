using ApiVersioning.Contracts.Response;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiVersioning.Controllers
{
    [ApiController]
    [Route("api/products")]
   // [Route("api/v{version:apiVersion}/products")] add versioning
    [ApiVersion("1.0", Deprecated = true)] //show that Api is deprecated
    [ApiVersion("2.0")]
    public class VersionongController : ControllerBase
    {

        [HttpGet("{productId}")]
        public IActionResult GetProductV1([FromRoute] Guid productId)
        {
            var product = new ProductResponse
            {
                Id = productId,
                ProductName = "API Router"
            };
            return Ok(product);
        }
        //case 1
        [HttpGet("{productId}")]
        [MapToApiVersion("2.0")]
        // !!! add in querystring ()=> ?api-version=2.0
        public IActionResult GetProductV2([FromRoute] Guid productId)
        {
            var product = new ProductResponse2
            {
                Id = productId,
                ProductName2 = "API Router"
            };
            return Ok(product);
        }
    }
}
