using MagicVilla_CouponAPI.Models.DTO;
using MagicVilla_CouponAPI.Models;

namespace MagicVilla_CouponAPI;

public sealed class MappingConfig
{
    public CouponCreateDTO ToCouponCreateDTO(Coupon source)
    {
        if (source is null) return null!;
        return new CouponCreateDTO
        {
            Name = source.Name,
            Percent = source.Percent,
            IsActive = source.IsActive,
        };
    }

    public Coupon ToCoupon(CouponCreateDTO source)
    {
        if (source is null) return null!;
        return new Coupon
        {
            Name = source.Name,
            Percent = source.Percent,
            IsActive = source.IsActive,
        };
    }

    public CouponUpdateDTO ToCouponUpdateDTO(Coupon source)
    {
        if (source is null) return null!;
        return new CouponUpdateDTO
        {
            Id = source.Id,
            Name = source.Name,
            Percent = source.Percent,
            IsActive = source.IsActive,
        };
    }

    public Coupon ToCoupon(CouponUpdateDTO source)
    {
        if (source is null) return null!;
        return new Coupon
        {
            Id = source.Id,
            Name = source.Name,
            Percent = source.Percent,
            IsActive = source.IsActive,
        };
    }

    public CouponDTO ToCouponDTO(Coupon source)
    {
        if (source is null) return null!;
        return new CouponDTO
        {
            Id = source.Id,
            Name = source.Name,
            Percent = source.Percent,
            IsActive = source.IsActive,
            Created = source.Created,
        };
    }

    public Coupon ToCoupon(CouponDTO source)
    {
        if (source is null) return null!;
        return new Coupon
        {
            Id = source.Id,
            Name = source.Name,
            Percent = source.Percent,
            IsActive = source.IsActive,
            Created = source.Created,
        };
    }

    public UserDTO ToUserDTO(LocalUser source)
    {
        if (source is null) return null!;
        return new UserDTO
        {
            ID = source.ID.ToString(System.Globalization.CultureInfo.InvariantCulture),
            UserName = source.UserName,
            Name = source.Name,
        };
    }

    public LocalUser ToLocalUser(UserDTO source)
    {
        if (source is null) return null!;
        return new LocalUser
        {
            ID = int.Parse(source.ID, System.Globalization.CultureInfo.InvariantCulture),
            UserName = source.UserName,
            Name = source.Name,
        };
    }

    public UserDTO ToUserDTO(ApplicationUser source)
    {
        if (source is null) return null!;
        return new UserDTO
        {
            ID = source.Id,
            UserName = source.UserName,
            Name = source.Name,
        };
    }

    public ApplicationUser ToApplicationUser(UserDTO source)
    {
        if (source is null) return null!;
        return new ApplicationUser
        {
            Name = source.Name,
            Id = source.ID,
            UserName = source.UserName,
        };
    }

    public List<CouponCreateDTO> ToCouponCreateDTOList(IEnumerable<Coupon> source) =>
        source?.Select(ToCouponCreateDTO).ToList() ?? new List<CouponCreateDTO>();

    public List<Coupon> ToCouponList(IEnumerable<CouponCreateDTO> source) =>
        source?.Select(ToCoupon).ToList() ?? new List<Coupon>();

    public List<CouponUpdateDTO> ToCouponUpdateDTOList(IEnumerable<Coupon> source) =>
        source?.Select(ToCouponUpdateDTO).ToList() ?? new List<CouponUpdateDTO>();

    public List<Coupon> ToCouponList(IEnumerable<CouponUpdateDTO> source) =>
        source?.Select(ToCoupon).ToList() ?? new List<Coupon>();

    public List<CouponDTO> ToCouponDTOList(IEnumerable<Coupon> source) =>
        source?.Select(ToCouponDTO).ToList() ?? new List<CouponDTO>();

    public List<Coupon> ToCouponList(IEnumerable<CouponDTO> source) =>
        source?.Select(ToCoupon).ToList() ?? new List<Coupon>();

    public List<UserDTO> ToUserDTOList(IEnumerable<LocalUser> source) =>
        source?.Select(ToUserDTO).ToList() ?? new List<UserDTO>();

    public List<LocalUser> ToLocalUserList(IEnumerable<UserDTO> source) =>
        source?.Select(ToLocalUser).ToList() ?? new List<LocalUser>();

    public List<UserDTO> ToUserDTOList(IEnumerable<ApplicationUser> source) =>
        source?.Select(ToUserDTO).ToList() ?? new List<UserDTO>();

    public List<ApplicationUser> ToApplicationUserList(IEnumerable<UserDTO> source) =>
        source?.Select(ToApplicationUser).ToList() ?? new List<ApplicationUser>();
}
