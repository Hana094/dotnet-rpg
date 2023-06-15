using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _dataContext = dataContext;
            _mapper = mapper;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);



        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var character = _mapper.Map<Character>(newCharacter);
            character.User = await _dataContext.Users.FirstOrDefaultAsync(u => u.id == GetUserId());
            _dataContext.Characters.Add(character);
            await _dataContext.SaveChangesAsync();
            serviceResponse.Data = await _dataContext.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .Where(c => c.User!.id == GetUserId()).Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters = await _dataContext.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .Where(c => c.User!.id == GetUserId()).ToListAsync();
            serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            var dbCharacter = await _dataContext.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.id == id && c.User!.id == GetUserId());
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            try
            {

                var character =
                await _dataContext.Characters
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.id == updatedCharacter.id);
                if (character == null || character.User!.id != GetUserId())
                {
                    throw new Exception($"Character with Id '{updatedCharacter.id}' not found");
                }

                character.Name = updatedCharacter.Name;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Strength = updatedCharacter.Strength;
                character.Defense = updatedCharacter.Defense;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Class = updatedCharacter.Class;

                await _dataContext.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (Exception ex)
            {

                serviceResponse.Succes = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }
        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            try
            {

                var character = await _dataContext.Characters.FirstOrDefaultAsync(c => c.User!.id == GetUserId() && c.id == id);
                if (character == null)
                {
                    throw new Exception($"Character with Id '{id}' not found");
                }

                _dataContext.Characters.Remove(character);

                await _dataContext.SaveChangesAsync();

                serviceResponse.Data = await _dataContext.Characters.Where(c => c.User!.id == GetUserId()).Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            }
            catch (Exception ex)
            {

                serviceResponse.Succes = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            var response = new ServiceResponse<GetCharacterDto>();
            try
            {
                var character = await _dataContext.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.id == newCharacterSkill.CharacterId && c.User!.id == GetUserId());

                if (character is null)
                {
                    response.Succes = false;
                    response.Message = "Character Not Found.";
                    return response;
                }
                var skill = await _dataContext.Skills
                    .FirstOrDefaultAsync(s => s.id == newCharacterSkill.SkillId);
                if (skill is null)
                {
                    response.Succes = false;
                    response.Message = "Skill Not Found.";
                    return response;
                }
                character.Skills!.Add(skill);
                await _dataContext.SaveChangesAsync();
                response.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (Exception ex)
            {
                response.Succes = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}