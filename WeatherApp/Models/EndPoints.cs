using System;
using System.Collections.Generic;
using FluentValidation;
using System.Runtime.Serialization;

namespace WeatherApp.Models
{
    public static class Enums
    {
        public static List<string> sord = new List<string>{"asc", "desc"};
        public static List<string> oper = new List<string>{"add", "edit", "delete"};
    }


    public class PagedReq
    {
        public uint page { get; set; }
        public uint rows { get; set; }
        public string sord { get; set; }
    }

    public class PagedReqValidator<T> : AbstractValidator<T> where T : PagedReq
    {
        public PagedReqValidator()
        {
            RuleFor(x => x.page).NotEmpty();
            RuleFor(x => x.rows).NotEmpty().Must(x => x <= Int32.MaxValue);
            RuleFor(x => x.sord).NotEmpty().Must(x => Enums.sord.Contains(x));
            RuleFor(x => x).Must(MulOfPageAndRowsLessThenInt32Max);
        }

        private bool MulOfPageAndRowsLessThenInt32Max(T req)
        {
            double skip = (req.page - 1) * req.rows;

            return skip <= Int32.MaxValue;
        }
    }

    public interface IPlaceIdField
    {
        int placeId { get; set; }
    }

    public class PlaceIdFieldValidator<T> : AbstractValidator<T> where T : IPlaceIdField
    {
        public PlaceIdFieldValidator()
        {
            RuleFor(x => x.placeId).Must(x => x >= 0);
        }
    }

    public interface IOperField
    {
        string oper { get; set; }
    }

    public class OperFieldValidator<T> : AbstractValidator<T> where T : IOperField
    {
        public OperFieldValidator()
        {
            RuleFor(x => x.oper).NotEmpty().Must(x => Enums.oper.Contains(x));
        }
    }

    public class GetLocationsReq : PagedReq
    {
    }

    public class GetLocationsReqValidator : PagedReqValidator<GetLocationsReq>
    {
    }

    public class SetLocationReq : IOperField, IPlaceIdField
    {
        public int placeId { get; set; }
        public string oper { get; set; }

        public string title { get; set; }
        public string openWeatherProvidersAlias { get; set; }
        public string yahooProvidersAlias { get; set; }

    }

    public class SetLocationReqValidator : AbstractValidator<SetLocationReq>
    {
        public SetLocationReqValidator()
        {

            Include(new OperFieldValidator<SetLocationReq>());
            Include(new PlaceIdFieldValidator<SetLocationReq>());

            // RuleFor(x => x.placeId).NotEmpty().When(x => x.oper != "add");

            RuleFor(x => x.title).NotEmpty();
            RuleFor(x => x.openWeatherProvidersAlias).NotEmpty();
            RuleFor(x => x.yahooProvidersAlias).NotEmpty();
        }
    }

    public class GetHistoryReq : PagedReq, IPlaceIdField
    {
        public int placeId { get; set; }
        public string provider { get; set; }
    }

    public class GetHistoryReqValidator : PagedReqValidator<GetHistoryReq>
    {
        public GetHistoryReqValidator()
        {
            Include(new PlaceIdFieldValidator<GetHistoryReq>());
        }
    }
}
