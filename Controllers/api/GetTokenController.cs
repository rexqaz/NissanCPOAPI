using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication.Extensions;

namespace WebApplication.Controllers.api
{
    [JwtAuthActionFilter]
    public class GetTokenController : BaseAPIController
    {
        // GET: api/GetToken
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Get()
        {
            var token = "Qai?J57U3cVDaOpUooiR/BNs0VMQ=upZouRecG-VYc0ORi6/yD-KhpkMl1wFZpa9QOrpjb6YfXC0Nj?a1ysty5jF=AzCn13Hvi-1mKgg2tS1C!/aMtatPvx2bkbpGfIw=pR1De74lpd5vnrw7SNqEZqMXwwv14vMsOfI9SogzLr6T3x5thQ-ZKlX2vYlEvgFsZC6!CT8szQ2pE6=HrKDdtDwOIsiiB=MKdH/R/mH4DoZlnH!pfaU1cIavXBBIbFW";
            return ReturnOK(new { token });
        }
    }
}
