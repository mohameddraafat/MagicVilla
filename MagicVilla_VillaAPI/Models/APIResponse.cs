using System.Net;
using MagicVilla_VillaAPI.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Models
{
	public class APIResponse
	{
		public APIResponse() 
		{
			ErrorMessages = new List<string>();
		}
		public HttpStatusCode StatusCode { get; set; }
		public bool IsSuccess { get; set; } = true;
		public List<string> ErrorMessages { get; set; }
		public object Result { get; set; }
	}
}
