using System.Collections.Generic;
using UploadClient.Models.Error;

namespace UploadClient.Contracts.Response
{
	public class ErrorResponse
	{
		public ErrorResponse() { }

		public ErrorResponse(ErrorModel error)
		{
			Errors.Add(error);
		}

		public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
	}
}
