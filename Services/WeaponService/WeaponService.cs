using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Dtos.Weapon;

namespace dotnet_rpg.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;
        public WeaponService(DataContext context, IHttpContextAccessor contextAccessor, IMapper mapper)
        {
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _context = context;

        }

        public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon)
        {
            var response = new ServiceResponse<GetCharacterDto>();
            try
            {
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.id == newWeapon.CharacterId && c.User!.id == int.Parse(_contextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!));
                if(character == null){
                    response.Succes = false;
                    response.Message = "Character not Found";    
                }

                var weapon = new Weapon { 
                    Name = newWeapon.Name,
                    Damage = newWeapon.Damage,
                    Character = character 
                };
                _context.Weapons.Add(weapon);
                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (System.Exception ex)
            {
                response.Succes = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}