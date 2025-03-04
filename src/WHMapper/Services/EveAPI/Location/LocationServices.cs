﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using WHMapper.Models.DTO;
using WHMapper.Models.DTO.EveAPI.Location;
using WHMapper.Models.DTO.ResponseMessage;
using WHMapper.Services.EveOnlineUserInfosProvider;

namespace WHMapper.Services.EveAPI.Location
{
    internal class LocationServices : AEveApiServices,ILocationServices
    {
        private readonly IEveUserInfosServices? _userService = null!;


        public LocationServices(HttpClient httpClient, TokenProvider _tokenProvider, IEveUserInfosServices userService) : base(httpClient, _tokenProvider)
        {
            _userService = userService;
        }

        public async Task<EveLocation?> GetLocation()
        {
            if (_userService != null)
            {
                string characterId = await _userService.GetCharactedID();
                return await base.Execute<EveLocation>(RequestSecurity.Authenticated, RequestMethod.Get, string.Format("/v2/characters/{0}/location/?datasource=tranquility", characterId));

            }
            return null;
        }
        
    }
}
