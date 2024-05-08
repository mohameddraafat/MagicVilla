using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTOs;
using MagicVilla_VillaAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VillaNumberAPIController : ControllerBase
	{
		protected APIResponse response;
		private readonly IVillaNumberRepository villaNoRepo;
		private readonly IVillaRepository villaRepo;
		private readonly IMapper mapper;

		public VillaNumberAPIController(IVillaNumberRepository _villaNoRepo, IMapper _mapper, IVillaRepository _villaRepo)
		{
			villaNoRepo = _villaNoRepo;
			mapper = _mapper;
			this.response = new APIResponse();
			villaRepo = _villaRepo;
		}

		#region READ
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<APIResponse>> GetVillasNumbers()
		{
			try
			{
				IEnumerable<VillaNumber> villasNumbersList = await villaNoRepo.GetAllAsync(includeProperties:"Villa");
				response.Result = mapper.Map<List<VillaNumberDTO>>(villasNumbersList);
				response.StatusCode = HttpStatusCode.OK;
				return Ok(response);
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.ErrorMessages = new List<string>() { ex.ToString() };
			}
			return response;
		}

		[HttpGet("{id:int}", Name = "GetVillaNumber")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
		{
			try
			{
				var villaNumber = await villaNoRepo.GetAsync(v => v.VillaNo == id);
				if (villaNumber == null)
				{
					response.StatusCode = HttpStatusCode.NotFound;
					return NotFound(response);
				}

				response.Result = mapper.Map<VillaNumberDTO>(villaNumber);
				response.StatusCode = HttpStatusCode.OK;
				return Ok(response);
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.ErrorMessages = new List<string>() { ex.ToString() };
			}
			return response;
		}
		#endregion

		#region CREATE
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)
		{
			try
			{
				if (await villaNoRepo.GetAsync(v => v.VillaNo == createDTO.VillaNo) != null)
				{
					ModelState.AddModelError("ErrorMessages", "Villa number already exists!");
					return BadRequest(ModelState);
				}

				if (await villaRepo.GetAsync(v => v.Id == createDTO.VillaID) == null)
				{
					ModelState.AddModelError("ErrorMessages", "Villa ID is invalid!");
					return BadRequest(ModelState);
				}

				if (createDTO == null)
					return BadRequest(createDTO);

				var villa = mapper.Map<VillaNumber>(createDTO);

				await villaNoRepo.CreateAsync(villa);
				response.Result = mapper.Map<VillaNumberDTO>(villa);
				response.StatusCode = HttpStatusCode.Created;

				return CreatedAtRoute("GetVilla", new { id = villa.VillaNo }, response);
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.ErrorMessages = new List<string>() { ex.ToString() };
			}
			return response;
		}
		#endregion

		#region DELETE
		[HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
		{
			try
			{
				if (id == 0)
				{
					response.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(response);
				}
				var villa = await villaNoRepo.GetAsync(v => v.VillaNo == id);

				if (villa == null)
				{
					response.StatusCode = HttpStatusCode.NotFound;
					return NotFound(response);
				}
				await villaNoRepo.RemoveAsync(villa);
				response.StatusCode = HttpStatusCode.OK;
				return Ok(response);
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.ErrorMessages = new List<string>() { ex.ToString() };
			}
			return response;
		}
		#endregion

		#region UPDATE
		[HttpPut("{id:int}", Name = "UpdateVillaNumber")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO updateDTO)
		{
			try
			{
				if (updateDTO == null || id != updateDTO.VillaNo)
				{
					response.StatusCode = HttpStatusCode.NotFound;
					return NotFound(response);
				}

				if (await villaRepo.GetAsync(v => v.Id == updateDTO.VillaID) == null)
				{
					ModelState.AddModelError("ErrorMessages", "Villa ID is invalid!");
					return BadRequest(ModelState);
				}

				var villaNumber = mapper.Map<VillaNumber>(updateDTO);

				await villaNoRepo.UpdateAsync(villaNumber);
				response.StatusCode = HttpStatusCode.OK;
				return Ok(response);
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.ErrorMessages = new List<string>() { ex.ToString() };
			}
			return response;
		}
		#endregion

	}
}
