using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{

    public class BuggyController : BaseAPIController
    {
        private readonly StoreContext _dbContext;

        public BuggyController(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("notfound")] // GET : api/buggy/notfound
        public ActionResult GetNotFoundRequest()
        {
            var product = _dbContext.Products.Find(100);
            if (product is null)
                return NotFound(new ApiResponse(404));

            return Ok(product);
        }


        [HttpGet("servererror")] // GET : api/buggy/servererror
        public ActionResult GetServerError()
        {
            var product = _dbContext.Products.Find(100);


            var productToReturn = product.ToString(); // will through exception [Null Reference Exception]

            return Ok(productToReturn);
        }


        [HttpGet("badrequest")] // GET : api/buggy/badrequest
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }


        [HttpGet("badrequest/{id}")] // GET : api/buggy/badrequest/one
        public ActionResult GetBadRequest(int id) // Validation Error
        {
            return Ok();
        }
    }
}
