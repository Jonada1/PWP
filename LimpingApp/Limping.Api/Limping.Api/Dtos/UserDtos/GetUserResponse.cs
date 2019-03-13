using Halcyon.HAL;
using Limping.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Limping.Api.Dtos.UserDtos
{
    public class GetUserResponse : HALResponse
    {
        public GetUserResponse(AppUser user, List<Link> links = null):base(new UserDto
        {
            Email = user.Email,
            Id = user.Id,
            UserName = user.UserName,
        })
        {
            this.AddEmbeddedCollection("limpingTests", user.LimpingTests);
            if(links == null)
            {
                this.AddLinks(new Link("self", $"/api/Users/GetById/{user.Id}"));
            } else
            {
                this.AddLinks(links);
            }
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
