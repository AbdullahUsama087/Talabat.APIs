using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.APIs.Controllers
{

    public class BasketController : BaseAPIController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }


        #region Get Basket
        [HttpGet] // Get : api/baskets?id=

        public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);
            return basket is null ? Ok(new CustomerBasket(id)) : Ok(basket);
        }
        #endregion

        #region Create Or Update Basket
        [HttpPost] // POST : api/baskets
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
        {
            var mappedBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basket);

            var createdOrUpdatedBasket = await _basketRepository.UpdateBasketAsync(mappedBasket);

            if (createdOrUpdatedBasket is null)
                return BadRequest(new ApiResponse(400));
            return createdOrUpdatedBasket;
        }
        #endregion

        #region Delete Basket
        [HttpDelete] // DELETE : api/baskets
        public async Task<ActionResult<bool>> DeleteBasket(string id)
            => await _basketRepository.DeleteBasketAsync(id);
        #endregion

    }
}
