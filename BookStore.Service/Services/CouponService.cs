using AutoMapper;
using BookStore.Core.Entities;
using BookStore.Core.Interfaces;
using BookStore.Service.DTOs;
using BookStore.Service.Interfaces;

namespace BookStore.Service.Services
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CouponService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CouponDto>> GetAllCouponsAsync()
        {
            var coupons = await _unitOfWork.Coupons.GetAllAsync();
            return _mapper.Map<IEnumerable<CouponDto>>(coupons);
        }

        public async Task<CouponDto> GetCouponByIdAsync(int id)
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
            return _mapper.Map<CouponDto>(coupon);
        }

        public async Task<CouponDto> CreateCouponAsync(CreateCouponDto createCouponDto)
        {
            var existingCoupon = await _unitOfWork.Coupons.GetByCodeAsync(createCouponDto.Code);
            if (existingCoupon != null)
            {
                throw new Exception("Kupon kodu zaten var.");
            }

            var coupon = _mapper.Map<Coupon>(createCouponDto);
            await _unitOfWork.Coupons.AddAsync(coupon);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<CouponDto>(coupon);
        }

        public async Task UpdateCouponAsync(int id, CreateCouponDto updateCouponDto)
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
            if (coupon == null)
            {
                throw new Exception("Kupon bulunamadı.");
            }

            _mapper.Map(updateCouponDto, coupon);
            coupon.UpdatedDate = DateTime.Now;

            _unitOfWork.Coupons.Update(coupon);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteCouponAsync(int id)
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
            if (coupon == null)
            {
                throw new Exception("Kupon bulunamadı.");
            }

            _unitOfWork.Coupons.Remove(coupon);
            await _unitOfWork.CommitAsync();
        }

        public async Task<CouponValidationResult> ValidateCouponAsync(string code)
        {
            var result = new CouponValidationResult();
            var coupon = await _unitOfWork.Coupons.GetByCodeAsync(code);

            if (coupon == null)
            {
                result.IsValid = false;
                result.ErrorMessage = "Geçersiz kupon kodu";
                return result;
            }

            var now = DateTime.Now;
            if (!coupon.IsActive)
            {
                result.IsValid = false;
                result.ErrorMessage = "Kupon aktif değil";
            }
            else if (now < coupon.ValidFrom)
            {
                result.IsValid = false;
                result.ErrorMessage = $"Kupon şu tarihe kadar geçerli değil: {coupon.ValidFrom.ToShortDateString()}";
            }
            else if (now > coupon.ValidTo)
            {
                result.IsValid = false;
                result.ErrorMessage = "Kupon artık geçerli değil";
            }
            else if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit)
            {
                result.IsValid = false;
                result.ErrorMessage = "Kupon kullanım limiti doldu";
            }
            else
            {
                result.IsValid = true;
                result.Coupon = _mapper.Map<CouponDto>(coupon);
            }

            return result;
        }
    }
}