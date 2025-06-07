using AutoMapper;
using BookStore.Core.Entities;
using BookStore.Core.Interfaces;
using BookStore.Service.DTOs;
using BookStore.Service.Interfaces;

namespace BookStore.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserDto> RegisterAsync(CreateUserDto createUserDto)
        {
            var existingUser = await _unitOfWork.Users.FindAsync(u => u.Username == createUserDto.Username || u.Email == createUserDto.Email);
            if (existingUser.Any())
                throw new Exception("Kullanıcı adı veya e-posta adresi zaten kullanılıyor.");

            var user = _mapper.Map<User>(createUserDto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.Username == loginDto.Username);
            if (!user.Any() || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.First().PasswordHash))
                throw new Exception("Geçersiz kullanıcı adı/parola");

            return _mapper.Map<UserDto>(user.First());
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            return _mapper.Map<UserDto>(user);
        }

        public async Task UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) throw new Exception("Kullanıcı bulunamadı.");

            _mapper.Map(updateUserDto, user);
            if (!string.IsNullOrEmpty(updateUserDto.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);

            user.UpdatedDate = DateTime.Now;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) throw new Exception("Kullanıcı bulunamadı.");

            _unitOfWork.Users.Remove(user);
            await _unitOfWork.CommitAsync();
        }
    }
}
