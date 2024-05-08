using System.ComponentModel.DataAnnotations;
//for possible future requirments
namespace MagicVilla_VillaAPI.Models.DTOs
{
	public class VillaNumberCreateDTO
	{
		[Required]
		public int VillaNo { get; set; }
		[Required]
		public int VillaID { get; set; }
		public string SpecialDetails { get; set; }
	}
}
