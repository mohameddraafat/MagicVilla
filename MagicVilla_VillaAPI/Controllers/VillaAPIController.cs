using AutoMapper;
using Azure;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTOs;
using MagicVilla_VillaAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VillaAPIController : ControllerBase
	{
		protected APIResponse response;
		private readonly IVillaRepository villaRepo;
		private readonly IMapper mapper;

		public VillaAPIController(IVillaRepository _villaRepo, IMapper _mapper)
		{
			villaRepo = _villaRepo;
			mapper = _mapper;
			this.response = new APIResponse();
		}

		#region READ
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<APIResponse>> GetVillas()
		{
			try
			{
				IEnumerable<Villa> VillasList = await villaRepo.GetAllAsync();
				response.Result = mapper.Map<List<VillaDTO>>(VillasList);
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

		[HttpGet("{id:int}", Name = "GetVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[Authorize]
		public async Task<ActionResult<APIResponse>> GetVilla(int id)
		{
			try
			{
				if (id == 0)
				{
					response.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(response);
				}
				var villa = await villaRepo.GetAsync(v => v.Id == id);

				if (villa == null)
				{
					response.StatusCode = HttpStatusCode.NotFound;
					return NotFound(response);
				}
				response.Result = mapper.Map<VillaDTO>(villa);
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
        [Authorize(Roles ="admin")]

        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)
		{
			try
			{
				if (await villaRepo.GetAsync(v => v.Name.ToLower() == createDTO.Name.ToLower()) != null)
				{
					ModelState.AddModelError("ErrorMessages", "Villa already Exists!");
					return BadRequest(ModelState);
				}


				if (createDTO == null)
					return BadRequest(createDTO);

				Villa villa = mapper.Map<Villa>(createDTO);

				await villaRepo.CreateAsync(villa);
				response.Result = mapper.Map<VillaDTO>(villa);
				response.StatusCode = HttpStatusCode.Created;

				return CreatedAtRoute("GetVilla", new { id = villa.Id }, response);
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
		[HttpDelete("{id:int}", Name = "DeleteVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]

        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
		{
			try
			{
				if (id == 0)
				{
					response.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(response);
				}
				var villa = await villaRepo.GetAsync(v => v.Id == id);

				if (villa == null)
				{
					response.StatusCode = HttpStatusCode.NotFound;
					return NotFound(response);
				}
				await villaRepo.RemoveAsync(villa);
				response.StatusCode = HttpStatusCode.NoContent;
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
		[HttpPut("{id:int}", Name = "UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]

        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
		{
			try
			{
				if (updateDTO == null || id != updateDTO.Id)
				{
					response.StatusCode = HttpStatusCode.NotFound;
					return NotFound(response);
				}

				Villa model = mapper.Map<Villa>(updateDTO);

				await villaRepo.UpdateAsync(model);
				response.StatusCode = HttpStatusCode.NoContent;
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