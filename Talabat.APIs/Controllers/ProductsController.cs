using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;

namespace Talabat.APIs.Controllers
{

    public class ProductsController : BaseAPIController
    {
        //private readonly IGenericRepository<Product> _productsRepo;
        //private readonly IGenericRepository<ProductBrand> _brandsRepo;
        //private readonly IGenericRepository<ProductType> _typesRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductsController(
            IUnitOfWork unitOfWork,
            //IGenericRepository<Product> productsRepo,
            //IGenericRepository<ProductBrand> brandsRepo,
            //IGenericRepository<ProductType> typesRepo,
            IMapper mapper)
        {
            //_productsRepo = productsRepo;
            //_brandsRepo = brandsRepo;
            //_typesRepo = typesRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Get All Products
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams specParams)
        {
            var spec = new ProductWithBrandAndTypeSpecification(specParams);

            var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            var countSpec = new ProductWithFiltrationForCountSpecification(specParams);

            var count = await _unitOfWork.Repository<Product>().GetCountWithSpecAsync(countSpec);

            //OkObjectResult result = new OkObjectResult(products);
            //return result;
            return Ok(new Pagination<ProductToReturnDto>(specParams.PageIndex, specParams.PageSize, count, data));
        }
        #endregion

        #region Get Product By Id
        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductWithBrandAndTypeSpecification(id);

            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);
            if (product is null)
                return NotFound(new ApiResponse(404));

            return Ok(_mapper.Map<Product, ProductToReturnDto>(product));
        }
        #endregion


        #region Get All Brands
        [HttpGet("brands")] // GET : api/products/brands
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetAllBrands()
        {
            var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();

            return Ok(brands);
        }
        #endregion

        #region Get All Types
        [HttpGet("types")] // GET : api/products/types
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetAllTypes()
        {
            var types = await _unitOfWork.Repository<ProductType>().GetAllAsync();

            return Ok(types);
        }
        #endregion
    }
}
