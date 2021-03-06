﻿using System.Web.Http;
using Emvelope.Integration.Models;

namespace Emvelope.Integration.Controllers
{
    public class BatteriesController : ApiController
    {
        public Battery[] Get()
        {
            var results = new[]
            {
                new Battery
                {
                    Id = 1,
                    Name = "Duracel",
                },
                new Battery
                {
                    Id = 1,
                    Name = "MegaVolt",
                }
            };

            return results;
        }

        public Battery Post([FromBody]NewBatteryModel battery)
        {
            return new Battery
            {
                Id = 4,
                Name = battery.Name
            };
        }
    }
}
