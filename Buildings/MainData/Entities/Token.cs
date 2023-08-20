using AppCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MainData.Entities;

public class Token : BaseEntity
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public TokenStatus Status { get; set; }
    public Guid UserId { get; set; }
    public TokenType Type { get; set; }
    public DateTime AccessExpiredAt { get; set; }
    public DateTime RefreshExpiredAt { get; set; }

}

public enum TokenStatus
{
    Active = 1, Inactive = 2
}

public enum TokenType
{
    SignInToken = 1,
    ResetPassword = 2,
    ActiveAccount = 3
}

public class TokenConfig : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.Property(a => a.UserId).IsRequired();
        builder.Property(a => a.AccessToken).IsRequired();
        builder.Property(a => a.RefreshToken).IsRequired();
        builder.Property(a => a.Type).IsRequired();
        builder.Property(a => a.AccessExpiredAt).IsRequired();
        builder.Property(a => a.RefreshExpiredAt).IsRequired();
        builder.Property(a => a.Status).IsRequired();
    }
}