using Dewit.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dewit.Infrastructure.Data.Config;

public class ConfigItemConfiguration: IEntityTypeConfiguration<ConfigItem>
{
    public void Configure(EntityTypeBuilder<ConfigItem> builder)
    {
        builder.HasKey(configItem => configItem.Id);

        builder.Property(configItem => configItem.Value)
            .IsRequired();
    }
}